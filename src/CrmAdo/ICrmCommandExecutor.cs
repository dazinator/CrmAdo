namespace CrmAdo
{
    public interface ICrmCommandExecutor
    {
        EntityResultSet ExecuteCommand(CrmDbCommand command);
        int ExecuteNonQueryCommand(CrmDbCommand command);
    }

   
}