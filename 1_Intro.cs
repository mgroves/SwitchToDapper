using Dapper;
using NUnit.Framework;

namespace DapperDemo
{
    [TestFixture]
    public class Intro : TestBase
    {
        [Test]
        public void Execute()
        {
            Connection.Execute("CREATE TABLE Foo (Field1 INTEGER, Field2 INTEGER)");
            Connection.Execute("INSERT INTO Foo (Field1, Field2) VALUES (42, 99)");

            var count = Connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Foo");
            Assert.That(count, Is.EqualTo(1));

            Connection.Execute("UPDATE Foo SET Field2 = 998");

            var field2 = Connection.ExecuteScalar<int>("SELECT Field2 FROM Foo");
            Assert.That(field2, Is.EqualTo(998));

            Connection.Execute("DELETE FROM Foo");
            count = Connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Foo");
            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void ExecuteCanUseParameters()
        {
            Connection.Execute("CREATE TABLE Person (Name TEXT, ShoeSize INTEGER)");
            Connection.Execute("INSERT INTO Person (Name, ShoeSize) VALUES (@Name, @ShoeSize)",
                new { Name = "Matt", ShoeSize = 13});

            var count = Connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Person");
            Assert.That(count, Is.EqualTo(1));
        }
        
        [Test]
        public void ExecuteCanUseTransaction()
        {
            Connection.Execute("CREATE TABLE Person (Name TEXT, ShoeSize INTEGER)");

            using (var trans = Connection.BeginTransaction())
            {
                Connection.Execute("INSERT INTO Person (Name, ShoeSize) VALUES (@Name, @ShoeSize)",
                    new {Name = "Matt", ShoeSize = 13},
                    trans);
                trans.Commit();
            }
            
            using (var trans = Connection.BeginTransaction())
            {
                Connection.Execute("INSERT INTO Person (Name, ShoeSize) VALUES (@Name, @ShoeSize)",
                    new {Name = "Claymore", ShoeSize = 10},
                    trans);
                trans.Rollback();
            }

            var count = Connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Person");
            Assert.That(count, Is.EqualTo(1));
        }
    }
}
