# Introduction

[Nuget: Fasterflect.Reflect](https://www.nuget.org/packages/Fasterflect.Reflect/)

This framework is based on [Fasterflect](https://github.com/buunguyen/Fasterflect), which was originally developed by Buu Nguyen and Morten Mertner. The extension methods in this version have been moved to a separate namespace to ensure that they do not clutter intellisense and auto-complete. Most of the methods been moved to Reflect and ReflectLookup static classes in order to achieve this. It also includes two new features: MultiSetter and DataReaderFactory.

## Benchmarks

Fasterflect is 50x faster than .NET reflection, 8x faster than FastMember, and 2-10x slower than direct access. Note also that Fasterflect only uses *objects* as arguments and return types. This slows down the methods because data needs to be boxed and unboxed. However, this is usually the only time that reflection is useful. If you know the generic types at compile time then you can probably use an interface instead of reflection.

|                         Property Getters |      Median |   Ratio |
|----------------------------------------- |------------:|--------:|
| Direct Access                            | 0.059 ns   | 0.02 |
| Delegate.CreateDelegate (T,string)       | 1.896 ns   | 0.60 |
| Fasterflect (object, object)             | **3.139 ns** | **1.00** |
| Magnum - Expression.Compile (object, object) | 10.806 ns | 3.44 |
| Sigil.ILEmit (T, string)                 | 12.765 ns  | 4.07 |
| Sigil.ILEmit (object, object)            | 13.906 ns   | 4.43 |
| FastMember (object, object)              | 31.356 ns  | 9.99 |
| MethodInfo.Invoke	                   | 126.890 ns | 40.43 |
| PropertyInfo                             | 134.323 ns  | 42.80 |
| PropertyInfo - uncached                  | 194.313 ns | 61.91 |
| Delegate.DynamicInvoke                   | 714.374 ns | 227.60 |

|                           Property Setters |     Median |  Ratio |
|------------------------------------------- |-----------:|-------:|
| Direct Access	                             | 1.266 ns	  | 0.33 |
| Delegate.CreateDelegate (T, string)	     | 3.048 ns	  | 0.79 |
| Sigil.ILEmit (T, string)	             | 2.987 ns   | 0.78 |
| Sigil.ILEmit (object, object)              | 3.735 ns   | 0.97 |
| Fasterflect (object, object)               | **3.838 ns** | **1.00** |
| Magnum - Expression.Compile (object, object) | 10.626 ns | 2.77 |
| FastMember (object, object)                | 33.402 ns | 8.70 |
| MethodInfo.Invoke                          | 196.676 ns | 51.24 |
| PropertyInfo                               | 202.662 ns | 52.80 |
| PropertyInfo - uncached                    | 265.131 ns | 69.08 |
| Delegate.DynamicInvoke                     | 795.812 ns | 207.35 |


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

Reflect is the static factory for all reflection-based delegates. Every delegate that is generated is stored in a temporary cache using a [WeakReference](https://docs.microsoft.com/en-us/dotnet/api/system.weakreference?view=netframework-4.8). This allows delegates to be garbage collected but also ensures that you do cannot waste memory by creating multiple instances of the same delegate.

| Method | Description |
| --- | --- |
| Reflect.Constructor() | ConstructorInfo |
| Reflect.Getter() | MemberInfo (PropertyInfo or FieldInfo) | 
| Reflect.PropertyGetter() | PropertyInfo |
| Reflect.FieldGetter() | FieldInfo |
| Reflect.Setter() | MemberInfo (PropertyInfo or FieldInfo) |
| Reflect.PropertySetter() | PropertyInfo |
| Reflect.FieldSetter() | FieldInfo |
| Reflect.MultiSetter() | Sets multiple properties/fields without using a loop. |
| Reflect.Method() | MethodInfo |
| Reflect.Mapper() | Maps the properties/fields of one type onto another type. This can be used as a shallow copy method if both types are the same. |
| Reflect.IndexerGetter() | value = object[index1, index2] |
| Reflect.IndexerSetter() | object[index1, index2] = value |
| Reflect.ArrayGetter() | value = array[index] |
| Reflect.ArraySetter() | array[index] = value |
| Reflect.DeepClone<T>() | Creates a deep clone of an object. |
| Reflect.ShallowClone<T>() | Creates a shallow clone of an object using MemberwiseClone. This can potentially throw an security exception because it requires accessing a protected method. |

## [ReflectLookup](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/ReflectLookup.cs)

This allows searching for reflection based objects using either FasterflectFlags or BindingFlags. This is up to 2x slower than .NET reflection, but it allows partial matching (string.Contains).

## ValueTypes/Structs

Value types must be wrapped with a [ValueTypeHolder](https://github.com/ffhighwind/Fasterflect/blob/master/Fasterflect/ValueTypeHolder.cs) to work with the Fasterflect delegates. This could have been prevented if I made the first argument of every delegate a ref. I have decided against this because value types are not supposed to be passed by reference and forcing the user to type ref for every call is tedious and it would reduce the performance for reference types.

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

This is based on FastMember's [ObjectReader](https://github.com/mgravell/fast-member/blob/master/FastMember/ObjectReader.cs). It can be used with SqlBulkCopy which is up to 100x faster than a loop of individual database inserts. It can also be used as a parameter for DataTable.Load() to convert a list of objects to a DataTable.

## [Emitter.EmitHelper](https://github.com/ffhighwind/Fasterflect/blob/master/Fasterflect/Emitter/EmitHelper.cs)

This is a wrapper around [System.Reflection.Emit.IlGenerator](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator?view=netframework-4.8) that is easier to read and simple to use.

# License

*The Apache 2.0 License*

Copyright (c) 2010  Buu Nguyen, Morten Mertner \
Copyright (c) 2018 Wesley Hamilton 

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.