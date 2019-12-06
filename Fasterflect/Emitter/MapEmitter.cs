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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Fasterflect.Emitter
{
	internal class MapEmitter : BaseEmitter
	{
		protected Type SourceType => CallInfo.SourceType;
		protected override Type TargetType => CallInfo.TargetType;
		protected MemberInfo[] Sources { get; }
		protected MemberInfo[] Targets { get; }

		public MapEmitter(Type sourceType, Type targetType, string[] sourceNames, string[] targetNames)
			: this(new MapCallInfo(sourceType, targetType, StringComparer.Ordinal, sourceNames, targetNames))
		{
		}

		public MapEmitter(MapCallInfo callInfo)
		{
			CallInfo = callInfo;
			//if()
		}

		public MapCallInfo CallInfo { get; }

		/*
		private IEnumerable<KeyValuePair<MemberInfo, MemberInfo>> GetMatchingMembers()
		{
			StringComparison comparison = BindingFlags.IsSet(FasterflectFlags.IgnoreCase)
											? StringComparison.OrdinalIgnoreCase
											: StringComparison.Ordinal;
			var query = from s in sourceType.Members(sourceMemberTypes, BaseInfo.BindingFlags, names)
						from t in BaseInfo.TargetType.Members(targetMemberTypes, BaseInfo.BindingFlags, names)
						where s.Name.Equals(t.Name, comparison) &&
							  t.Type().IsAssignableFrom(s.Type()) &&
							  s.IsReadable() && t.IsWritable()
						select new { Source = s, Target = t };
			return query.ToDictionary(k => k.Source, v => v.Target);
		}
		*/

		protected internal override DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod(SourceType.Name, SourceType, null, new[] { typeof(object), typeof(object) });
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
			for (int i = 0, count = Sources.Length; i < count; ++i) {
				if (handleInnerStruct)
					Generator.ldloca_s(0).end(); // load &localStr
				else
					Generator.ldarg_1.castclass(TargetType).end(); // ((TargetType)target)
				Generator.ldarg_0.castclass(SourceType);
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