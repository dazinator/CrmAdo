using System;
using System.Collections.Generic;
using System.Text;
using SQLGeneration.Builders;
using SQLGeneration.Generators;

namespace DynamicsCrmDataProvider.Tests
{
    public class LogUtils
    {
        public const int IndentLevelWidth = 10;

        public static void LogCommand(ICommand command, StringBuilder logBuilder, int indentLevel = 0)
        {
            var formatter = new Formatter();
            var commandText = formatter.GetCommandText(command);

            var indent = GetIndent(indentLevel);

            logBuilder.AppendLine(string.Format("{0}Command, Type: {1}", indent, command.GetType().FullName));
            logBuilder.AppendLine(string.Format("{0} Text: {1}", indent, commandText));

            if (command is SelectBuilder)
            {
                var selCommand = command as SelectBuilder;
                LogFrom(selCommand.From, logBuilder, indentLevel);
            }
           
        }

        public static void LogFrom(IEnumerable<IJoinItem> from, StringBuilder stringBuilder, int indentLevel = 0)
        {
            foreach (var f in from)
            {
                if (f != null)
                {
                    if (f is Join)
                    {
                        LogUtils.LogJoin(f as Join, stringBuilder, 0);
                    }
                    else if (f is AliasedSource)
                    {
                        LogUtils.LogAliasedSource(f as AliasedSource, stringBuilder, 0);
                    }
                }
            }
        }

        public static string GetIndent(int level)
        {
            var indentAmount = level * IndentLevelWidth;
            string indent = string.Empty;
            for (int i = 0; i < indentAmount; i++)
            {
                indent = indent + " ";
            }
            return indent;
        }

        public static void LogJoin(Join join, StringBuilder stringBuilder, int level = 0)
        {

            var indent = GetIndent(level);
            if (join != null)
            {
                stringBuilder.AppendLine(string.Format("{0}Join, Type: {1}", indent, join.GetType().FullName));
                var binaryJoin = join as BinaryJoin;
                if (binaryJoin != null)
                {

                    if (binaryJoin.LeftHand != null)
                    {
                        stringBuilder.AppendLine(string.Format("{0} Left Side:", indent));
                        LogJoin(binaryJoin.LeftHand, stringBuilder, level + 1);
                    }
                    if (binaryJoin.RightHand != null)
                    {
                        stringBuilder.AppendLine(string.Format("{0} Right Side:", indent));
                        AliasedSource asource = binaryJoin.RightHand;
                        LogAliasedSource(asource, stringBuilder, level + 1);
                    }

                }
                var filteredJoin = join as FilteredJoin;
                if (filteredJoin != null)
                {
                    stringBuilder.AppendLine(string.Format("{0} On filters:", indent));
                    foreach (var on in filteredJoin.OnFilters)
                    {
                        stringBuilder.AppendLine(string.Format("{0}  Filter Type: {1}", indent, on.GetType().FullName));
                    }
                }

            }
        }

        public static void LogAliasedSource(AliasedSource source, StringBuilder stringBuilder, int level = 0)
        {
            var indent = GetIndent(level);
            if (source != null)
            {
                stringBuilder.AppendLine(string.Format("{0}Aliased Source, Type: {1}", indent, source.GetType().FullName));
                if (source.Alias != null)
                {
                    stringBuilder.AppendLine(string.Format("{0} Alias: {1}", indent, source.Alias));
                }

                if (source.Source != null)
                {
                    IRightJoinItem joinItem = source.Source;
                    var sourceName = joinItem.GetSourceName();
                    stringBuilder.AppendLine(string.Format("{0} Source name: {1}", indent, sourceName));
                    stringBuilder.AppendLine(string.Format("{0} Source Join Item type: {1}", indent, joinItem.GetType().FullName));
                    if (joinItem.IsTable)
                    {
                        var table = joinItem as Table;
                        stringBuilder.AppendLine(string.Format("{0}   Table name: {1}", indent, table.Name));
                        stringBuilder.AppendLine(string.Format("{0}   Table qualifier: {1}", indent, table.Qualifier));
                    }
                    else
                    {
                        var join = joinItem as Join;
                        if (join != null)
                        {

                        }
                        else
                        {
                            throw new NotSupportedException();
                        }

                    }
                }

            }

        }
    }
}