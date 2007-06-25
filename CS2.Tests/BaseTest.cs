using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using NUnit.Framework;
using Rhino.Mocks;

namespace CS2.Tests
{
    public abstract class BaseTest
    {
        protected IWindsorContainer container;
        protected MockRepository mocks;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            container = new WindsorContainer(new XmlInterpreter());
            mocks = new MockRepository();
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            container.Dispose();
        }
    }
}