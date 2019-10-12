using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fasterflect
{
	/// <summary>
	/// Container class for permanently caching a generic <see cref="ConstructorInvoker"/> delegate.
	/// </summary>
	/// <typeparam name="T">The type of object</typeparam>
	public static class Reflect<T>
	{
		/// <summary>
		/// Delegate for an empty constructor for the given type if one exists, otherwise it calls
		/// <see cref="System.Runtime.Serialization.FormatterServices.GetUninitializedObject"/>.
		/// </summary>
		public static readonly ConstructorInvoker New = null;

		/// <summary>
		/// Returns true if <typeparamref name="T"/> has a default constructor, otherwise false.
		/// </summary>
		public static bool HasDefaultConstructor => New == GetUninitializedObject;

		static Reflect()
		{
			New = Reflect.Constructor(typeof(T)) ?? GetUninitializedObject;
		}

		private static object GetUninitializedObject(params object[] parameters)
		{
			return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
		}
	}
}
