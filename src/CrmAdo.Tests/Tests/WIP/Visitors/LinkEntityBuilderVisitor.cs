using CrmAdo.Tests.WIP;
using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Tests.WIP.Visitors
{

    /// <summary>
    /// A <see cref="BuilderVisitor"/> that sets a <see cref="LinkEntity"/> when it visits an <see cref="EqualToFilter" present on a Join./> 
    /// </summary>
    public class LinkEntityBuilderVisitor : BaseOrganizationRequestBuilderVisitor
    {

        public LinkEntity LinkEntity { get; set; }
        public EqualToFilter EqualToFilter = null;

        public Column LeftColumn { get; set; }
        public Column RightColumn { get; set; }

        public Table LeftTable { get; set; }
        public Table RightTable { get; set; }

        // public string LeftEntityName { get; set; }

        public LinkEntityBuilderVisitor(LinkEntity linkEntity)
        {
            LinkEntity = linkEntity;
        }

        protected override void VisitEqualToFilter(EqualToFilter item)
        {
            EqualToFilter = item;

            //TODO: Tidy this up use more of visitor pattern?   
            LeftColumn = item.LeftHand as Column;
            RightColumn = item.RightHand as Column;
            // GuardOnColumn(leftColumn);
            //  GuardOnColumn(rightColumn);

            LeftTable = LeftColumn.Source.Source as Table;
            RightTable = RightColumn.Source.Source as Table;
            //  LeftEntityName = 

            LinkEntity.LinkFromEntityName = GetTableLogicalEntityName(LeftTable);
            LinkEntity.LinkToEntityName = GetTableLogicalEntityName(RightTable);
            LinkEntity.LinkFromAttributeName = GetColumnLogicalAttributeName(LeftColumn);
            LinkEntity.LinkToAttributeName = GetColumnLogicalAttributeName(RightColumn);
        }

    }
}
