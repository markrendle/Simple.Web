using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.Tests
{
    using System.Linq.Expressions;
    using Helpers;
    using Xunit;

    public class ExpressionHelperTests
    {
        [Fact]
        public void GetsConstantValue()
        {
            Expression<Func<int>> target = () => 42;
            object actual;
            Assert.True(ExpressionHelper.TryGetValue(target.Body, out actual));
            Assert.Equal(42, actual);
        }

        [Fact]
        public void GetsPropertyValue()
        {
            var str = "Marvin";
            Expression<Func<int>> target = () => str.Length;
            object actual;
            Assert.True(ExpressionHelper.TryGetValue(target.Body, out actual));
            Assert.Equal(6, actual);
        }
    }
}
