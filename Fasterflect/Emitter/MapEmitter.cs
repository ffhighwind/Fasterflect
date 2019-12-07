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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Fasterflect.Emitter
{
	internal class MapEmitter : BaseEmitter
	{
		protected Type OtherType { get; }
		protected override Type TargetType { get; }
		protected IList<MemberInfo> Sources { get; }
		protected IList<MemberInfo> Targets { get; }

		public MapEmitter(Type sourceType, Type otherType, IList<MemberInfo> sources, IList<MemberInfo> targets)
		{
			TargetType = sourceType;
			OtherType = otherType;
			Sources = sources;
			Targets = targets == null || targets.Count == 0 ? sources : targets;
		}

		public MapEmitter(MapCallInfo callInfo)
		{
			TargetType = callInfo.SourceType;
			OtherType = callInfo.TargetType;
			if (callInfo.Sources.Count == 0) {
				IList<MemberInfo> sources = ReflectLookup.Members(TargetType, MemberTypes.Field | MemberTypes.Property, callInfo.Flags);
				Targets = ReflectLookup.Members(OtherType, MemberTypes.Field | MemberTypes.Property, callInfo.Flags, sources.Select(m => m.Name).ToArray());
				Sources = ReflectLookup.MembersExact(TargetType, callInfo.Flags, Targets.Select(m => m.Name).ToArray());
			}
			else {
				Sources = ReflectLookup.MembersExact(TargetType, callInfo.Flags, callInfo.Sources.ToArray());
				Targets = callInfo.Targets == null ? Sources : ReflectLookup.MembersExact(OtherType, callInfo.Flags, callInfo.Targets.ToArray());
			}
		}

		protected internal override DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod(TargetType.Name, TargetType, null, new[] { typeof(object), typeof(object) });
		}

		protected internal override Delegate CreateDelegate()
		{
			bool handleInnerStruct = ShouldHandleInnerStruct;
			if (handleInnerStruct) {
				Generator.ldarg_1.end();                     // load arg-1 (target)
				Generator.DeclareLocal(TargetType); // TargetType localStr;
				Generator
					.castclass(typeof(ValueTypeHolder)) // (ValueTypeHolder)wrappedStruct
					.callvirt(StructGetMethod) // <stack>.get_Value()
					.unbox_any(TargetType) // unbox <stack>
					.stloc(0); // localStr = <stack>
			}
			for (int i = 0, count = Sources.Count; i < count; ++i) {
				if (handleInnerStruct)
					Generator.ldloca_s(0).end(); // load &localStr
				else
					Generator.ldarg_1.castclass(TargetType).end(); // ((TargetType)target)
				Generator.ldarg_0.castclass(OtherType);
				GenerateGetMemberValue(Sources[i]);
				GenerateSetMemberValue(Targets[i]);
			}
			if (handleInnerStruct) {
				StoreLocalToInnerStruct(1, 0);     // ((ValueTypeHolder)this)).Value = tmpStr
			}
			Generator.ret();
			return Method.CreateDelegate(typeof(ObjectMapper));
		}

		private void GenerateGetMemberValue(MemberInfo member)
		{
			if (member is FieldInfo field) {
				Generator.ldfld(field);
			}
			else {
				MethodInfo method = ((PropertyInfo)member).GetGetMethod(true);
				Generator.callvirt(method, null);
			}
		}

		private void GenerateSetMemberValue(MemberInfo member)
		{
			if (member is FieldInfo field) {
				Generator.stfld(field);
			}
			else {
				MethodInfo method = ((PropertyInfo)member).GetSetMethod(true);
				Generator.callvirt(method, null);
			}
		}
	}
}