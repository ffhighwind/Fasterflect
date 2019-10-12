using System;
using System.Reflection;
using Fasterflect.Emitter;
using Fasterflect.Extensions;

namespace Fasterflect
{
	/// <summary>
	/// Helper class for producing Reflection-based delegates and other utility methods.
	/// </summary>
	public static class Reflect
	{
		#region Constructor
		/// <summary>
		/// Creates a <see cref="ConstructorInvoker"/> which invokes the given <see cref="ConstructorInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> that the <see cref="ConstructorInvoker"/> will construct.</param>
		/// <param name="parameterTypes">The <see cref="ConstructorInfo"/>'s parameters types.</param>
		/// <returns>A <see cref="ConstructorInvoker"/> which invokes the given <see cref="ConstructorInfo"/>.</returns>
		public static ConstructorInvoker Constructor(Type type, params Type[] parameterTypes)
		{
			return Constructor(type, FasterflectFlags.InstanceAnyVisibility, parameterTypes);
		}

		/// <summary>
		/// Creates a <see cref="ConstructorInvoker"/> which invokes the given <see cref="ConstructorInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> that the <see cref="ConstructorInvoker"/> will construct.</param>
		/// <param name="bindingFlags">The <see cref="FasterflectFlags"/> to use when searching for the <see cref="ConstructorInfo"/>.</param>
		/// <param name="parameterTypes">The <see cref="ConstructorInfo"/>'s parameters types.</param>
		/// <returns>A <see cref="ConstructorInvoker"/> which invokes the given <see cref="ConstructorInfo"/>.</returns>
		public static ConstructorInvoker Constructor(Type type, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			return (ConstructorInvoker) new CtorInvocationEmitter(type, bindingFlags, parameterTypes).GetDelegate();
		}

		/// <summary>
		/// Creates a <see cref="ConstructorInvoker"/> which invokes the given <see cref="ConstructorInfo"/>.
		/// </summary>
		/// <param name="ctor">The <see cref="ConstructorInfo"/> that the <see cref="ConstructorInvoker"/> will invoke.</param>
		/// <returns>A <see cref="ConstructorInvoker"/>gate which invokes the given <see cref="ConstructorInfo"/>.</returns>
		public static ConstructorInvoker Constructor(ConstructorInfo ctor)
		{
			return (ConstructorInvoker) new CtorInvocationEmitter(ctor, FasterflectFlags.InstanceAnyDeclaredOnly).GetDelegate();
		}
		#endregion

