using SQLGeneration.Builders;

namespace CrmAdo
{
    public interface ISqlParser
    {
        /// <summary>
        /// Parses the sql command text, with any paramaters and returns it.
        /// </summary>
        /// <returns></returns>
        ICommand Parse(string sqlCommandText, string placeHolderToken);
    }
}