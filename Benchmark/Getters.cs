#region License

// The MIT License
// 
// Copyright (c) 2016 Matt Warren
// Copyright (c) 2019 Wesley Hamilton
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 
// The latest version of this file can be found at https://github.com/ffhighwind/fasterflect

#endregion

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Fasterflect;
using FastMember;
using Magnum.Reflection;
using Sigil;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Benchmark
{
	[MemoryDiagnoser]
	public class Getters
	{
		private static readonly BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		private static readonly string propertyName = "PublicHost";

		private static readonly TestUri testUri;
		private static readonly Type @class;
		private static readonly PropertyInfo property;

		private static readonly FastProperty fastProperty; // Magnum
		private static readonly TypeAccessor fastMember; // FastMember
		private static readonly Func<TestUri, string> funcTstring; // ILEmit
		private static readonly Func<object, object> funcObjObj; // ILEmit (object)
		public static readonly Func<TestUri, string> getDelegate; // Delegate
		public static readonly Delegate getDelegateDynamic; // Dynamic Delegate
		public static readonly MemberGetter ffgetter; // Fasterflect
		public static readonly MethodInfo getgetMethod;

		private static readonly bool allowNonPublicFieldAccess = true;

		static Getters()
		{
			testUri = new TestUri("SomeHost");
			@class = testUri.GetType();
			property = @class.GetProperty(propertyName, bindingFlags);
			fastProperty = new FastProperty(property, bindingFlags);
			getgetMethod = property.GetGetMethod(nonPublic: allowNonPublicFieldAccess);

			// Using FastMember - https://github.com/mgravell/fast-member
			fastMember = TypeAccessor.Create(@class, allowNonPublicAccessors: allowNonPublicFieldAccess);

			Type funcType = Type.GetType("System.Func`2[Benchmark.TestUri, System.String]");
			getDelegate = (Func<TestUri, string>)Delegate.CreateDelegate(funcType, getgetMethod);
			getDelegateDynamic = Delegate.CreateDelegate(funcType, getgetMethod);

			Emit<Func<TestUri, string>> getterEmiter = Emit<Func<TestUri, string>>
				.NewDynamicMethod("GetTestUriProperty")
				.LoadArgument(0)
				.Call(getgetMethod)
				.Return();
			funcTstring = getterEmiter.CreateDelegate();

			ffgetter = Reflect.PropertyGetter(property);

			Emit<Func<object, object>> objGetterEmiterFunc = Emit<Func<object, object>>
				.NewDynamicMethod("GetMSTestUriProperty")
				.LoadArgument(0)
				.CastClass(typeof(TestUri))
				.Call(getgetMethod)
				.Return();
			funcObjObj = objGetterEmiterFunc.CreateDelegate();
		}

		[Benchmark(Description = "Direct Access")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public string Direct_NoInlining()
		{
			return testUri.PublicHost;
		}

		[Benchmark(Description = "Delegate.CreateDelegate (T,string)")]
		public string Func_Tstring()
		{
			return getDelegate(testUri);
		}

		[Benchmark(Baseline = true, Description = "Fasterflect (object, object)")]
		public string Fasterflect()
		{
			return (string)ffgetter(testUri);
		}

		[Benchmark(Description = "Magnum - Expression.Compile (object, object)")]
		public string Magnum_CompiledExpressionTrees()
		{
			return (string)fastProperty.Get(testUri);
		}

		[Benchmark(Description = "Sigil.ILEmit (T, string)")]
		public string ILEmit_FuncTstring()
		{
			return funcTstring(testUri);
		}

		[Benchmark(Description = "Sigil.ILEmit (object, object)")]
		public string ILEmit_FuncObjObj()
		{
			return (string)funcObjObj(testUri);
		}

		[Benchmark(Description = "FastMember (object, object)")]
		public string FastMember()
		{
			return (string)fastMember[testUri, propertyName];
		}

		[Benchmark(Description = "MethodInfo.Invoke")]
		public string MethodInfoInvoke()
		{
			return (string)getgetMethod.Invoke(testUri, null);
		}

		[Benchmark(Description = "PropertyInfo")]
		public string PropertyInfoCached()
		{
			return (string)property.GetValue(testUri, null);
		}

		[Benchmark(Description = "PropertyInfo - uncached")]
		public string PropertyInfo()
		{
			Type @class = testUri.GetType();
			PropertyInfo property = @class.GetProperty(propertyName, bindingFlags);
			return (string)property.GetValue(testUri, null);
		}

		[Benchmark(Description = "Delegate.DynamicInvoke")]
		public string DelegateDynamicInvoke()
		{
			return (string)getDelegateDynamic.DynamicInvoke(testUri);
		}
	}
}