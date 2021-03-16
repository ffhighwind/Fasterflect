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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Fasterflect.Emitter
{
	internal abstract class InvocationEmitter : BaseEmitter
	{
		protected InvocationEmitter(Type type, ConstructorInfo ctor = null)
		{
			if (ctor == null) {
				TargetType = type;
				IsStatic = false;
				IsGeneric = false;
				ParamTypes = Type.EmptyTypes;
			}
			else {
				TargetType = ctor.DeclaringType;
				MemberInfo = ctor;
				IsStatic = ctor.IsStatic;
				IsGeneric = ctor.IsGenericMethod;
				ParamTypes = ctor.GetParameters().ToTypeArray();
				MemberInfo = ctor;
			}
		}

		protected InvocationEmitter(MethodInfo method)
		{
			TargetType = method.DeclaringType;
			MemberInfo = method;
			IsStatic = method.IsStatic;
			IsGeneric = method.IsGenericMethod;
			ParamTypes = method.GetParameters().ToTypeArray();
		}

		protected MemberInfo MemberInfo { get; }
		protected bool IsGeneric { get; }
		protected Type[] ParamTypes { get; }
		protected bool HasNoParam => ParamTypes == Type.EmptyTypes;
		protected bool HasRefParam => ParamTypes.Any(t => t.IsByRef);

		protected byte CreateLocalsForByRefParams(byte paramArrayIndex, MethodBase invocationInfo)
		{
			byte numberOfByRefParams = 0;
			ParameterInfo[] parameters = invocationInfo.GetParameters();
			for (int i = 0, count = ParamTypes.Length; i < count; ++i) {
				Type paramType = ParamTypes[i];
				if (paramType.IsByRef) {
					Type type = paramType.GetElementType();
					Gen.DeclareLocal(type);
					if (!parameters[i].IsOut) // no initialization necessary is 'out' parameter
					{
						Gen.Emit(OpCodes.Ldarg, (short) paramArrayIndex);
						Gen.Emit(OpCodes.Ldc_I4, i);
						Gen.Emit(OpCodes.Ldelem_Ref);
						if (type == typeof(object)) {
							Gen.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
						}
						stloc_s(numberOfByRefParams);
					}
					numberOfByRefParams++;
				}
			}
			return numberOfByRefParams;
		}

		protected void AssignByRefParamsToArray(int paramArrayIndex)
		{
			byte currentByRefParam = 0;
			for (int i = 0, count = ParamTypes.Length; i < count; ++i) {
				Type paramType = ParamTypes[i];
				if (paramType.IsByRef) {
					
					Gen.Emit(OpCodes.Ldarg, paramArrayIndex);
					Gen.Emit(OpCodes.Ldc_I4, i);
					Gen.Emit(OpCodes.Ldloc, (short) currentByRefParam);
					currentByRefParam++;
					Type type = paramType.GetElementType();
					if (type.IsValueType)
						Gen.Emit(OpCodes.Box, type);
					Gen.Emit(OpCodes.Stelem_Ref);
				}
			}
		}

		protected void PushParamsOrLocalsToStack(int paramArrayIndex)
		{
			byte currentByRefParam = 0;
			for (int i = 0, count = ParamTypes.Length; i < count; ++i) {
				Type paramType = ParamTypes[i];
				if (paramType.IsByRef) {
					Gen.Emit(OpCodes.Ldloca_S, currentByRefParam);
					currentByRefParam++;
				}
				else {					
					ldarg(paramArrayIndex);
					Gen.Emit(OpCodes.Ldc_I4, i);
					Gen.Emit(OpCodes.Ldelem_Ref);
					if (paramType != typeof(object)) {
						Gen.Emit(paramType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, paramType);
					}					
				}
			}
		}
	}
}