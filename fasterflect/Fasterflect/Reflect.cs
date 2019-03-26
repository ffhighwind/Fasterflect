using System;
using System.Reflection;
using Fasterflect.Emitter;
using Fasterflect.Extensions;

namespace Fasterflect
{
	/// <summary>
	/// Helper class for producing Reflection-based delegates and other utility methods.
	/// </summary>
	public static partial class Reflect
	{
		#region Constructor
		/// <summary>
		/// Creates a delegate which can invoke the constructor whose parameter types are <paramref name="parameterTypes" />
		/// on the given <paramref name="type"/>.  Leave <paramref name="parameterTypes"/> empty if the constructor
		/// has no argument.
		/// </summary>
		public static ConstructorInvoker Constructor(Type type, params Type[] parameterTypes)
		{
			return Constructor(type, FasterflectFlags.InstanceAnyVisibility, parameterTypes);
		}

		/// <summary>
		/// Creates a delegate which can invoke the constructor whose parameter types are <paramref name="parameterTypes" />
		/// and matching <paramref name="bindingFlags"/> on the given <paramref name="type"/>.  
		/// Leave <paramref name="parameterTypes"/> empty if the constructor has no argument. 
		/// </summary>
		public static ConstructorInvoker Constructor(Type type, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			return (ConstructorInvoker) new CtorInvocationEmitter(type, bindingFlags, parameterTypes).GetDelegate();
		}
		#endregion

