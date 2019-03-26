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
using Fasterflect.Extensions.Services;

namespace Fasterflect.Extensions.Internal
{
	/// <summary>
	/// Container class for TryCreateInstanceWithValues and TryCallMethodWithValues extensions.
	/// </summary>
	public static partial class TryInvokeWithValuesExtensions
	{
		/// <summary>
		/// Obtains the list of methods for <paramref name="obj"/> using the supplied parameter values
		/// and invokes the best match. This overload requires that the supplied <paramref name="parameterValues"/> 
		/// are all used in the order in which they are supplied. Parameter values can be null.
		/// 
		/// This method is very liberal and attempts to convert values that are not otherwise
		/// considered compatible, such as between strings and enums or numbers, Guids and byte[16], etc.
		/// You should carefully test any usage to ensure correct program behavior.
		/// </summary>
		/// <param name="obj">The object whose method is to be invoked.</param>
		/// <param name="methodName">The name of the method to be invoked.</param>
		/// <param name="parameterValues">The values to use when invoking the method.</param>
		/// <returns>The result of the invocation.</returns>
		internal static object TryCallMethodWithValues(this object obj, string methodName, params object[] parameterValues)
		{
			return TryCallMethodWithValues(obj, null, methodName,
				obj is Type ? FasterflectFlags.StaticAnyVisibility : FasterflectFlags.InstanceAnyVisibility, parameterValues);
		}

		/// <summary>
		/// Obtains the list of methods for <paramref name="obj"/> using the supplied parameter values
		/// and invokes the best match. This overload requires that the supplied <paramref name="parameterValues"/> 
		/// are all used in the order in which they are supplied. Parameter values can be null.
		/// 
		/// This method is very liberal and attempts to convert values that are not otherwise
		/// considered compatible, such as between strings and enums or numbers, Guids and byte[16], etc.
		/// You should carefully test any usage to ensure correct program behavior.
		/// 
		/// If the default conversion rule doesn't do what you want, you can supply a custom converter.
		/// If it is null, default conversion rule is used.
		/// </summary>
		/// <param name="obj">The object whose method is to be invoked.</param>
		/// <param name="converter">The converter delegate used to perform user-defined conversion.</param>
		/// <param name="methodName">The name of the method to be invoked.</param>
		/// <param name="flags">Binding flags for look up methods.</param>
		/// <param name="parameterValues">The values to use when invoking the method.</param>
		/// <returns>The result of the invocation.</returns>
		internal static object TryCallMethodWithValues(this object obj, ParameterConverter converter, string methodName, BindingFlags flags, params object[] parameterValues)
		{
			return TryCallMethodWithValues(obj, converter, methodName, Type.EmptyTypes, flags, parameterValues);
		}


		/// <summary>
		/// Obtains the list of methods for <paramref name="obj"/> using the supplied parameter values
		/// and invokes the best match. This overload requires that the supplied <paramref name="parameterValues"/> 
		/// are all used in the order in which they are supplied. Parameter values can be null.
		/// 
		/// This method is very liberal and attempts to convert values that are not otherwise
		/// considered compatible, such as between strings and enums or numbers, Guids and byte[16], etc.
		/// You should carefully test any usage to ensure correct program behavior.
		/// 
		/// If the default conversion rule doesn't do what you want, you can supply a custom converter.
		/// If it is null, default conversion rule is used.
		/// </summary>
		/// <param name="obj">The object whose method is to be invoked.</param>
		/// <param name="converter">The converter delegate used to perform user-defined conversion.</param>
		/// <param name="methodName">The name of the method to be invoked.</param>
		/// <param name="genericTypes">The type parameter types of the method if it's a generic method.</param>
		/// <param name="flags">Binding flags for look up methods.</param>
		/// <param name="parameterValues">The values to use when invoking the method.</param>
		/// <returns>The result of the invocation.</returns>
		internal static object TryCallMethodWithValues(this object obj, ParameterConverter converter, string methodName,
			Type[] genericTypes, BindingFlags flags, params object[] parameterValues)
		{
			Type type = obj is Type ? (Type) obj : obj.GetType();
			IEnumerable<MethodInfo> methods = type.Methods(genericTypes, null, flags, methodName)
							  .Select(m => m.IsGenericMethodDefinition ? m.MakeGeneric(genericTypes) : m);
			try {
				return Fasterflect.Extensions.Services.TryInvokeWithValuesExtensions.TryCall(converter, methods.Cast<MethodBase>(), obj, parameterValues);
			}
			catch (MissingMemberException) {
				object[] values = parameterValues ?? new object[0];
				throw new MissingMethodException(string.Format("Unable to locate a matching method {0} on type {1} for parameters: {2}",
																 methodName, type.Name,
																 string.Join(", ", values.Select(v => v == null ? "null" : v.ToString()).ToArray())));
			}
		}
	}
}
