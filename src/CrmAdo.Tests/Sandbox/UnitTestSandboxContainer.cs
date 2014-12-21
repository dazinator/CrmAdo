using CrmAdo.Dynamics;
using CrmAdo.IoC;
using Microsoft.Xrm.Sdk;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;

namespace CrmAdo.Tests
{
    /// <summary>
    /// A container provider that has methods for unit test time. Such as registering instances of mock services
    /// into the container.
    /// </summary> 
    public class UnitTestSandboxContainer : IContainerProvider, IDisposable
    {

        private IContainerProvider _previousContainerProvider = null;
        private TinyIoCContainer _container = new TinyIoCContainer();

        public TinyIoCContainer Container { get { return _container; } }

        public UnitTestSandboxContainer()
        {
            // Get the current container provider so we can revert after we are disposed.
            _previousContainerProvider = IoC.ContainerServices.GetCurrentContainerProvider();
            // Register ourselves as the container provider (which is used to construct the container on first use),
            // and pass in a flag to indicate that we want the current container to be re-created using our provider.
            // The flag forces any previously cached container to be removed.
            IoC.ContainerServices.SetContainerProvider(this, true);
        }

        /// <summary>
        /// Creates a Mock of the given type, and registers the instance of it into the container.
        /// </summary>
        /// <typeparam name="RegisterType">The type to generate the mock for, and to register into the container.</typeparam>       
        /// <returns>The mock instance of the given type.</returns>
        public RegisterType RegisterMockInstance<RegisterType>()
           where RegisterType : class
        {
            var mockInstance = MockRepository.GenerateMock<RegisterType>();
            var options = _container.Register(typeof(RegisterType), mockInstance);
            return mockInstance;
        }

        /// <summary>
        /// Creates a Mock of the given type, and registers the instance of it into the container.
        /// </summary>
        /// <typeparam name="RegisterType">The type to generate the mock for, and to register into the container.</typeparam>   
        /// <typeparam name="TAlsoImplements">If you want the generated mock to also implement an additional interface, you can specify that here.</typeparam>    
        /// <returns>The mock instance of the given type.</returns>
        public RegisterType RegisterMockInstance<RegisterType, TAlsoImplements>()
           where RegisterType : class
        {
            var mockInstance = MockRepository.GenerateMock<RegisterType, TAlsoImplements>();
            var options = _container.Register(typeof(RegisterType), mockInstance);
            return mockInstance;
        }

        public IContainer GetContainer()
        {
            return Container;
        }

        /// <summary>
        /// Disposes the sandbox, this will restore any previous container provider.
        /// </summary>
        public void Dispose()
        {
            // Restore previous container provider.
            IoC.ContainerServices.SetContainerProvider(_previousContainerProvider, true);
        }

    }
}
