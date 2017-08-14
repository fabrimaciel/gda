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
