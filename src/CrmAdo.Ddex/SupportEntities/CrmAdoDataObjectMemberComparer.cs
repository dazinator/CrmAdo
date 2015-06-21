using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{
    public class CrmAdoDataObjectMemberComparer : AdoDotNetObjectMemberComparer
    {
        public CrmAdoDataObjectMemberComparer()
            : base()
        {

        }

        public CrmAdoDataObjectMemberComparer(IVsDataConnection site)
            : base(site)
        {

        }

        protected override bool RequiresQuoting(string identifierPart)
        {
            // return true;
            var result = base.RequiresQuoting(identifierPart);
            return result;
        }
        public override int Compare(string typeName, object[] identifier, int identifierPart, object value)
        {
            var result = base.Compare(typeName, identifier, identifierPart, value);
            return result;
        }

        public override int Compare(string typeName, string propertyName, object value1, object value2)
        {
            var result = base.Compare(typeName, propertyName, value1, value2);
            return result;
        }
        public override bool Equals(object obj)
        {
            var result = base.Equals(obj);
            return result;
        }
    }
}
