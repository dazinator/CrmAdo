using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider.SupportEntities
{
    public class CrmAdoDataConnectionEquivalencyComparer : DataConnectionEquivalencyComparer
    {
        protected override bool AreEquivalent(IVsDataConnectionProperties connectionProperties1, IVsDataConnectionProperties connectionProperties2)
        {
            var result = base.AreEquivalent(connectionProperties1, connectionProperties2);
            return result;
        }
    }
}
