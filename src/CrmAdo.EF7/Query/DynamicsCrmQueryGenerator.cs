﻿using Microsoft.Data.Entity.Relational.Query.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework.Query
{    
    public class DynamicsCrmQueryGenerator : DefaultSqlQueryGenerator
    {
        protected override string DelimitIdentifier(string identifier)
        {
            return "[" + identifier.Replace("]", "]]") + "]";
        }
    }
}