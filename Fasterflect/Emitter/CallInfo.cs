#region License
// Copyright © 2010 Buu Nguyen, Morten Mertner
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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Fasterflect.Emitter
{
	/// <summary>
	/// Stores all necessary information to construct a dynamic method.
	/// </summary>
	[DebuggerStepThrough]
	internal class CallInfo
	{
		public Type TargetType => MemberInfo.DeclaringType;
		public MemberTypes MemberTypes => MemberInfo.MemberType;
		public string Name => MemberInfo.Name;

		public Type[] ParamTypes {
			get {
				if (MemberInfo is MethodInfo method) {
					return method.GetParameters().ToTypeArray();
				}
				return ((ConstructorInfo)MemberInfo).GetParameters().ToTypeArray();
			}
		}
		public MemberInfo MemberInfo { get; }
		public bool IsStatic { get; }
		public bool IsGeneric { get; }

		public CallInfo(MethodInfo methodInfo)
		{
			MemberInfo = methodInfo;
			IsStatic = methodInfo.IsStatic;
			IsGeneric = methodInfo.IsGenericMethod;
		}

		public CallInfo(ConstructorInfo constructorInfo)
		{
			MemberInfo = constructorInfo;
			IsStatic = constructorInfo.IsStatic;
			IsGeneric = constructorInfo.IsGenericMethod;
		}

		public bool HasNoParam => ParamTypes == Type.EmptyTypes;

		public bool HasRefParam => ParamTypes.Any(t => t.IsByRef);

		public override bool Equals(object obj)
		{
			return !(obj is CallInfo other)
				? false
				: TargetType.Equals(other.TargetType)
				&& (ReferenceEquals(MemberInfo, other.MemberInfo) || MemberInfo.Equals(other.MemberInfo));
		}

		public override int GetHashCode()
		{
			int hashCode = -1437685449;
			hashCode = hashCode * -1521134295 + TargetType.GetHashCode();
			hashCode = hashCode * -1521134295 + MemberInfo.GetHashCode();
			return hashCode;
		}
	}
}