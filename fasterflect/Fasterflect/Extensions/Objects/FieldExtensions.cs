using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fasterflect.Extensions.Utilities;

namespace Fasterflect.Extensions.Objects
{
	/// <summary>
	/// Extension methods for locating and accessing fields.
	/// </summary>
	public static class FieldExtensions
	{
		#region Field Access
		/// <summary>
		/// Sets the field specified by <paramref name="name"/> on the given <paramref name="obj"/>
		/// to the specified <paramref name="value" />.
		/// </summary>
		/// <returns><paramref name="obj"/>.</returns>
		public static object SetFieldValue(this object obj, string name, object value)
		{
			Fasterflect.Extensions.FieldExtensions.DelegateForSetFieldValue(obj.GetTypeAdjusted(), name)(obj, value);
			return obj;
		}

		/// <summary>
		/// Gets the value of the field specified by <paramref name="name"/> on the given <paramref name="obj"/>.
		/// </summary>
		public static object GetFieldValue(this object obj, string name)
		{
			return Fasterflect.Extensions.FieldExtensions.DelegateForGetFieldValue(obj.GetTypeAdjusted(), name)(obj);
		}

		/// <summary>
		/// Sets the field specified by <paramref name="name"/> and matching <paramref name="bindingFlags"/>
		/// on the given <paramref name="obj"/> to the specified <paramref name="value" />.
		/// </summary>
		/// <returns><paramref name="obj"/>.</returns>
		public static object SetFieldValue(this object obj, string name, object value, Flags bindingFlags)
		{
			Fasterflect.Extensions.FieldExtensions.DelegateForSetFieldValue(obj.GetTypeAdjusted(), name, bindingFlags)(obj, value);
			return obj;
		}

		/// <summary>
		/// Gets the value of the field specified by <paramref name="name"/> and matching <paramref name="bindingFlags"/>
		/// on the given <paramref name="obj"/>.
		/// </summary>
		public static object GetFieldValue(this object obj, string name, Flags bindingFlags)
		{
			return Fasterflect.Extensions.FieldExtensions.DelegateForGetFieldValue(obj.GetTypeAdjusted(), name, bindingFlags)(obj);
		}
		#endregion

		#region Field Combined

		#region TryGetValue
		/// <summary>
		/// Gets the first (public or non-public) instance field with the given <paramref name="name"/> on the given
		/// <paramref name="obj"/> object. Returns the value of the field if a match was found and null otherwise.
		/// </summary>
		/// <remarks>
		/// When using this method it is not possible to distinguish between a missing field and a field whose value is null.
		/// </remarks>
		/// <param name="obj">The source object on which to find the field</param>
		/// <param name="name">The name of the field whose value should be retrieved</param>
		/// <returns>The value of the field or null if no field was found</returns>
		public static object TryGetFieldValue(this object obj, string name)
		{
			return TryGetFieldValue(obj, name, Flags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Gets the first field with the given <paramref name="name"/> on the given <paramref name="obj"/> object.
		/// Returns the value of the field if a match was found and null otherwise.
		/// Use the <paramref name="bindingFlags"/> parameter to limit the scope of the search.
		/// </summary>
		/// <remarks>
		/// When using this method it is not possible to distinguish between a missing field and a field whose value is null.
		/// </remarks>
		/// <param name="obj">The source object on which to find the field</param>
		/// <param name="name">The name of the field whose value should be retrieved</param>
		/// <param name="bindingFlags">A combination of Flags that define the scope of the search</param>
		/// <returns>The value of the field or null if no field was found</returns>
		public static object TryGetFieldValue(this object obj, string name, Flags bindingFlags)
		{
			try {
				return obj.GetFieldValue(name, bindingFlags);
			}
			catch (MissingFieldException) {
				return null;
			}
		}
		#endregion

		#region TrySetValue
		/// <summary>
		/// Sets the first (public or non-public) instance field with the given <paramref name="name"/> on the 
		/// given <paramref name="obj"/> object to supplied <paramref name="value"/>. Returns true if a value
		/// was assigned to a field and false otherwise.
		/// </summary>
		/// <param name="obj">The source object on which to find the field</param>
		/// <param name="name">The name of the field whose value should be retrieved</param>
		/// <param name="value">The value that should be assigned to the field</param>
		/// <returns>True if the value was assigned to a field and false otherwise</returns>
		public static bool TrySetFieldValue(this object obj, string name, object value)
		{
			return TrySetFieldValue(obj, name, value, Flags.InstanceAnyVisibility);
		}

		/// <summary>
		/// Sets the first field with the given <paramref name="name"/> on the given <paramref name="obj"/> object
		/// to the supplied <paramref name="value"/>. Returns true if a value was assigned to a field and false otherwise.
		/// Use the <paramref name="bindingFlags"/> parameter to limit the scope of the search.
		/// </summary>
		/// <param name="obj">The source object on which to find the field</param>
		/// <param name="name">The name of the field whose value should be retrieved</param>
		/// <param name="value">The value that should be assigned to the field</param>
		/// <param name="bindingFlags">A combination of Flags that define the scope of the search</param>
		/// <returns>True if the value was assigned to a field and false otherwise</returns>
		public static bool TrySetFieldValue(this object obj, string name, object value, Flags bindingFlags)
		{
			try {
				obj.SetFieldValue(name, value, bindingFlags);
				return true;
			}
			catch (MissingFieldException) {
				return false;
			}
		}
		#endregion

		#endregion
	}
}