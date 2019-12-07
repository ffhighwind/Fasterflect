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
			if(memberInfo is PropertyInfo property) {
				IsStatic = (property.GetGetMethod(true) ?? property.GetSetMethod(true)).IsStatic;
			}
			else {
				FieldInfo field = (FieldInfo)memberInfo;
				IsStatic = field.IsStatic;
			}
		}

		public MemberGetEmitter(FieldInfo field)
		{
			MemberInfo = field;
			IsStatic = field.IsStatic;
		}

		public MemberGetEmitter(PropertyInfo property)
		{
			MemberInfo = property;
			IsStatic = (property.GetGetMethod(true) ?? property.GetSetMethod(true)).IsStatic;
		}

		protected MemberInfo MemberInfo { get; }
		protected override Type TargetType => MemberInfo.DeclaringType;
		protected override bool IsStatic { get; }

		protected internal override DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod("getter", TargetType, typeof(object), new[] { typeof(object) });
		}

		protected internal override Delegate CreateDelegate()
		{
			bool handleInnerStruct = ShouldHandleInnerStruct;
			if (handleInnerStruct) {
				Generator.ldarg_0                         // load arg-0 (this)
						 .DeclareLocal(TargetType);       // TargetType tmpStr
				LoadInnerStructToLocal(0);                // tmpStr = ((ValueTypeHolder)this)).Value
				Generator.DeclareLocal(typeof(object));   // object result;
			}
			else if (!IsStatic) {
				Generator.ldarg_0                    // load arg-0 (this)
						 .castclass(TargetType);     // (TargetType)this
			}
			if (MemberInfo is FieldInfo field) {
				if (field.DeclaringType.IsEnum) { // special enum handling as ldsfld does not support enums
					Generator.ldc_i4((int)field.GetValue(field.DeclaringType)).boxIfValueType(field.FieldType);
				}
				else {
					Generator.ldfld(field.IsStatic, field)        // (this|tmpStr).field OR TargetType.field
							 .boxIfValueType(field.FieldType);    // (object)<stack>
				}
			}
			else {
				PropertyInfo prop = (PropertyInfo)MemberInfo;
				MethodInfo getMethod = prop.GetGetMethod(true);
				Generator.call(getMethod.IsStatic || IsTargetTypeStruct, getMethod)  // (this|tmpStr).prop OR TargetType.prop
						 .boxIfValueType(prop.PropertyType);                         // (object)<stack>
			}
			if (handleInnerStruct) {
				Generator.stloc_1.end();      // resultLocal = <stack>
				StoreLocalToInnerStruct(0);   // ((ValueTypeHolder)this)).Value = tmpStr
				Generator.ldloc_1.end();      // push resultLocal
			}
			Generator.ret();
			return Method.CreateDelegate(typeof(MemberGetter));
		}
	}
}