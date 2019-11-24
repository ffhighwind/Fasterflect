using System;
using Fasterflect;

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
		public static void Main(string[] args)
		{
			ConstructorInvoker ctor = Reflect.Constructor(typeof(Person), typeof(string), typeof(int));
			MemberGetter getName = Reflect.Getter(typeof(Person), "Name");
			MemberGetter getAge = Reflect.FieldGetter(typeof(Person), "Age");
			MemberSetter setAge = Reflect.Setter(typeof(Person), "Age");

			Person person = (Person) ctor("John Doe", 21);
			setAge(person, 35);
			Console.WriteLine(getName(person));
			Console.WriteLine(getAge(person));
			Console.WriteLine("Complete...");
			Console.ReadLine();
		}
	}

	// Output:
	// John Doe
	// 35
	// Complete...
}
