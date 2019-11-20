# Intro

This framework is based on [Fasterflect](https://github.com/buunguyen/fasterflect), developed originally by Buu Nguyen and Morten Mertner. 
This modified framework includes one new reflection-based delegate: MultiSetter. The reason I recommend using this framework the original 
is because the extension methods are separated into a namespace so that they do not clutter intellisense and autocomplete.

## [Reflect](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/Reflect.cs)

[Delegates](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/Delegates.cs)

This is the factory for all reflection-based delegates. Every delegate is stored in a temporary cache, which means that they will be 
garbage collected unless you keep a reference to them. However, this also means that you cannot create multiple instances of a delegate
and will not waste memory by trying to do so.

| Factory Method | Delegate | Description |
| --- | --- | --- |
| Reflect.Constructor(...) | ConstructorInvoker | |
| Reflect.Getter(...) | MemberGetter | | 
| Reflect.PropertyGetter(...) | MemberGetter | |
| Reflect.FieldGetter(...) | MemberGetter | |
| Reflect.Setter(...) | MemberSetter | Allows
| Reflect.PropertySetter(...) | MemberSetter | |
| Reflect.FieldSetter(...) | MemberSetter | |
| Reflect.MultiSetter(...) | MultiSetter | Sets multiple properties/fields with one method call. This is generated as a single delegate which means it is faster than setting each member in a loop. |
| Reflect.Method(...) | MethodInvoker |  |
| Reflect.Mapper(...) | ObjectMapper | Maps and copies the properties/fields of one type onto another type. This can also be used as a shallow cloning method if both types are the same. |
| Reflect.IndexerGetter(...) | MethodInvoker | Indexer e.g. list[index] |
| Reflect.IndexerSetter(...) | MethodInvoker | Indexer e.g. list[index] = value |
| Reflect.ArrayGetter(...) | ArrayElementGetter |  |
| Reflect.DeepClone<T>(...) | T | Creates a deep clone of an object. This is an expensive operation, so I recommend creating a shallow copy in most cases. |
| Reflect.ShallowClone<T>(...) | T | Creates a shallow clone of an object using MemberwiseClone. This will throw an exception in certain circumstances because MemberwiseClone is a private method. |

## [ValueTypeHolder](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/ValueTypeHolder.cs)

Value types must be wrapped with a ValueTypeHolder to work with the reflection methods. The alternative to this approach would be 
to make the first argument a ref object for all delegates. I have decided against doing this because value types are not supposed 
to be passed by reference and forcing the user to type ref for every call is tedious and it would reduce the performance for reference types.

## [ReflectLookup](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/ReflectLookup.cs)

This allows searching for reflection based objects using Fasterflect flags instead of BindingFlags. I do not think it is very useful, 
but it does include some good ideas such as case insensitive searches. I would recommend using the standard typeof(T).GetXXX() 
reflection access over these methods.

## [Extensions](https://github.com/ffhighwind/fasterflect/tree/master/Fasterflect/Fasterflect/Extensions)

All extension methods are in the namespace Fasterflect.Extensions. This was done because most of these extensions are nuisance, especially 
the extension methods for "object". The only reason I did not remove these is because they are used everywhere and it would be too much 
work to remove them.

## [Emitter.EmitHelper](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/Emitter/EmitHelper.cs)

This is a wrapper around System.Reflection.Emit.IlGenerator which is easier to read and simple to use. Instead of typing
ilGenerator.Emit(OpCodes.XXX, yyy) you would  say emitter.xxx(yyy). It is a helpful class if you already know how to write IL, 
but not very useful otherwise.

### Copyright
Copyright 2010 Buu Nguyen, Morten Mertner
Copyright 2018 Wesley Hamilton