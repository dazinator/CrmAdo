using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{
    public class CrmAdoObjectIdentifierConverter : AdoDotNetObjectIdentifierConverter
    {
        public CrmAdoObjectIdentifierConverter(IVsDataConnection site): base(site)
        {

        }

        public CrmAdoObjectIdentifierConverter()
            : base()
        {

        }    

        protected override string BuildString(string typeName, string[] identifierParts, Microsoft.VisualStudio.Data.Services.DataObjectIdentifierFormat format)
        {
            var result = base.BuildString(typeName, identifierParts, format);
            return result;
        }

        protected override string FormatPart(string typeName, object identifierPart, Microsoft.VisualStudio.Data.Services.DataObjectIdentifierFormat format)
        {
            var result = base.FormatPart(typeName, identifierPart, format);
            return result;
        }

        protected override bool RequiresQuoting(string identifierPart)
        {
            var result = base.RequiresQuoting(identifierPart);
            return result;
        }

        protected override string[] SplitIntoParts(string typeName, string identifier)
        {
            var result = base.SplitIntoParts(typeName, identifier);
            return result;
        }

        protected override object UnformatPart(string typeName, string identifierPart)
        {
            var result = base.UnformatPart(typeName, identifierPart);
            return result;
        }

        public override bool Equals(object obj)
        {
            var result = base.Equals(obj);
            return result;
        }

        protected override char CompositeIdentifierSeparator
        {
            get
            {
                return base.CompositeIdentifierSeparator;
            }
        }

        protected override void OnSiteChanged(EventArgs e)
        {
            base.OnSiteChanged(e);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
