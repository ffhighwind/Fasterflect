using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Fasterflect;
using Fasterflect.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Issues
{
	[TestClass]
	public class AmbiguousMatchTest
	{
		#region Sample Classes
		private class Foo
		{
			public object Property { get; set; }
		}
		private class Bar : Foo
		{
			public new string Property { get; set; }
		}
		#endregion

		[TestMethod]
		public void Test_PropertyLookupWithNameAndEXHFlagShouldNotThrowAmbiguousMatchException()
		{
			PropertyInfo propertyInfo = typeof(Bar).Property("Property", FasterflectFlags.InstanceAnyVisibility | FasterflectFlags.ExcludeHiddenMembers);
			Assert.IsNotNull(propertyInfo);
			Assert.AreEqual(typeof(Bar), propertyInfo.DeclaringType);
		}

		[TestMethod]
		public void Test_PropertiesLookupWithNameAndEXHFlagShouldFindSingleResult()
		{
			PropertyInfo propertyInfo = typeof(Bar).Properties(FasterflectFlags.InstanceAnyVisibility | FasterflectFlags.ExcludeHiddenMembers, "Property").Single();
			Assert.IsNotNull(propertyInfo);
			Assert.AreEqual(typeof(Bar), propertyInfo.DeclaringType);
		}
	}
}
