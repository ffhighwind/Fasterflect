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
using System.Text;
namespace Fasterflect.Extensions
{
	/// <summary>
	/// Extension methods for locating, inspecting and invoking methods.
	/// </summary>
	public static partial class MethodExtensions
	{
		/// <summary>
		/// Invokes the method specified by <paramref name="name"/> on the given <paramref name="obj"/> 
		/// using <paramref name="parameters"/> as arguments. 
		/// Leave <paramref name="parameters"/> empty if the method has no arguments.
		/// </summary>
		/// <returns>The return value of the method.</returns>
		/// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
		/// <remarks>
		/// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
		/// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
		/// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
		/// </remarks>
		/// <seealso cref="CallMethod(object,string,System.Type[],object[])"/>
		internal static object CallMethod(this object obj, string name, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), null, name, FasterflectFlags.StaticInstanceAnyVisibility, parameters.ToTypeArray())
				(obj, parameters);
		}

		/// <summary>
		/// Invoke generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="CallMethod(object,string,object[])"/>
		internal static object CallMethod(this object obj, Type[] genericTypes, string name, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), genericTypes, name, FasterflectFlags.StaticInstanceAnyVisibility, parameters.ToTypeArray())
				(obj, parameters);
		}

		/// <summary>
		/// Invokes the method specified by <paramref name="name"/> on the given <paramref name="obj"/> 
		/// using <paramref name="parameters"/> as arguments.
		/// Method parameter types are specified by <paramref name="parameterTypes"/>.
		/// </summary>
		/// <returns>The return value of the method.</returns>
		/// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
		internal static object CallMethod(this object obj, string name, Type[] parameterTypes, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), null, name, FasterflectFlags.StaticInstanceAnyVisibility, parameterTypes)
				(obj, parameters);
		}

		/// <summary>
		/// Invoke generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="CallMethod(object,string,Type[],object[])"/>
		internal static object CallMethod(this object obj, Type[] genericTypes, string name, Type[] parameterTypes, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), genericTypes, name, FasterflectFlags.StaticInstanceAnyVisibility, parameterTypes)
				(obj, parameters);
		}

		/// <summary>
		/// Invokes the method specified by <paramref name="name"/> on the given <paramref name="obj"/>
		/// matching <paramref name="bindingFlags"/> using <paramref name="parameters"/> as arguments.
		/// Leave <paramref name="parameters"/> empty if the method has no argument.
		/// </summary>
		/// <returns>The return value of the method.</returns>
		/// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
		/// <remarks>
		/// All elements of <paramref name="parameters"/> must not be <c>null</c>.  Otherwise, 
		/// <see cref="NullReferenceException"/> is thrown.  If you are not sure as to whether
		/// any element is <c>null</c> or not, use the overload that accepts <c>paramTypes</c> array.
		/// </remarks>
		/// <seealso cref="CallMethod(object,string,System.Type[],Fasterflect.FasterflectFlags,object[])"/>
		internal static object CallMethod(this object obj, string name, FasterflectFlags bindingFlags, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), null, name, bindingFlags, parameters.ToTypeArray())
				(obj, parameters);
		}

		/// <summary>
		/// Invoke generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="CallMethod(object,string,FasterflectFlags,object[])"/>
		internal static object CallMethod(this object obj, Type[] genericTypes, string name, FasterflectFlags bindingFlags, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), genericTypes, name, bindingFlags)
				(obj, parameters);
		}

		/// <summary>
		/// Invokes a method specified by <paramref name="name"/> on the given <paramref name="obj"/> 
		/// matching <paramref name="bindingFlags"/> using <paramref name="parameters"/> as arguments.
		/// Method parameter types are specified by <paramref name="parameterTypes"/>.
		/// </summary>
		/// <returns>The return value of the method.</returns>
		/// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
		internal static object CallMethod(this object obj, string name, Type[] parameterTypes, FasterflectFlags bindingFlags, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), null, name, bindingFlags, parameterTypes)
				(obj, parameters);
		}

		/// <summary>
		/// Invoke generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="CallMethod(object,string,Type[],FasterflectFlags,object[])"/>
		internal static object CallMethod(this object obj, Type[] genericTypes, string name, Type[] parameterTypes, FasterflectFlags bindingFlags,
			params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), genericTypes, name, bindingFlags, parameterTypes)
				(obj, parameters);
		}
	}
}
