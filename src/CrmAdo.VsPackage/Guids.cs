// Guids.cs
// MUST match guids.h
using System;

namespace CrmAdo.DdexProvider
{
    static class GuidList
    {
        public const string guidCrmAdo_VsPackagePkgString = "7a1193f1-db85-4bef-ba54-604efcb169a3";
        public const string guidCrmAdo_VsPackageCmdSetString = "d6c12c21-c53f-4b89-8f15-6b2af52e2398";

        public static readonly Guid guidCrmAdo_VsPackageCmdSet = new Guid(guidCrmAdo_VsPackageCmdSetString);

        public const string guidCrmAdo_DdexProviderDataProviderString = "ad13f37d-7d44-4ac5-a498-28fb48b908dc";
        public const string guidCrmAdo_DdexProviderDataSourceString = "867f28cf-bfe4-465a-91d3-88f250aaa667";
        public const string guidCrmAdo_DdexProviderObjectFactoryString = "64b5ec8b-663f-4cdc-bd79-ac99d49e8da6";

        public const string guidCrmAdo_DdexProviderTechnologyString = "77AB9A9D-78B9-4ba7-91AC-873F5338F1D2";
        
    };
}