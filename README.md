# Intro

[Nuget: Fasterflect.Reflect](https://www.nuget.org/packages/Fasterflect.Reflect/)

This framework is based on [Fasterflect](https://github.com/buunguyen/Fasterflect), which was originally developed by Buu Nguyen and Morten Mertner. The extension methods in this version have been moved to a separate namespace to ensure that they do not clutter intellisense and auto-complete. Most of the methods been moved to Reflect and ReflectLookup static classes in order to achieve this. It also includes two new features: MultiSetter and DataReaderFactory.

## Benchmarks

Fasterflect is 50x faster than .NET reflection, 8x faster than FastMember, and 2-10x slower than direct access. Note also that Fasterflect only uses *objects* as arguments and return types. This slows down the methods because data needs to be boxed and unboxed. However, this is usually the only time that reflection is useful. If you know the generic types at compile time then you likely have direct access to the properties or you can implement an interface that lets you bypass the need for reflection.

|                         Property Getters |        Mean |   Ratio |
|----------------------------------------- |------------:|--------:|
| Direct Access                            |   0.0000 ns |   0.000 |
| Delegate.CreateDelegate (Func<T,string>) |   1.7718 ns |   0.582 |
| **Fasterflect**                          | **3.0426 ns** | **1.000** |
| Magnum (Expression.Compile)              |  10.6231 ns |   3.492 |
| ILEmit (Func<T,string>)                  |  12.3850 ns |   4.071 |
| ILEmit Func<object,object>               |  13.9188 ns |   4.574 |
| FastMember                               |  30.7529 ns |  10.109 |
| MethodInfo.Invoke                        | 124.4104 ns |  40.895 |
| PropertyInfo (cached)                    | 133.4586 ns |  43.869 |
| PropertyInfo (uncached)                  | 193.6173 ns |  63.643 |
| Delegate.DynamicInvoke                   | 701.8960 ns | 230.719 |

|                           Property Setters |       Mean |  Ratio |
|------------------------------------------- |-----------:|-------:|
| Direct Access                              |   1.300 ns |   0.33 |
| Delegate.CreateDelegate (Action<T,string>) |   3.036 ns |   0.78 |
| ILEmit (Action<T,string>)                  |   3.081 ns |   0.79 |
| ILEmit (Action<object,object>)             |   3.905 ns |   1.00 |
| **Fasterflect**                            | **3.907 ns** | **1.00** |
| Magnum (Expression.Compile)                |  10.508 ns |   2.69 |
| FastMember                                 |  32.130 ns |   8.22 |
| MethodInfo.Invoke                          | 191.305 ns |  48.96 |
| PropertyInfo (cached)                      | 200.824 ns |  51.39 |
| PropertyInfo (uncached)                    | 259.840 ns |  66.51 |
| Delegate.DynamicInvoke                     | 763.633 ns | 195.48 |

[Other Benchmarks](https://github.com/ffhighwind/Fasterflect/wiki/Benchmarks)

## Example

```csharp
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
		MultiSetter setBoth = Reflect.MultiSetter(typeof(Person), "Age", "Name");

		Person person = (Person) ctor("John Doe", 21);
		setAge(person, 30);
		Console.WriteLine(getName(person));
		Console.WriteLine(getAge(person));
		setBoth(person, 35, "John Wick");
		Console.WriteLine(getName(person));
		Console.WriteLine(getAge(person));
		Console.ReadLine();

		// Output:
		// John Doe
		// 30
		// John Wick
		// 35
	}
}
}

```

## [Reflect](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Reflect.cs)

Reflect is the static factory for all reflection-based delegates. Every delegate that is generated is stored in a temporary cache (WeakReference). This allows delegates to be garbage collected but also ensures that you do cannot waste memory by creating multiple instances of the same delegate.

| Method | Description |
| --- | --- |
| Reflect.Constructor() | ConstructorInfo |
| Reflect.Getter() | PropertyInfo or FieldInfo | 
| Reflect.PropertyGetter() | PropertyInfo |
| Reflect.FieldGetter() | FieldInfo |
| Reflect.Setter() | PropertyInfo or FieldInfo |
| Reflect.PropertySetter() | PropertyInfo |
| Reflect.FieldSetter() | FieldInfo |
| Reflect.MultiSetter() | Sets multiple properties/fields without using a loop. |
| Reflect.Method() | MethodInfo |
| Reflect.Mapper() | Maps the properties/fields of one type onto another type. This can also be used as a shallow cloning method if both types are the same. |
| Reflect.IndexerGetter() | value = object[index1, index2] |
| Reflect.IndexerSetter() | object[index1, index2] = value |
| Reflect.ArrayGetter() | value = array[index] |
| Reflect.ArraySetter() | array[index] = value |
| Reflect.DeepClone<T>() | Creates a deep clone of an object. |
| Reflect.ShallowClone<T>() | Creates a shallow clone of an object using MemberwiseClone. This can throw an exception. |

## [ReflectLookup](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/ReflectLookup.cs)

This allows searching for reflection based objects using Fasterflect flags instead of BindingFlags. This is up to 2x slower than normal reflection, but it allows partial matching (string.Contains) and case insensitive search.

## [ValueTypeHolder](https://github.com/ffhighwind/Fasterflect/blob/master/Fasterflect/ValueTypeHolder.cs)

Value types (structs) must be wrapped with a ValueTypeHolder to work with the reflection delegates. The alternative to this approach would be to make the first argument of every delegate a ref. I have decided against this because value types are not supposed to be passed by reference and forcing the user to type ref for every call is tedious and it would reduce the performance for reference types.

```csharp
using System;
using Fasterflect;

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

		// Output:
		// Charlie
		// 8
		// Buster
		// 10
	}
}
}
```

## [DataReaderFactory](https://github.com/ffhighwind/Fasterflect/blob/master/Fasterflect/DataReaderFactory.cs)

This is based on FastMember's [ObjectReader](https://github.com/mgravell/fast-member/blob/master/FastMember/ObjectReader.cs). It can be used with SqlBulkCopy which is 100x as fast as a loop of individual database inserts. It can also be used as a parameter for DataTable.Load() to convert a list of objects to a DataTable.

## [Emitter.EmitHelper](https://github.com/ffhighwind/Fasterflect/blob/master/Fasterflect/Emitter/EmitHelper.cs)

This is a wrapper around System.Reflection.Emit.IlGenerator that is easier to read and simple to use.

# License

*The Apache 2.0 License*

Copyright (c) 2010  Buu Nguyen, Morten Mertner \
Copyright (c) 2018 Wesley Hamilton 

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.