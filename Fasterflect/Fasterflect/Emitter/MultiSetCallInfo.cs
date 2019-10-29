using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fasterflect.Emitter
{
	internal class MultiSetCallInfo : CallInfo
	{
		public readonly MemberInfo[] members;

		public MultiSetCallInfo(Type targetType, FasterflectFlags flags, string[] names)
			: base(targetType, null, flags, System.Reflection.MemberTypes.Custom, "Fasterflect_MultiSet", null, null, false)
		{
			bool useProperties = flags.IsSet(System.Reflection.BindingFlags.SetProperty);
			bool useFields = flags.IsSet(System.Reflection.BindingFlags.SetField);
			if (!useProperties && !useFields) {
				useProperties = true;
				useFields = true;
			}
			List<MemberInfo> memberList = new List<MemberInfo>(names.Length);
			IList<FieldInfo> fields = useFields ? MemberFilter.Filter<FieldInfo>(targetType.GetRuntimeFields().ToList(), flags) : new List<FieldInfo>();
			IList<PropertyInfo> properties = useProperties ? MemberFilter.Filter<PropertyInfo>(targetType.GetRuntimeProperties().ToList(), flags) : new List<PropertyInfo>();
			string[] arr = new string[1];
			for (int i = 0, count = names.Length; i < count; i++) {
				arr[0] = names[i];
				MemberInfo minfo = null;
				FieldInfo finfo = fields.Filter(flags, arr).FirstOrDefault();
				if (finfo != null) {
					if (!finfo.IsInitOnly) {
						minfo = finfo;
					}
				}
				else {
					PropertyInfo pinfo = properties.Filter(flags, arr).FirstOrDefault();
					minfo = pinfo;
					if (pinfo != null && !pinfo.CanWrite) {
						minfo = null;
					}
				}
				memberList.Add(minfo);
			}
			for (int i = memberList.Count - 1; i >= 0; i--) {
				if (memberList[i] != null) {
					break;
				}
				memberList.RemoveAt(i);
			}
			members = memberList.ToArray();
		}

		public override bool Equals(object obj)
		{
			if (obj is MultiSetCallInfo other) {
				if (other.members.Length == members.Length) {
					for (int i = 0, count = other.members.Length; i < members.Length; i++) {
						if (other.members[i] != members[i])
							return false;
					}
					return true;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hash = base.GetHashCode();
			for (int i = 0, count = members.Length; i < count; i++) {
				hash += members[i].GetHashCode();
			}
			return hash;
		}
	}
}
