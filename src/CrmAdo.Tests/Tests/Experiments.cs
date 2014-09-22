using System;
using System.Text;
using NUnit.Framework;
using SQLGeneration.Builders;
using SQLGeneration.Generators;

namespace CrmAdo.Tests
{
    public class Experiments
    {

        [Category("Experimentation")]
        [Test]
        public void Experiment_With_Joins()
        {

            var logBuilder = new StringBuilder();

            string joinType = "INNER";
            var sql = string.Format("Select C.contactid, C.firstname, C.lastname From contact AS C {0} JOIN customeraddress AS A on C.contactid = A.contactid", joinType);

            var commandText = sql;
            var commandBuilder = new CommandBuilder();
            var builder = commandBuilder.GetCommand(commandText) as SelectBuilder;
            LogUtils.LogCommand(builder, logBuilder);
            logBuilder.AppendLine();

            var nestedJoinsSql =
                string.Format(
                    "Select contactid, firstname, lastname From contact INNER JOIN customeraddress on contact.id = customeraddress.contactid INNER JOIN occupant on customeraddress.addressid = occupant.addressid ",
                    joinType);

            builder = commandBuilder.GetCommand(nestedJoinsSql) as SelectBuilder;
            LogUtils.LogCommand(builder, logBuilder);
            logBuilder.AppendLine();

            var anotherJoinSql =
             string.Format(
                 "Select contactid, firstname, lastname From contact INNER JOIN customeraddress on contact.id = customeraddress.contactid INNER JOIN occupant on contact.contactid = occupant.contactid ",
                 joinType);

            builder = commandBuilder.GetCommand(anotherJoinSql) as SelectBuilder;
            LogUtils.LogCommand(builder, logBuilder);
            logBuilder.AppendLine();


            var moreSql =
            string.Format(
                "Select C.contactid, C.firstname, C.lastname, O.fullname From contact C INNER JOIN customeraddress A on c.id = A.contactid LEFT JOIN occupant O on C.contactid = O.contactid ",
                joinType);

            builder = commandBuilder.GetCommand(moreSql) as SelectBuilder;
            LogUtils.LogCommand(builder, logBuilder);
            logBuilder.AppendLine();


            Console.Write(logBuilder.ToString());

        }

    }
}
