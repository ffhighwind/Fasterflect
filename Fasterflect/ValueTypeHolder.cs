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

namespace Fasterflect
{
	/// <summary>
	/// A wrapper for value type.  Must be used in order for Fasterflect to 
	/// work with value type such as struct.
	/// </summary>
	public class ValueTypeHolder
	{
		/// <summary>
		/// Creates a wrapper for <paramref name="value"/> value type.  The wrapper
		/// can then be used with Fasterflect.
		/// </summary>
		/// <param name="value">The value type to be wrapped.  
		/// Must be a derivative of <see cref="ValueType"/>.</param>
		public ValueTypeHolder(object value)
		{
			Value = (ValueType)value;
		}

		/// <summary>
		/// The actual struct wrapped by this instance.
		/// </summary>
		public ValueType Value { get; set; }

		/// <summary>
		/// Casts a <see cref="ValueType"/> to a <see cref="ValueTypeHolder"/>.
		/// </summary>
		/// <param name="o"></param>
		public static implicit operator ValueTypeHolder(ValueType o)
		{
			return new ValueTypeHolder(o);
		}

		/// <summary>
		/// Casts a <see cref="ValueTypeHolder"/> to a <see cref="ValueType"/>.
		/// </summary>
		/// <param name="o"></param>
		public static implicit operator ValueType(ValueTypeHolder o)
		{
			return o.Value;
		}
	}
}