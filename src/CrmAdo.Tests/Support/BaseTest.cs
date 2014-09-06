using System;
using NUnit.Framework;

namespace CrmAdo.Tests
{
    [TestFixture]
    [Category("Unit Test")]
    public abstract class BaseTest<TTestSubject>
    {
        protected virtual TTestSubject CreateTestSubject()
        {
            return Activator.CreateInstance<TTestSubject>();
        }

        protected virtual TTestSubject CreateTestSubject(params object[] args)
        {
            return (TTestSubject)Activator.CreateInstance(typeof(TTestSubject), args);
        }

    }
}