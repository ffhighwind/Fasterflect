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
	public class Setters
	{
		private static readonly BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
		private static readonly string propertyName = "PublicHost";

		private static readonly TestUri testUri;
		private static readonly Type @class;
		private static readonly PropertyInfo property;

		private static readonly FastProperty fastProperty; // Magnum
		private static readonly TypeAccessor fastMember; // FastMember
		private static readonly Action<TestUri, string> setter; // ILEmit
		private static readonly Action<object, object> objsetter; // ILEmit (object)
		public static readonly Action<TestUri, string> setDelegate; // Delegate
		public static readonly Delegate setDelegateDynamic; // Dynamic Delegate
		public static readonly MemberSetter ffsetter; // Fasterflect
		public static readonly MethodInfo getsetMethod;

		private static readonly bool allowNonPublicFieldAccess = true;

		static Setters()
		{
			testUri = new TestUri("SomeHost");
			@class = testUri.GetType();
			property = @class.GetProperty(propertyName, bindingFlags);
			fastProperty = new FastProperty(property, bindingFlags);
			getsetMethod = property.GetSetMethod(nonPublic: allowNonPublicFieldAccess);
			fastMember = TypeAccessor.Create(@class, allowNonPublicAccessors: allowNonPublicFieldAccess);

			Type actionType = Type.GetType("System.Action`2[Benchmark.TestUri, System.String]");
			setDelegate = (Action<TestUri, string>)Delegate.CreateDelegate(actionType, getsetMethod);
			setDelegateDynamic = Delegate.CreateDelegate(actionType, getsetMethod);

			var setterEmiter = Emit<Action<TestUri, string>>
				.NewDynamicMethod("SetTestUriProperty")
				.LoadArgument(0)
				.LoadArgument(1)
				.Call(getsetMethod)
				.Return();
			setter = setterEmiter.CreateDelegate();

			ffsetter = Reflect.PropertySetter(property);

			var objSetterEmiterAction = Emit<Action<object, object>>
				.NewDynamicMethod("SetObjTestUriProperty")
				.LoadArgument(0)
				.CastClass(typeof(TestUri))
				.LoadArgument(1)
				.CastClass(typeof(string))
				.Call(getsetMethod)
				.Return();
			objsetter = objSetterEmiterAction.CreateDelegate();
		}

		[Benchmark(Description = "Direct Access")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Direct_NoInlining()
		{
			testUri.PublicHost = "Testing";
		}

		[Benchmark(Description = "Delegate.CreateDelegate (T, string)")]
		public void ActionTstring()
		{
			setDelegate(testUri, "Testing");
		}

		[Benchmark(Description = "Sigil.ILEmit (T, string)")]
		public void ILEmit_ActionTstring()
		{
			setter(testUri, "Testing");
		}

		[Benchmark(Description = "Sigil.ILEmit (object, object)")]
		public void ILEmit_ActionObjObj()
		{
			objsetter(testUri, "Testing");
		}

		[Benchmark(Baseline = true, Description = "Fasterflect (object, object)")]
		public void Fasterflect()
		{
			ffsetter(testUri, "Testing");
		}

		[Benchmark(Description = "Magnum - Expression.Compile (object, object)")]
		public void Magnum_CompiledExpressionTrees()
		{
			fastProperty.Set(testUri, "Testing");
		}

		[Benchmark(Description = "FastMember (object, object)")]
		public void FastMember()
		{
			fastMember[testUri, propertyName] = "Testing";
		}

		[Benchmark(Description = "MethodInfo.Invoke")]
		public void MethodInfoInvoke()
		{
			getsetMethod.Invoke(testUri, new object[] { "Testing" });
		}

		[Benchmark(Description = "PropertyInfo")]
		public void PropertyInfo_Cached()
		{
			property.SetValue(testUri, "Testing", null);
		}

		[Benchmark(Description = "PropertyInfo - uncached")]
		public void PropertyInfo()
		{
			Type @class = testUri.GetType();
			PropertyInfo property = @class.GetProperty(propertyName, bindingFlags);
			property.SetValue(testUri, "Testing", null);
		}

		[Benchmark(Description = "Delegate.DynamicInvoke")]
		public void DelegateDynamicInvoke()
		{
			setDelegateDynamic.DynamicInvoke(testUri, "Testing");
		}
	}
}