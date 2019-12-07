using System;
using System.Collections.Generic;

namespace Fasterflect.Emitter
{
	internal class CtorInfo
	{
		public CtorInfo(Type targetType, FasterflectFlags bindingFlags, IList<Type> parameterTypes)
		{
			TargetType = targetType; ;
			Flags = bindingFlags;
			ParamTypes = parameterTypes;
		}

		public Type TargetType { get; }
		public FasterflectFlags Flags { get; }
		public IList<Type> ParamTypes { get; }

		public override bool Equals(object obj)
		{
			if (!(obj is CtorInfo other)
				|| !TargetType.Equals(other.TargetType)
				|| Flags != other.Flags
				|| ParamTypes.Count != other.ParamTypes.Count)
				return false;
			for (int i = 0, count = ParamTypes.Count; i < count; ++i) {
				if (ParamTypes[i].Equals(other.ParamTypes[i])) {
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = 854595903;
			hashCode = hashCode * -1521134295 + TargetType.GetHashCode();
			hashCode = hashCode * -1521134295 + Flags.GetHashCode();
			for (int i = 0, count = ParamTypes.Count; i < count; ++i) {
				hashCode = hashCode * -1521134295 + ParamTypes[i].GetHashCode();
			}
			return hashCode;
		}
	}
}
