﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Harvester.Core.Messaging;

/* Copyright (c) 2012 CBaxter
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE. 
 */

namespace Harvester.Core.Filters
{
    internal class StringComparisonFilterBase : ComparisonFilterBase
    {
        public struct StringOperation
        {
            public readonly String MethodName;
            public readonly Object FalseResult;

            public StringOperation(String methodName, Object falseResult)
            {
                Verify.NotWhitespace(methodName, "methodName");

                MethodName = methodName;
                FalseResult = falseResult;
            }
        }

        protected StringComparisonFilterBase(IHaveExtendedProperties extendedProperties, IEnumerable<IFilterMessages> children, StringOperation operation)
            : base(extendedProperties, children, (lhs, rhs) => CreateExpression(lhs, rhs, operation))
        { }

        private static BinaryExpression CreateExpression(Expression memberExpression, Expression valueExpression, StringOperation operation)
        {
            Verify.NotNull(valueExpression, "valueExpression");
            Verify.NotNull(memberExpression, "memberExpression");
            Verify.True(typeof(String).IsAssignableFrom(memberExpression.Type), "memberExpression", "Expression type must by String.");
            Verify.True(memberExpression.NodeType == ExpressionType.MemberAccess, "memberExpression", "MemberAccess expression expected.");

            return Expression.NotEqual(
                        Expression.Call(
                            memberExpression,
                            typeof(String).GetMethod(operation.MethodName, new[] { typeof(String), typeof(StringComparison) }),
                            new[] { valueExpression, Expression.Constant(StringComparison.OrdinalIgnoreCase) }
                        ),
                        Expression.Constant(operation.FalseResult)
                    );
        }
    }


    internal class ContainsFilter : StringComparisonFilterBase
    {
        public ContainsFilter(IHaveExtendedProperties extendedProperties, IEnumerable<IFilterMessages> children)
            : base(extendedProperties, children, new StringOperation("IndexOf", -1))
        { }
    }

    internal class EndsWithFilter : StringComparisonFilterBase
    {
        public EndsWithFilter(IHaveExtendedProperties extendedProperties, IEnumerable<IFilterMessages> children)
            : base(extendedProperties, children, new StringOperation("EndsWith", false))
        { }
    }

    internal class StartsWithFilter : StringComparisonFilterBase
    {
        public StartsWithFilter(IHaveExtendedProperties extendedProperties, IEnumerable<IFilterMessages> children)
            : base(extendedProperties, children, new StringOperation("StartsWith", false))
        { }
    }
}
