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
using System.Reflection;
using System.Reflection.Emit;
using Fasterflect.Extensions;

namespace Fasterflect.Emitter
{
	internal class MemberSetEmitter : BaseEmitter
	{
		public MemberSetEmitter(MemberInfo memberInfo)
		{
			MemberInfo = memberInfo;
			if (memberInfo is PropertyInfo property) {
				IsStatic = (property.GetGetMethod(true) ?? property.GetSetMethod(true)).IsStatic;
			}
			else {
				FieldInfo field = (FieldInfo)memberInfo;
				IsStatic = field.IsStatic;
			}
			TargetType = MemberInfo.DeclaringType;
		}

		public MemberInfo MemberInfo { get; }

		protected internal override DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod("setter", TargetType, null, new[] { typeof(object), typeof(object) });
		}

		protected internal override Delegate CreateDelegate()
		{
			bool handleInnerStruct = ShouldHandleInnerStruct;
			if (!IsStatic) {
				Gen.Emit(OpCodes.Ldarg_0);        // load arg-0 (this)            
				if (handleInnerStruct) {
					Gen.DeclareLocal(TargetType); // TargetType tmpStr
					LoadInnerStructToLocal(0);    // tmpStr = ((ValueTypeHolder)this)).Value;          
				}
				else {
					Gen.Emit(OpCodes.Castclass, TargetType); // (TargetType)this
				}
			}
			Gen.Emit(OpCodes.Ldarg_1);               // load value-to-be-set;   

			Type type = MemberInfo.Type();
			if (type != typeof(object)) {
				Gen.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
			}
			if (MemberInfo is FieldInfo field) {
				Gen.Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field); // (this|tmpStr).field = value-to-be-set;
			}
			else {
				PropertyInfo prop = (PropertyInfo) MemberInfo;
				MethodInfo setMethod = prop.GetSetMethod(true);
				Gen.Emit(setMethod.IsStatic || IsTargetTypeStruct ? OpCodes.Call : OpCodes.Callvirt, setMethod); // (this|tmpStr).set_Prop(value-to-be-set);
			}
			if (handleInnerStruct) {
				StoreLocalToInnerStruct(0); // ((ValueTypeHolder)this)).Value = tmpStr
			}
			Gen.Emit(OpCodes.Ret);
			return Method.CreateDelegate(typeof(MemberSetter));
		}
	}
}