using Fasterflect;
using FasterflectTest.SampleModel.People;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace FasterflectTest.Invocation
{
	[TestClass]
	public class MultiSetTest : BaseInvocationTest
	{
		[TestMethod]
		public void TestPerson1()
		{
			Person person = new Person();
			person.Age = 0;
			person.Name = "";
			Type type = person.GetType();
			MultiSetter setter = Reflect.MultiSetter(type, "Age", "Name");
			setter(person, 5, "Jack Black");
			Assert.IsTrue(person.Age == 5 && person.Name == "Jack Black");
		}

		[TestMethod]
		public void TestPerson2()
		{
			Person person = new Person();
			person.Age = 0;
			person.Name = "";
			Type type = person.GetType();
			MultiSetter setter = Reflect.MultiSetter(type, "Name", "Age");
			setter(person, "John Doe", 2);
			Assert.IsTrue(person.Age == 2 && person.Name == "John Doe");
		}

		[TestMethod]
		public void TestPersonStruct1()
		{
			PersonStruct person = new PersonStruct();
			person.Age = 0;
			person.Name = "";
			person.MetersTravelled = 0;

			ValueTypeHolder h = person;
			Type type = person.GetType();
			MultiSetter setter = Reflect.MultiSetter(type, "name", "totalPeopleCreated", "Age", "MetersTravelled");
			FieldInfo field = type.GetField("totalPeopleCreated", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField);
			setter(h, "Jack Black", 15, 5, 2.0d);
			person = (PersonStruct)h;
			Assert.IsTrue(person.Name == "Jack Black" && person.Age == 5 && person.MetersTravelled == 2 && ((int)field.GetValue(null) == 15));
		}

		[TestMethod]
		public void TestPersonStruct2()
		{
			PersonStruct person = new PersonStruct();
			person.Age = 0;
			person.Name = "";
			person.MetersTravelled = 0;

			ValueTypeHolder h = person;
			Type type = person.GetType();
			MultiSetter setter = Reflect.MultiSetter(type, "totalPeopleCreated", "Age", "name", "MetersTravelled");
			FieldInfo field = type.GetField("totalPeopleCreated", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField);
			setter(h, 12, 6, "John Doe", 2.1d);
			person = (PersonStruct)h;
			Assert.IsTrue(person.Name == "John Doe" && person.Age == 6 && person.MetersTravelled == 2.1 && ((int)field.GetValue(null) == 12));
		}
	}
}
