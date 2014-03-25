using SQLGeneration.Builders;
using SQLGeneration.Generators;

namespace CrmAdo
{
    public class SqlGenerationParser : ISqlParser
    {

        public SqlGenerationParser()
        {
        }

        public ICommand Parse(string sqlCommandText, string placeholderToken)
        {
            var commandText = sqlCommandText;
            var commandBuilder = new CommandBuilder();
            var options = new CommandBuilderOptions();
            options.PlaceholderPrefix = placeholderToken;
            var builder = commandBuilder.GetCommand(commandText, options);
            return builder;
        }
    }
}