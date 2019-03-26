#region License
// Copyright 2010 Buu Nguyen, Morten Mertner
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
// The latest version of this file can be found at http://fasterflect.codeplex.com/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fasterflect.Extensions
{
	/// <summary>
	/// Container class for event/delegate extensions.
	/// </summary>
	internal static partial class EventExtensions
	{
		/// <summary>
		/// Assigns a dynamic handler for a static delegate or event.
		/// </summary>
		/// <param name="target">The object where the delegate or event belongs to.</param>
		/// <param name="fieldName">The field name of the delegate or event.</param>
		/// <param name="func">The function which will be invoked whenever the delegate or event is fired.</param>
		/// <returns>The return value of the invocation.</returns>
		public static Type AssignHandler(this object target, string fieldName,
			Func<object[], object> func)
		{
			return Fasterflect.Extensions.Services.EventExtensions.InternalAddHandler(target.GetType(), fieldName, func, target, true);
		}


		/// <summary>
		/// Adds a dynamic handler for an instance delegate.
		/// </summary>
		/// <param name="target">The object where the delegate belongs to.</param>
		/// <param name="fieldName">The field name of the delegate.</param>
		/// <param name="func">The function which will be invoked whenever the delegate is invoked.</param>
		/// <returns>The return value of the invocation.</returns>
		public static Type AddHandler(this object target, string fieldName,
			Func<object[], object> func)
		{
			return Fasterflect.Extensions.Services.EventExtensions.InternalAddHandler(target.GetType(), fieldName, func, target, false);
		}

		/// <summary>
		/// Invokes an instance delegate using supplied parameters.
		/// </summary>
		/// <param name="target">The object where the delegate belongs to.</param>
		/// <param name="delegateName">The field name of the delegate.</param>
		/// <param name="parameters">The parameters used to invoke the delegate.</param>
		/// <returns>The return value of the invocation.</returns>
		public static object InvokeDelegate(this object target, string delegateName, params object[] parameters)
		{
			return ((Delegate) target.GetFieldValue(delegateName)).DynamicInvoke(parameters);
		}
	}
}
