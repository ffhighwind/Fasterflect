using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fasterflect
{
	/// <summary>
	/// Container class for permanently caching generic <see cref="ConstructorInvoker"/> delegates.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public static class Reflect<T>
	{
		/// <summary>
		/// Delegate for an empty constructor of the given type if one exists, otherwise <see langword="null"/>.
		/// </summary>
		public static readonly ConstructorInvoker New = null;

		/// <summary>
		/// Delegate for an empty constructor for the given type if one exists, otherwise it calls
		/// <see cref="System.Runtime.Serialization.FormatterServices.GetUninitializedObject"/>.
		/// </summary>
		public static readonly ConstructorInvoker Create = null;

		static Reflect()
		{
			New = Reflect.Constructor(typeof(T));
			Create = New ?? GetUninitializedObject;
		}

		private static object GetUninitializedObject(params object[] parameters)
		{
			return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
		}
	}
}
