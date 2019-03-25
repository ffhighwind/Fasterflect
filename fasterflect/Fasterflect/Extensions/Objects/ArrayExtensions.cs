using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fasterflect.Extensions.Objects
{
	/// <summary>
	/// Extension methods for working with arrays.
	/// </summary>
	public static class ArrayExtensions
	{
		#region Array Access
		/// <summary>
		/// Sets <paramref name="value"/> to the element at position <paramref name="index"/> of <paramref name="array"/>.
		/// </summary>
		/// <returns><paramref name="array"/>.</returns>
		public static object SetElement(this object array, long index, object value)
		{
			((Array) array).SetValue(value, index);
			return array;
		}

		/// <summary>
		/// Gets the element at position <paramref name="index"/> of <paramref name="array"/>.
		/// </summary>
		public static object GetElement(this object array, long index)
		{
			return ((Array) array).GetValue(index);
		}
		#endregion
	}
}