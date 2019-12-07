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

using Fasterflect.Extensions;
using System;
using System.Reflection;
using System.Reflection.Emit;

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
		}

		public MemberSetEmitter(PropertyInfo property)
		{
			MemberInfo = property;
			IsStatic = (property.GetGetMethod(true) ?? property.GetSetMethod(true)).IsStatic;
		}

		public MemberSetEmitter(FieldInfo field)
		{
			MemberInfo = field;
			IsStatic = field.IsStatic;
		}

		public MemberInfo MemberInfo { get; }

		protected override Type TargetType => MemberInfo.DeclaringType;
		protected override bool IsStatic { get; }

		protected internal override DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod("setter", TargetType, null, new[] { typeof(object), typeof(object) });
		}

		protected internal override Delegate CreateDelegate()
		{
			bool handleInnerStruct = ShouldHandleInnerStruct;
			if (IsStatic) {
				Generator.ldarg_1.end();                   // load value-to-be-set
			}
			else {
				Generator.ldarg_0.end();                   // load arg-0 (this)
				if (handleInnerStruct) {
					Generator.DeclareLocal(TargetType);    // TargetType tmpStr
					LoadInnerStructToLocal(0);             // tmpStr = ((ValueTypeHolder)this)).Value;
					Generator.ldarg_1.end();               // load value-to-be-set;
				}
				else {
					Generator.castclass(TargetType)   // (TargetType)this
					         .ldarg_1.end();          // load value-to-be-set;
				}
			}
			Generator.CastFromObject(MemberInfo.Type());   // unbox | cast value-to-be-set
			if (MemberInfo is FieldInfo field) {
				Generator.stfld(field.IsStatic, field);    // (this|tmpStr).field = value-to-be-set;
			}
			else {
				PropertyInfo prop = (PropertyInfo) MemberInfo;
				MethodInfo setMethod = prop.GetSetMethod(true);
				Generator.call(setMethod.IsStatic || IsTargetTypeStruct, setMethod);   // (this|tmpStr).set_Prop(value-to-be-set);
			}
			if (handleInnerStruct) {
				StoreLocalToInnerStruct(0); // ((ValueTypeHolder)this)).Value = tmpStr
			}
			Generator.ret();
			return Method.CreateDelegate(typeof(MemberSetter));
		}
	}
}