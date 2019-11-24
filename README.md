# Intro

This framework is based on [Fasterflect](https://github.com/buunguyen/fasterflect), which was originally developed by 
Buu Nguyen and Morten Mertner. The reason I recommend using this framework the original is because the extension methods 
have been moved to a separate namespace to ensure that they do not clutter intellisense and auto-complete. Almost all of them
have been changed to internal as well because this most of the functionality should be accessed from ReflectLookup instead. 
The extensions that are still available include Type specific ones such as pretty printing. It also includes one new feature: MultiSetter.

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

public class Program {
public static void Main(string[] args)
{
	ConstructorInvoker ctor = Reflect.Constructor(typeof(person), typeof(string), typeof(int));
	var getName = Reflect.Getter(typeof(Person), "Name");
	var getAge = Reflect.FieldGetter(typeof(Person), "Age");
	var setAge = Reflect.Setter(typeof(Person), "Age");

	Person person = (Person) ctor("John Doe", 21);
	setAge(person, 35);
	Console.Writeline(person.Name);
	Console.WriteLine(person.Age);
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

Reflect is the static factory for all reflection-based delegates. Every delegate that is generated is stored in a temporary cache.
This ensures that you do not create multiple instances of a delegate and will not waste memory by trying to do so.

| Method | Description |
| --- | --- |
| Reflect.Constructor() | ConstructorInfo |
| Reflect.Getter() | PropertyInfo.GetMethod() or FieldInfo | 
| Reflect.PropertyGetter() | PropertyInfo.GetMethod() |
| Reflect.FieldGetter() | FieldInfo |
| Reflect.Setter() | PropertyInfo.SetMethod() or FieldInfo |
| Reflect.PropertySetter() | PropertyInfo.SetMethod() |
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

Value types must be wrapped with a ValueTypeHolder to work with the reflection delegates. The alternative to this approach would be 
to make the first argument a ref object for all delegates. I have decided against doing this because value types are not supposed 
to be passed by reference and forcing the user to type ref for every call is tedious and it would reduce the performance for reference types.

## [ReflectLookup](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/ReflectLookup.cs)

This allows searching for reflection based objects using Fasterflect flags instead of BindingFlags. I do not think it is very useful, 
but it does include some good ideas such as case insensitive searches. I would recommend using the standard reflection access over these methods.

## [Emitter.EmitHelper](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/Emitter/EmitHelper.cs)

This is a wrapper around System.Reflection.Emit.IlGenerator that is easier to read and simple to use. Instead of typing
ilGenerator.Emit(OpCodes.XXX, yyy) you would type emitter.xxx(yyy).

# License

The Apache 2.0 License

Copyright (c) 2010  Buu Nguyen, Morten Mertner \
Copyright (c) 2018 Wesley Hamilton \

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at 

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.