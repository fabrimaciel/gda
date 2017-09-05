/* 
 * GDA - Generics Data Access, is framework to object-relational mapping 
 * (a programming technique for converting data between incompatible 
 * type systems in databases and Object-oriented programming languages) using c#.
 * 
 * Copyright (C) 2010  <http://www.colosoft.com.br/gda> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace GDA.Common.Helper
{
	public class MethodInvokable
	{
		/// <summary>
		/// The <see cref="MethodInvoker"/> wrapping the method to call.
		/// </summary>
		public readonly MethodInvoker MethodInvoker;

		/// <summary>
		/// A value indicating how good a match this is (for a given set of parameters). This value 
		/// is used by the <see cref="MethodDispatcher"/> to decide which method to invoke from a 
		/// set of choices.
		/// </summary>
		public readonly int MatchIndicator;

		/// <summary>
		/// The parameter values used when invoking the method.
		/// </summary>
		public readonly object[] ParameterValues;

		/// <summary>
		/// Construct a new MethodInvokable instance in preparation for executing
		/// the actual method call.
		/// </summary>
		/// <param name="invoker">The <see cref="MethodInvoker"/> wrapping the method to call.</param>
		/// <param name="matchIndicator">A value indicating how good a match this is (for a given
		/// set of parameters). This value is used by the <see cref="MethodDispatcher"/> to decide
		/// which method to invoke from a set of choices.</param>
		/// <param name="parameterValues">The parameter values used when invoking the method.</param>
		public MethodInvokable(MethodInvoker invoker, int matchIndicator, object[] parameterValues)
		{
			this.MethodInvoker = invoker;
			this.MatchIndicator = matchIndicator;
			this.ParameterValues = parameterValues;
		}

		/// <summary>
		/// Perform the actual method invocation.
		/// </summary>
		/// <param name="target">The object to call the method on.</param>
		/// <returns>The return value of the method call.</returns>
		public object Invoke(object target)
		{
			return MethodInvoker.Invoke(target, ParameterValues);
		}
	}
}
