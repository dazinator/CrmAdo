using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{
    public class CrmAdoVsCommand : Microsoft.VisualStudio.Data.Framework.DataCommand
    {
        public override IVsDataParameter CreateParameter()
        {
            return base.CreateParameter();
        }

        public override IVsDataParameter[] DeriveParameters(string command, DataCommandType commandType, int commandTimeout)
        {
            return base.DeriveParameters(command, commandType, commandTimeout);
        }

        public override IVsDataReader DeriveSchema(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            return base.DeriveSchema(command, commandType, parameters, commandTimeout);
        }

        public override IVsDataReader Execute(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            return base.Execute(command, commandType, parameters, commandTimeout);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int ExecuteWithoutResults(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            return base.ExecuteWithoutResults(command, commandType, parameters, commandTimeout);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        protected override void OnSiteChanged(EventArgs e)
        {
            base.OnSiteChanged(e);
        }
        public override string Prepare(string command, DataCommandType commandType, IVsDataParameter[] parameters, int commandTimeout)
        {
            return base.Prepare(command, commandType, parameters, commandTimeout);
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
