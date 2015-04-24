using System.Linq;
using Dapper;
using DapperDemo.Entities;
using NUnit.Framework;

namespace DapperDemo
{
    [TestFixture]
    public class MultiMapping : TestBase
    {
        [Test]
        public void MultiMapDefaultConvention()
        {
            Connection.Execute(
                @"CREATE TABLE ComicBookCompanies (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT)");
            var dc = Connection.ExecuteScalar<int>(
                @"INSERT INTO ComicBookCompanies (Name)
                  VALUES ('D.C.');
                  SELECT last_insert_rowid();");

            Connection.Execute(
                @"CREATE TABLE Heroes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    HeroName TEXT,
                    ComicBookCompanyId INTEGER)");
            Connection.Execute(@"INSERT INTO Heroes (HeroName, ComicBookCompanyId)
                VALUES (@HeroName, @ComicBookCompanyId)", new[]
                {
                    new {HeroName = "Batman", ComicBookCompanyId = dc},
                    new {HeroName = "Superman", ComicBookCompanyId = dc},
                });

            var sql = @"SELECT h.Id, h.HeroName, c.Id, c.Name
                    FROM Heroes h
                    INNER JOIN ComicBookCompanies c on c.Id = h.ComicBookCompanyId";

            var firstHero = Connection
                .Query<Hero, ComicBookCompany, Hero>(sql,
                    (hero, company) => {
                        hero.ComicBookCompany = company;
                        return hero;
                    })
                .First();

            Assert.That(firstHero.HeroName, Is.EqualTo("Batman"));
            Assert.That(firstHero.Id, Is.EqualTo(1));
            Assert.That(firstHero.ComicBookCompany.Name, Is.EqualTo("D.C."));
            Assert.That(firstHero.ComicBookCompany.Id, Is.EqualTo(1));
        }
        
        [Test]
        public void MultiMapCustomSplit()
        {
            Connection.Execute(@"CREATE TABLE ComicBookCompanies (CompanyId INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT)");
            var dc = Connection.ExecuteScalar<int>(@"INSERT INTO ComicBookCompanies (Name) VALUES ('D.C.'); select last_insert_rowid();");

            Connection.Execute(@"CREATE TABLE Heroes (HeroId INTEGER PRIMARY KEY AUTOINCREMENT, HeroName TEXT, ComicBookCompanyId INTEGER)");
            Connection.Execute(@"INSERT INTO Heroes (HeroName, ComicBookCompanyId) VALUES (@HeroName, @ComicBookCompanyId)", new[]
                {
                    new {HeroName = "Batman", ComicBookCompanyId = dc},
                    new {HeroName = "Superman", ComicBookCompanyId = dc},
                });

            var sql = @"SELECT h.HeroId, h.HeroName, c.CompanyId, c.Name
                    FROM Heroes h
                    INNER JOIN ComicBookCompanies c on c.CompanyId = h.ComicBookCompanyId";

            var firstHero = Connection
                .Query<Hero, ComicBookCompany, Hero>(sql,
                    (hero, company) =>
                    {
                        hero.ComicBookCompany = company; 
                        return hero;
                    },
                 splitOn: "CompanyId")
                .First();

            Assert.That(firstHero.HeroName, Is.EqualTo("Batman"));
            Assert.That(firstHero.HeroId, Is.EqualTo(1));
            Assert.That(firstHero.ComicBookCompany.Name, Is.EqualTo("D.C."));
            Assert.That(firstHero.ComicBookCompany.CompanyId, Is.EqualTo(1));
        }
        
        [Test]
        public void MoreMultiMapCustomSplit()
        {
            Connection.Execute(@"
                CREATE TABLE ComicBookCompanies (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT)");
            var dc = Connection.ExecuteScalar<int>(@"
                INSERT INTO ComicBookCompanies (Name) VALUES ('D.C.');
                select last_insert_rowid();");

            Connection.Execute(@"
                CREATE TABLE Heroes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    HeroName TEXT,
                    ComicBookCompanyId INTEGER)");
            var batman = Connection.ExecuteScalar<int>(@"
                INSERT INTO Heroes (HeroName, ComicBookCompanyId) VALUES (@HeroName, @ComicBookCompanyId);
                select last_insert_rowid();",
                new {HeroName = "Batman", ComicBookCompanyId = dc});

            Connection.Execute(@"
                CREATE TABLE Sidekicks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SidekickName TEXT,
                    HeroId INTEGER)");
            Connection.Execute(@"
                    INSERT INTO Sidekicks (SidekickName, HeroId) VALUES (@SidekickName, @HeroId)",
                new {SidekickName = "Robin", HeroId = batman});

            var sql = @"SELECT s.Id, s.SidekickName, h.Id, h.HeroName, c.Id, c.Name
                    FROM Sidekicks s
                    INNER JOIN Heroes h ON s.HeroId = h.Id
                    INNER JOIN ComicBookCompanies c on c.Id = h.ComicBookCompanyId";

            var firstSide = Connection
                .Query<Sidekick, Hero, ComicBookCompany, Sidekick>(sql, (side, hero, company) =>
                {
                    side.Hero = hero;
                    hero.ComicBookCompany = company;
                    return side;
                })
                .First();

            Assert.That(firstSide.SidekickName, Is.EqualTo("Robin"));
            Assert.That(firstSide.Id, Is.EqualTo(1));
            Assert.That(firstSide.Hero.HeroName, Is.EqualTo("Batman"));
            Assert.That(firstSide.Hero.Id, Is.EqualTo(1));
            Assert.That(firstSide.Hero.ComicBookCompany.Name, Is.EqualTo("D.C."));
            Assert.That(firstSide.Hero.ComicBookCompany.Id, Is.EqualTo(1));
        }

        // and so on, up to seven entities being multi-mapped
    }
}