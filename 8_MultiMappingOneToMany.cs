using System.Collections.Generic;
using System.Linq;
using Dapper;
using DapperDemo.Entities;
using NUnit.Framework;

namespace DapperDemo
{
    public class MultiMappingOneToMany : TestBase
    {
        [Test]
        public void CanUseLookupForOneToMany()
        {
            Connection.Execute(
                @"CREATE TABLE Starship (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT)");
            var tos = Connection.ExecuteScalar<int>(
                @"INSERT INTO Starship (Name)
                  VALUES ('USS Enterprise NCC 1701');
                  SELECT last_insert_rowid();");

            Connection.Execute(
                @"CREATE TABLE CrewMember (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT,
                    StarshipId INTEGER)");
            Connection.Execute(@"INSERT INTO CrewMember (Name, StarshipId)
                VALUES (@Name, @StarshipId)", new[]
                {
                    new {Name = "Kirk", StarshipId = tos},
                    new {Name = "Spock", StarshipId = tos},
                });

            var sql = @"SELECT s.Id, s.Name, c.Id, c.Name
                        FROM Starship s
                        INNER JOIN CrewMember c ON c.StarshipId = s.Id";

            var lookup = new List<Starship>();
            Connection.Query<Starship, CrewMember, Starship>(sql,
                    (starship, crewmember) =>
                    {
                        // get ship from lookup (if any)
                        var ship = lookup.FirstOrDefault(s => s.Id == starship.Id);
                        // if it's not there, add it
                        if (ship == null)
                        {
                            lookup.Add(starship);
                            ship = starship;
                        }
                        // if there is no CrewMembers collection, instantiate it
                        ship.CrewMembers = ship.CrewMembers ?? new List<CrewMember>();
                        // add the crewmember from this row
                        ship.CrewMembers.Add(crewmember);
                        // function must return something
                        return null;
                    });
            
            Assert.That(lookup.Count, Is.EqualTo(1));
            Assert.That(lookup[0].Name, Is.EqualTo("USS Enterprise NCC 1701"));
            Assert.That(lookup[0].CrewMembers.Count, Is.EqualTo(2));
            Assert.That(lookup[0].CrewMembers[0].Name, Is.EqualTo("Kirk"));
            Assert.That(lookup[0].CrewMembers[1].Name, Is.EqualTo("Spock"));
        }
    }
}

