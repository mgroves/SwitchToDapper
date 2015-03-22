using System.Collections.Generic;
using System.Linq;
using Dapper;
using DapperDemo.Entities;
using NUnit.Framework;

namespace DapperDemo
{
    [TestFixture]
    public class Query : TestBase
    {
        [Test]
        public void UsingQueryDynamic()
        {
            Connection.Execute("CREATE TABLE Person (Name TEXT, ShoeSize INTEGER)");
            Connection.Execute("INSERT INTO Person (Name, ShoeSize) VALUES (@Name, @ShoeSize)",
                new { Name = "Matt", ShoeSize = 13 });

            IEnumerable<dynamic> result = Connection.Query("SELECT * FROM Person");

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Matt"));
            Assert.That(result.First().ShoeSize, Is.EqualTo(13));
        }
        
        [Test]
        public void UsingQueryDynamicToList()
        {
            Connection.Execute("CREATE TABLE Person (Name TEXT, ShoeSize INTEGER)");
            Connection.Execute("INSERT INTO Person (Name, ShoeSize) VALUES (@Name, @ShoeSize)",
                new { Name = "Matt", ShoeSize = 13 });

            var result = Connection.Query("SELECT * FROM Person").ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Matt"));
            Assert.That(result[0].ShoeSize, Is.EqualTo(13));
        }
        
        [Test]
        public void UsingQueryT()
        {
            Connection.Execute("CREATE TABLE Person (Name TEXT, ShoeSize INTEGER)");
            Connection.Execute("INSERT INTO Person (Name, ShoeSize) VALUES (@Name, @ShoeSize)",
                new { Name = "Matt", ShoeSize = "13" });

            var result = Connection.Query<Person>("SELECT * FROM Person").ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Matt"));
            Assert.That(result[0].ShoeSize, Is.EqualTo(13));
        }

        [Test]
        public void MismatchedColumnNames()
        {
            Connection.Execute("CREATE TABLE Person (Name TEXT, ShoeSize INTEGER)");
            Connection.Execute("INSERT INTO Person (Name, ShoeSize) VALUES (@Name, @ShoeSize)",
                new { Name = "Matt", ShoeSize = "13" });

            var result = Connection.Query<Person>(
                "SELECT Name AS PersonName, ShoeSize FROM Person")  // column name doesn't match property name
                .ToList();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo(null));      // null
            Assert.That(result[0].ShoeSize, Is.EqualTo(13));            
        }
    }
}