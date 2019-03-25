using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fasterflect.Extensions.Utilities;

namespace Fasterflect.Extensions.Objects
{
	/// <summary>
	/// Extension methods for locating, inspecting and invoking methods.
	/// </summary>
	public static class MethodExtensions
	{
		#region Method Invocation
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
		public static object CallMethod(this object obj, string name, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), null, name, Flags.StaticInstanceAnyVisibility, parameters.ToTypeArray())
				(obj, parameters);
		}

		/// <summary>
		/// Invoke generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="CallMethod(object,string,object[])"/>
		public static object CallMethod(this object obj, Type[] genericTypes, string name, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), genericTypes, name, Flags.StaticInstanceAnyVisibility, parameters.ToTypeArray())
				(obj, parameters);
		}

		/// <summary>
		/// Invokes the method specified by <paramref name="name"/> on the given <paramref name="obj"/> 
		/// using <paramref name="parameters"/> as arguments.
		/// Method parameter types are specified by <paramref name="parameterTypes"/>.
		/// </summary>
		/// <returns>The return value of the method.</returns>
		/// <remarks>If the method has no return type, <c>null</c> is returned.</remarks>
		public static object CallMethod(this object obj, string name, Type[] parameterTypes, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), null, name, Flags.StaticInstanceAnyVisibility, parameterTypes)
				(obj, parameters);
		}

		/// <summary>
		/// Invoke generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="CallMethod(object,string,Type[],object[])"/>
		public static object CallMethod(this object obj, Type[] genericTypes, string name, Type[] parameterTypes, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), genericTypes, name, Flags.StaticInstanceAnyVisibility, parameterTypes)
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
		/// <seealso cref="CallMethod(object,string,System.Type[],Fasterflect.Flags,object[])"/>
		public static object CallMethod(this object obj, string name, Flags bindingFlags, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), null, name, bindingFlags, parameters.ToTypeArray())
				(obj, parameters);
		}

		/// <summary>
		/// Invoke generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="CallMethod(object,string,Flags,object[])"/>
		public static object CallMethod(this object obj, Type[] genericTypes, string name, Flags bindingFlags, params object[] parameters)
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
		public static object CallMethod(this object obj, string name, Type[] parameterTypes, Flags bindingFlags, params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), null, name, bindingFlags, parameterTypes)
				(obj, parameters);
		}

		/// <summary>
		/// Invoke generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="CallMethod(object,string,Type[],Flags,object[])"/>
		public static object CallMethod(this object obj, Type[] genericTypes, string name, Type[] parameterTypes, Flags bindingFlags,
			params object[] parameters)
		{
			return Fasterflect.Extensions.MethodExtensions.DelegateForCallMethod(obj.GetTypeAdjusted(), genericTypes, name, bindingFlags, parameterTypes)
				(obj, parameters);
		}
		#endregion
	}
}
