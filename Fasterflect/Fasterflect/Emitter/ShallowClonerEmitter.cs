using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Fasterflect.Extensions;

namespace Fasterflect.Emitter
{
	internal class ShallowClonerEmitter<T> : BaseEmitter
		where T : new()
	{
		public ShallowClonerEmitter()
			: base(new ShallowClonerCallInfo(typeof(T)))
		{
		}

		public ShallowClonerEmitter(FasterflectFlags flags)
			: base(new ShallowClonerCallInfo(typeof(T), flags))
		{
		}

		protected internal override DynamicMethod CreateDynamicMethod()
		{
			return CreateDynamicMethod("shallowcloner", CallInfo.TargetType, null,
				new[] { typeof(T), typeof(T) });
		}

		protected internal override Delegate CreateDelegate()
		{
			ShallowClonerCallInfo callinfo = (ShallowClonerCallInfo) CallInfo;
			MemberInfo[] members = callinfo.members;
			if (CallInfo.IsTargetTypeStruct && CallInfo.HasNoParam) // no-arg struct needs special initialization
			{
				Generator.DeclareLocal(CallInfo.TargetType);     // TargetType tmp
				Generator.ldloca_s(0)                            // &tmp
						 .initobj(CallInfo.TargetType)           // init_obj(&tmp)
						 .ldloc_0.end();                         // load tmp
			}
			else {
				ConstructorInfo ctorInfo = LookupUtils.GetConstructor(CallInfo);
				Generator.newobj(ctorInfo);       // ctor (<stack>)
			}

			//bool handleInnerStruct = CallInfo.ShouldHandleInnerStruct;
			for (int i = 0, count = members.Length; i < count; i++) {
				MemberInfo method = members[i];
				if (method == null)
					continue;
				if (method is FieldInfo field) {
					if (i != count - 1)
						Generator.dup.end();
					Generator.ldarg_1.ldc_i4(i).ldelem_ref.end();
					Generator.CastFromObject(field.FieldType);
					Generator.stfld(field.IsStatic, field);        // (this|tmpStr).field = value-to-be-set;
				}
				else {
					PropertyInfo property = (PropertyInfo) method;
					MethodInfo setMethod = property.GetSetMethod(true);
					if (i != count - 1)
						Generator.dup.end();
					Generator.ldarg_1.ldc_i4(i).ldelem_ref.end();
					Generator.CastFromObject(property.PropertyType);
					Generator.call(setMethod.IsStatic || CallInfo.IsTargetTypeStruct, setMethod); // (this|tmpStr).set_Prop(value-to-be-set);
				}
			}
			//if (handleInnerStruct) {
			//	StoreLocalToInnerStruct(0); // ((ValueTypeHolder)this)).Value = tmpStr
			//}
			Generator.ret();
			return Method.CreateDelegate(typeof(Func<Type, Type>));
		}
	}
}
