using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System;
using System.Reflection;
using FastMember;
using Sigil;
//using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows;
using Magnum.Reflection;
using BenchmarkDotNet.Diagnosers;
using System.Runtime.CompilerServices;
using Fasterflect;

namespace Benchmark
{
	[MemoryDiagnoser]
	public class Getters
	{
		private static readonly BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		private static readonly string propertyName = "PublicHost";

		private static readonly TestUri testUri;
		private static readonly Object @object;
		private static readonly Type @class;
		private static readonly PropertyInfo property;

		private static readonly FastProperty fastProperty; // Magnum
		private static readonly TypeAccessor fastMember; // FastMember
		private static readonly Func<TestUri, string> funcTstring; // ILEmit
		private static readonly MemberGetter objgetter; // ILEmit (object)
		private static readonly Func<object, object> funcObjObj; // ILEmit (object)
		public static readonly Func<TestUri, string> getDelegate; // Delegate
		public static readonly Delegate getDelegateDynamic; // Dynamic Delegate
		public static readonly MemberGetter ffgetter; // Fasterflect
		public static readonly MethodInfo getgetMethod;

		//private static bool allowNonPublicFieldAccess = false;
		private static readonly bool allowNonPublicFieldAccess = true;

		static Getters()
		{
			testUri = new TestUri("SomeHost");
			@object = testUri;
			@class = testUri.GetType();
			property = @class.GetProperty(propertyName, bindingFlags);
			fastProperty = new FastProperty(property, bindingFlags);
			getgetMethod = property.GetGetMethod(nonPublic: allowNonPublicFieldAccess);

			// Using FastMember - https://github.com/mgravell/fast-member
			fastMember = TypeAccessor.Create(@class, allowNonPublicAccessors: allowNonPublicFieldAccess);

			Type funcType = Type.GetType("System.Func`2[Benchmark.TestUri, System.String]");
			getDelegate = (Func<TestUri, string>)Delegate.CreateDelegate(funcType, getgetMethod);
			getDelegateDynamic = Delegate.CreateDelegate(funcType, getgetMethod);

			var actionType = Type.GetType("System.Action`2[Benchmark.TestUri, System.String]");
			//setDelegate = (Action<TestUri, string>)Delegate.CreateDelegate(actionType, property.GetSetMethod(nonPublic: allowNonPublicFieldAccess));
			//setDelegateDynamic = Delegate.CreateDelegate(actionType, property.GetSetMethod(nonPublic: allowNonPublicFieldAccess));

			//var setterEmiter = Emit<Action<TestUri, string>>
			//	.NewDynamicMethod("SetTestUriProperty")
			//	.LoadArgument(0)
			//	.LoadArgument(1)
			//	.Call(property.GetSetMethod(nonPublic: allowNonPublicFieldAccess))
			//	.Return();
			//setter = setterEmiter.CreateDelegate();

			Emit<Func<TestUri, string>> getterEmiter = Emit<Func<TestUri, string>>
				.NewDynamicMethod("GetTestUriProperty")
				.LoadArgument(0)
				.Call(getgetMethod)
				.Return();
			funcTstring = getterEmiter.CreateDelegate();

			ffgetter = Reflect.PropertyGetter(property);
			//ffsetter = Reflect.PropertySetter(property);

			//var objSetterEmiter = Emit<Action<object, object>>
			//	.NewDynamicMethod("SetObjTestUriProperty")
			//	.LoadArgument(0)
			//	.CastClass(typeof(TestUri))
			//	.LoadArgument(1)
			//	.CastClass(typeof(string))
			//	.Call(property.GetSetMethod(nonPublic: allowNonPublicFieldAccess))
			//	.Return();
			//objsetter = objSetterEmiter.CreateDelegate();

			Emit<MemberGetter> objGetterEmiter = Emit<MemberGetter>
				.NewDynamicMethod("GetObjTestUriProperty")
				.LoadArgument(0)
				.CastClass(typeof(TestUri))
				.Call(property.GetGetMethod(nonPublic: allowNonPublicFieldAccess))
				.Return();
			objgetter = objGetterEmiter.CreateDelegate();

			Emit<Func<object, object>> objGetterEmiterFunc = Emit<Func<object, object>>
				.NewDynamicMethod("GetMSTestUriProperty")
				.LoadArgument(0)
				.CastClass(typeof(TestUri))
				.Call(property.GetGetMethod(nonPublic: allowNonPublicFieldAccess))
				.Return();
			funcObjObj = objGetterEmiterFunc.CreateDelegate();
		}
		
		[Benchmark]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string Direct_Inlining()
		{
			return testUri.PublicHost;
		}

		[Benchmark]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public string Direct_NoInlining()
		{
			return testUri.PublicHost;
		}

		[Benchmark]
		public string Func_Tstring()
		{
			return getDelegate(testUri);
		}


		[Benchmark(Baseline = true)]
		public string Fasterflect()
		{
			return (string)ffgetter(testUri);
		}

		[Benchmark]
		public string ILEmit_MemberGetter()
		{
			return (string)objgetter(testUri);
		}

		[Benchmark]
		public string Magnum_CompiledExpressionTrees()
		{
			return (string)fastProperty.Get(testUri);
		}

		[Benchmark]
		public string ILEmit_FuncTstring()
		{
			return funcTstring(testUri);
		}

		[Benchmark]
		public string ILEmit_FuncObjObj()
		{
			return (string)funcObjObj(testUri);
		}

		[Benchmark]
		public string FastMember()
		{
			return (string)fastMember[testUri, propertyName];
		}

		[Benchmark]
		public string MethodInfoInvoke()
		{
			return (string)getgetMethod.Invoke(testUri, null);
		}

		[Benchmark]
		public string PropertyInfoCached()
		{
			return (string)property.GetValue(testUri, null);
		}

		[Benchmark]
		public string PropertyInfo()
		{
			Type @class = testUri.GetType();
			PropertyInfo property = @class.GetProperty(propertyName, bindingFlags);
			return (string)property.GetValue(testUri, null);
		}

		[Benchmark]
		public string DelegateDynamicInvoke()
		{
			return (string)getDelegateDynamic.DynamicInvoke(testUri);
		}
	}
}