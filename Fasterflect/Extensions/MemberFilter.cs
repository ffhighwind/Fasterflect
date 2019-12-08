#region License
// Copyright 2009 Buu Nguyen (http://www.buunguyen.net/blog)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fasterflect.codeplex.com/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fasterflect.Extensions
{
	internal static class MemberFilter
	{
		internal static bool IsReservedName(this string name)
		{
			name = name.ToLowerInvariant();
			return name == ".ctor" || name == ".cctor";
		}

		internal static string TrimExplicitlyImplementedName(this string name)
		{
			int index = name.IsReservedName() ? -1 : name.LastIndexOf('.') + 1;
			return index > 0 ? name.Substring(index) : name;
		}

		/// <summary>
		/// This method applies name filtering to a set of members.
		/// </summary>
		public static List<T> Filter<T>(this IEnumerable<T> members, FasterflectFlags bindingFlags, params string[] names)
			where T : MemberInfo
		{
			List<T> result = new List<T>();
			bool ignoreCase = bindingFlags.IsSet(FasterflectFlags.IgnoreCase);
			bool isPartial = bindingFlags.IsSet(FasterflectFlags.PartialNameMatch);
			bool trimExplicit = bindingFlags.IsSet(FasterflectFlags.TrimExplicitlyImplemented);

			foreach (T member in members) {
				string memberName = trimExplicit ? member.Name.TrimExplicitlyImplementedName() : member.Name;
				for (int j = 0; j < names.Length; j++) {
					string name = names[j];
					StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
					bool match = isPartial ? memberName.Contains(name) : memberName.Equals(name, comparison);
					if (match) {
						result.Add(member);
						break;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// This method applies type parameter type filtering to a set of methods.
		/// </summary>
		public static List<T> Filter<T>(this IEnumerable<T> methods, Type[] genericTypes)
			where T : MethodBase
		{
			List<T> result = new List<T>();
			foreach (T method in methods) {
				if (method.ContainsGenericParameters) {
					Type[] genericArgs = method.GetGenericArguments();
					if (genericArgs.Length != genericTypes.Length)
						continue;
					result.Add(method);
				}
			}
			return result;
		}

		/// <summary>
		/// This method applies method parameter type filtering to a set of methods.
		/// </summary>
		public static List<T> Filter<T>(this IEnumerable<T> methods, FasterflectFlags bindingFlags, Type[] paramTypes)
			where T : MethodBase
		{
			List<T> result = new List<T>();

			bool exact = bindingFlags.IsSet(FasterflectFlags.ExactBinding);
			foreach (T method in methods) {
				// verify parameters
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length != paramTypes.Length) {
					continue;
				}
				// verify parameter type compatibility
				bool match = true;
				for (int j = 0; j < paramTypes.Length; j++) {
					Type type = paramTypes[j];
					ParameterInfo parameter = parameters[j];
					Type parameterType = parameter.ParameterType;
					bool ignoreParameterModifiers = !exact;
					if (ignoreParameterModifiers && parameterType.IsByRef) {
						string name = parameterType.FullName;
						parameterType = Type.GetType(name.Substring(0, name.Length - 1)) ?? parameterType;
					}
					match &= parameterType.IsGenericParameter || parameterType.ContainsGenericParameters || (exact ? type == parameterType : parameterType.IsAssignableFrom(type));
					if (!match) {
						break;
					}
				}
				if (match) {
					result.Add(method);
				}
			}
			return result;
		}

		/// <summary>
		/// This method applies member type filtering to a set of members.
		/// </summary>
		public static List<T> Filter<T>(this IEnumerable<T> members, MemberTypes memberTypes)
			where T : MemberInfo
		{
			List<T> result = new List<T>();

			foreach (T member in members) {
				bool match = (member.MemberType & memberTypes) == member.MemberType;
				if (!match) {
					continue;
				}
				result.Add(member);
			}
			return result;
		}

		/// <summary>
		/// This method applies flags-based filtering to a set of members.
		/// </summary>
		public static List<T> Filter<T>(this IEnumerable<T> members, FasterflectFlags bindingFlags) where T : MemberInfo
		{
			List<T> result = new List<T>();
			List<string> properties = new List<string>();

			bool excludeHidden = bindingFlags.IsSet(FasterflectFlags.ExcludeHiddenMembers);
			bool excludeBacking = bindingFlags.IsSet(FasterflectFlags.ExcludeBackingMembers);
			bool excludeExplicit = bindingFlags.IsSet(FasterflectFlags.ExcludeExplicitlyImplemented);

			foreach (T member in members) {
				bool exclude = false;
				if (excludeHidden) {
					MethodBase method = member as MethodBase;
					// filter out anything but methods/constructors based on their name only
					exclude |= method == null && result.Any(m => m.Name == member.Name);
					// filter out methods that do not have a unique signature (this prevents overloads from being excluded by the ExcludeHiddenMembers flag)
					exclude |= method != null && result.Where(m => m is MethodBase).Cast<MethodBase>().Any(m => m.Name == member.Name && m.HasParameterSignature(method.GetParameters()));
				}
				if (!exclude && excludeBacking) {
					exclude |= member is FieldInfo && member.Name[0] == '<';
					if (member is MethodInfo methodInfo) {
						// filter out property backing methods
						exclude |= member.Name.Length > 4 && member.Name.Substring(1, 3) == "et_";
						// filter out base implementations when an overrride exists
						exclude |= result.ContainsOverride(methodInfo);
					}
					if (member is PropertyInfo property) {
						MethodInfo propertyGetter = property.GetGetMethod(true);
						exclude |= propertyGetter.IsVirtual && properties.Contains(property.Name);
						if (!exclude) {
							properties.Add(property.Name);
						}
					}
				}
				exclude |= excludeExplicit && member.Name.Contains(".") && !member.Name.IsReservedName();
				if (exclude)
					continue;
				result.Add(member);
			}
			return result;
		}

		private static bool ContainsOverride<T>(this IEnumerable<T> candidates, MethodInfo method) where T : MemberInfo
		{
			if (!method.IsVirtual)
				return false;
			IList<ParameterInfo> parameters = method.Parameters();
			foreach (MethodInfo candidate in candidates.Select(c => c as MethodInfo)) {
				if (candidate != null && candidate.IsVirtual && method.Name == candidate.Name) {
					if (parameters.Select(p => p.ParameterType).SequenceEqual(candidate.Parameters().Select(p => p.ParameterType))) {
						return true;
					}
				}
			}
			return false;
		}
	}
}