		#region Getters
		internal static MemberGetter Getter(Type type, string name, MemberTypes memberType, FasterflectFlags bindingFlags)
		{
			return (MemberGetter) new MemberGetEmitter(type, bindingFlags, memberType, name).GetDelegate();
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given member.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the member.</param>
		/// <param name="name">The name of the member.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given member.</returns>
		public static MemberGetter Getter(Type type, string name)
		{
			return Getter(type, name, MemberTypes.Field | MemberTypes.Property, FasterflectFlags.StaticInstanceAnyVisibility);
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given member.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the member.</param>
		/// <param name="name">The name of the member.</param>
		/// <param name="bindingFlags">The <see cref="FasterflectFlags"/> to filter the member.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given member.</returns>
		public static MemberGetter Getter(Type type, string name, FasterflectFlags bindingFlags)
		{
			return Getter(type, name, MemberTypes.Field | MemberTypes.Property, bindingFlags);
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given <see cref="FieldInfo"/>.
		/// </summary>
		/// <param name="fieldInfo">The <see cref="FieldInfo"/> whose value will be returned.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given <see cref="FieldInfo"/>.</returns>
		public static MemberGetter FieldGetter(FieldInfo fieldInfo)
		{
			FasterflectFlags bindingFlags = fieldInfo.IsStatic ? FasterflectFlags.StaticAnyVisibility : FasterflectFlags.InstanceAnyVisibility;
			return (MemberGetter) new MemberGetEmitter(fieldInfo, bindingFlags).GetDelegate();
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="propInfo">The <see cref="PropertyInfo"/> whose value will be returned.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberGetter PropertyGetter(PropertyInfo propInfo)
		{
			FasterflectFlags bindingFlags = propInfo.IsStatic() ? FasterflectFlags.StaticAnyVisibility : FasterflectFlags.InstanceAnyVisibility;
			return (MemberGetter) new MemberGetEmitter(propInfo, bindingFlags).GetDelegate();
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the <see cref="PropertyInfo"/>.</param>
		/// <param name="name">The name of the <see cref="PropertyInfo"/>.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberGetter PropertyGetter(Type type, string name)
		{
			return Getter(type, name, MemberTypes.Property, FasterflectFlags.StaticInstanceAnyVisibility);
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the <see cref="PropertyInfo"/>.</param>
		/// <param name="name">The name of the <see cref="PropertyInfo"/>.</param>
		/// <param name="bindingFlags">The <see cref="FasterflectFlags"/> to filter the <see cref="PropertyInfo"/>.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberGetter PropertyGetter(Type type, string name, FasterflectFlags bindingFlags)
		{
			return Getter(type, name, MemberTypes.Property, bindingFlags);
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given <see cref="FieldInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the <see cref="FieldInfo"/>.</param>
		/// <param name="name">The name of the <see cref="FieldInfo"/>.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given <see cref="FieldInfo"/>.</returns>
		public static MemberGetter FieldGetter(Type type, string name)
		{
			return Getter(type, name, MemberTypes.Field, FasterflectFlags.StaticInstanceAnyVisibility);
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given <see cref="FieldInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the <see cref="FieldInfo"/>.</param>
		/// <param name="name">The name of the <see cref="FieldInfo"/>.</param>
		/// <param name="bindingFlags">The <see cref="FasterflectFlags"/> to filter the <see cref="FieldInfo"/>.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given <see cref="FieldInfo"/>.</returns>
		public static MemberGetter FieldGetter(Type type, string name, FasterflectFlags bindingFlags)
		{
			return Getter(type, name, MemberTypes.Field, bindingFlags);
		}
		#endregion

		#region Setters
		internal static MemberSetter Setter(Type type, string name, MemberTypes memberType, FasterflectFlags bindingFlags)
		{
			CallInfo callInfo = new CallInfo(type, null, bindingFlags, memberType, name, null, null, false);
			return (MemberSetter) new MemberSetEmitter(callInfo).GetDelegate();
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given member.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose member will be set.</param>
		/// <param name="name">The name of the member to set.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given member.</returns>
		public static MemberSetter Setter(Type type, string name)
		{
			return Setter(type, name, MemberTypes.Field | MemberTypes.Property, FasterflectFlags.StaticInstanceAnyVisibility);
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given member.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose member will be set.</param>
		/// <param name="name">The name of the member to set.</param>
		/// <param name="bindingFlags">The <see cref="FasterflectFlags"/> to filter the member.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given member.</returns>
		public static MemberSetter Setter(Type type, string name, FasterflectFlags bindingFlags)
		{
			return Setter(type, name, MemberTypes.Field | MemberTypes.Property, FasterflectFlags.StaticInstanceAnyVisibility);
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="FieldInfo"/>.
		/// </summary>
		/// <param name="fieldInfo">The <see cref="FieldInfo"/> whose value will be set.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given <see cref="FieldInfo"/>.</returns>
		public static MemberSetter FieldSetter(FieldInfo fieldInfo)
		{
			FasterflectFlags bindingFlags = fieldInfo.IsStatic ? FasterflectFlags.StaticAnyVisibility : FasterflectFlags.InstanceAnyVisibility;
			return (MemberSetter) new MemberSetEmitter(fieldInfo, bindingFlags).GetDelegate();
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="propInfo">The <see cref="PropertyInfo"/> whose value will be set.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberSetter PropertySetter(PropertyInfo propInfo)
		{
			FasterflectFlags bindingFlags = propInfo.IsStatic() ? FasterflectFlags.StaticAnyVisibility : FasterflectFlags.InstanceAnyVisibility;
			return (MemberSetter) new MemberSetEmitter(propInfo, bindingFlags).GetDelegate();
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="propInfo">The <see cref="PropertyInfo"/> whose value will be set.</param>
		/// <param name="bindingFlags">The <see cref="FasterflectFlags"/> to filter the <see cref="PropertyInfo"/>.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberSetter PropertySetter(PropertyInfo propInfo, FasterflectFlags bindingFlags)
		{
			return (MemberSetter) new MemberSetEmitter(propInfo, bindingFlags).GetDelegate();
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose member will be set.</param>
		/// <param name="name">The name of the member to set.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberSetter PropertySetter(Type type, string name)
		{
			return Setter(type, name, MemberTypes.Property, FasterflectFlags.StaticInstanceAnyVisibility);
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose <see cref="PropertyInfo"/> will be set.</param>
		/// <param name="name">The name of the <see cref="PropertyInfo"/> to set.</param>
		/// <param name="bindingFlags">The <see cref="FasterflectFlags"/> to filter the member.</param>
		/// <returns>A<see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberSetter PropertySetter(Type type, string name, FasterflectFlags bindingFlags)
		{
			return Setter(type, name, MemberTypes.Property, bindingFlags);
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="FieldInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="FieldInfo"/> whose value will be set.</param>
		/// <param name="name">The name of the <see cref="FieldInfo"/> to set.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given <see cref="FieldInfo"/>.</returns>
		public static MemberSetter FieldSetter(Type type, string name)
		{
			return Setter(type, name, MemberTypes.Field, FasterflectFlags.StaticInstanceAnyVisibility);
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="FieldInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="FieldInfo"/> whose value will be set.</param>
		/// <param name="name">The name of the <see cref="FieldInfo"/> to set.</param>
		/// <param name="bindingFlags">The <see cref="FasterflectFlags"/> to filter the <see cref="FieldInfo"/>.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given <see cref="FieldInfo"/>.</returns>
		public static MemberSetter FieldSetter(Type type, string name, FasterflectFlags bindingFlags)
		{
			return Setter(type, name, MemberTypes.Field, bindingFlags);
		}
		#endregion

		#region MultiSetter
		/// <summary>
		/// Creates a <see cref="MultiMemberSetter"/> which sets the values of the given members.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose members will be set.</param>
		/// <param name="memberNames">The names of the members to set.</param>
		/// <returns>A <see cref="MultiMemberSetter"/> which sets the values of the given members.</returns>
		public static MultiMemberSetter MultiSetter(Type type, params string[] memberNames)
		{
			return MultiSetter(type, FasterflectFlags.InstancePublicDeclaredOnly, memberNames);
		}

		/// <summary>
		/// Creates a <see cref="MultiMemberSetter"/> which sets the values of the given members.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose members will be set.</param>
		/// <param name="bindingFlags">The <see cref="FasterflectFlags"/> to filter the members.</param>
		/// <param name="memberNames">The names of the members to set.</param>
		/// <returns>A <see cref="MultiMemberSetter"/> which sets the values of the given members.</returns>
		public static MultiMemberSetter MultiSetter(Type type, FasterflectFlags bindingFlags, params string[] memberNames)
		{
			return (MultiMemberSetter) new MultiSetEmitter(type, bindingFlags, memberNames).GetDelegate();
		}
		#endregion

		#region Indexers
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
		#endregion

		#region Methods
		/// <summary>
		/// Creates a <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.
		/// </summary>
		/// <param name="methodInfo">The <see cref="MethodInfo"/> to invoke.</param>
		/// <returns>A <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.</returns>
		public static MethodInvoker Method(MethodInfo methodInfo)
		{
			FasterflectFlags bindingFlags = methodInfo.IsStatic ? FasterflectFlags.StaticAnyVisibility : FasterflectFlags.InstanceAnyVisibility;
			return (MethodInvoker) new MethodInvocationEmitter(methodInfo, bindingFlags).GetDelegate();
		}

		/// <summary>
		/// Creates a <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> that the object that has the <see cref="MethodInfo"/>.</param>
		/// <param name="name">The name of the <see cref="MethodInfo"/>.</param>
		/// <param name="parameterTypes">The <see cref="Type"/>s of <see cref="MethodInfo"/>'s parameters.</param>
		/// <returns>A <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.</returns>
		public static MethodInvoker Method(Type type, string name, params Type[] parameterTypes)
		{
			return Method(type, null, name, FasterflectFlags.StaticInstanceAnyVisibility, parameterTypes);
		}

		/// <summary>
		/// Creates a <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> that the object that has the <see cref="MethodInfo"/>.</param>
		/// <param name="name">The name of the <see cref="MethodInfo"/>.</param>
		/// <param name="genericTypes">The generic <see cref="Type"/>s of the <see cref="MethodInfo"/>'s parameters.</param>
		/// <param name="parameterTypes">The <see cref="Type"/>s of the <see cref="MethodInfo"/>'s parameters.</param>
		/// <returns>A <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.</returns>
		public static MethodInvoker Method(Type type, string name, Type[] genericTypes, params Type[] parameterTypes)
		{
			return Method(type, genericTypes, name, FasterflectFlags.StaticInstanceAnyVisibility, parameterTypes);
		}

		/// <summary>
		/// Creates a <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> that the object that has the <see cref="MethodInfo"/>.</param>
		/// <param name="name">The name of the <see cref="MethodInfo"/>.</param>
		/// <param name="bindingFlags">The <see cref="FasterflectFlags"/> to filter the <see cref="MethodInfo"/>.</param>
		/// <param name="parameterTypes">The <see cref="Type"/>s of the <see cref="MethodInfo"/>'s parameters.</param>
		/// <returns>A <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.</returns>
		public static MethodInvoker Method(Type type, string name, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			return Method(type, null, name, bindingFlags, parameterTypes);
		}

		/// <summary>
		/// Creates a <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> that the object that has the <see cref="MethodInfo"/>.</param>
		/// <param name="name">The name of the <see cref="MethodInfo"/>.</param>
		/// <param name="bindingFlags">The <see cref="FasterflectFlags"/> to filter the <see cref="MethodInfo"/>.</param>
		/// <param name="genericTypes">The generic <see cref="Type"/>s of the <see cref="MethodInfo"/>'s parameters.</param>
		/// <param name="parameterTypes">The <see cref="Type"/>s of the <see cref="MethodInfo"/>'s parameters.</param>
		/// <returns>A <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.</returns>
		public static MethodInvoker Method(Type type, Type[] genericTypes, string name, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			CallInfo callInfo = new CallInfo(type, genericTypes, bindingFlags, MemberTypes.Method, name, parameterTypes, null, true);
			return (MethodInvoker) new MethodInvocationEmitter(callInfo).GetDelegate();
		}
		#endregion

		#region Array Access
		/// <summary>
		/// Creates a <see cref="ArrayElementGetter"/> which retrieves an element of an array.
		/// </summary>
		/// <param name="arrayType">The <see cref="Type"/> of the array's elements.</param>
		/// <returns>A <see cref="ArrayElementGetter"/> which retrieves an element of an array.</returns>
		public static ArrayElementGetter ArrayGetter(Type arrayType)
		{
			return (ArrayElementGetter) new ArrayGetEmitter(arrayType).GetDelegate();
		}

		/// <summary>
		/// Creates a <see cref="ArrayElementGetter"/> which sets an element of an array.
		/// </summary>
		/// <param name="arrayType">The <see cref="Type"/> of the array's elements.</param>
		/// <returns>A <see cref="ArrayElementGetter"/> which sets an element of an array.</returns>
		public static ArrayElementSetter ArraySetter(Type arrayType)
		{
			return (ArrayElementSetter) new ArraySetEmitter(arrayType).GetDelegate();
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
