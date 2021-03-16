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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Fasterflect.Extensions;

namespace Fasterflect.Emitter
{
	internal class MultiSetEmitter : BaseEmitter
	{
		public MultiSetEmitter(Type targetType, IList<MemberInfo> members)
		{
			TargetType = targetType;
			Members = members;
			for (int i = 0, count = Members.Count; i < count; ++i) {
				bool isstatic = Members[i].IsStatic();
				if (!isstatic) {
					IsStatic = false;
					return;
				}
			}
			IsStatic = true;
		}

		public MultiSetEmitter(MultiSetCallInfo callInfo) 
			: this(callInfo.TargetType, ReflectLookup.MembersExact(callInfo.TargetType, callInfo.Flags, callInfo.Members.ToArray()))
		{
		}

		protected IList<MemberInfo> Members { get; }

		protected internal override DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod("multisetter", TargetType, null,
				new[] { typeof(object), typeof(object).MakeArrayType() });
		}

		protected internal override Delegate CreateDelegate()
		{
			bool handleInnerStruct = ShouldHandleInnerStruct;
			if (!IsStatic) {
				Gen.Emit(OpCodes.Ldarg_0);  // load arg-0 (this)
				if (handleInnerStruct) {
					Gen.DeclareLocal(TargetType);  // TargetType tmpStr
					Gen.Emit(OpCodes.Castclass, typeof(ValueTypeHolder)); // (ValueTypeHolder)wrappedStruct
					Gen.Emit(OpCodes.Callvirt, StructGetMethod);          // <stack>.get_Value()
					Gen.Emit(OpCodes.Unbox_Any, TargetType);              // unbox <stack>
					Gen.Emit(OpCodes.Stloc_0);                            // localStr = <stack>
				}
				else {
					Gen.Emit(OpCodes.Castclass, TargetType); // (TargetType)this
				}
			}
			IList<MemberInfo> members = Members;
			for (int i = 0, count = members.Count; i < count; ++i) {
				MemberInfo method = members[i];

				if (method == null)
					continue;
				if (method is FieldInfo field) {
					if (!field.IsStatic) {
						if (handleInnerStruct) {
							Gen.Emit(OpCodes.Ldloca_S, (byte) 0);
						}
						else if (i != count - 1) {
							Gen.Emit(OpCodes.Dup);
						}
					}
					Gen.Emit(OpCodes.Ldarg_1);
					Gen.Emit(OpCodes.Ldc_I4, i);
					Gen.Emit(OpCodes.Ldelem_Ref);
					if (field.FieldType != typeof(object)) {
						Gen.Emit(field.FieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, field.FieldType);
					}
					Gen.Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field); // (this|tmpStr).field = value-to-be-set
				}
				else {
					PropertyInfo property = (PropertyInfo) method;
					MethodInfo setMethod = property.GetSetMethod(true) ?? throw new MemberAccessException(TargetType.FullName + "." + property.Name);
					if (!setMethod.IsStatic) {
						if (handleInnerStruct) {
							Gen.Emit(OpCodes.Ldloca_S, (byte) 0);
						}
						else if (i != count - 1) {
							Gen.Emit(OpCodes.Dup);
						}
					}
					Gen.Emit(OpCodes.Ldarg_1);
					Gen.Emit(OpCodes.Ldc_I4, i);
					Gen.Emit(OpCodes.Ldelem_Ref);
					if (property.PropertyType != typeof(object)) {
						Gen.Emit(property.PropertyType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, property.PropertyType);
					}
					Gen.Emit(setMethod.IsStatic || IsTargetTypeStruct ? OpCodes.Call : OpCodes.Callvirt, setMethod);  // (this|tmpStr).set_Prop(value-to-be-set)
				}
			}
			if (handleInnerStruct) {
				StoreLocalToInnerStruct(0);   // ((ValueTypeHolder)this)).Value = tmpStr
			}
			//Gen.Emit(OpCodes.Ldnull); // load null
			Gen.Emit(OpCodes.Ret);
			return Method.CreateDelegate(typeof(MultiSetter));
		}
	}
}
