using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fasterflect.Emitter
{
	internal class ShallowClonerCallInfo : CallInfo
	{
		public readonly MemberInfo[] members;

		public ShallowClonerCallInfo(Type targetType)
			: this(targetType, FasterflectFlags.InstanceAnyVisibility)
		{
		}

		public ShallowClonerCallInfo(Type targetType, FasterflectFlags flags)
			: base(targetType, null, FasterflectFlags.InstanceAnyVisibility, MemberTypes.Custom, "Fasterflect_ShallowCloner", null, null, false)
		{
			List<MemberInfo> memberList = new List<MemberInfo>();
			IList<FieldInfo> fields = MemberFilter.Filter<FieldInfo>(targetType.GetRuntimeFields().ToList(), BindingFlags);
			IList<PropertyInfo> properties = MemberFilter.Filter<PropertyInfo>(targetType.GetRuntimeProperties().ToList(), BindingFlags);

			for (int i = 0; i < fields.Count; i++) {
				if (!fields[i].IsInitOnly)
					memberList.Add(fields[i]);
			}
			for (int i = 0; i < properties.Count; i++) {
				if (properties[i].CanRead && properties[i].CanWrite)
					memberList.Add(properties[i]);
			}
			members = memberList.ToArray();
		}

		public override bool Equals(object obj)
		{
			return obj is ShallowClonerCallInfo other 
				&& other.TargetType == TargetType 
				&& other.BindingFlags == BindingFlags;
		}

		public override int GetHashCode()
		{
			int hash = 56721417 * TargetType.GetHashCode();
			hash = hash * 297891 ^ BindingFlags.GetHashCode();
			return hash;
		}
	}
}
