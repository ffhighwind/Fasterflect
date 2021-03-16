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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Fasterflect.Extensions;

namespace Fasterflect.Emitter
{
	internal class MapEmitter : BaseEmitter
	{
		protected Type SourceType { get; }
		protected IList<MemberInfo> Sources { get; private set; }
		protected IList<MemberInfo> Targets { get; private set; }

		public MapEmitter(Type sourceType, Type targetType, IList<MemberInfo> sources, IList<MemberInfo> targets)
		{
			TargetType = targetType;
			SourceType = sourceType;
			Sources = sources;
			Targets = targets;
		}

		public MapEmitter(MapCallInfo callInfo)
		{
			TargetType = callInfo.TargetType;
			SourceType = callInfo.SourceType;
			StringComparison comparison = (callInfo.Flags & BindingFlags.IgnoreCase) != 0
				? StringComparison.OrdinalIgnoreCase
				: StringComparison.Ordinal;
			IEnumerable<MemberInfo> sources = callInfo.SourceType.Members(MemberTypes.Field | MemberTypes.Property, callInfo.Flags, callInfo.Sources.ToArray()).Where(s => s.IsReadable());
			List<MemberInfo> targets = callInfo.TargetType.Members(MemberTypes.Field | MemberTypes.Property, callInfo.Flags, callInfo.Targets.ToArray()).Where(t => t.IsWritable()).ToList();
			Sources = new List<MemberInfo>();
			Targets = new List<MemberInfo>();
			if (callInfo.Targets.Count == 0) {
				foreach (MemberInfo source in sources) {
					foreach (MemberInfo target in targets) {
						if (source.Name.Equals(target.Name, comparison)
							&& target.Type().IsAssignableFrom(source.Type())) {
							Sources.Add(source);
							Targets.Add(target);
						}
					}
				}
			}
			else {
				foreach (MemberInfo source in sources) {
					foreach (MemberInfo target in targets) {
						if (target.Type().IsAssignableFrom(source.Type())) {
							Sources.Add(source);
							Targets.Add(target);
						}
					}
				}
			}
		}

		protected internal override DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod(TargetType.Name, TargetType, null, new[] { typeof(object), typeof(object) });
		}

		protected internal override Delegate CreateDelegate()
		{
			bool handleInnerStruct = ShouldHandleInnerStruct;
			if (handleInnerStruct) {
				Gen.Emit(OpCodes.Ldarg_1);     // load arg-1 (target)          
				Gen.DeclareLocal(TargetType);  // TargetType localStr;
				Gen.Emit(OpCodes.Castclass, typeof(ValueTypeHolder)); // (ValueTypeHolder)wrappedStruct
				Gen.Emit(OpCodes.Callvirt, StructGetMethod);          // <stack>.get_Value()
				Gen.Emit(OpCodes.Unbox_Any, TargetType); // unbox <stack>
				Gen.Emit(OpCodes.Stloc_0); // localStr = <stack>                      
			}
			for (int i = 0, count = Sources.Count; i < count; ++i) {
				if (handleInnerStruct) {
					Gen.Emit(OpCodes.Ldloca_S, (byte) 0); // load &localStr
				}
				else {
					Gen.Emit(OpCodes.Ldarg_1);
					Gen.Emit(OpCodes.Castclass, TargetType);  // ((TargetType)target)
				}
				Gen.Emit(OpCodes.Ldarg_0);
				Gen.Emit(OpCodes.Castclass, SourceType);
				GenerateGetMemberValue(Sources[i]);
				GenerateSetMemberValue(Targets[i]);
			}
			if (handleInnerStruct) {
				StoreLocalToInnerStruct(1, 0);     // ((ValueTypeHolder)this)).Value = tmpStr
			}
			Gen.Emit(OpCodes.Ret);
			return Method.CreateDelegate(typeof(ObjectMapper));
		}

		private void GenerateGetMemberValue(MemberInfo member)
		{
			if (member is FieldInfo field) {
				Gen.Emit(OpCodes.Ldfld, field);
			}
			else {
				MethodInfo method = ((PropertyInfo)member).GetGetMethod(true);
				Gen.EmitCall(OpCodes.Callvirt, method, null);
			}
		}

		private void GenerateSetMemberValue(MemberInfo member)
		{
			if (member is FieldInfo field) {
				Gen.Emit(OpCodes.Stfld, field);
			}
			else {
				MethodInfo method = ((PropertyInfo)member).GetSetMethod(true);
				Gen.EmitCall(OpCodes.Callvirt, method, null);
			}
		}
	}
}