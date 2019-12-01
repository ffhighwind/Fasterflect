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
using FasterflectTest.SampleModel.People;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FasterflectTest.Invocation
{
	[TestClass]
	public class ArrayTest : BaseInvocationTest
	{
		public ArrayTest() : base(typeof(Person[]), typeof(PersonStruct[])) { }

		[TestMethod]
		public void TestConstructArrays()
		{
			RunWith((Type type) => {
				object obj = type.CreateInstance(10);
				Assert.IsNotNull(obj);
				Assert.AreEqual(10, obj.GetPropertyValue("Length"));
			});
		}

		[TestMethod]
		public void TestGetSetElements()
		{
			RunWith((Type type) => {
				object array = type.CreateInstance(10);
				object instance = type.GetElementType().CreateInstance().WrapIfValueType();
				instance.SetFieldValue("name", "John");
				array.SetElement(1, instance.UnwrapIfWrapped());
				VerifyFields(array.GetElement(1).WrapIfValueType(), new { name = "John" });
			});
		}

		[TestMethod]
		public void TestGetSetElementsOnIntArray()
		{
			object array = typeof(int[]).CreateInstance(20);
			array.SetElement(5, 10);
			Assert.AreEqual(10, array.GetElement(5));
		}

		[TestMethod]
		public void TestGetSetElementsOnArrayProperty()
		{
			object employee = EmployeeType.CreateInstance();
			employee.SetPropertyValue("Subordinates", new Employee[10]);
			object subordinates = employee.GetPropertyValue("Subordinates");
			subordinates.SetElement(5, employee);
			Assert.AreEqual(employee, subordinates.GetElement(5));
		}
	}
}
