using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.IoC
{
    public static class ContainerServices
    {

        private static readonly object _TypeLock = new object();
        private static IContainerProvider _ContainerProvider = new DefaultContainerProvider();

        private static IContainer _Current = null;

        public static IContainer CurrentContainer()
        {
            //create and configure container if one does not yet exist
            if (_Current == null)
            {
                lock (_TypeLock)
                {
                    if (_Current == null)
                    {
                        _Current = _ContainerProvider.GetContainer();
                    }
                }
            }

            return _Current;
        }

        /// <summary>
        /// Can be called to set an alternate container provider. This should be called on application start
        /// prior to the container being initialised (which happens when ContainerServices.CurrentContainer() is called
        /// for the first time.)
        /// </summary>
        /// <param name="containerProvider"></param>
        public static void SetContainerProvider(IContainerProvider containerProvider, bool clearCurrentContainer)
        {
            _ContainerProvider = containerProvider;
            if (clearCurrentContainer == true)
            {
                _Current = null;
            }
        }

        public static IContainerProvider GetCurrentContainerProvider()
        {
            return _ContainerProvider;
        }

    }
}
