using System;
using System.Reflection;

namespace Fasterflect.Emitter
{
	internal class MemberCallInfo
	{
		public MemberCallInfo(Type type, string name, MemberTypes memberTypes, FasterflectFlags bindingFlags)
		{
			TargetType = type;
			BindingFlags = bindingFlags;
			MemberName = name;
			MemberType = memberTypes;
		}

		public Type TargetType { get; }
		public FasterflectFlags BindingFlags { get; }
		public string MemberName { get; }
		public MemberTypes MemberType { get; set; }

		public override bool Equals(object obj)
		{
			return obj is MemberCallInfo other &&
				other != null &&
				TargetType.Equals(other.TargetType) &&
				BindingFlags ==  other.BindingFlags &&
				MemberName == other.MemberName &&
				(other.MemberType & MemberType) != 0;
		}

		public override int GetHashCode()
		{
			int hashCode = 1298308050;
			hashCode = hashCode * -1521134295 + TargetType.GetHashCode();
			hashCode = hashCode * -1521134295 + BindingFlags.GetHashCode();
			hashCode = hashCode * -1521134295 + MemberName.GetHashCode();
			return hashCode;
		}
	}
}
