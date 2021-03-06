﻿using Moq;
using NBi.Core.Query;
using NBi.Core.Scalar.Resolver;
using NBi.Core.Variable;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBi.Testing.Unit.Core.Query
{
    public class QueryParameterTest
    {
        [Test]
        public void GetValue_Literal_CorrectValue()
        {
            var param = new QueryParameter("param", "Canada");
            Assert.That(param.GetValue(), Is.EqualTo("Canada"));
        }

        [Test]
        public void GetValue_Variable_CorrectValue()
        {
            var variable = new TestVariable(new CSharpScalarResolver<object>("Math.Min(30, 50)"));
            var resolver = new GlobalVariableScalarResolver<object>("alpha", new Dictionary<string, ITestVariable>() {{ "alpha", variable }});

            var param = new QueryParameter("param", resolver);
            Assert.That(param.GetValue(), Is.EqualTo(30));
        }

        [Test]
        public void GetValue_Variable_GetValueCalledOnce()
        {
            var internalResolverMock = new Mock<IScalarResolver<object>>();
            internalResolverMock.Setup(x => x.Execute()).Returns(It.IsAny<object>());
            var resolver = new GlobalVariableScalarResolver<object>("alpha", new Dictionary<string, ITestVariable>() { { "alpha", new TestVariable(internalResolverMock.Object) } });

            var param = new QueryParameter("param", resolver);
            param.GetValue();
            internalResolverMock.Verify(x => x.Execute(), Times.Once);
        }
    }
}
