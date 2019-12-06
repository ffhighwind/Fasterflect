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

		protected abstract Type TargetType { get; }
		protected virtual bool IsStatic => false;
		/// <summary>
		/// The CIL should handle inner struct only when the target type is 
		/// a value type or the wrapper ValueTypeHolder type.  In addition, the call 
		/// must also be executed in the non-static context since static 
		/// context doesn't need to handle inner struct case.
		/// </summary>
		public bool ShouldHandleInnerStruct => IsTargetTypeStruct && !IsStatic;
		public bool IsTargetTypeStruct => TargetType.IsValueType;
		protected DynamicMethod Method;
		protected EmitHelper Generator;

		protected internal Delegate GetDelegate()
		{
			Method = CreateDynamicMethod();
			Generator = new EmitHelper(Method.GetILGenerator());
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
			Generator
				.castclass(typeof(ValueTypeHolder)) // (ValueTypeHolder)wrappedStruct
				.callvirt(StructGetMethod) // <stack>.get_Value()
				.unbox_any(TargetType) // unbox <stack>
				.stloc(localPosition) // localStr = <stack>
				.ldloca_s(localPosition); // load &localStr
		}

		protected void StoreLocalToInnerStruct(byte localPosition)
		{
			StoreLocalToInnerStruct(0, localPosition); // 0: 'this'
		}

		protected void StoreLocalToInnerStruct(byte argPosition, byte localPosition)
		{
			Generator
				.ldarg(argPosition)
				.castclass(typeof(ValueTypeHolder)) // wrappedStruct = (ValueTypeHolder)this
				.ldloc(localPosition) // load localStr
				.boxIfValueType(TargetType) // box <stack>
				.callvirt(StructSetMethod); // wrappedStruct.set_Value(<stack>)
		}
	}
}