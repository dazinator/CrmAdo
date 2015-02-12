using CrmAdo.EntityFramework.Metadata;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Migrations.MigrationsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.EntityFramework.Migrations
{
    public class DynamicsCrmMigrationSqlGenerator : MigrationSqlGenerator
    {
        public virtual void Generate(
            CreateDatabaseOperation operation,
            IModel model,
            SqlBatchBuilder builder)
        {
            //Check.NotNull(operation, nameof(operation));
            // Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE DATABASE ")
                .Append(DelimitIdentifier(operation.Name))
                .EndBatch()
                .Append("IF SERVERPROPERTY('EngineEdition') <> 5 EXECUTE sp_executesql N'ALTER DATABASE ")
                .Append(DelimitIdentifier(operation.Name))
                .Append(" SET READ_COMMITTED_SNAPSHOT ON';");
        }

        public virtual void Generate(
            DropDatabaseOperation operation,
            IModel model,
            SqlBatchBuilder builder)
        {
            //Check.NotNull(operation, nameof(operation));
            //Check.NotNull(builder, nameof(builder));

            builder
                .Append("IF SERVERPROPERTY('EngineEdition') <> 5 EXECUTE sp_executesql N'ALTER DATABASE ")
                .Append(DelimitIdentifier(operation.Name))
                .Append(" SET SINGLE_USER WITH ROLLBACK IMMEDIATE'")
                .EndBatch()
                .Append("DROP DATABASE ")
                .Append(DelimitIdentifier(operation.Name))
                .Append(";");
        }

        protected override void Generate(RenameSequenceOperation operation, IModel model, SqlBatchBuilder builder)
        {
            //Check.NotNull(operation, nameof(operation));
            //Check.NotNull(builder, nameof(builder));

            GenerateRename(operation.Name, operation.Schema, operation.NewName, "OBJECT", builder);
        }

        protected override void Generate(MoveSequenceOperation operation, IModel model, SqlBatchBuilder builder)
        {
            //Check.NotNull(operation, nameof(operation));
            //Check.NotNull(builder, nameof(builder));

            GenerateMove(operation.Name, operation.Schema, operation.NewSchema, builder);
        }

        protected override void Generate(RenameTableOperation operation, IModel model, SqlBatchBuilder builder)
        {
            //Check.NotNull(operation, nameof(operation));
            //Check.NotNull(builder, nameof(builder));

            GenerateRename(operation.Name, operation.Schema, operation.NewName, "OBJECT", builder);
        }

        protected override void Generate(MoveTableOperation operation, IModel model, SqlBatchBuilder builder)
        {
            //Check.NotNull(operation, nameof(operation));
            //Check.NotNull(builder, nameof(builder));

            GenerateMove(operation.Name, operation.Schema, operation.NewSchema, builder);
        }

        protected override void GenerateColumn(ColumnModel column, SqlBatchBuilder builder)
        {
            //Check.NotNull(column, nameof(column));
            //Check.NotNull(builder, nameof(builder));

            var computedSql = column[DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.ColumnComputedExpression];
            if (computedSql == null)
            {
                base.GenerateColumn(column, builder);

                return;
            }

            builder
                .Append(DelimitIdentifier(column.Name))
                .Append(" ")
                .Append("AS ")
                .Append(computedSql);
        }

        protected override void Generate(RenameColumnOperation operation, IModel model, SqlBatchBuilder builder)
        {
            //Check.NotNull(operation, nameof(operation));
            //Check.NotNull(builder, nameof(builder));

            GenerateRename(
                EscapeLiteral(operation.Table) + "." + EscapeLiteral(operation.Name),
                operation.Schema,
                operation.NewName,
                "COLUMN",
                builder);
        }

        protected override void GenerateIndexTraits(CreateIndexOperation operation, SqlBatchBuilder builder)
        {
            //Check.NotNull(operation, nameof(operation));
            //Check.NotNull(builder, nameof(builder));

            if (operation[DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.Clustered] == bool.TrueString)
            {
                builder.Append("CLUSTERED ");
            }
        }

        protected override void Generate(RenameIndexOperation operation, IModel model, SqlBatchBuilder builder)
        {
            //Check.NotNull(operation, nameof(operation));
            //Check.NotNull(builder, nameof(builder));

            GenerateRename(
                EscapeLiteral(operation.Table) + "." + EscapeLiteral(operation.Name),
                operation.Schema,
                operation.NewName,
                "INDEX",
                builder);
        }

        protected override string DelimitIdentifier(string identifier)
        {
            return "[" + EscapeIdentifier(identifier) + "]";
        }
        protected override string EscapeIdentifier(string identifier)
        {
            return identifier.Replace("]", "]]");
        }


        protected override void GenerateColumnTraits(ColumnModel column, SqlBatchBuilder builder)
        {
            //Check.NotNull(column, nameof(column));
            //Check.NotNull(builder, nameof(builder));

            if (column[DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.ValueGeneration] ==
                DynamicsCrmValueGenerationStrategy.Identity.ToString())
            {
                builder.Append(" IDENTITY");
            }
        }

        protected override void GeneratePrimaryKeyTraits(AddPrimaryKeyOperation operation, SqlBatchBuilder builder)
        {
            //Check.NotNull(operation, nameof(operation));
            //Check.NotNull(builder, nameof(builder));

            if (operation[DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.Clustered] != bool.TrueString)
            {
                builder.Append(" NONCLUSTERED");
            }
        }

        private void GenerateRename(
             string name,
             string schema,
             string newName,
             string objectType,
             SqlBatchBuilder builder)
        {
            builder.Append("EXECUTE sp_rename @objname = N");

            if (!string.IsNullOrWhiteSpace(schema))
            {
                builder
                .Append(Literal(schema))
                .Append(".");
            }

            builder
                .Append(Literal(name))
                .Append(", @newname = N")
                .Append(Literal(newName))
                .Append(", @objtype = N")
                .Append(Literal(objectType))
                .Append(";");
        }

        private void GenerateMove(
            string name,
            string schema,
            string newSchema,
            SqlBatchBuilder builder)
        {
            builder
               .Append("ALTER SCHEMA ")
               .Append(DelimitIdentifier(newSchema))
               .Append(" TRANSFER ")
               .Append(DelimitIdentifier(name, schema))
               .Append(";");
        }

    }
}
