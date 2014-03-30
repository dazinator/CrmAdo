namespace CrmAdo
{
    /// <summary>
    /// Responsible for categorising a particular sql command into whether it is a SELECT, INSERT, UPDATE, or DELETE statement. 
    /// </summary>
    public class SqlStatementTypeChecker : ISqlStatementTypeChecker
    {
        public SqlStatementType GetCommandType(string commandText)
        {
            var commandTextLength = commandText.Length;
            var trimmedCommandText = commandText.TrimStart();
            if (commandTextLength >= 6)
            {
                var first6Characters = trimmedCommandText.Substring(0, 6);
                switch (first6Characters.ToUpper())
                {
                    case "SELECT":
                        return SqlStatementType.Select;
                    case "UPDATE":
                        return SqlStatementType.Update;
                    case "INSERT":
                        return SqlStatementType.Insert;
                    case "DELETE":
                        return SqlStatementType.Delete;
                    default:
                        return SqlStatementType.Other;

                }
            }
            return SqlStatementType.Other;
        }
    }
}