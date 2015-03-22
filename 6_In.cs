using Dapper;
using NUnit.Framework;

namespace DapperDemo
{
    [TestFixture]
    public class In : TestBase
    {
        [Test]
        public void UsingInKeyword()
        {
            Connection.Execute("CREATE TABLE Person (Name TEXT, ShoeSize INTEGER)");
            Connection.Execute("INSERT INTO Person (Name, ShoeSize) VALUES (@Name, @ShoeSize)",
                new []
                {
                    new { Name = "Matt", ShoeSize = 13 },
                    new { Name = "Walter", ShoeSize = 10 },
                    new { Name = "Mike", ShoeSize = 13 },
                    new { Name = "Claymore", ShoeSize = 11 },
                });

            var count = Connection.ExecuteScalar<int>(
                "SELECT COUNT(1) FROM Person WHERE ShoeSize IN @ShoeSizes",
                new {ShoeSizes = new[] {10, 11}});

            Assert.That(count, Is.EqualTo(2));
        }
    }
}