		#region Getters
		/// <summary>
		/// Creates a delegate which can get the value of the field identified by <paramref name="fieldInfo"/>.
		/// </summary>
		public static MemberGetter Getter(FieldInfo fieldInfo)
		{
			FasterflectFlags flags = fieldInfo.IsStatic ? FasterflectFlags.StaticAnyVisibility : FasterflectFlags.InstanceAnyVisibility;
			return (MemberGetter) new MemberGetEmitter(fieldInfo, flags).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can get the value of the property <param name="propInfo"/>.
		/// </summary>
		public static MemberGetter Getter(PropertyInfo propInfo)
		{
			FasterflectFlags flags = propInfo.IsStatic() ? FasterflectFlags.StaticAnyVisibility : FasterflectFlags.InstanceAnyVisibility;
			return (MemberGetter) new MemberGetEmitter(propInfo, flags).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can get the value of the property <param name="propInfo"/> matching the
		/// specified <param name="bindingFlags" />.
		/// </summary>
		public static MemberGetter Getter(PropertyInfo propInfo, FasterflectFlags bindingFlags)
		{
			return (MemberGetter) new MemberGetEmitter(propInfo, bindingFlags).GetDelegate();
		}
		#endregion

		#region Setters
		/// <summary>
		/// Creates a delegate which can set the value of the field identified by <paramref name="fieldInfo"/>.
		/// </summary>
		public static MemberSetter Setter(FieldInfo fieldInfo)
		{
			FasterflectFlags flags = fieldInfo.IsStatic ? FasterflectFlags.StaticAnyVisibility : FasterflectFlags.InstanceAnyVisibility;
			return (MemberSetter) new MemberSetEmitter(fieldInfo, flags).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can set the value of the property <paramref name="propInfo"/>.
		/// </summary>
		public static MemberSetter Setter(PropertyInfo propInfo)
		{
			FasterflectFlags flags = propInfo.IsStatic() ? FasterflectFlags.StaticAnyVisibility : FasterflectFlags.InstanceAnyVisibility;
			return (MemberSetter) new MemberSetEmitter(propInfo, flags).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can set the value of the property <param name="propInfo"/> matching the
		/// specified <param name="bindingFlags" />.
		/// </summary>
		public static MemberSetter Setter(PropertyInfo propInfo, FasterflectFlags bindingFlags)
		{
			return (MemberSetter) new MemberSetEmitter(propInfo, bindingFlags).GetDelegate();
		}
		#endregion

		#region Property Access
		/// <summary>
		/// Creates a delegate which can set the value of the property specified by <param name="name"/>
		/// on the given <param name="type"/>.
		/// </summary>
		public static MemberSetter PropertySetter(Type type, string name)
		{
			return PropertySetter(type, name, FasterflectFlags.StaticInstanceAnyVisibility);
		}

		/// <summary>
		/// Creates a delegate which can set the value of the property specified by <param name="name"/>
		/// matching <param name="bindingFlags"/> on the given <param name="type"/>.
		/// </summary>
		public static MemberSetter PropertySetter(Type type, string name, FasterflectFlags bindingFlags)
		{
			CallInfo callInfo = new CallInfo(type, null, bindingFlags, MemberTypes.Property, name, null, null, false);
			return (MemberSetter) new MemberSetEmitter(callInfo).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can get the value of the property specified by <param name="name"/>
		/// on the given <param name="type"/>.
		/// </summary>
		public static MemberGetter PropertyGetter(Type type, string name)
		{
			return PropertyGetter(type, name, FasterflectFlags.StaticInstanceAnyVisibility);
		}

		/// <summary>
		/// Creates a delegate which can get the value of the property specified by <param name="name"/>
		/// matching <param name="bindingFlags"/> on the given <param name="type"/>.
		/// </summary>
		public static MemberGetter PropertyGetter(Type type, string name, FasterflectFlags bindingFlags)
		{
			CallInfo callInfo = new CallInfo(type, null, bindingFlags, MemberTypes.Property, name, null, null, true);
			return (MemberGetter) new MemberGetEmitter(callInfo).GetDelegate();
		}
		#endregion

		#region Field Access
		/// <summary>
		/// Creates a delegate which can set the value of the field specified by <paramref name="name"/> on 
		/// the given <paramref name="type"/>.
		/// </summary>
		public static MemberSetter FieldSetter(Type type, string name)
		{
			return FieldSetter(type, name, FasterflectFlags.StaticInstanceAnyVisibility);
		}

		/// <summary>
		/// Creates a delegate which can get the value of the field specified by <paramref name="name"/> on 
		/// the given <paramref name="type"/>.
		/// </summary>
		public static MemberGetter FieldGetter(Type type, string name)
		{
			return FieldGetter(type, name, FasterflectFlags.StaticInstanceAnyVisibility);
		}

		/// <summary>
		/// Creates a delegate which can set the value of the field specified by <paramref name="name"/> and
		/// matching <paramref name="bindingFlags"/> on the given <paramref name="type"/>.
		/// </summary>
		public static MemberSetter FieldSetter(Type type, string name, FasterflectFlags bindingFlags)
		{
			CallInfo callInfo = new CallInfo(type, null, bindingFlags, MemberTypes.Field, name, null, null, false);
			return (MemberSetter) new MemberSetEmitter(callInfo).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can get the value of the field specified by <paramref name="name"/> and
		/// matching <paramref name="bindingFlags"/> on the given <paramref name="type"/>.
		/// </summary>
		public static MemberGetter FieldGetter(Type type, string name, FasterflectFlags bindingFlags)
		{
			CallInfo callInfo = new CallInfo(type, null, bindingFlags, MemberTypes.Field, name, null, null, true);
			return (MemberGetter) new MemberGetEmitter(callInfo).GetDelegate();
		}
		#endregion

		#region Indexers
		/// <summary>
		/// Creates a delegate which can set an indexer
		/// </summary>
		/// <param name="type">The type which the indexer belongs to.</param>
		/// <param name="parameterTypes">The types of the indexer parameters (must be in the right order), plus
		/// the type of the indexer.</param>
		/// <returns>A delegate which can set an indexer.</returns>
		/// <example>
		/// If the indexer is of type <c>string</c> and accepts one parameter of type <c>int</c>, this 
		/// method should be invoked as follow:
		/// <code>
		/// MethodInvoker invoker = type.DelegateForSetIndexer(new Type[]{typeof(int), typeof(string)});
		/// </code>
		/// </example>
		public static MethodInvoker IndexerSetter(Type type, params Type[] parameterTypes)
		{
			return IndexerSetter(type, FasterflectFlags.InstanceAnyVisibility, parameterTypes);
		}

		/// <summary>
		/// Creates a delegate which can get the value of an indexer.
		/// </summary>
		/// <param name="type">The type which the indexer belongs to.</param>
		/// <param name="parameterTypes">The types of the indexer parameters (must be in the right order).</param>
		/// <returns>The delegate which can get the value of an indexer.</returns>
		public static MethodInvoker IndexerGetter(Type type, params Type[] parameterTypes)
		{
			return IndexerGetter(type, FasterflectFlags.InstanceAnyVisibility, parameterTypes);
		}

		/// <summary>
		/// Creates a delegate which can set an indexer matching <paramref name="bindingFlags"/>.
		/// </summary>
		/// <param name="type">The type which the indexer belongs to.</param>
		/// <param name="bindingFlags">The binding flags used to lookup the indexer.</param>
		/// <param name="parameterTypes">The types of the indexer parameters (must be in the right order), plus
		/// the type of the indexer.</param>
		/// <returns>A delegate which can set an indexer.</returns>
		/// <example>
		/// If the indexer is of type <c>string</c> and accepts one parameter of type <c>int</c>, this 
		/// method should be invoked as follow:
		/// <code>
		/// MethodInvoker invoker = type.DelegateForSetIndexer(new Type[]{typeof(int), typeof(string)});
		/// </code>
		/// </example>
		public static MethodInvoker IndexerSetter(Type type, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			return (MethodInvoker) new MethodInvocationEmitter(type, bindingFlags, Constants.IndexerSetterName, parameterTypes).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can get the value of an indexer matching <paramref name="bindingFlags"/>.
		/// </summary>
		/// <param name="type">The type which the indexer belongs to.</param>
		/// <param name="bindingFlags">The binding flags used to lookup the indexer.</param>
		/// <param name="parameterTypes">The types of the indexer parameters (must be in the right order).</param>
		/// <returns>The delegate which can get the value of an indexer.</returns>
		public static MethodInvoker IndexerGetter(Type type, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			return (MethodInvoker) new MethodInvocationEmitter(type, bindingFlags, Constants.IndexerGetterName, parameterTypes).GetDelegate();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a delegate which can invoke the instance method identified by <paramref name="methodInfo"/>.
		/// </summary>
		public static MethodInvoker Method(MethodInfo methodInfo)
		{
			FasterflectFlags flags = methodInfo.IsStatic ? FasterflectFlags.StaticAnyVisibility : FasterflectFlags.InstanceAnyVisibility;
			return (MethodInvoker) new MethodInvocationEmitter(methodInfo, flags).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can invoke the method <paramref name="name"/> with arguments matching
		/// <paramref name="parameterTypes"/> on the given <paramref name="type"/>.
		/// Leave <paramref name="parameterTypes"/> empty if the method has no arguments.
		/// </summary>
		public static MethodInvoker Method(Type type, string name, params Type[] parameterTypes)
		{
			return Method(type, null, name, FasterflectFlags.StaticInstanceAnyVisibility, parameterTypes);
		}

		/// <summary>
		/// Create a delegate to invoke a generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="Method(Type,string,Type[])"/>
		public static MethodInvoker Method(Type type, Type[] genericTypes, string name, params Type[] parameterTypes)
		{
			return Method(type, genericTypes, name, FasterflectFlags.StaticInstanceAnyVisibility, parameterTypes);
		}

		/// <summary>
		/// Creates a delegate which can invoke the method <paramref name="name"/> with arguments matching
		/// <paramref name="parameterTypes"/> and matching <paramref name="bindingFlags"/> on the given <paramref name="type"/>.
		/// Leave <paramref name="parameterTypes"/> empty if the method has no arguments.
		/// </summary>
		public static MethodInvoker Method(Type type, string name, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			return Method(type, null, name, bindingFlags, parameterTypes);
		}

		/// <summary>
		/// Create a delegate to invoke a generic method.  See the overload with same parameters except for <paramref name="genericTypes"/>.
		/// </summary>
		/// <seealso cref="Method(Type,string,FasterflectFlags,Type[])"/>
		public static MethodInvoker Method(Type type, Type[] genericTypes, string name, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			CallInfo callInfo = new CallInfo(type, genericTypes, bindingFlags, MemberTypes.Method, name, parameterTypes, null, true);
			return (MethodInvoker) new MethodInvocationEmitter(callInfo).GetDelegate();
		}
		#endregion

		#region Array Access
		/// <summary>
		/// Creates a delegate which can set element of <paramref name="arrayType"/>.
		/// </summary>
		public static ArrayElementSetter ArraySetter(Type arrayType)
		{
			return (ArrayElementSetter) new ArraySetEmitter(arrayType).GetDelegate();
		}

		/// <summary>
		/// Creates a delegate which can retrieve element of <paramref name="arrayType"/>.
		/// </summary>
		public static ArrayElementGetter ArrayGetter(Type arrayType)
		{
			return (ArrayElementGetter) new ArrayGetEmitter(arrayType).GetDelegate();
		}
		#endregion

		#region ObjectMappers
		/// <summary>
		/// Creates a delegate that can map values from fields and properties on the source object to fields and properties with the 
		/// same name on the target object.
		/// </summary>
		/// <param name="sourceType">The type of the source object.</param>
		/// <param name="targetType">The type of the target object.</param>
		/// <param name="names">The optional list of member names against which to filter the members that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.PartialNameMatch"/> to 
		/// filter members by substring and <see href="Flags.IgnoreCase"/> to ignore case.</param>
		public static ObjectMapper Mapper(Type sourceType, Type targetType, params string[] names)
		{
			return Mapper(sourceType, targetType, FasterflectFlags.InstanceAnyVisibility, names);
		}

		/// <summary>
		/// Creates a delegate that can map values from fields and properties on the source object to fields and properties with the 
		/// same name on the target object.
		/// </summary>
		/// <param name="sourceType">The type of the source object.</param>
		/// <param name="targetType">The type of the target object.</param>
		/// <param name="bindingFlags">The <see href="Flags"/> used to define the scope when locating members.</param>
		/// <param name="names">The optional list of member names against which to filter the members that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.PartialNameMatch"/> to 
		/// filter members by substring and <see href="Flags.IgnoreCase"/> to ignore case.</param>
		public static ObjectMapper Mapper(Type sourceType, Type targetType, FasterflectFlags bindingFlags, params string[] names)
		{
			const MemberTypes memberTypes = MemberTypes.Field | MemberTypes.Property;
			return Mapper(sourceType, targetType, memberTypes, memberTypes, bindingFlags, names);
		}

		/// <summary>
		/// Creates a delegate that can map values from fields and properties on the source object to fields and properties with the 
		/// same name on the target object.
		/// </summary>
		/// <param name="sourceType">The type of the source object.</param>
		/// <param name="targetType">The type of the target object.</param>
		/// <param name="sourceTypes">The member types (Fields, Properties or both) to include on the source.</param>
		/// <param name="targetTypes">The member types (Fields, Properties or both) to include on the target.</param>
		/// <param name="bindingFlags">The <see href="Flags"/> used to define the scope when locating members. If
		/// <paramref name="sourceTypes"/> is different from <paramref name="targetTypes"/> the flag value
		/// <see cref="FasterflectFlags.IgnoreCase"/> will automatically be applied.</param>
		/// <param name="names">The optional list of member names against which to filter the members that are
		/// to be mapped. If this parameter is <c>null</c> or empty no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-sensitive match. Pass <see href="Flags.PartialNameMatch"/> to 
		/// filter members by substring and <see href="Flags.IgnoreCase"/> to ignore case.</param>
		public static ObjectMapper Mapper(Type sourceType, Type targetType, MemberTypes sourceTypes, MemberTypes targetTypes,
							   FasterflectFlags bindingFlags, params string[] names)
		{
			MapEmitter emitter = new MapEmitter(sourceType, targetType, sourceTypes, targetTypes, bindingFlags, names);
			return (ObjectMapper) emitter.GetDelegate();
		}
		#endregion

		#region Deep Clone
		/// <summary>
		/// Produces a deep clone of the <paramref name="source"/> object. Reference integrity is maintained and
		/// every unique object in the graph is cloned only once.
		/// A current limitation of this method is that all objects in the graph must have a default constructor.
		/// </summary>
		/// <typeparam name="T">The type of the object to clone.</typeparam>
		/// <param name="source">The object to clone.</param>
		/// <returns>A deep clone of the source object.</returns>
		public static T DeepClone<T>(T source) where T : class, new()
		{
			return Fasterflect.Extensions.CloneExtensions.DeepClone<T>(source);
		}
		#endregion
	}
}
