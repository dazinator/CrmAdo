using System;

namespace CrmAdo
{
    [Obsolete]
    public interface ISqlStatementTypeChecker
    {
        SqlStatementType GetCommandType(string commandText);
    }
}