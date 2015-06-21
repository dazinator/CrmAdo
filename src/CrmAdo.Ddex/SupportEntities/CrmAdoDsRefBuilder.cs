using Microsoft.VisualStudio.Data.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{
    public class CrmAdoDsRefBuilder : DSRefBuilder
    {

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        protected override void AppendToDSRef(object dsRef, string typeName, object[] identifier, object[] parameters)
        {
            base.AppendToDSRef(dsRef, typeName, identifier, parameters);
            return;
        }

    }
}
