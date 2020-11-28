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
	internal class MethodInvocationEmitter : InvocationEmitter
	{
		public MethodInvocationEmitter(MethodInfo method)
			: base(method)
		{
		}

		protected internal override DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod("invoke", TargetType, typeof(object),
				new[] { typeof(object), typeof(object).MakeArrayType() });
		}

		protected internal override Delegate CreateDelegate()
		{
			MethodInfo method = (MethodInfo) MemberInfo;
			const byte paramArrayIndex = 1;
			bool hasReturnType = method.ReturnType != typeof(void);
			byte startUsableLocalIndex = 0;
			if (HasRefParam) {
				startUsableLocalIndex = CreateLocalsForByRefParams(paramArrayIndex, method);
				// create by_ref_locals from argument array
				Gen.DeclareLocal(hasReturnType ? method.ReturnType : typeof(object)); // T result;
				GenerateInvocation(method, paramArrayIndex, (byte) (startUsableLocalIndex + 1));
				if (hasReturnType) {
					stloc_s(startUsableLocalIndex); // result = <stack>;
				}
				AssignByRefParamsToArray(paramArrayIndex); // store by_ref_locals back to argument array
			}
			else {
				Gen.DeclareLocal(hasReturnType ? method.ReturnType : typeof(object)); // T result;
				GenerateInvocation(method, paramArrayIndex, (byte) (startUsableLocalIndex + 1));
				if (hasReturnType) {
					stloc_s(startUsableLocalIndex); // result = <stack>;
				}				
			}
			if (ShouldHandleInnerStruct) {
				StoreLocalToInnerStruct((short) (startUsableLocalIndex + 1)); // ((ValueTypeHolder)this)).Value = tmpStr; 
			}
			if (hasReturnType) {				
				Gen.Emit(OpCodes.Ldloc, (short)startUsableLocalIndex); // push result;
				if (method.ReturnType.IsValueType) {
					Gen.Emit(OpCodes.Box, method.ReturnType); // box result;
				}
			}
			else {
				Gen.Emit(OpCodes.Ldnull); // load null
			}
			Gen.Emit(OpCodes.Ret);
			return Method.CreateDelegate(typeof(MethodInvoker));
		}

		protected void GenerateInvocation(MethodInfo methodInfo, byte paramArrayIndex, byte structLocalPosition)
		{
			if (!IsStatic) {
				Gen.Emit(OpCodes.Ldarg_0); // load arg-0 (this/null);
				if (ShouldHandleInnerStruct) {
					Gen.DeclareLocal(TargetType); // TargetType tmpStr;
					LoadInnerStructToLocal(structLocalPosition); // tmpStr = ((ValueTypeHolder)this)).Value;
				}
				else {
					Gen.Emit(OpCodes.Castclass, TargetType); // (TargetType)arg-0;
				}
			}
			PushParamsOrLocalsToStack(paramArrayIndex); // push arguments and by_ref_locals
			Gen.Emit(methodInfo.IsStatic || IsTargetTypeStruct ? OpCodes.Call : OpCodes.Callvirt, methodInfo); // call OR callvirt
		}
	}
}
 
 