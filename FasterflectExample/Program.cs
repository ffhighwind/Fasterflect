using Fasterflect;
using System;

namespace FasterflectExample
{
	public class Person
	{
		public Person(string name, int age)
		{
			Name = name;
			Age = age;
		}
		public string Name { get; set; }
		public int Age;
	}

	public class Program
	{
		public static void Main()
		{
			ConstructorInvoker ctor = Reflect.Constructor(typeof(Person), typeof(string), typeof(int));
			MemberGetter getName = Reflect.Getter(typeof(Person), "Name");
			MemberGetter getAge = Reflect.FieldGetter(typeof(Person), "Age");
			MemberSetter setAge = Reflect.Setter(typeof(Person), "Age");
			MultiSetter setBoth = Reflect.MultiSetter(typeof(Person), "Age", "Name");

			Person person = (Person)ctor("John Doe", 21);
			setAge(person, 30);
			Console.WriteLine(getName(person));
			Console.WriteLine(getAge(person));
			setBoth(person, 35, "John Wick");
			Console.WriteLine(getName(person));
			Console.WriteLine(getAge(person));
			Console.ReadLine();
		}
	}

	// Output:
	// John Doe
	// 30
	// John Wick
	// 35
}
