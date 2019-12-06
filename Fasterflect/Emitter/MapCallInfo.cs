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
		public string[] Sources { get; }
		public string[] Targets { get; }
		public StringComparer Comparer { get; }

		public MapCallInfo(Type sourceType, Type targetType, StringComparer comparer, string[] sourceNames, string[] targetNames)
		{
			SourceType = sourceType;
			TargetType = targetType;
			Sources = sourceNames;
			Targets = targetNames == null || targetNames == sourceNames ? Constants.EmptyStringArray : targetNames;
			Comparer = comparer;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is MapCallInfo other)) {
				return false;
			}
			if (other.SourceType != SourceType
				|| other.TargetType != TargetType
				|| other.Sources.Length != Sources.Length) {
				return false;
			}
			for (int i = 0, count = Sources.Length; i < count; ++i) {
				if (!Comparer.Equals(Sources[i], other.Sources[i]))
					return false;
			}
			if(Targets == null) {
				for (int i = 0, count = Targets.Length; i < count; ++i) {
					if (!Comparer.Equals(Targets[i], other.Targets[i]))
						return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = 167991888;
			hashCode = hashCode * -1521134295 + TargetType.GetHashCode();
			hashCode = hashCode * -1521134295 + SourceType.GetHashCode();
			for (int i = 0, count = Sources.Length; i < count; ++i) {
				hashCode = hashCode * -1521134295 + Sources[i].GetHashCode();
			}
			for (int i = 0, count = Targets.Length; i < count; ++i) {
				hashCode = hashCode * -1521134295 + Targets[i].GetHashCode();
			}
			return hashCode;
		}
	}
}