using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Relational;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RelationalStrings = Microsoft.Data.Entity.Relational.Strings;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;

namespace CrmAdo.EntityFramework.Update
{
  
        public class DynamicsCrmModificationCommandBatch : ReaderModificationCommandBatch
        {
            private const int DefaultNetworkPacketSizeBytes = 4096;
            private const int MaxScriptLength = 65536 * DefaultNetworkPacketSizeBytes / 2;
            private const int MaxParameterCount = 2100;
            private const int MaxRowCount = 1000;
            private int _parameterCount = 1; // Implicit parameter for the command text
            private readonly int _maxBatchSize;
            private readonly List<ModificationCommand> _bulkInsertCommands = new List<ModificationCommand>();
            private int _commandsLeftToLengthCheck = 50;

            /// <summary>
            ///     This constructor is intended only for use when creating test doubles that will override members
            ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
            ///     behavior including but not limited to throwing <see cref="NullReferenceException" />.
            /// </summary>
            protected DynamicsCrmModificationCommandBatch()
            {
            }

            public DynamicsCrmModificationCommandBatch(DynamicsCrmSqlGenerator sqlGenerator, int? maxBatchSize)
                : base(sqlGenerator)
            {
                if (maxBatchSize.HasValue
                    && maxBatchSize.Value <= 0)
                {
                    throw new ArgumentOutOfRangeException("maxBatchSize", RelationalStrings.InvalidCommandTimeout);
                }

                _maxBatchSize = Math.Min(maxBatchSize ?? Int32.MaxValue, MaxRowCount);
            }

            protected override bool CanAddCommand(ModificationCommand modificationCommand)
            {
                if (_maxBatchSize <= ModificationCommands.Count)
                {
                    return false;
                }

                var additionalParameterCount = CountParameters(modificationCommand);

                if (_parameterCount + additionalParameterCount >= MaxParameterCount)
                {
                    return false;
                }

                _parameterCount += additionalParameterCount;
                return true;
            }

            protected override bool IsCommandTextValid()
            {
                if (--_commandsLeftToLengthCheck < 0)
                {
                    var commandTextLength = GetCommandText().Length;
                    if (commandTextLength >= MaxScriptLength)
                    {
                        return false;
                    }

                    var avarageCommandLength = commandTextLength / ModificationCommands.Count;
                    var expectedAdditionalCommandCapacity = (MaxScriptLength - commandTextLength) / avarageCommandLength;
                    _commandsLeftToLengthCheck = Math.Max(1, expectedAdditionalCommandCapacity / 4);
                }

                return true;
            }

            private int CountParameters(ModificationCommand modificationCommand)
            {
                var parameterCount = 0;
                foreach (var columnModification in modificationCommand.ColumnModifications)
                {
                    if (columnModification.ParameterName != null)
                    {
                        parameterCount++;
                    }

                    if (columnModification.OriginalParameterName != null)
                    {
                        parameterCount++;
                    }
                }

                return parameterCount;
            }

            protected override void ResetCommandText()
            {
                base.ResetCommandText();
                _bulkInsertCommands.Clear();
            }

            protected override string GetCommandText()
            {
                return base.GetCommandText() + GetBulkInsertCommandText(ModificationCommands.Count);
            }

            private string GetBulkInsertCommandText(int lastIndex)
            {
                if (_bulkInsertCommands.Count == 0)
                {
                    return string.Empty;
                }

                var stringBuilder = new StringBuilder();
                var grouping = ((DynamicsCrmSqlGenerator)SqlGenerator).AppendBulkInsertOperation(stringBuilder, _bulkInsertCommands);
                for (var i = lastIndex - _bulkInsertCommands.Count; i < lastIndex; i++)
                {
                    ResultSetEnds[i] = grouping == DynamicsCrmSqlGenerator.ResultsGrouping.OneCommandPerResultSet;
                }

                ResultSetEnds[lastIndex - 1] = true;

                return stringBuilder.ToString();
            }

            protected override void UpdateCachedCommandText(int commandPosition)
            {
                var newModificationCommand = ModificationCommands[commandPosition];

                if (newModificationCommand.EntityState == EntityState.Added)
                {
                    if (_bulkInsertCommands.Count > 0
                        && _bulkInsertCommands[0].SchemaQualifiedName != newModificationCommand.SchemaQualifiedName)
                    {
                        CachedCommandText.Append(GetBulkInsertCommandText(commandPosition));
                        _bulkInsertCommands.Clear();
                    }
                    _bulkInsertCommands.Add(newModificationCommand);

                    LastCachedCommandIndex = commandPosition;
                }
                else
                {
                    CachedCommandText.Append(GetBulkInsertCommandText(commandPosition));
                    _bulkInsertCommands.Clear();

                    base.UpdateCachedCommandText(commandPosition);
                }
            }

            public override IRelationalPropertyExtensions GetPropertyExtensions(IProperty property)
            {
                //Check.NotNull(property, "property");

                return property.DynamicsCrm();
            }
        }
    
}
