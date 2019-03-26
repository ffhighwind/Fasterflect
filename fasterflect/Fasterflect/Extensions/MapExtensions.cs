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
using System.Reflection;
using Fasterflect.Emitter;

namespace Fasterflect.Extensions
{
	/// <summary>
	/// Extension methods for mapping (copying) members from one object instance to another.
	/// </summary>
	public static partial class MapExtensions
	{
		#region Map
		/// <summary>
		/// Creates a delegate that can map values from fields and properties on the source object to fields and properties with the 
		/// same name on the target object.
		/// </summary>
		/// <param name="sourceType">The type of the source object.</param>
		/// <param name="targetType">The type of the target object.</param>
		/// <param name="names">The optional list of member names against which to filter the members that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.PartialNameMatch"/> to 
		/// filter members by substring and <see href="Flags.IgnoreCase"/> to ignore case.</param>
		public static ObjectMapper DelegateForMap(this Type sourceType, Type targetType, params string[] names)
		{
			return DelegateForMap(sourceType, targetType, FasterflectFlags.InstanceAnyVisibility, names);
		}

		/// <summary>
		/// Creates a delegate that can map values from fields and properties on the source object to fields and properties with the 
		/// same name on the target object.
		/// </summary>
		/// <param name="sourceType">The type of the source object.</param>
		/// <param name="targetType">The type of the target object.</param>
		/// <param name="bindingFlags">The <see href="Flags"/> used to define the scope when locating members.</param>
		/// <param name="names">The optional list of member names against which to filter the members that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.PartialNameMatch"/> to 
		/// filter members by substring and <see href="Flags.IgnoreCase"/> to ignore case.</param>
		public static ObjectMapper DelegateForMap(this Type sourceType, Type targetType, FasterflectFlags bindingFlags, params string[] names)
		{
			const MemberTypes memberTypes = MemberTypes.Field | MemberTypes.Property;
			return DelegateForMap(sourceType, targetType, memberTypes, memberTypes, bindingFlags, names);
		}

		/// <summary>
		/// Creates a delegate that can map values from fields and properties on the source object to fields and properties with the 
		/// same name on the target object.
		/// </summary>
		/// <param name="sourceType">The type of the source object.</param>
		/// <param name="targetType">The type of the target object.</param>
		/// <param name="sourceTypes">The member types (Fields, Properties or both) to include on the source.</param>
		/// <param name="targetTypes">The member types (Fields, Properties or both) to include on the target.</param>
		/// <param name="bindingFlags">The <see href="Flags"/> used to define the scope when locating members. If
		/// <paramref name="sourceTypes"/> is different from <paramref name="targetTypes"/> the flag value
		/// <see cref="FasterflectFlags.IgnoreCase"/> will automatically be applied.</param>
		/// <param name="names">The optional list of member names against which to filter the members that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.PartialNameMatch"/> to 
		/// filter members by substring and <see href="Flags.IgnoreCase"/> to ignore case.</param>
		public static ObjectMapper DelegateForMap(this Type sourceType, Type targetType, MemberTypes sourceTypes, MemberTypes targetTypes,
							   FasterflectFlags bindingFlags, params string[] names)
		{
			MapEmitter emitter = new MapEmitter(sourceType, targetType, sourceTypes, targetTypes, bindingFlags, names);
			return (ObjectMapper) emitter.GetDelegate();
		}
		#endregion
	}
}