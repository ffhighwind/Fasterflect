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

#endregion License

using System;
using System.Reflection.Emit;

namespace Fasterflect.Emitter
{
	internal class ArrayGetEmitter : BaseEmitter
	{
		public ArrayGetEmitter(Type targetType)
		{
			TargetType = targetType;
		}

		protected internal override DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod("get_ArrayIndex", TargetType, typeof(object), new[] { typeof(object), typeof(int) });
		}

		protected internal override Delegate CreateDelegate()
		{
			Type elementType = TargetType.GetElementType();
			Gen.Emit(OpCodes.Ldarg_0);
			Gen.Emit(OpCodes.Castclass, TargetType);
			Gen.Emit(OpCodes.Ldarg_1);
			Gen.Emit(OpCodes.Ldelem, elementType);
			if (TargetType.IsValueType)
				Gen.Emit(OpCodes.Box, TargetType);
			Gen.Emit(OpCodes.Ret);
			return Method.CreateDelegate(typeof(ArrayElementGetter));
		}
	}
}
 