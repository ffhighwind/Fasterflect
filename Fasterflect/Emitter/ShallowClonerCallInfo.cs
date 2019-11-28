#region License

// Copyright 2010 Buu Nguyen, Morten Mertner
// Copyright 2018 Wesley Hamilton
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
// The latest version of this file can be found at https://github.com/ffhighwind/fasterflect

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fasterflect.Emitter
{
	internal class ShallowClonerCallInfo : CallInfo
	{
		public readonly MemberInfo[] members;

		public ShallowClonerCallInfo(Type targetType)
			: this(targetType, FasterflectFlags.InstanceAnyVisibility)
		{
		}

		public ShallowClonerCallInfo(Type targetType, FasterflectFlags flags)
			: base(targetType, null, FasterflectFlags.InstanceAnyVisibility, MemberTypes.Custom, "Fasterflect_ShallowCloner", null, null, false)
		{
			List<MemberInfo> memberList = new List<MemberInfo>();
			IList<FieldInfo> fields = MemberFilter.Filter<FieldInfo>(targetType.GetRuntimeFields().ToList(), BindingFlags);
			IList<PropertyInfo> properties = MemberFilter.Filter<PropertyInfo>(targetType.GetRuntimeProperties().ToList(), BindingFlags);

			for (int i = 0; i < fields.Count; i++) {
				if (!fields[i].IsInitOnly)
					memberList.Add(fields[i]);
			}
			for (int i = 0; i < properties.Count; i++) {
				if (properties[i].CanRead && properties[i].CanWrite)
					memberList.Add(properties[i]);
			}
			members = memberList.ToArray();
		}

		public override bool Equals(object obj)
		{
			return obj is ShallowClonerCallInfo other 
				&& other.TargetType == TargetType 
				&& other.BindingFlags == BindingFlags;
		}

		public override int GetHashCode()
		{
			int hash = 56721417 * TargetType.GetHashCode();
			hash = hash * 297891 ^ BindingFlags.GetHashCode();
			return hash;
		}
	}
}
