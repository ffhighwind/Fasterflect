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

namespace Fasterflect.Emitter
{
	internal class MemberGetEmitter : BaseEmitter
	{
		public MemberGetEmitter(MemberInfo memberInfo)
		{
			MemberInfo = memberInfo;
			if (memberInfo is PropertyInfo property) {
				IsStatic = (property.GetGetMethod(true) ?? property.GetSetMethod(true)).IsStatic;
			}
			else {
				FieldInfo field = (FieldInfo) memberInfo;
				IsStatic = field.IsStatic;
			}
			TargetType = MemberInfo.DeclaringType;
		}

		protected MemberInfo MemberInfo { get; }

		protected internal override DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod("getter", TargetType, typeof(object), new[] { typeof(object) });
		}

		protected internal override Delegate CreateDelegate()
		{
			bool handleInnerStruct = ShouldHandleInnerStruct;
			if (handleInnerStruct) {
				Gen.Emit(OpCodes.Ldarg_0); // load arg-0 (this)
				Gen.DeclareLocal(TargetType); // TargetType tmpStr
				LoadInnerStructToLocal(0);         // tmpStr = ((ValueTypeHolder)this)).Value
				Gen.DeclareLocal(typeof(object)); // object result;
			}
			else if (!IsStatic) {
				Gen.Emit(OpCodes.Ldarg_0); // load arg-0 (this)
				Gen.Emit(OpCodes.Castclass, TargetType); // (TargetType)this
			}
			if (MemberInfo is FieldInfo field) {
				if (field.DeclaringType.IsEnum) // special enum handling as ldsfld does not support enums
					Gen.Emit(OpCodes.Ldc_I4, (int) field.GetValue(field.DeclaringType));
				else
					Gen.Emit(field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field);  // (this|tmpStr).field OR TargetType.field
				if (field.FieldType.IsValueType)
					Gen.Emit(OpCodes.Box, field.FieldType); // (object)<stack>
			}
			else {
				PropertyInfo prop = (PropertyInfo) MemberInfo;
				MethodInfo getMethod = prop.GetGetMethod(true);
				Gen.Emit(getMethod.IsStatic || IsTargetTypeStruct ? OpCodes.Call : OpCodes.Callvirt, getMethod); // (this|tmpStr).prop OR TargetType.prop
				if (prop.PropertyType.IsValueType)
					Gen.Emit(OpCodes.Box, prop.PropertyType); // (object)<stack>
			}
			if (handleInnerStruct) {
				Gen.Emit(OpCodes.Stloc_1);  // resultLocal = <stack>
				StoreLocalToInnerStruct(0); // ((ValueTypeHolder)this)).Value = tmpStr
				Gen.Emit(OpCodes.Ldloc_1);  // push resultLocal
			}
			Gen.Emit(OpCodes.Ret);
			return Method.CreateDelegate(typeof(MemberGetter));
		}
	}
}