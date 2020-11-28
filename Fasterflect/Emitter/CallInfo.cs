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
// The latest version of this file can be found at https://github.com/ffhighwind/fasterflect

#endregion

using System;
using System.Diagnostics;
using System.Reflection;

namespace Fasterflect.Emitter
{
	[DebuggerStepThrough]
	internal class CallInfo
	{
		public CallInfo(Type targetType, string name, FasterflectFlags bindingFlags, Type[] genericTypes, Type[] parameterTypes)
		{
			TargetType = targetType;
			Name = name;
			BindingFlags = bindingFlags;
			ParameterTypes = parameterTypes;
			GenericTypes = genericTypes;
		}

		public Type TargetType { get; }
		public string Name { get; }
		public FasterflectFlags BindingFlags { get; }
		public Type[] ParameterTypes { get; }
		public Type[] GenericTypes { get; }

		public override bool Equals(object obj)
		{
			if (obj is CallInfo other &&
				Name == other.Name &&
				TargetType.Equals(other.TargetType) &&
				ParameterTypes.Length == other.ParameterTypes.Length &&
				GenericTypes.Length == other.GenericTypes.Length &&
				BindingFlags == other.BindingFlags) {
				for (int i = 0, count = ParameterTypes.Length; i < count; ++i) {
					if (!ParameterTypes[i].Equals(other.ParameterTypes[i])) {
						return false;
					}
				}
				for (int i = 0, count = GenericTypes.Length; i < count; ++i) {
					if (!ParameterTypes[i].Equals(other.GenericTypes[i])) {
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = -937504810;
			hashCode = hashCode * -1521134295 + TargetType.GetHashCode();
			hashCode = hashCode * -1521134295 + BindingFlags.GetHashCode();
			hashCode = hashCode * -1521134295 + Name.GetHashCode();
			for (int i = 0, count = ParameterTypes.Length; i < count; ++i) {
				hashCode = hashCode * -1521134295 + ParameterTypes[i].GetHashCode();
			}
			for (int i = 0, count = GenericTypes.Length; i < count; ++i) {
				hashCode = hashCode * -1521134295 + GenericTypes[i].GetHashCode();
			}
			return hashCode;
		}
	}
}
