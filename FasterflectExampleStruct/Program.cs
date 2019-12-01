using Fasterflect;
using System;

namespace FasterflectExampleStruct
{
	public struct Animal
	{
		public Animal(string name, int age)
		{
			Name = name;
			Age = age;
		}
		public string Name { get; set; }
		public int Age;
	};

	public static class Program
	{
		public static void Main(string[] args)
		{
			ConstructorInvoker ctor = Reflect.Constructor(typeof(Animal), typeof(string), typeof(int));
			MemberGetter getName = Reflect.Getter(typeof(Animal), "Name");
			MemberGetter getAge = Reflect.FieldGetter(typeof(Animal), "Age");
			MemberSetter setAge = Reflect.Setter(typeof(Animal), "Age");
			MultiSetter setBoth = Reflect.MultiSetter(typeof(Animal), "Age", "Name");

			Animal animal = (Animal)ctor("Charlie", 5);
			ValueTypeHolder holder = new ValueTypeHolder(animal); // IMPORTANT!
			setAge(holder, 8);
			Console.WriteLine(getName(holder));
			Console.WriteLine(getAge(holder));
			setBoth(holder, 10, "Buster");
			Console.WriteLine(getName(holder));
			Console.WriteLine(getAge(holder));
			Console.ReadLine();
		}
	}

	// Output:
	// Charlie
	// 8
	// Buster
	// 10
}
