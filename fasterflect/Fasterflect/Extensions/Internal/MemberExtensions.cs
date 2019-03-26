#region License
// Copyright 2010 Buu Nguyen, Morten Mertner
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
using System.Text;
using Fasterflect.Extensions;

namespace Fasterflect.Extensions
{
	/// <summary>
	/// Extension methods for locating and accessing fields or properties, for situations where
	/// you do not care which it is.
	/// </summary>
	public static partial class MemberExtensions
	{
		#region TryGetValue
		/// <summary>
		/// Gets the first (public or non-public) instance member with the given <paramref name="name"/> on the given
		/// <paramref name="obj"/> object. Returns the value of the member if a match was found and null otherwise.
		/// </summary>
		/// <remarks>
		/// When using this method it is not possible to distinguish between a missing member and a member whose value is null.
		/// </remarks>
		/// <param name="obj">The source object on which to find the member</param>
		/// <param name="name">The name of the member whose value should be retrieved</param>
		/// <returns>The value of the member or null if no member was found</returns>
		internal static object TryGetValue(this object obj, string name)
		{
			return TryGetValue(obj, name, FasterflectFlags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Gets the first member with the given <paramref name="name"/> on the given <paramref name="obj"/> object.
		/// Returns the value of the member if a match was found and null otherwise.
		/// Use the <paramref name="bindingFlags"/> parameter to limit the scope of the search.
		/// </summary>
		/// <remarks>
		/// When using this method it is not possible to distinguish between a missing member and a member whose value is null.
		/// </remarks>
		/// <param name="obj">The source object on which to find the member</param>
		/// <param name="name">The name of the member whose value should be retrieved</param>
		/// <param name="bindingFlags">A combination of Flags that define the scope of the search</param>
		/// <returns>The value of the member or null if no member was found</returns>
		internal static object TryGetValue(this object obj, string name, FasterflectFlags bindingFlags)
		{
			Type type = obj.GetType();
			MemberInfo info = type.Member(name, bindingFlags);
			if (info == null) {
				return null;
			}
			bool valid = info is FieldInfo || info is PropertyInfo;
			return valid ? info.Get(obj) : null;
		}
		#endregion

		#region TrySetValue
		/// <summary>
		/// Sets the first (public or non-public) instance member with the given <paramref name="name"/> on the 
		/// given <paramref name="obj"/> object to the supplied <paramref name="value"/>. Returns true 
		/// if a value was assigned to a member and false otherwise.
		/// </summary>
		/// <param name="obj">The source object on which to find the member</param>
		/// <param name="name">The name of the member whose value should be retrieved</param>
		/// <param name="value">The value that should be assigned to the member</param>
		/// <returns>True if the value was assigned to a member and false otherwise</returns>
		internal static bool TrySetValue(this object obj, string name, object value)
		{
			return TrySetValue(obj, name, value, FasterflectFlags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Sets the first member with the given <paramref name="name"/> on the given <paramref name="obj"/> object
		/// to the supplied <paramref name="value"/>. Returns true if a value was assigned to a member and false otherwise.
		/// Use the <paramref name="bindingFlags"/> parameter to limit the scope of the search.
		/// </summary>
		/// <param name="obj">The source object on which to find the member</param>
		/// <param name="name">The name of the member whose value should be retrieved</param>
		/// <param name="value">The value that should be assigned to the member</param>
		/// <param name="bindingFlags">A combination of Flags that define the scope of the search</param>
		/// <returns>True if the value was assigned to a member and false otherwise</returns>
		internal static bool TrySetValue(this object obj, string name, object value, FasterflectFlags bindingFlags)
		{
			Type type = obj.GetType();
			PropertyInfo property = type.Property(name, bindingFlags);
			if (property != null && property.CanWrite) {
				property.Set(obj, value);
				return true;
			}
			FieldInfo field = type.Field(name, bindingFlags);
			if (field != null) {
				field.Set(obj, value);
				return true;
			}
			return false;
		}
		#endregion
	}
}
