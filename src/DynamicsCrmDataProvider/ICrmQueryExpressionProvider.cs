using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;

namespace DynamicsCrmDataProvider
{
    public interface ICrmQueryExpressionProvider
    {
        /// <summary>
        /// Creates a QueryExpression from the given Select command.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        QueryExpression CreateQueryExpression(CrmDbCommand command);
    }
}