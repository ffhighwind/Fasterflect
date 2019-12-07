#region License

// Copyright © 2010 Buu Nguyen, Morten Mertner
// Copyright © 2018 Wesley Hamilton
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
using System.Reflection;

namespace Fasterflect
{
	internal static class Constants
	{
		/// <summary>
		/// new Type[] { typeof(object) }
		/// </summary>
		public static readonly Type[] ArrayOfObjectType = new Type[] { typeof(object) };
#if NET35 || NET45
		/// <summary>
		/// new object[0]
		/// </summary>
		public static readonly object[] EmptyObjectArray = new object[0];
		/// <summary>
		/// new string[0]
		/// </summary>
		public static readonly string[] EmptyStringArray = new string[0];
		/// <summary>
		/// new PropertyInfo[0]
		/// </summary>
		public static readonly PropertyInfo[] EmptyPropertyInfoArray = new PropertyInfo[0];
		/// <summary>
		/// new MemberInfo[0]
		/// </summary>
		public static readonly MemberInfo[] EmptyMemberInfoArray = new MemberInfo[0];
		/// <summary>
		/// new FieldInfo[0]
		/// </summary>
		public static readonly FieldInfo[] EmptyFieldInfoArray = new FieldInfo[0];
		/// <summary>
		/// new MethodInfo[0]
		/// </summary>
		public static readonly MethodInfo[] EmptyMethodInfoArray = new MethodInfo[0];
#else
		/// <summary>
		/// <see cref="Array.Empty{T}"/>
		/// </summary>
		public static object[] EmptyObjectArray => Array.Empty<object>();
		/// <summary>
		/// <see cref="Array.Empty{T}"/>
		/// </summary>
		public static string[] EmptyStringArray => Array.Empty<string>();
		/// <summary>
		/// <see cref="Array.Empty{T}"/>
		/// </summary>
		public static PropertyInfo[] EmptyPropertyInfoArray => Array.Empty<PropertyInfo>();
		/// <summary>
		/// <see cref="Array.Empty{T}"/>
		/// </summary>
		public static MemberInfo[] EmptyMemberInfoArray => Array.Empty<MemberInfo>();
		/// <summary>
		/// <see cref="Array.Empty{T}"/>
		/// </summary>
		public static FieldInfo[] EmptyFieldInfoArray => Array.Empty<FieldInfo>();
		/// <summary>
		/// <see cref="Array.Empty{T}"/>
		/// </summary>
		public static MethodInfo[] EmptyMethodInfoArray => Array.Empty<MethodInfo>();
#endif
	}
}
