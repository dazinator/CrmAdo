namespace CrmAdo
{
    public interface ISqlStatementTypeChecker
    {
        SqlStatementType GetCommandType(string commandText);
    }
}