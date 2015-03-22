using System.Data.SQLite;
using Dapper;
using NUnit.Framework;

namespace DapperDemo
{
    [TestFixture]
    public class Errors : TestBase
    {
        [Test]
        [ExpectedException(typeof(SQLiteException))]
        public void InvalidSql()
        {
            Connection.Execute("this is invalid SQL");
        }
    }
}