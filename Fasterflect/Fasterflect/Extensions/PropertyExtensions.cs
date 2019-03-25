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
using System.Linq.Expressions;
using System.Reflection;
using Fasterflect.Emitter;
using Fasterflect.Extensions.Utilities;

namespace Fasterflect.Extensions
{
	/// <summary>
	/// Extension methods for locating and accessing properties.
	/// </summary>
	public static class PropertyExtensions
	{
		#region Property Access
		/// <summary>
		/// Creates a delegate which can set the value of the property specified by <param name="name"/>
		/// on the given <param name="type"/>.
		/// </summary>
		public static MemberSetter DelegateForSetPropertyValue(this Type type, string name)
		{
			return Reflect.PropertySetter(type, name);
		}

		/// <summary>
		/// Creates a delegate which can get the value of the property specified by <param name="name"/>
		/// on the given <param name="type"/>.
		/// </summary>
		public static MemberGetter DelegateForGetPropertyValue(this Type type, string name)
		{
			return Reflect.PropertyGetter(type, name);
		}

		/// <summary>
		/// Creates a delegate which can set the value of the property specified by <param name="name"/>
		/// matching <param name="bindingFlags"/> on the given <param name="type"/>.
		/// </summary>
		public static MemberSetter DelegateForSetPropertyValue(this Type type, string name, Flags bindingFlags)
		{
			return Reflect.PropertySetter(type, name, bindingFlags);
		}

		/// <summary>
		/// Creates a delegate which can get the value of the property specified by <param name="name"/>
		/// matching <param name="bindingFlags"/> on the given <param name="type"/>.
		/// </summary>
		public static MemberGetter DelegateForGetPropertyValue(this Type type, string name, Flags bindingFlags)
		{
			return Reflect.PropertyGetter(type, name, bindingFlags);
		}
		#endregion

		#region Indexer Access
		/// <summary>
		/// Creates a delegate which can set an indexer
		/// </summary>
		/// <param name="type">The type which the indexer belongs to.</param>
		/// <param name="parameterTypes">The types of the indexer parameters (must be in the right order), plus
		/// the type of the indexer.</param>
		/// <returns>A delegate which can set an indexer.</returns>
		/// <example>
		/// If the indexer is of type <c>string</c> and accepts one parameter of type <c>int</c>, this 
		/// method should be invoked as follow:
		/// <code>
		/// MethodInvoker invoker = type.DelegateForSetIndexer(new Type[]{typeof(int), typeof(string)});
		/// </code>
		/// </example>
		public static MethodInvoker DelegateForSetIndexer(this Type type, params Type[] parameterTypes)
		{
			return Reflect.SetIndexer(type, Flags.InstanceAnyVisibility, parameterTypes);
		}

		/// <summary>
		/// Creates a delegate which can get the value of an indexer.
		/// </summary>
		/// <param name="type">The type which the indexer belongs to.</param>
		/// <param name="parameterTypes">The types of the indexer parameters (must be in the right order).</param>
		/// <returns>The delegate which can get the value of an indexer.</returns>
		public static MethodInvoker DelegateForGetIndexer(this Type type, params Type[] parameterTypes)
		{
			return Reflect.GetIndexer(type, Flags.InstanceAnyVisibility, parameterTypes);
		}

		/// <summary>
		/// Creates a delegate which can set an indexer matching <paramref name="bindingFlags"/>.
		/// </summary>
		/// <param name="type">The type which the indexer belongs to.</param>
		/// <param name="bindingFlags">The binding flags used to lookup the indexer.</param>
		/// <param name="parameterTypes">The types of the indexer parameters (must be in the right order), plus
		/// the type of the indexer.</param>
		/// <returns>A delegate which can set an indexer.</returns>
		/// <example>
		/// If the indexer is of type <c>string</c> and accepts one parameter of type <c>int</c>, this 
		/// method should be invoked as follow:
		/// <code>
		/// MethodInvoker invoker = type.DelegateForSetIndexer(new Type[]{typeof(int), typeof(string)});
		/// </code>
		/// </example>
		public static MethodInvoker DelegateForSetIndexer(this Type type, Flags bindingFlags, params Type[] parameterTypes)
		{
			return Reflect.SetIndexer(type, bindingFlags, parameterTypes);
		}

		/// <summary>
		/// Creates a delegate which can get the value of an indexer matching <paramref name="bindingFlags"/>.
		/// </summary>
		/// <param name="type">The type which the indexer belongs to.</param>
		/// <param name="bindingFlags">The binding flags used to lookup the indexer.</param>
		/// <param name="parameterTypes">The types of the indexer parameters (must be in the right order).</param>
		/// <returns>The delegate which can get the value of an indexer.</returns>
		public static MethodInvoker DelegateForGetIndexer(this Type type, Flags bindingFlags, params Type[] parameterTypes)
		{
			return Reflect.GetIndexer(type, bindingFlags, parameterTypes);
		}
		#endregion

		#region Property Lookup (Single)
		/// <summary>
		/// Gets the property identified by <paramref name="name"/> on the given <paramref name="type"/>. This method 
		/// searches for public and non-public instance properties on both the type itself and all parent classes.
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo Property(this Type type, string name)
		{
			return Reflect.Lookup.Property(type, name);
		}

		/// <summary>
		/// Gets the property identified by <paramref name="name"/> on the given <paramref name="type"/>. 
		/// Use the <paramref name="bindingFlags"/> parameter to define the scope of the search.
		/// </summary>
		/// <returns>A single PropertyInfo instance of the first found match or null if no match was found.</returns>
		public static PropertyInfo Property(this Type type, string name, Flags bindingFlags)
		{
			return Reflect.Lookup.Property(type, name, bindingFlags);
		}
		#endregion

		#region Property Lookup (Multiple)
		/// <summary>
		/// Gets all public and non-public instance properties on the given <paramref name="type"/>,
		/// including properties defined on base types. The result can optionally be filtered by specifying
		/// a list of property names to include using the <paramref name="names"/> parameter.
		/// </summary>
		/// <returns>A list of matching instance properties on the type.</returns>
		/// <param name="type">The type whose public properties are to be retrieved.</param>
		/// <param name="names">A list of names of properties to be retrieved. If this is <c>null</c>, 
		/// all properties are returned.</param>
		/// <returns>A list of all public properties on the type filted by <paramref name="names"/>.
		/// This value will never be null.</returns>
		public static IList<PropertyInfo> Properties(this Type type, params string[] names)
		{
			return Reflect.Lookup.Properties(type, names);
		}

		/// <summary>
		/// Gets all properties on the given <paramref name="type"/> that match the specified <paramref name="bindingFlags"/>,
		/// including properties defined on base types.
		/// </summary>
		/// <returns>A list of all matching properties on the type. This value will never be null.</returns>
		public static IList<PropertyInfo> Properties(this Type type, Flags bindingFlags, params string[] names)
		{
			return Reflect.Lookup.Properties(type, bindingFlags, names);
		}

		private static IList<PropertyInfo> GetProperties(Type type, Flags bindingFlags)
		{
			return Reflect.Lookup.Properties(type, bindingFlags);
		}
		#endregion
	}
}