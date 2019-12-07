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
using Fasterflect.Extensions;

namespace Fasterflect.Emitter
{
	internal class MapEmitter : BaseEmitter
	{
		protected Type SourceType { get; }
		protected override Type TargetType { get; }
		protected IList<MemberInfo> Sources { get; private set; }
		protected IList<MemberInfo> Targets { get; private set; }

		public MapEmitter(Type sourceType, Type targetType, IList<MemberInfo> sources, IList<MemberInfo> targets)
		{
			TargetType = targetType;
			SourceType = sourceType;
			Sources = sources;
			Targets = targets;
		}

		public MapEmitter(MapCallInfo callInfo)
		{
			TargetType = callInfo.TargetType;
			SourceType = callInfo.SourceType;
			StringComparison comparison = callInfo.Flags.IsSet(FasterflectFlags.IgnoreCase)
											? StringComparison.OrdinalIgnoreCase
											: StringComparison.Ordinal;
			IEnumerable<MemberInfo> sources = callInfo.SourceType.Members(MemberTypes.Field | MemberTypes.Property, callInfo.Flags, callInfo.Sources.ToArray()).Where(s => s.IsReadable());
			List<MemberInfo> targets = callInfo.TargetType.Members(MemberTypes.Field | MemberTypes.Property, callInfo.Flags, callInfo.Targets.ToArray()).Where(t => t.IsWritable()).ToList();
			Sources = new List<MemberInfo>();
			Targets = new List<MemberInfo>();
			if (callInfo.Targets.Count == 0) {
				foreach (MemberInfo source in sources) {
					foreach (MemberInfo target in targets) {
						if (source.Name.Equals(target.Name, comparison)
							&& target.Type().IsAssignableFrom(source.Type())) {
							Sources.Add(source);
							Targets.Add(target);
						}
					}
				}
			}
			else {
				foreach (MemberInfo source in sources) {
					foreach (MemberInfo target in targets) {
						if (target.Type().IsAssignableFrom(source.Type())) {
							Sources.Add(source);
							Targets.Add(target);
						}
					}
				}
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
				Generator.ldarg_1.end();                // load arg-1 (target)
				Generator.DeclareLocal(TargetType);     // TargetType localStr;
				Generator
					.castclass(typeof(ValueTypeHolder))   // (ValueTypeHolder)wrappedStruct
					.callvirt(StructGetMethod)            // <stack>.get_Value()
					.unbox_any(TargetType)                // unbox <stack>
					.stloc(0);                            // localStr = <stack>
			}
			for (int i = 0, count = Sources.Count; i < count; ++i) {
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