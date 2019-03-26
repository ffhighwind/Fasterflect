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

using System;
using System.Reflection;
using Fasterflect;
using Fasterflect.Extensions;
using FasterflectTest.SampleModel.Animals;
using FasterflectTest.SampleModel.Animals.Interfaces;
using FasterflectTest.SampleModel.People;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Fasterflect.Extensions.Internal;

namespace FasterflectTest.Invocation
{
	[TestClass]
	public class ConstructorTest : BaseInvocationTest
	{
		[TestMethod]
		public void TestInvokeNoArgCtor()
		{
			object person = PersonType.CreateInstance();
			Assert.IsNotNull(person);
		}

		[TestMethod]
		public void TestInvokeCtorWithCorrectBindingFlags()
		{
			RunWith(type => type.CreateInstance(FasterflectFlags.Instance | FasterflectFlags.NonPublic));
		}

		[TestMethod]
		[ExpectedException(typeof(MissingMemberException))]
		public void TestInvokeCtorWithIncorrectBindingFlags()
		{
			RunWith(type => type.CreateInstance(FasterflectFlags.Public | FasterflectFlags.Instance));
		}

		[TestMethod]
		public void TestInvokeCtorWithPrimitiveArguments()
		{
			RunWith(type =>
			  {
				  object person = type.CreateInstance("John", 10).WrapIfValueType();
				  VerifyFields(person, new { name = "John", age = 10 });
			  });
		}

		[TestMethod]
		public void TestInvokeCtorWithComplexArgument()
		{
			RunWith(type =>
			  {
				  object person = type.CreateInstance("John", 10);
				  object copy = type.CreateInstance(person).WrapIfValueType();
				  VerifyFields(copy, new { name = "John", age = 10 });
			  });
		}

		[TestMethod]
		public void TestInvokeCtorWithComplexArgumentCoveriant()
		{
			object employee = EmployeeType.CreateInstance("John", 10);
			object copy = PersonType.CreateInstance(employee).WrapIfValueType();
			VerifyFields(copy, new { name = "John", age = 10 });
		}

		[TestMethod]
		public void TestInvokeCtorWithOutArgument()
		{
			RunWith(type =>
			  {
				  object[] arguments = new object[] { "John", 10, 0 };
				  object person = type.CreateInstance(new[] { typeof(string), typeof(int), typeof(int).MakeByRefType() },
													arguments).WrapIfValueType();
				  VerifyFields(person, new { name = "John", age = 10 });
				  Assert.IsTrue((int) arguments[2] > 0);
			  });
		}

		[TestMethod]
		public void TestInvokeCtorWithArrayArgument()
		{
			object employee = EmployeeType.CreateInstance(new[] { EmployeeType.MakeArrayType() },
														new[] { new Employee[0] });
			Assert.IsNotNull(employee);
			Assert.AreEqual(0, employee.GetPropertyValue("Subordinates").GetPropertyValue("Length"));
		}

		[TestMethod]
		[ExpectedException(typeof(MissingMemberException))]
		public void TestInvokeMissingCtor()
		{
			RunWith(type => type.CreateInstance("oneStringArgument"));
		}

		[TestMethod]
		public void TestInvokeCtorWithNullParametersTheRightWay()
		{
			RunWith(type =>
			  {
				  object person = type.CreateInstance(new[] { typeof(string), typeof(int) },
													null, 10).WrapIfValueType();
				  VerifyFields(person, new { name = (string) null, age = 10 });
			  });
		}

		[TestMethod]
		public void TestInvokeNoArgCtorViaConstructorInfo()
		{
			ConstructorInfo ctorInfo = PersonType.Constructor();
			object person = ctorInfo.CreateInstance().WrapIfValueType();
			VerifyFields(person, new { name = string.Empty, age = 0 });
		}

		[TestMethod]
		public void TestInvokeCtorViaConstructorInfo()
		{
			RunWith(type =>
			  {
				  ConstructorInfo ctorInfo = type.Constructor(typeof(string), typeof(int));
				  object person = ctorInfo.CreateInstance(null, 10).WrapIfValueType();
				  VerifyFields(person, new { name = (string) null, age = 10 });
			  });
		}

		[TestMethod]
		public void TestInvokeViaAssemblyScanner()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			IList<ISwimmable> list1 = assembly.CreateInstances<ISwimmable>();
			Assert.AreEqual(1, list1.Count);
			Assert.IsInstanceOfType(list1[0], typeof(ISwimmable));

			IList<ISlide> list2 = assembly.CreateInstances<ISlide>();
			Assert.AreEqual(1, list2.Count);
			Assert.IsInstanceOfType(list2[0], typeof(ISlide));

			IList<Mammal> list3 = assembly.CreateInstances<Mammal>();
			Assert.AreEqual(2, list3.Count); // Elephant + Lion (Giraffe has no default ctor)
			list3.ForEach(o => Assert.IsInstanceOfType(o, typeof(Mammal)));
		}
	}
}