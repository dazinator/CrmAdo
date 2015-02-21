using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.Entity.DynamicsCrm;
using Microsoft.Data.Entity.ValueGeneration;

namespace Microsoft.Data.Entity.DynamicsCrm
{           
        public class DynamicsCrmSequenceValueGeneratorFactory
        {
            private readonly SqlStatementExecutor _executor;

            public DynamicsCrmSequenceValueGeneratorFactory(SqlStatementExecutor executor)
            {
               // Check.NotNull(executor, nameof(executor));

                _executor = executor;
            }

            public virtual ValueGenerator Create(
                IProperty property,
                DynamicsCrmSequenceValueGeneratorState generatorState,
                DynamicsCrmConnection connection)
            {
               // Check.NotNull(property, nameof(property));
               // Check.NotNull(generatorState, nameof(generatorState));
               // Check.NotNull(connection, nameof(connection));

                if (property.PropertyType.UnwrapNullableType() == typeof(long))
                {
                    return new DynamicsCrmSequenceValueGenerator<long>(_executor, generatorState, connection);
                }

                if (property.PropertyType.UnwrapNullableType() == typeof(int))
                {
                    return new DynamicsCrmSequenceValueGenerator<int>(_executor, generatorState, connection);
                }

                if (property.PropertyType.UnwrapNullableType() == typeof(short))
                {
                    return new DynamicsCrmSequenceValueGenerator<short>(_executor, generatorState, connection);
                }

                if (property.PropertyType.UnwrapNullableType() == typeof(byte))
                {
                    return new DynamicsCrmSequenceValueGenerator<byte>(_executor, generatorState, connection);
                }

                if (property.PropertyType.UnwrapNullableType() == typeof(ulong))
                {
                    return new DynamicsCrmSequenceValueGenerator<ulong>(_executor, generatorState, connection);
                }

                if (property.PropertyType.UnwrapNullableType() == typeof(uint))
                {
                    return new DynamicsCrmSequenceValueGenerator<uint>(_executor, generatorState, connection);
                }

                if (property.PropertyType.UnwrapNullableType() == typeof(ushort))
                {
                    return new DynamicsCrmSequenceValueGenerator<ushort>(_executor, generatorState, connection);
                }

                if (property.PropertyType.UnwrapNullableType() == typeof(sbyte))
                {
                    return new DynamicsCrmSequenceValueGenerator<sbyte>(_executor, generatorState, connection);
                }

                throw new ArgumentException("DynamicsCrmSequenceValueGeneratorFactory");
                //Strings.InvalidValueGeneratorFactoryProperty(
                //    "DynamicsCrmSequenceValueGeneratorFactory", property.Name, property.EntityType.SimpleName));
            }
        }

  
}
