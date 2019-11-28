using System;
using Fasterflect;
using Fasterflect.Extensions;
using Fasterflect.Extensions.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Services
{
	[TestClass]
	public class EventHandlerTest
	{
		private class EventSource
		{
			private delegate void VoidOp();
			private delegate string StringOp(string s);
			public delegate int IntOp(int i);

#pragma warning disable 0169
			// ReSharper disable UnusedMember.Local
			private readonly VoidOp voidOp;
			private readonly StringOp stringOp;
			private readonly IntOp intOp;

			private static readonly VoidOp StaticVoidOp;
			private static readonly StringOp StaticStringOp;
			private static readonly IntOp StaticIntOp;

			private event IntOp intEvent;
			private static event IntOp StaticIntEvent;
			// ReSharper restore UnusedMember.Local
#pragma warning restore 0169

			public int TriggerEvents(int i)
			{
				return intEvent(i);
			}

			public static int TriggerStaticEvents(int i)
			{
				return StaticIntEvent(i);
			}
		}

		[TestMethod]
		public void Test_assign_static_no_arg_void_return_delegate()
		{
			Type type = typeof(EventSource);
			bool call = false;
			type.AssignHandler("StaticVoidOp", args => call = true);
			type.InvokeDelegate("StaticVoidOp");
			Assert.AreEqual(true, call);
		}

		[TestMethod]
		public void Test_handle_static_int_arg_int_return_delegate()
		{
			Type type = typeof(EventSource);
			int sum = 0;
			type.AddHandler("StaticIntOp", args => sum += (int) args[0] * 2);
			type.AddHandler("StaticIntOp", args => sum += (int) args[0] * 3);
			int result = (int) type.InvokeDelegate("StaticIntOp", 2);
			Assert.AreEqual(10, sum);
			Assert.AreEqual(10, result);
		}

		[TestMethod]
		public void Test_assign_static_string_arg_string_return_delegate()
		{
			Type type = typeof(EventSource);
			type.AddHandler("StaticStringOp", args => (string) args[0] + "1");
			type.AddHandler("StaticStringOp", args => (string) args[0] + "2");
			string result = (string) type.InvokeDelegate("StaticStringOp", "A");
			Assert.AreEqual("A2", result);
		}

		[TestMethod]
		public void Test_assign_instance_no_arg_void_return_delegate()
		{
			object target = typeof(EventSource).CreateInstance();
			bool call = false;
			target.AssignHandler("voidOp", args => call = true);
			target.InvokeDelegate("voidOp");
			Assert.AreEqual(true, call);
		}

		[TestMethod]
		public void Test_handle_instance_int_arg_int_return_delegate()
		{
			object target = typeof(EventSource).CreateInstance();
			int sum = 0;
			target.AddHandler("intOp", args => sum += (int) args[0] * 2);
			target.AddHandler("intOp", args => sum += (int) args[0] * 3);
			int result = (int) target.InvokeDelegate("intOp", 2);
			Assert.AreEqual(10, sum);
			Assert.AreEqual(10, result);
		}

		[TestMethod]
		public void Test_assign_instance_string_arg_string_return_delegate()
		{
			object target = typeof(EventSource).CreateInstance();
			target.AddHandler("stringOp", args => (string) args[0] + "1");
			target.AddHandler("stringOp", args => (string) args[0] + "2");
			string result = (string) target.InvokeDelegate("stringOp", "A");
			Assert.AreEqual("A2", result);
		}

		[TestMethod]
		public void Test_handle_instance_int_arg_int_return_event()
		{
			object target = typeof(EventSource).CreateInstance();
			int sum = 0;
			target.AddHandler("intEvent", args => sum += (int) args[0] * 2);
			target.AddHandler("intEvent", args => sum += (int) args[0] * 3);
			object result = target.CallMethod("TriggerEvents", 2);
			Assert.AreEqual(10, sum);
			Assert.AreEqual(10, result);
		}

		[TestMethod]
		public void Test_handle_static_int_arg_int_return_event()
		{
			Type type = typeof(EventSource);
			int sum = 0;
			type.AddHandler("StaticIntEvent", args => sum += (int) args[0] * 2);
			type.AddHandler("StaticIntEvent", args => sum += (int) args[0] * 3);
			object result = type.CallMethod("TriggerStaticEvents", 2);
			Assert.AreEqual(10, sum);
			Assert.AreEqual(10, result);
		}

	}
}
