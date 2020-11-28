#region License

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
// The latest version of this file can be found at https://github.com/ffhighwind/fasterflect

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Fasterflect.Emitter
{
	/// <summary>
	/// Stores all necessary information to construct a dynamic method for member mapping.
	/// </summary>
	[DebuggerStepThrough]
	internal class MultiSetCallInfo
	{
		public Type TargetType { get; }
		public FasterflectFlags Flags { get; }
		public IList<string> Members { get; }

		public MultiSetCallInfo(Type targetType, IList<MemberInfo> members) : this(targetType, 
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, members.Select(m => m.Name).ToArray())
		{
		}

		public MultiSetCallInfo(Type targetType, FasterflectFlags bindingFlags, IList<string> members)
		{
			TargetType = targetType;
			Flags = bindingFlags;
			Members = members;
		}

		public override bool Equals(object obj)
		{
			if (obj is MultiSetCallInfo other) {
				if (other.Members.Count == Members.Count && other.Flags == Flags) {
					for (int i = 0, count = Members.Count; i < count; ++i) {
						if (!Members[i].Equals(other.Members[i]))
							return false;
					}
					return true;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = 688758924;
			hashCode = hashCode * -1521134295 + TargetType.GetHashCode();
			hashCode = hashCode * -1521134295 + Flags.GetHashCode();
			for (int i = 0, count = Members.Count; i < count; ++i) {
				hashCode = hashCode * -1521134295 + Members[i].GetHashCode();
			}
			return hashCode;
		}
	}
}
