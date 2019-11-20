# Intro

This framework is based on [Fasterflect](https://github.com/buunguyen/fasterflect), which was originally developed by 
Buu Nguyen and Morten Mertner. The reason I recommend using this framework the original is because the extension methods 
are separated into a namespace so that they do not clutter intellisense and autocomplete. It also includes one new feature: 
MultiSetter.

## [Reflect](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/Reflect.cs)

This is the factory for all reflection-based delegates. Every delegate is stored in a temporary cache, which means that they will be 
garbage collected unless you keep a reference to them. However, this also means that you cannot create multiple instances of a delegate
and will not waste memory by trying to do so.

| Method | Return | Description |
| --- | --- | --- |
| Reflect.Constructor() | ConstructorInvoker | ConstructorInfo.Invoke() |
| Reflect.Getter() | MemberGetter | PropertyInfo.GetMethod().GetValue() or FieldInfo.GetValue() | 
| Reflect.PropertyGetter() | MemberGetter | PropertyInfo.GetMethod().GetValue() |
| Reflect.FieldGetter() | MemberGetter | FieldInfo.GetValue() |
| Reflect.Setter() | MemberSetter | PropertyInfo.SetMethod().SetValue() or FieldInfo.SetValue() |
| Reflect.PropertySetter() | MemberSetter | PropertyInfo.SetMethod().SetValue() |
| Reflect.FieldSetter() | MemberSetter | FieldInfo.SetValue() |
| Reflect.MultiSetter() | MultiSetter | Sets multiple properties/fields with one method call. This is generated as a single delegate which means it is faster than setting each member in a loop. |
| Reflect.Method() | MethodInvoker | MethodInfo.Invoke() |
| Reflect.Mapper() | ObjectMapper | Maps the properties/fields of one type onto another type. This can also be used as a shallow cloning method if both types are the same. |
| Reflect.IndexerGetter() | MethodInvoker | value = object[index1, index2] |
| Reflect.IndexerSetter() | MethodInvoker | object[index1, index2] = value |
| Reflect.ArrayGetter() | ArrayElementGetter | value = object[index] |
| Reflect.ArraySetter() | ArrayElementGetter | object[index] = value |
| Reflect.DeepClone<T>() | T | Creates a deep clone of an object. |
| Reflect.ShallowClone<T>() | T | Creates a shallow clone of an object using MemberwiseClone. This will throw an exception in certain circumstances because MemberwiseClone is a private method. |

## [ValueTypeHolder](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/ValueTypeHolder.cs)

Value types must be wrapped with a ValueTypeHolder to work with the reflection methods. The alternative to this approach would be 
to make the first argument a ref object for all delegates. I have decided against doing this because value types are not supposed 
to be passed by reference and forcing the user to type ref for every call is tedious and it would reduce the performance for reference types.

## [ReflectLookup](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/ReflectLookup.cs)

This allows searching for reflection based objects using Fasterflect flags instead of BindingFlags. I do not think it is very useful, 
but it does include some good ideas such as case insensitive searches. I would recommend using the standard reflection access over these methods.

## [Extensions](https://github.com/ffhighwind/fasterflect/tree/master/Fasterflect/Fasterflect/Extensions)

All extension methods are in the namespace. This was done because most of the extensions are pointless and clutter the intellisense
and autocomplete. I especially dislike the extension methods on "object". The only reason I did not remove these is because they are 
used everywhere and it is too much work to remove them.

## [Emitter.EmitHelper](https://github.com/ffhighwind/fasterflect/blob/master/Fasterflect/Fasterflect/Emitter/EmitHelper.cs)

This is a wrapper around System.Reflection.Emit.IlGenerator that is easier to read and simple to use. Instead of typing
ilGenerator.Emit(OpCodes.XXX, yyy) you would type emitter.xxx(yyy).

### Copyright
Copyright 2010 Buu Nguyen, Morten Mertner \
Copyright 2018 Wesley Hamilton