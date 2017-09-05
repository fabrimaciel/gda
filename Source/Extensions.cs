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

#if CLS_3_5
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace GDA
{
    public static class Extensions
    {
        internal static MemberExpression RemoveUnary(Expression toUnwrap)
        {
            if (toUnwrap is UnaryExpression)
                return (MemberExpression)((UnaryExpression)toUnwrap).Operand;

            return toUnwrap as MemberExpression;
        }

        internal static MemberInfo GetMember<T>(this Expression<Func<T, object>> expression)
        {
            var memberExp = RemoveUnary(expression.Body);

            if (memberExp == null)
                return null;

            return memberExp.Member;
        }

        internal static MemberInfo GetMember<T, Result>(this Expression<Func<T, Result>> expression)
        {
            var memberExp = RemoveUnary(expression.Body);

            if (memberExp == null)
                return null;

            return memberExp.Member;
        }
    }
}

#endif
