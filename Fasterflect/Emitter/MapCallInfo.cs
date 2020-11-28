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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Fasterflect.Emitter
{
	/// <summary>
	/// Stores all necessary information to construct a dynamic method for member mapping.
	/// </summary>
	[DebuggerStepThrough]
	internal class MapCallInfo
	{
		public Type TargetType { get; }
		public Type SourceType { get; }
		public IList<string> Sources { get; }
		public IList<string> Targets { get; }
		public FasterflectFlags Flags { get; }

		public MapCallInfo(Type sourceType, Type targetType, IList<MemberInfo> sources, IList<MemberInfo> targets)
			: this(sourceType, targetType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, sources.Select(m => m.Name).ToArray(), targets.Select(m => m.Name).ToArray())
		{
		}

		public MapCallInfo(Type sourceType, Type targetType, FasterflectFlags flags, IList<string> sourceNames, IList<string> targetNames)
		{
			SourceType = sourceType;
			TargetType = targetType;
			Sources = sourceNames;
			Targets = targetNames ?? sourceNames;
			Flags = flags;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is MapCallInfo other)) {
				return false;
			}
			if (other.SourceType != SourceType
				|| other.TargetType != TargetType
				|| other.Sources.Count != Sources.Count
				|| other.Flags != Flags
				|| other.Targets.Count != Targets.Count) {
				return false;
			}
			for (int i = 0, count = Sources.Count; i < count; ++i) {
				if (!string.Equals(Sources[i], other.Sources[i]))
					return false;
			}
			for (int i = 0, count = Targets.Count; i < count; ++i) {
				if (!string.Equals(Targets[i], other.Targets[i]))
					return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = 167991888;
			hashCode = hashCode * -1521134295 + TargetType.GetHashCode();
			hashCode = hashCode * -1521134295 + SourceType.GetHashCode();
			hashCode = hashCode * -1521134295 + Flags.GetHashCode();
			for (int i = 0, count = Sources.Count; i < count; ++i) {
				hashCode = hashCode * -1521134295 + Sources[i].GetHashCode();
			}
			for (int i = 0, count = Targets.Count; i < count; ++i) {
				hashCode = hashCode * -1521134295 + Targets[i].GetHashCode();
			}
			return hashCode;
		}
	}
}