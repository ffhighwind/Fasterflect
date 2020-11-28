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
	internal class CtorInvocationEmitter : InvocationEmitter
	{
		public CtorInvocationEmitter(Type type, ConstructorInfo ctor)
			: base(type, ctor)
		{
		}

		protected internal override DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod("ctor", TargetType, typeof(object), Constants.ArrayOfObjectType);
		}

		protected internal override Delegate CreateDelegate()
		{
			if (IsTargetTypeStruct && HasNoParam) { // no-arg struct needs special initialization
				Gen.DeclareLocal(TargetType); // TargetType tmp
				Gen.Emit(OpCodes.Ldloca_S, (byte)0);  // &tmp
				Gen.Emit(OpCodes.Initobj, TargetType); // init_obj(&tmp)
				Gen.Emit(OpCodes.Ldloc_0); // load tmp
			}
			else if (TargetType.IsArray) {
				Gen.Emit(OpCodes.Ldarg_0); // load args[] (method arguments)
				Gen.Emit(OpCodes.Ldc_I4_0); // load 0
				Gen.Emit(OpCodes.Ldelem_Ref); // load args[0] (length)
				Gen.Emit(OpCodes.Unbox_Any, typeof(int)); // unbox stack
				Gen.Emit(OpCodes.Newarr, TargetType.GetElementType()); // new T[args[0]]
			}
			else {
				ConstructorInfo ctorInfo = (ConstructorInfo) MemberInfo;
				byte startUsableLocalIndex = 0;
				if (HasRefParam) {
					startUsableLocalIndex = CreateLocalsForByRefParams(0, ctorInfo); // create by_ref_locals from argument array
					Gen.DeclareLocal(TargetType); // TargetType tmp;
				}
				PushParamsOrLocalsToStack(0); // push arguments and by_ref_locals
				Gen.Emit(OpCodes.Newobj, ctorInfo); // ctor (<stack>)
				if (HasRefParam) {
					stloc_s(startUsableLocalIndex); // tmp = <stack>;
					AssignByRefParamsToArray(0);    // store by_ref_locals back to argument array
					Gen.Emit(OpCodes.Ldloc, (short)startUsableLocalIndex); // tmp
				}
			}
			if (TargetType.IsValueType)
				Gen.Emit(OpCodes.Box, TargetType);
			Gen.Emit(OpCodes.Ret); // return (box)<stack>;
			return Method.CreateDelegate(typeof(ConstructorInvoker));
		}
	}
}
 
 