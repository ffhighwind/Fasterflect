#region License

// Copyright © 2010 Buu Nguyen, Morten Mertner
// Copyright © 2018 Wesley Hamilton
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at https://github.com/ffhighwind/fasterflect

#endregion

using Fasterflect.Emitter;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Fasterflect
{
	/// <summary>
	/// Helper class for producing Reflection-based delegates and other utility methods.
	/// </summary>
	public static class Reflect
	{
		private static readonly Cache<FieldInfo, MemberGetter> FieldGetters = new Cache<FieldInfo, MemberGetter>();
		private static readonly Cache<FieldInfo, MemberSetter> FieldSetters = new Cache<FieldInfo, MemberSetter>();
		private static readonly Cache<PropertyInfo, MemberGetter> PropertyGetters = new Cache<PropertyInfo, MemberGetter>();
		private static readonly Cache<PropertyInfo, MemberSetter> PropertySetters = new Cache<PropertyInfo, MemberSetter>();
		private static readonly Cache<ConstructorInfo, ConstructorInvoker> Constructors = new Cache<ConstructorInfo, ConstructorInvoker>();
		private static readonly Cache<MethodInfo, MethodInvoker> Methods = new Cache<MethodInfo, MethodInvoker>();
		private static readonly Cache<Type, ArrayElementSetter> ArraySetters = new Cache<Type, ArrayElementSetter>();
		private static readonly Cache<Type, ArrayElementGetter> ArrayGetters = new Cache<Type, ArrayElementGetter>();
		private static readonly Cache<MethodInfo, MethodInvoker> IndexerSetters = new Cache<MethodInfo, MethodInvoker>();
		private static readonly Cache<MethodInfo, MethodInvoker> IndexerGetters = new Cache<MethodInfo, MethodInvoker>();
		private static readonly Cache<MultiSetCallInfo, MultiSetter> MultiSetters = new Cache<MultiSetCallInfo, MultiSetter>();
		private static readonly Cache<MapCallInfo, ObjectMapper> Mappers = new Cache<MapCallInfo, ObjectMapper>();

		#region Constructor
		/// <summary>
		/// Creates a <see cref="ConstructorInvoker"/> which invokes the given <see cref="ConstructorInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> that the <see cref="ConstructorInvoker"/> will construct.</param>
		/// <param name="parameterTypes">The <see cref="ConstructorInfo"/>'s parameters types.</param>
		/// <returns>A <see cref="ConstructorInvoker"/> which invokes the given <see cref="ConstructorInfo"/>.</returns>
		public static ConstructorInvoker Constructor(Type type, params Type[] parameterTypes)
		{
			ConstructorInfo ctor = ReflectLookup.Constructor(type, parameterTypes);
			ConstructorInvoker value = Constructor(ctor);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="ConstructorInvoker"/> which invokes the given <see cref="ConstructorInfo"/>.
		/// </summary>
		/// <param name="ctor">The <see cref="ConstructorInfo"/> that the <see cref="ConstructorInvoker"/> will invoke.</param>
		/// <returns>A <see cref="ConstructorInvoker"/> which invokes the given <see cref="ConstructorInfo"/>.</returns>
		public static ConstructorInvoker Constructor(ConstructorInfo ctor)
		{
			ConstructorInvoker value = Constructors.Get(ctor);
			if (value != null)
				return value;
			value = (ConstructorInvoker)new CtorInvocationEmitter(ctor).GetDelegate();
			Constructors.Insert(ctor, value);
			return value;
		}
		#endregion

		#region Getters
		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given member.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the member.</param>
		/// <param name="name">The name of the member.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given member.</returns>
		public static MemberGetter Getter(Type type, string name)
		{
			MemberInfo member = type.GetMember(name, BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault();
			MemberGetter value = Getter(member);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given member.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the member.</param>
		/// <param name="name">The name of the member.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> to filter the member.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given member.</returns>
		public static MemberGetter Getter(Type type, string name, FasterflectFlags bindingFlags)
		{
			MemberInfo member = ReflectLookup.Member(type, name, bindingFlags);
			MemberGetter value = Getter(member);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given member.
		/// </summary>
		/// <param name="memberInfo">The <see cref="MemberInfo"/> whos value will be returned.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given member.</returns>
		public static MemberGetter Getter(MemberInfo memberInfo)
		{
			if (memberInfo.MemberType == MemberTypes.Field) {
				return FieldGetter((FieldInfo)memberInfo);
			}
			return PropertyGetter((PropertyInfo)memberInfo);
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given <see cref="FieldInfo"/>.
		/// </summary>
		/// <param name="field">The <see cref="FieldInfo"/> whose value will be returned.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given <see cref="FieldInfo"/>.</returns>
		public static MemberGetter FieldGetter(FieldInfo field)
		{
			MemberGetter value = FieldGetters.Get(field);
			if (value != null)
				return value;
			value = (MemberGetter)new MemberGetEmitter(field).GetDelegate();
			FieldGetters.Insert(field, value);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given <see cref="FieldInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the <see cref="FieldInfo"/>.</param>
		/// <param name="name">The name of the <see cref="FieldInfo"/>.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given <see cref="FieldInfo"/>.</returns>
		public static MemberGetter FieldGetter(Type type, string name)
		{
			MemberInfo member = type.GetMember(name, BindingFlags.GetField | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault();
			MemberGetter value = FieldGetter((FieldInfo)member);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given <see cref="FieldInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the <see cref="FieldInfo"/>.</param>
		/// <param name="name">The name of the <see cref="FieldInfo"/>.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> to filter the <see cref="FieldInfo"/>.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given <see cref="FieldInfo"/>.</returns>
		public static MemberGetter FieldGetter(Type type, string name, FasterflectFlags bindingFlags)
		{
			FieldInfo field = ReflectLookup.Field(type, name, bindingFlags);
			MemberGetter value = FieldGetter(field);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="property">The <see cref="PropertyInfo"/> whose value will be returned.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberGetter PropertyGetter(PropertyInfo property)
		{
			MemberGetter value = PropertyGetters.Get(property);
			if (value != null)
				return value;
			value = (MemberGetter)new MemberGetEmitter(property).GetDelegate();
			PropertyGetters.Insert(property, value);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the <see cref="PropertyInfo"/>.</param>
		/// <param name="name">The name of the <see cref="PropertyInfo"/>.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberGetter PropertyGetter(Type type, string name)
		{
			MemberInfo member = type.GetMember(name, BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault();
			MemberGetter value = PropertyGetter((PropertyInfo)member);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberGetter"/> which gets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> of the <see cref="PropertyInfo"/>.</param>
		/// <param name="name">The name of the <see cref="PropertyInfo"/>.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> to filter the <see cref="PropertyInfo"/>.</param>
		/// <returns>A <see cref="MemberGetter"/> which gets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberGetter PropertyGetter(Type type, string name, FasterflectFlags bindingFlags)
		{
			PropertyInfo property = ReflectLookup.Property(type, name, bindingFlags);
			MemberGetter value = PropertyGetter(property);
			return value;
		}
		#endregion

		#region Setters
		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given member.
		/// </summary>
		/// <param name="memberInfo">The <see cref="MemberInfo"/> whos value will be set.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given member.</returns>
		public static MemberSetter Setter(MemberInfo memberInfo)
		{
			if (memberInfo.MemberType == MemberTypes.Field) {
				return FieldSetter((FieldInfo)memberInfo);
			}
			return PropertySetter((PropertyInfo)memberInfo);
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given member.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose member will be set.</param>
		/// <param name="name">The name of the member to set.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given member.</returns>
		public static MemberSetter Setter(Type type, string name)
		{
			MemberInfo member = type.GetMember(name, BindingFlags.SetProperty | BindingFlags.SetField | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault();
			MemberSetter value = Setter(member);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given member.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose member will be set.</param>
		/// <param name="name">The name of the member to set.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> to filter the member.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given member.</returns>
		public static MemberSetter Setter(Type type, string name, FasterflectFlags bindingFlags)
		{
			MemberInfo member = ReflectLookup.Member(type, name, bindingFlags);
			MemberSetter value = Setter(member);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="FieldInfo"/>.
		/// </summary>
		/// <param name="field">The <see cref="FieldInfo"/> whose value will be set.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given <see cref="FieldInfo"/>.</returns>
		public static MemberSetter FieldSetter(FieldInfo field)
		{
			MemberSetter value = FieldSetters.Get(field);
			if (value != null)
				return value;
			value = (MemberSetter)new MemberSetEmitter(field).GetDelegate();
			FieldSetters.Insert(field, value);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="property">The <see cref="PropertyInfo"/> whose value will be set.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberSetter PropertySetter(PropertyInfo property)
		{
			MemberSetter value = PropertySetters.Get(property);
			if (value != null)
				return value;
			value = (MemberSetter)new MemberSetEmitter(property).GetDelegate();
			PropertySetters.Insert(property, value);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose member will be set.</param>
		/// <param name="name">The name of the member to set.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberSetter PropertySetter(Type type, string name)
		{
			MemberInfo member = type.GetMember(name, BindingFlags.SetProperty | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault();
			MemberSetter value = PropertySetter((PropertyInfo)member);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose <see cref="PropertyInfo"/> will be set.</param>
		/// <param name="name">The name of the <see cref="PropertyInfo"/> to set.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> to filter the member.</param>
		/// <returns>A<see cref="MemberSetter"/> which sets the value of the given <see cref="PropertyInfo"/>.</returns>
		public static MemberSetter PropertySetter(Type type, string name, FasterflectFlags bindingFlags)
		{
			PropertyInfo property = ReflectLookup.Property(type, name, bindingFlags);
			MemberSetter value = PropertySetter(property);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="FieldInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="FieldInfo"/> whose value will be set.</param>
		/// <param name="name">The name of the <see cref="FieldInfo"/> to set.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given <see cref="FieldInfo"/>.</returns>
		public static MemberSetter FieldSetter(Type type, string name)
		{
			MemberInfo member = type.GetMember(name, BindingFlags.SetField | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault();
			MemberSetter value = FieldSetter((FieldInfo)member);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MemberSetter"/> which sets the value of the given <see cref="FieldInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="FieldInfo"/> whose value will be set.</param>
		/// <param name="name">The name of the <see cref="FieldInfo"/> to set.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> to filter the <see cref="FieldInfo"/>.</param>
		/// <returns>A <see cref="MemberSetter"/> which sets the value of the given <see cref="FieldInfo"/>.</returns>
		public static MemberSetter FieldSetter(Type type, string name, FasterflectFlags bindingFlags)
		{
			FieldInfo field = ReflectLookup.Field(type, name, bindingFlags);
			MemberSetter value = FieldSetter(field);
			return value;
		}
		#endregion

		#region MultiSetter
		/// <summary>
		/// Creates a <see cref="Fasterflect.MultiSetter"/> which sets the values of the given members.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose members will be set.</param>
		/// <param name="memberNames">The names of the members to set.</param>
		/// <returns>A <see cref="Fasterflect.MultiSetter"/> which sets the values of the given members.</returns>
		public static MultiSetter MultiSetter(Type type, params string[] memberNames)
		{
			return MultiSetter(type, FasterflectFlags.StaticInstanceAnyVisibility | BindingFlags.SetProperty | BindingFlags.SetField, memberNames);
		}

		/// <summary>
		/// Creates a <see cref="Fasterflect.MultiSetter"/> which sets the values of the given members.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose members will be set.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> to filter the members.</param>
		/// <param name="memberNames">The names of the members to set.</param>
		/// <returns>A <see cref="Fasterflect.MultiSetter"/> which sets the values of the given members.</returns>
		public static MultiSetter MultiSetter(Type type, FasterflectFlags bindingFlags, params string[] memberNames)
		{
			IList<MemberInfo> members = ReflectLookup.Members(type, MemberTypes.Field | MemberTypes.Property, bindingFlags, memberNames);
			MultiSetter value = MultiSetter(type, members);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="Fasterflect.MultiSetter"/> which sets the values of the given members.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> whose members will be set.</param>
		/// <param name="members">The members to set.</param>
		/// <returns>A <see cref="Fasterflect.MultiSetter"/> which sets the values of the given members.</returns>
		public static MultiSetter MultiSetter(Type type, IList<MemberInfo> members)
		{
			MultiSetCallInfo callInfo = new MultiSetCallInfo(type, members);
			MultiSetter value = MultiSetters.Get(callInfo);
			if (value != null)
				return value;
			value = (MultiSetter)new MultiSetEmitter(callInfo).GetDelegate();
			MultiSetters.Insert(callInfo, value);
			return value;
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
			return IndexerGetter(type, FasterflectFlags.InstanceAnyVisibility | BindingFlags.InvokeMethod, parameterTypes);
		}

		/// <summary>
		/// Creates a delegate which can get the value of an indexer matching <paramref name="bindingFlags"/>.
		/// </summary>
		/// <param name="type">The type which the indexer belongs to.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> used to lookup the indexer.</param>
		/// <param name="parameterTypes">The types of the indexer parameters (must be in the right order).</param>
		/// <returns>The delegate which can get the value of an indexer.</returns>
		public static MethodInvoker IndexerGetter(Type type, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			MethodInfo method = ReflectLookup.Method(type, "[]=", parameterTypes, bindingFlags);
			MethodInvoker value = IndexerGetter(method);
			return value;
		}

		internal static MethodInvoker IndexerGetter(MethodInfo method)
		{
			MethodInvoker value = IndexerGetters.Get(method);
			if (value != null)
				return value;
			value = (MethodInvoker)new MethodInvocationEmitter(method).GetDelegate();
			IndexerGetters.Insert(method, value);
			return value;
		}

		/// <summary>
		/// Creates a delegate which can set an indexer
		/// </summary>
		/// <param name="type">The type which the indexer belongs to.</param>
		/// <param name="parameterTypes">The types of the indexer parameters (must be in the right order), plus
		/// the type of the indexer.</param>
		/// <returns>A delegate which can set an indexer.</returns>
		public static MethodInvoker IndexerSetter(Type type, params Type[] parameterTypes)
		{
			return IndexerSetter(type, FasterflectFlags.InstanceAnyVisibility | BindingFlags.InvokeMethod, parameterTypes);
		}

		/// <summary>
		/// Creates a delegate which can set an indexer matching <paramref name="bindingFlags"/>.
		/// </summary>
		/// <param name="type">The type which the indexer belongs to.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> used to lookup the indexer.</param>
		/// <param name="parameterTypes">The types of the indexer parameters (must be in the right order), plus
		/// the type of the indexer.</param>
		/// <returns>A delegate which can set an indexer.</returns>
		/// <example>
		/// If the indexer is of type <see cref="string"/> and accepts one parameter of type <see langword="int"/>, this 
		/// method should be invoked as follow:
		/// <code>
		/// MethodInvoker invoker = type.DelegateForSetIndexer(new Type[]{typeof(int), typeof(string)});
		/// </code>
		/// </example>
		public static MethodInvoker IndexerSetter(Type type, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			MethodInfo method = ReflectLookup.Method(type, "=[]", parameterTypes, bindingFlags);
			MethodInvoker value = IndexerSetter(method);
			return value;
		}

		internal static MethodInvoker IndexerSetter(MethodInfo method)
		{
			MethodInvoker value = IndexerSetters.Get(method);
			if (value != null)
				return value;
			value = (MethodInvoker)new MethodInvocationEmitter(method).GetDelegate();
			IndexerSetters.Insert(method, value);
			return value;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.
		/// </summary>
		/// <param name="method">The <see cref="MethodInfo"/> to invoke.</param>
		/// <returns>A <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.</returns>
		public static MethodInvoker Method(MethodInfo method)
		{
			MethodInvoker value = Methods.Get(method);
			if (value != null)
				return value;
			value = (MethodInvoker)new MethodInvocationEmitter(method).GetDelegate();
			IndexerGetters.Insert(method, value);
			return value;
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
			MethodInfo method = ReflectLookup.Method(type, name, parameterTypes);
			MethodInvoker value = Method(method);
			return value;
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
			MethodInfo method = ReflectLookup.Method(type, genericTypes, name, parameterTypes);
			MethodInvoker value = Method(method);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> that the object that has the <see cref="MethodInfo"/>.</param>
		/// <param name="name">The name of the <see cref="MethodInfo"/>.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> to filter the <see cref="MethodInfo"/>.</param>
		/// <param name="parameterTypes">The <see cref="Type"/>s of the <see cref="MethodInfo"/>'s parameters.</param>
		/// <returns>A <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.</returns>
		public static MethodInvoker Method(Type type, string name, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			MethodInfo method = ReflectLookup.Method(type, name, parameterTypes, bindingFlags);
			MethodInvoker value = Method(method);
			return value;
		}

		/// <summary>
		/// Creates a <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.
		/// </summary>
		/// <param name="type">The <see cref="Type"/> that the object that has the <see cref="MethodInfo"/>.</param>
		/// <param name="name">The name of the <see cref="MethodInfo"/>.</param>
		/// <param name="bindingFlags">The <see cref="BindingFlags"/> or <see cref="FasterflectFlags"/> to filter the <see cref="MethodInfo"/>.</param>
		/// <param name="genericTypes">The generic <see cref="Type"/>s of the <see cref="MethodInfo"/>'s parameters.</param>
		/// <param name="parameterTypes">The <see cref="Type"/>s of the <see cref="MethodInfo"/>'s parameters.</param>
		/// <returns>A <see cref="MethodInvoker"/> which invokes the given <see cref="MethodInfo"/>.</returns>
		public static MethodInvoker Method(Type type, Type[] genericTypes, string name, FasterflectFlags bindingFlags, params Type[] parameterTypes)
		{
			MethodInfo method = ReflectLookup.Method(type, genericTypes, name, parameterTypes, bindingFlags);
			MethodInvoker value = Method(method);
			return value;
		}
		#endregion

		#region Array Access
		/// <summary>
		/// Creates an <see cref="ArrayElementGetter"/> which retrieves an element of an array.
		/// </summary>
		/// <param name="arrayType">The <see cref="Type"/> of the array's elements.</param>
		/// <returns>An <see cref="ArrayElementGetter"/> which retrieves an element of an array.</returns>
		public static ArrayElementGetter ArrayGetter(Type arrayType)
		{
			ArrayElementGetter value = ArrayGetters.Get(arrayType);
			if (value != null)
				return value;
			value = (ArrayElementGetter)new ArrayGetEmitter(arrayType).GetDelegate();
			ArrayGetters.Insert(arrayType, value);
			return value;
		}

		/// <summary>
		/// Creates an <see cref="ArrayElementGetter"/> which sets an element of an array.
		/// </summary>
		/// <param name="arrayType">The <see cref="Type"/> of the array's elements.</param>
		/// <returns>An <see cref="ArrayElementGetter"/> which sets an element of an array.</returns>
		public static ArrayElementSetter ArraySetter(Type arrayType)
		{
			ArrayElementSetter value = ArraySetters.Get(arrayType);
			if (value != null)
				return value;
			value = (ArrayElementSetter)new ArraySetEmitter(arrayType).GetDelegate();
			ArraySetters.Insert(arrayType, value);
			return value;
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
		/// to be mapped. If this parameter is an empty list then no name filtering will be applied. The default 
		/// behavior is to check for an exact, case-sensitive match.</param>
		/// <returns>An <see cref="ObjectMapper"/> which sets the target using the matching source members.</returns>
		public static ObjectMapper Mapper(Type sourceType, Type targetType, params string[] names)
		{
			ObjectMapper value = Mapper(sourceType, targetType, FasterflectFlags.InstanceAnyVisibility, names, Constants.EmptyStringArray);
			return value;
		}

		/// <summary>
		/// Creates a delegate that can map values from fields and properties on the source object to fields and properties with the 
		/// same name on the target object.
		/// </summary>
		/// <param name="sourceType">The type of the source object.</param>
		/// <param name="targetType">The type of the target object.</param>
		/// <param name="sourceNames">The member names (Fields, Properties or both) to include on the source.</param>
		/// <param name="targetNames">The member names (Fields, Properties or both) to include on the target.</param>
		/// <returns>An <see cref="ObjectMapper"/> which sets the target using the matching source members.</returns>
		public static ObjectMapper Mapper(Type sourceType, Type targetType, string[] sourceNames, string[] targetNames)
		{
			ObjectMapper value = Mapper(sourceType, targetType, FasterflectFlags.InstanceAnyVisibility, sourceNames, targetNames);
			return value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourceType">The type of the source object.</param>
		/// <param name="targetType">The type of the target object.</param>
		/// <param name="bindingFlags"></param>
		/// <param name="names">The optional list of member names against which to filter the members that are
		/// to be mapped. If this parameter is an empty string then no name filtering will be applied.</param>
		/// <returns>An <see cref="ObjectMapper"/> which sets the target using the matching source members.</returns>
		/// <returns></returns>
		public static ObjectMapper Mapper(Type sourceType, Type targetType, FasterflectFlags bindingFlags, params string[] names)
		{
			ObjectMapper value = Mapper(sourceType, targetType, bindingFlags, names, names);
			return value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourceType">The type of the source object.</param>
		/// <param name="targetType">The type of the target object.</param>
		/// <param name="bindingFlags"></param>
		/// <param name="sourceNames">The member names (Fields, Properties or both) to include on the source.</param>
		/// <param name="targetNames">The member names (Fields, Properties or both) to include on the target.</param>
		/// <returns>An <see cref="ObjectMapper"/> which sets the target using the matching source members.</returns>
		internal static ObjectMapper Mapper(Type sourceType, Type targetType, FasterflectFlags bindingFlags, string[] sourceNames, string[] targetNames)
		{
			MapCallInfo callInfo = new MapCallInfo(sourceType, targetType, bindingFlags, sourceNames, targetNames);
			ObjectMapper value = Mappers.Get(callInfo);
			if (value != null)
				return value;
			value = (ObjectMapper)new MapEmitter(callInfo).GetDelegate();
			Mappers.Insert(callInfo, value);
			return value;
		}
		#endregion

		#region Clone
		/// <summary>
		/// Produces a deep clone of the <paramref name="source"/> object. Reference integrity is maintained and
		/// every unique object in the graph is cloned only once. All objects in the graph must have a default constructor.
		/// </summary>
		/// <typeparam name="T">The type of the object to clone.</typeparam>
		/// <param name="source">The object to clone.</param>
		/// <returns>A deep clone of the source object.</returns>
		public static T DeepClone<T>(T source) where T : class, new()
		{
			T clone = Extensions.DeepClone.DeepCloneExtensions.DeepClone(source);
			return clone;
		}

		/// <summary>
		/// Clones an object via shallow copy.
		/// </summary>
		/// <typeparam name="T">The type of object to clone.</typeparam>
		/// <param name="obj">The object to clone</param>
		/// <returns>A shallow clone of the source object.</returns>
		public static T ShallowClone<T>(T obj) where T : class
		{
			if (obj == null)
				return null;
			MethodInfo inst = obj.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
			T clone = (T)inst?.Invoke(obj, null);
			return clone;
		}
		#endregion
	}
}
