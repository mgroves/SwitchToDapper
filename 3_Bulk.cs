using Dapper;
using NUnit.Framework;

namespace DapperDemo
{
    [TestFixture]
    public class Bulk : TestBase
    {
        [Test]
        public void MultiInsert()
        {
            Connection.Execute("CREATE TABLE Person (Name TEXT, ShoeSize INTEGER)");
            Connection.Execute("INSERT INTO Person (Name, ShoeSize) VALUES (@Name, @ShoeSize)",
                new[]
                {
                    new {Name = "Matt", ShoeSize = 13},
                    new {Name = "Claymore", ShoeSize = 10},
                    new {Name = "Saul", ShoeSize = 11},
                    new {Name = "Mike", ShoeSize = 12},
                    new {Name = "Walter", ShoeSize = 10},
                });

            var count = Connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Person");

            Assert.That(count, Is.EqualTo(5));
        }
    }
}