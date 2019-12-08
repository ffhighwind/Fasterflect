using System;
using Fasterflect;

namespace FasterflectExamplePrimitive
{
	public static class Program
	{
		public static void Main()
		{
			ConstructorInvoker ctor = Reflect.Constructor(typeof(long));
			MemberGetter getValue = Reflect.Getter(typeof(long), "m_value");
			MemberSetter setValue = Reflect.Setter(typeof(long), "m_value");
			long integer = (long)ctor();
			ValueTypeHolder holder = new ValueTypeHolder(integer); // IMPORTANT!
			setValue(holder, 8L);
			Console.WriteLine(getValue(holder));
			Console.ReadLine();
		}

		// Output:
		// 8
	}
}
