#region License
// Copyright 2010 Buu Nguyen, Morten Mertner
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

using Fasterflect.Extensions;
using FasterflectTest.Common;
using FasterflectTest.SampleModel.People;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Services
{
	[TestClass]
	public class MapTest : BaseTest
	{
		#region Helper class for testing Map with fields-based source
		private class Source
		{
			private int employeeId;
			private string name;
			private int age;
			private double metersTravelled;

			public Source(int employeeId, string name, int age, double metersTravelled)
			{
				this.employeeId = employeeId;
				this.name = name;
				this.age = age;
				this.metersTravelled = metersTravelled;
			}

			public Source(string name, int age, double metersTravelled)
			{
				this.name = name;
				this.age = age;
				this.metersTravelled = metersTravelled;
			}

			public int EmployeeId {
				get => employeeId;
				set => employeeId = value;
			}

			public string Name {
				get => name;
				set => name = value;
			}

			public int Age {
				get => age;
				set => age = value;
			}

			public double MetersTravelled {
				get => metersTravelled;
				set => metersTravelled = value;
			}
		}
		#endregion

		public MapTest() : base(new[] { typeof(Person), typeof(PersonStruct) })
		{
		}

		#region Map
		[TestMethod]
		public void TestMapHashCode()
		{
			Assert.AreNotEqual(typeof(Person).DelegateForMap(typeof(PersonStruct)), typeof(PersonStruct).DelegateForMap(typeof(Person)));
		}

		[TestMethod]
		public void TestMap()
		{
			RunWith((object person) => {
				object source = new { name = "John", age = 10, metersTravelled = 120d };
				source.Map(person);
				VerifyFields(person, source);
			});
		}
		#endregion

		#region MapProperties
		[TestMethod]
		public void TestMapProperties()
		{
			RunWith((object person) => {
				object source = new { Name = "John", Age = 10, MetersTravelled = 120d };
				source.MapProperties(person);
				VerifyProperties(person, source);
			});
		}

		[TestMethod]
		public void TestMapPropertiesWithFilter()
		{
			RunWith((object person) => {
				int currentAge = (int)person.GetPropertyValue("Age");
				object source = new { Name = "John", Age = currentAge + 10, MetersTravelled = 120d };
				source.MapProperties(person, "Name", "MetersTravelled");
				VerifyProperties(person, new { Age = currentAge });
			});
		}

		[TestMethod]
		public void TestMapPropertiesViaSubclass()
		{
			object employee = typeof(Employee).CreateInstance();
			object source = new { EmployeeId = 5, Name = "John", Age = 20, MetersTravelled = 120d };
			source.MapProperties(employee);
			VerifyProperties(employee, source);
		}
		#endregion

		#region MapPropertiesToFields
		[TestMethod]
		public void TestMapPropertiesToFields()
		{
			RunWith((object person) => {
				object source = new { name = "John", age = 10, metersTravelled = 120d };
				source.MapPropertiesToFields(person);
				VerifyFields(person, source);
			});
			//DateTime birthday = new DateTime(1973, 1, 27);
			//object source = new { id = 42, birthDay = birthday, name = "Arthur Dent" };
			//Person target = new Person();
			//source.MapPropertiesToFields(target);
			//Verify(new Person(42, birthday, "Arthur Dent"), target);
		}

		//[TestMethod]
		//public void TestMapPropertiesToFieldsWithStatic()
		//{
		//    RunWith((Type type) =>
		//       {
		//           var totalPeopleCreated = (int) type.GetFieldValue("totalPeopleCreated") + 1;
		//           type.SetFields(new { totalPeopleCreated });
		//           VerifyFields(type, new { totalPeopleCreated });
		//       });
		//}

		[TestMethod]
		public void TestMapPropertiesToFieldsWithFilter()
		{
			RunWith((object person) => {
				int currentAge = (int)person.GetFieldValue("age");
				object source = new { name = "John", age = currentAge + 10, metersTravelled = 120d };
				source.MapPropertiesToFields(person, "name", "metersTravelled");
				VerifyFields(person, new { age = currentAge });
			});
		}

		[TestMethod]
		public void TestMapPropertiesToFieldsViaSubclass()
		{
			object employee = typeof(Employee).CreateInstance();
			object source = new { employeeId = 5, name = "John", age = 20, metersTravelled = 120d };
			source.MapPropertiesToFields(employee);
			VerifyFields(employee, source);
		}
		#endregion

		#region MapFields
		[TestMethod]
		public void TestMapFields()
		{
			RunWith((object person) => {
				object source = new Source("John", 10, 120d);
				source.MapFields(person);
				VerifyFields(person, new { name = "John", age = 10, metersTravelled = 120d });
			});
		}

		[TestMethod]
		public void TestMapFieldsWithFilter()
		{
			RunWith((object person) => {
				int currentAge = (int)person.GetFieldValue("age");
				Source source = new Source("John", currentAge + 10, 120d);
				source.MapFields(person, "Name", "MetersTravelled");
				VerifyFields(person, new { age = currentAge });
			});
		}

		[TestMethod]
		public void TestMapFieldsViaSubclass()
		{
			object employee = typeof(Employee).CreateInstance();
			Source source = new Source(5, "John", 10, 120d);
			source.MapFields(employee);
			VerifyFields(employee, source);
		}
		#endregion

		#region MapFieldsToProperties
		[TestMethod]
		public void TestMapFieldsToProperties()
		{
			RunWith((object person) => {
				Source source = new Source("John", 10, 120d);
				source.MapFieldsToProperties(person);
				VerifyProperties(person, new { Name = "John", Age = 10, MetersTravelled = 120d });
			});
		}

		[TestMethod]
		public void TestMapFieldsToPropertiesWithFilter()
		{
			RunWith((object person) => {
				int currentAge = (int)person.GetPropertyValue("Age");
				Source source = new Source("John", currentAge + 10, 120d);
				source.MapFieldsToProperties(person, "name", "metersTravelled");
				VerifyProperties(person, new { Age = currentAge });
			});
		}

		[TestMethod]
		public void TestMapFieldsToPropertiesViaSubclass()
		{
			object employee = typeof(Employee).CreateInstance();
			Source source = new Source(5, "John", 20, 120d);
			source.MapFieldsToProperties(employee);
			VerifyProperties(employee, source);
		}
		#endregion
	}
}