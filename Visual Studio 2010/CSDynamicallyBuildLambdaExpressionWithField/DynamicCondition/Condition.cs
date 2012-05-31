/*************************************** 模块头 *************************************\
* 模块名:  Condition.cs
* 项目名： CSDynamicallyBuildLambdaExpressionWithField
* 版权 (c) Microsoft Corporation.
*
* Condition.cs 文件定义了一个与 System.Linq.Expressions 命名空间相关的 LINQ 运算符.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicCondition
{
    public static class DynamicQuery
    {
        #region Condition Classes

        public abstract class Condition
        {

            #region Fields

            // 用来确保通过多个查询,得到了特定的 ParameterExpression 的同一实例.
            private static Dictionary<string, ParameterExpression> paramTable =
                new Dictionary<string, ParameterExpression>();

            // 将在运行时,传递到 LINQ to SQL 的表达式树.
            protected internal LambdaExpression LambdaExpr;

            // 枚举所有可以执行的不同比较.
            public enum Compare : int
            {
                Or = ExpressionType.Or,
                And = ExpressionType.And,
                Xor = ExpressionType.ExclusiveOr,
                Not = ExpressionType.Not,
                Equal = ExpressionType.Equal,
                Like = ExpressionType.TypeIs + 1,
                NotEqual = ExpressionType.NotEqual,
                OrElse = ExpressionType.OrElse,
                AndAlso = ExpressionType.AndAlso,
                LessThan = ExpressionType.LessThan,
                GreaterThan = ExpressionType.GreaterThan,
                LessThanOrEqual = ExpressionType.LessThanOrEqual,
                GreaterThanOrEqual = ExpressionType.GreaterThanOrEqual
            }
            #endregion

            /// <summary>
            /// 构造一个Condition，用T表示元素类型，S表示值类型.
            /// </summary>
            public static Condition<T, S> Create<T, S>(IEnumerable<T> dataSource, string propertyName,
                Compare condType, S value)
            {

                return new Condition<T, S>(propertyName, condType, value);
            }

            /// <summary>
            /// 构造一个Condition，用T表示元素类型，S表示值类型.
            /// 在运行时才能知道值类型的情况下，这是很有用的.
            /// </summary>
            public static Condition<T> Create<T>(IEnumerable<T> dataSource, string propertyName,
                Compare condType, object value, Type valueType)
            {

                return new Condition<T>(propertyName, condType, value, valueType);
            }


            /// <summary>
            /// 创造一个由两个其他条件组合而成的条件.
            /// </summary>
            /// <typeparam name="T"> 条件针对时执行的类型</typeparam>
            /// <param name="cond1">第一个条件</param>
            /// <param name="condType">使用在条件上的运算符 </param>
            /// <param name="cond2">第二个条件 </param>
            /// <returns>一个新的条件,是根据指定的运算符，将两个条件组合而得.</returns>
            /// <remarks></remarks>
            public static Condition<T> Combine<T>(Condition<T> cond1, Compare condType, Condition<T> cond2)
            {
                return Condition<T>.Combine(cond1, condType, cond2);
            }

            /// <summary>
            /// 根据指定的运算符，组合多个条件.
            /// </summary>
            public static Condition<T> Combine<T>(Condition<T> cond1, Compare condType,
                params Condition<T>[] conditions)
            {
                return Condition<T>.Combine(cond1, condType, conditions);
            }


            #region Protected Methods
            /// <summary>
            /// 根据指定的运算符（condType），将两个表达式组合起来.
            /// </summary>
            protected static Expression CombineExpression(Expression left, Compare condType, Expression right)
            {

                // 基于运算符，连结表达式.
                switch (condType)
                {
                    case Compare.Or:
                        return Expression.Or(left, right);
                    case Compare.And:
                        return Expression.And(left, right);
                    case Compare.Xor:
                        return Expression.ExclusiveOr(left, right);
                    case Compare.Equal:
                        return Expression.Equal(left, right);
                    case Compare.OrElse:
                        return Expression.OrElse(left, right);
                    case Compare.AndAlso:
                        return Expression.AndAlso(left, right);
                    case Compare.NotEqual:
                        return Expression.NotEqual(left, right);
                    case Compare.LessThan:
                        return Expression.LessThan(left, right);
                    case Compare.GreaterThan:
                        return Expression.GreaterThan(left, right);
                    case Compare.LessThanOrEqual:
                        return Expression.LessThanOrEqual(left, right);
                    case Compare.GreaterThanOrEqual:
                        return Expression.GreaterThanOrEqual(left, right);
                    case Compare.Like:

                        //对于 Like 运算符，我们对 VB 运行时中的 LikeString 方法调用进行了编码. 
                        var m = typeof(Microsoft.VisualBasic.CompilerServices.Operators).GetMethod("LikeString");

                        return Expression.Call(m, left, right, Expression.Constant(Microsoft.VisualBasic.CompareMethod.Binary));

                    default:
                        throw new ArgumentException("Not a valid Condition Type", "condType", null);
                }
            }

            /// <summary>
            /// 由于这两个的类型参数必须相同，我们可以将通常为Func(Of T, T, Boolean)的，转换为Func(Of T, Boolean).
            /// </summary>
            protected static Func<T, Boolean> CombineFunc<T>(Func<T, Boolean> d1, Compare condType, Func<T, Boolean> d2)
            {

                // 返回组合 d1 和 d2 委托的一个委托.
                switch (condType)
                {
                    case Compare.Or:
                        return (x) => d1(x) | d2(x);
                    case Compare.And:
                        return (x) => d1(x) & d2(x);
                    case Compare.Xor:
                        return (x) => d1(x) ^ d2(x);
                    case Compare.Equal:
                        return (x) => d1(x) == d2(x);
                    case Compare.OrElse:
                        return (x) => d1(x) || d2(x);
                    case Compare.AndAlso:
                        return (x) => d1(x) && d2(x);
                    case Compare.NotEqual:
                        return (x) => d1(x) != d2(x);
                    case Compare.LessThan:
                        return (x) => int.Parse(d1(x).ToString()) < int.Parse(d2(x).ToString());
                    case Compare.GreaterThan:
                        return (x) => int.Parse(d1(x).ToString()) > int.Parse(d2(x).ToString());
                    case Compare.LessThanOrEqual:
                        return (x) => int.Parse(d1(x).ToString()) <= int.Parse(d2(x).ToString());
                    case Compare.GreaterThanOrEqual:
                        return (x) => int.Parse(d1(x).ToString()) >= int.Parse(d2(x).ToString());
                    default:
                        throw new ArgumentException("Not a valid Condition Type", "condType");
                }
            }

            /// <summary>
            /// 确保，对于给定的类型 t ,我们得到了 ParameterExpression 的同一实例.
            /// </summary>
            protected static ParameterExpression GetParamInstance(Type dataType)
            {

                // 参数匹配是按引用，而不是按名称，所以我们将该实例缓存到一个字典中.
                if (!(paramTable.ContainsKey(dataType.Name)))
                {
                    paramTable.Add(dataType.Name, Expression.Parameter(dataType, dataType.Name));
                }

                return paramTable[dataType.Name];
            }
            #endregion

        }

        public class Condition<T> : Condition
        {

            // 包含编译好的表达式树，可以在本地运行的委托.
            internal Func<T, bool> del;

            #region Constructors

            internal Condition()
            {
            }

            internal Condition(string propName, Compare condType, object value, Type valueType)
            {
                // 拆分数组，以处理处理嵌套属性的访问.
                var s = propName.Split('.');

                // 为 propName 获得 PropertyInfo 实例.
                var pInfo = typeof(T).GetProperty(s[0]);
                var paramExpr = GetParamInstance(typeof(T));
                var callExpr = Expression.MakeMemberAccess(paramExpr, pInfo);

                // 为每一个指定的成员，构造一个附加的 MemberAccessExpression.
                // 例如，如果用户表示 "myCustomer.Order.OrderID = 4"，为此我
                // 们需要一个附加的 MemberAccessExpression."
                for (var i = 1; i <= s.GetUpperBound(0); i++)
                {
                    pInfo = pInfo.PropertyType.GetProperty(s[i]);
                    callExpr = Expression.MakeMemberAccess(callExpr, pInfo);
                }

                // ConstantExpression,表示运算符左边的值.
                var valueExpr = Expression.Constant(value, valueType);

                Expression b = CombineExpression(callExpr, condType, valueExpr);
                LambdaExpr = Expression.Lambda<Func<T, bool>>(b, new ParameterExpression[] { paramExpr });

                // 将 lambda 表达式编译到一个委托中.
                del = (Func<T, bool>)(LambdaExpr.Compile());
            }

            #endregion

            #region Methods

            // 根据指定的运算符，组合两个条件.
            internal static Condition<T> Combine(Condition<T> cond1, Compare condType, Condition<T> cond2)
            {
                Condition<T> c = new Condition<T>();

                Expression b = CombineExpression(cond1.LambdaExpr.Body, condType, cond2.LambdaExpr.Body);

                var paramExpr = new ParameterExpression[] { GetParamInstance(typeof(T)) };

                // 构建 Lambda 表达式，编译委托.
                c.LambdaExpr = Expression.Lambda<Func<T, bool>>(b, paramExpr);
                c.del = Condition.CombineFunc(cond1.del, condType, cond2.del);

                return c;
            }

            // 根据指定的运算符，组合多个条件.
            internal static Condition<T> Combine(Condition<T> cond1, Compare condType, params Condition<T>[] conditions)
            {
                var finalCond = cond1;
                foreach (var c in conditions)
                {
                    finalCond = Condition.Combine(finalCond, condType, c);
                }

                return finalCond;
            }

            // 在本地，而不是远程，运行查询.
            public bool Matches(T row)
            {
                return del(row); //将行传递到委托,看是否匹配.
            }

            #endregion

            #region Overloaded Operators

            // 运算符重载 - 允许像"(condition1 Or condition2) And condition3"这样的语法.
            public static Condition<T> operator &(Condition<T> c1, Condition<T> c2)
            {
                return Condition.Combine(c1, Compare.And, c2);
            }

            public static Condition<T> operator |(Condition<T> c1, Condition<T> c2)
            {
                return Condition.Combine(c1, Compare.Or, c2);
            }

            public static Condition<T> operator ^(Condition<T> c1, Condition<T> c2)
            {
                return Condition.Combine(c1, Compare.Xor, c2);
            }

            #endregion

        }

        // 表示一个像"object.Property = value"这样的条件.
        // 在本例中，对象是 T 类型的，值是 S 类型的.
        //
        // 尽管对此的大多数逻辑已经包含在基类中，定义第二个泛型参数，意味着用
        // 户不需要传递一个System.Type的参数——它可以被推断出来.
        public class Condition<T, S> : Condition<T>
        {
            internal Condition(string propName, Compare condType, S value)
                : base(propName, condType,
                    value, typeof(S))
            {
            }
        }

        #endregion

        #region Extension Methods

        /// <summary>
        /// 根据指定的条件，过滤一个 IQueryable(Of T).
        /// </summary>
        public static IQueryable<T> Where<T>(this IQueryable<T> source, Condition<T> condition)
        {

            var callExpr = Expression.Call(typeof(Queryable), "Where", new Type[] { source.ElementType },
                source.Expression, Expression.Quote(condition.LambdaExpr));

            return (IQueryable<T>)(source.Provider.CreateQuery(callExpr));
        }

        /// <summary>
        /// 根据指定的条件，过滤一个 IEnumerable(Of T).
        /// </summary>
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Condition<T> condition)
        {
            return source.Where(condition.del);
        }

        /// <summary>
        ///  可以取消，任何实现 IEnumerable(Of T) 类型的扩展方法.
        ///  它构造了一个用 T 作为元素类型，用 S 作为值类型的 Condition.
        /// </summary>
        public static Condition<T, S> CreateCondition<T, S>(this IEnumerable<T> dataSource, string propName,
            DynamicCondition.DynamicQuery.Condition.Compare condType, S value)
        {

            return Condition.Create(dataSource, propName, condType, value);
        }
        #endregion
    }
}