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

            logBuilder.AppendLine(string.Format("{0} {1}", indent, command.GetType().FullName.ToUpper()));
            logBuilder.AppendLine(string.Format("{0} Command Text: {1}", indent, commandText));

            if (command is SelectBuilder)
            {
                var selCommand = command as SelectBuilder;
                logBuilder.AppendLine(string.Format("{0} PROJECTION ", indent));
                LogProjection(selCommand.Projection, logBuilder, indentLevel);

                logBuilder.AppendLine(string.Format("{0} FROM ", indent));
                LogFrom(selCommand.From, logBuilder, indentLevel);
            }

        }

        private static void LogProjection(IEnumerable<AliasedProjection> projection, StringBuilder logBuilder, int indentLevel)
        {
            var indent = GetIndent(indentLevel);
            foreach (var f in projection)
            {
                if (f != null)
                {
                    if (!string.IsNullOrEmpty(f.Alias))
                    {
                        logBuilder.AppendLine(string.Format("{0} Column Alias: {1}", indent, f.Alias));
                    }
                    LogUtils.LogProjectionItem(f.ProjectionItem, logBuilder, indentLevel); 
                }
            }
        }

        private static void LogProjectionItem(IProjectionItem projectionItem, StringBuilder logBuilder, int indentLevel)
        {
            var indent = GetIndent(indentLevel);
            var column = projectionItem as Column;
            if (column != null)
            {
                logBuilder.AppendLine(string.Format("{0} Column Name: {1}, Qualify? {2}", indent, column.Name, column.Qualify.GetValueOrDefault()));
                if (column.Source != null)
                {
                    logBuilder.AppendLine(string.Format("{0} Column Source: ", indent));
                    LogAliasedSource(column.Source, logBuilder, indentLevel + 1);
                }
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
                        LogUtils.LogJoin(f as Join, stringBuilder, indentLevel);
                    }
                    else if (f is AliasedSource)
                    {
                        LogUtils.LogAliasedSource(f as AliasedSource, stringBuilder, indentLevel);
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
                stringBuilder.AppendLine(string.Format("{0} {1}", indent, join.GetType().Name));
                var binaryJoin = join as BinaryJoin;
                if (binaryJoin != null)
                {

                    if (binaryJoin.LeftHand != null)
                    {
                        stringBuilder.AppendLine(string.Format("{0} Left:", indent));
                        LogJoin(binaryJoin.LeftHand, stringBuilder, level + 1);
                    }
                    if (binaryJoin.RightHand != null)
                    {
                        stringBuilder.AppendLine(string.Format("{0} Right:", indent));
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
                stringBuilder.AppendLine(string.Format("{0} {1}", indent, source.GetType().FullName));
                if (!string.IsNullOrEmpty(source.Alias))
                {
                    stringBuilder.AppendLine(string.Format("{0} Alias: {1}", indent, source.Alias));
                }

                if (source.Source != null)
                {
                    IRightJoinItem joinItem = source.Source;
                    var sourceName = joinItem.GetSourceName();
                    stringBuilder.AppendLine(string.Format("{0} {1}", indent, joinItem.GetType().FullName));
                    stringBuilder.AppendLine(string.Format("{0} Source Name: {1}", indent, sourceName));
                 
                    if (joinItem.IsTable)
                    {
                        var table = joinItem as Table;
                        stringBuilder.AppendLine(string.Format("{0}   Table Name: {1}", indent, table.Name));
                        stringBuilder.AppendLine(string.Format("{0}   Table Qualifier: {1}", indent, table.Qualifier));
                    }
                    else
                    {
                        var join = joinItem as Join;
                        if (join != null)
                        {
                            LogJoin(join, stringBuilder, level + 1);
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