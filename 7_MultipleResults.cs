using System.Linq;
using Dapper;
using NUnit.Framework;

namespace DapperDemo
{
    [TestFixture]
    public class MultipleResults : TestBase
    {
        [Test]
        public void MultipleResultsSingleQuery()
        {
            Connection.Execute("CREATE TABLE Foo (Field1 INTEGER)");
            Connection.Execute("CREATE TABLE Bar (Field2 INTEGER)");
            Connection.Execute("CREATE TABLE Baz (Field3 INTEGER)");

            Connection.Execute("INSERT INTO Foo (Field1) VALUES (1)");
            Connection.Execute("INSERT INTO Bar (Field2) VALUES (2)");
            Connection.Execute("INSERT INTO Baz (Field3) VALUES (3)");

            var sql = @"SELECT Field1 FROM Foo;
                        SELECT Field2 FROM Bar;
                        SELECT Field3 FROM Baz";

            using (var multi = Connection.QueryMultiple(sql))
            {
                var foo = multi.Read().First();
                var bar = multi.Read().First();
                var baz = multi.Read().First();

                Assert.That(foo.Field1, Is.EqualTo(1));
                Assert.That(bar.Field2, Is.EqualTo(2));
                Assert.That(baz.Field3, Is.EqualTo(3));
            }
        }
    }
}