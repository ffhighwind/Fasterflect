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
	internal abstract class BaseEmitter
	{
		protected static readonly MethodInfo StructGetMethod =
			typeof(ValueTypeHolder).GetMethod("get_Value", BindingFlags.Public | BindingFlags.Instance);

		protected static readonly MethodInfo StructSetMethod =
			typeof(ValueTypeHolder).GetMethod("set_Value", BindingFlags.Public | BindingFlags.Instance);

		protected Type TargetType { get; set; }
		protected bool IsStatic { get; set; }
		/// <summary>
		/// The CIL should handle inner struct only when the target type is 
		/// a value type or the wrapper ValueTypeHolder type.  In addition, the call 
		/// must also be executed in the non-static context since static 
		/// context doesn't need to handle inner struct case.
		/// </summary>
		public bool ShouldHandleInnerStruct => IsTargetTypeStruct && !IsStatic;
		public bool IsTargetTypeStruct => TargetType.IsValueType;
		protected DynamicMethod Method;
		protected ILGenerator Gen;

		protected internal Delegate GetDelegate()
		{
			Method = CreateDynamicMethod();
			Gen = Method.GetILGenerator();
			return CreateDelegate();
		}

		protected internal abstract DynamicMethod CreateDynamicMethod();
		protected internal abstract Delegate CreateDelegate();

		protected internal static DynamicMethod CreateDynamicMethod(string name, Type targetType, Type returnType,
			Type[] paramTypes)
		{
			return new DynamicMethod(name, MethodAttributes.Static | MethodAttributes.Public,
				CallingConventions.Standard, returnType, paramTypes,
				targetType.IsArray ? targetType.GetElementType() : targetType,
				true);
		}

		protected void LoadInnerStructToLocal(byte localPosition)
		{	
			Gen.Emit(OpCodes.Castclass, typeof(ValueTypeHolder));
			Gen.Emit(OpCodes.Callvirt, StructGetMethod);
			Gen.Emit(OpCodes.Unbox_Any, TargetType);
			stloc_s(localPosition);
			Gen.Emit(OpCodes.Ldloca_S, localPosition);
		}

		/// <summary>
		/// Calls ILGenerator.Emit(<see cref="OpCodes.Stloc_S"/>, byte) that
		/// pops the current value from the top of the evaluation stack and stores it 
		/// in the local variable list at index (short form).
		/// </summary>
		/// <param name="index">A local variable index.</param>
		/// <seealso cref="OpCodes.Stloc_S">OpCodes.Stloc_S</seealso>
		/// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
		protected void stloc_s(byte index)
		{
			switch (index) {
				case 0:
					Gen.Emit(OpCodes.Stloc_0);
					break;
				case 1:
					Gen.Emit(OpCodes.Stloc_1);
					break;
				case 2:
					Gen.Emit(OpCodes.Stloc_2);
					break;
				case 3:
					Gen.Emit(OpCodes.Stloc_3);
					break;
				default:
					Gen.Emit(OpCodes.Stloc_S, index);
					return;
			}
		}

		/// <summary>
		/// Calls ILGenerator.Emit(<see cref="OpCodes.Ldarg"/>, short) or 
		/// ILGenerator.Emit(<see cref="OpCodes.Ldarg_S"/>, byte) that
		/// loads an argument (referenced by a specified index value) onto the stack.
		/// </summary>
		/// <param name="index">Index of the argument that is pushed onto the stack.</param>
		/// <seealso cref="OpCodes.Ldarg">OpCodes.Ldarg</seealso>
		/// <seealso cref="System.Reflection.Emit.ILGenerator.Emit(OpCode,short)">ILGenerator.Emit</seealso>
		public void ldarg(int index)
		{
			switch (index) {
				case 0:
					Gen.Emit(OpCodes.Ldarg_0);
					break;
				case 1:
					Gen.Emit(OpCodes.Ldarg_1);
					break;
				case 2:
					Gen.Emit(OpCodes.Ldarg_2);
					break;
				case 3:
					Gen.Emit(OpCodes.Ldarg_3);
					break;
				default:
					if (index <= byte.MaxValue)
						Gen.Emit(OpCodes.Ldarg_S, (byte)index);
					else if (index <= short.MaxValue)
						Gen.Emit(OpCodes.Ldarg, (short)index);
					else
						throw new ArgumentOutOfRangeException(nameof(index));
					break;
			}
		}

		protected void StoreLocalToInnerStruct(short localPosition)
		{
			StoreLocalToInnerStruct(0, localPosition); // 0: 'this'
		}

		protected void StoreLocalToInnerStruct(short argPosition, short localPosition)
		{			
			Gen.Emit(OpCodes.Ldarg, argPosition);
			Gen.Emit(OpCodes.Castclass, typeof(ValueTypeHolder));
			Gen.Emit(OpCodes.Ldloc, localPosition);
			if (TargetType.IsValueType)
				Gen.Emit(OpCodes.Box, TargetType);
			Gen.Emit(OpCodes.Callvirt, StructSetMethod);		
		}
	}
}