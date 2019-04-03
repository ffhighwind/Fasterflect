using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FasterflectTest.SampleModel.Animals;
using FasterflectTest.SampleModel.People;
using Fasterflect;
using System.Reflection;

namespace FasterflectTest.Invocation
{
	[TestClass]
	public class MultiSetTest : BaseInvocationTest
	{
		[TestMethod]
		public void TestSnake()
		{
			Snake snake = new Snake();
			//animal.ClimateRequirements = SampleModel.Animals.Enumerations.Climate.Cold;
			snake.BirthDay = new DateTime(5, 5, 5);
			//animal.HasDeadlyBite = false;
			//animal.ID = 5;
			//animal.MovementCapabilities = SampleModel.Animals.Enumerations.MovementCapabilities.Water;

			MultiMemberSetter setter = Reflect.MultiSetter(snake.GetType(), "BirthDay", "ID");
			setter(snake, new DateTime(2019, 1, 1), 201);
			Assert.IsTrue(snake.BirthDay == new DateTime(2019, 1, 1) && snake.ID != 201);
		}

		[TestMethod]
		public void TestPerson()
		{
			Person person = new Person();
			person.Age = 2;
			person.Name = "John Doe";

			MultiMemberSetter setter = Reflect.MultiSetter(person.GetType(), "Age", "Name");
			setter(person, 5, "Jack Black");
			Assert.IsTrue(person.Age == 5 && person.Name == "Jack Black");
		}

		[TestMethod]
		public void TestPersonStructA()
		{
			PersonStruct person = new PersonStruct();
			person.Age = 2;
			person.Name = "John Doe";
			person.MetersTravelled = 201;

			PropertyInfo prop = person.GetType().GetProperty("Age");
			MultiMemberSetter setter = Reflect.MultiSetter(person.GetType(), "Age");

			ValueTypeHolder h = person;
			setter(h, 5);
			person = (PersonStruct) h.Value;
			Assert.IsTrue(person.Age == 5);
		}

		[TestMethod]
		public void TestPersonStruct()
		{
			PersonStruct person = new PersonStruct();
			person.Age = 2;
			person.Name = "John Doe";
			person.MetersTravelled = 201;

			ValueTypeHolder h = person;

			MultiMemberSetter setter = Reflect.MultiSetter(person.GetType(), "Name", "Age", "MetersTravelled");
			setter(h, "Jack Black", 5, 2.0d);
			person = (PersonStruct) h;
			Assert.IsTrue(person.Name == "Jack Black" && person.Age == 5 && person.MetersTravelled == 2);


			//MethodInvoker setter = Reflect.MultiSetter(person.GetType(), "Age", "Name", "MetersTravelled");
			//setter(h, 5, "Jack Black", 6);
			//person = (PersonStruct) h;
			//Assert.IsTrue(person.Age == 5 && person.Name == "Jack Black" && person.MetersTravelled == 6);
		}
	}
}
