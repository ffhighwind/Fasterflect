# Intro

[Nuget: fasterflect.reflect](https://www.nuget.org/packages/fasterflect.reflect/)

This framework is based on [Fasterflect](https://github.com/buunguyen/fasterflect), which was originally developed by Buu Nguyen and Morten Mertner. The extension methods in this version have been moved to a separate namespace to ensure that they do not clutter intellisense and auto-complete. Most of the search methods have been moved to ReflectLookup instead. It also includes one new feature: MultiSetter.

## Benchmarks

Cached Fasterflect is 50x faster than .NET reflection, and 2-10x slower than direct property access. Note also that Fasterflect only uses objects as arguments and return types.
This means it is slowed down by needing to box and unbox data, which also means that it performs slightly worse than generic delegates that have the exact type. However, this is usually the only
time that reflection is useful. If you know the generic types at compile time then you likely have direct access to the properties or you can implement an interface that lets you 
bypass the need for reflection.

### Getters
|                                Getters |        Mean |   Ratio |
|--------------------------------------- |------------:|--------:|
|        Direct Access (inlining)        |   0.0000 ns |   0.000 |
|     Direct Access (no inlining)        |   0.0000 ns |   0.000 |
| Delegate.CreateDelegate Func<T,string> |   1.7718 ns |   0.582 |
|                     Fasterflect        |   3.0426 ns |   1.000 |
|     Magnum (Expression.Compile)        |  10.6231 ns |   3.492 |
|           ILEmit Func<T,string>        |  12.3850 ns |   4.071 |
|             ILEmit MemberGetter        |  13.8618 ns |   4.556 |
|     ILEmit Func<\object,object>        |  13.9188 ns |   4.574 |
|                      FastMember        |  30.7529 ns |  10.109 |
|               MethodInfo.Invoke        | 124.4104 ns |  40.895 |
|           PropertyInfo (cached)        | 133.4586 ns |  43.869 |
|                    PropertyInfo        | 193.6173 ns |  63.643 |
|          Delegate.DynamicInvoke        | 701.8960 ns | 230.719 |

### Setters
|                                  Setters |       Mean |  Ratio |
|----------------------------------------- |-----------:|-------:|
|       Direct Access (inlining)           |   1.300 ns |   0.33 |
|    Direct Access (no inlining)           |   1.299 ns |   0.33 |
| Delegate.CreateDelegate Action<T,string> |   3.036 ns |   0.78 |
|        ILEmit Action<T,string>           |   3.081 ns |   0.79 |
|            ILEmit MemberSetter           |   3.900 ns |   1.00 |
|  ILEmit Action<\object,object>           |   3.905 ns |   1.00 |
|                    Fasterflect           |   3.907 ns |   1.00 |
|    Magnum (Expression.Compile)           |  10.508 ns |   2.69 |
|                     FastMember           |  32.130 ns |   8.22 |
|              MethodInfo.Invoke           | 191.305 ns |  48.96 |
|          PropertyInfo (cached)           | 200.824 ns |  51.39 |
|                   PropertyInfo           | 259.840 ns |  66.51 |
|         Delegate.DynamicInvoke           | 763.633 ns | 195.48 |

[Other Benchmarks](https://github.com/ffhighwind/fasterflect/wiki/Benchmarks)
All times are recorded in miliseconds using 2 million iterations.

## Example

```csharp
using Fasterflect;

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

public static class Program 
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

// Output
// John Doe
// 35
// Complete...
```

## [Reflect](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/Reflect.cs)

Reflect is the static factory for all reflection-based delegates. Every delegate that is generated is stored in a temporary cache. This ensures that you do not create multiple instances of a delegate and will not waste memory by trying to do so.

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
| Reflect.ArrayGetter() | value = object[index] |
| Reflect.ArraySetter() | object[index] = value |
| Reflect.DeepClone<T>() | Creates a deep clone of an object. |
| Reflect.ShallowClone<T>() | Creates a shallow clone of an object using MemberwiseClone. This can throw an exception. |

## [ValueTypeHolder](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/ValueTypeHolder.cs)

Value types (structs) must be wrapped with a ValueTypeHolder to work with the reflection delegates. The alternative to this approach would be to make the first argument a ref object for all delegates. I have decided against doing this because value types are not supposed to be passed by reference and forcing the user to type ref for every call is tedious and it would reduce the performance for reference types.

## [ReflectLookup](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/ReflectLookup.cs)

This allows searching for reflection based objects using Fasterflect flags instead of BindingFlags. I do not think it is very useful, but it does include some good ideas such as case insensitive searches.

## [Emitter.EmitHelper](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/Emitter/EmitHelper.cs)

This is a wrapper around System.Reflection.Emit.IlGenerator that is easier to read and simple to use. Instead of typing ilGenerator.Emit(OpCodes.XXX, yyy) you would type emitter.xxx(yyy).

# License

*The Apache 2.0 License*

Copyright (c) 2010  Buu Nguyen, Morten Mertner \
Copyright (c) 2018 Wesley Hamilton \

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 \

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.