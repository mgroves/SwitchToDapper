using System.Data;
using System.Data.SQLite;
using NUnit.Framework;

namespace DapperDemo
{
    public abstract class TestBase
    {
        protected IDbConnection Connection;

        [SetUp]
        public virtual void Setup()
        {
            Connection = new SQLiteConnection("Data Source=:memory:");
            Connection.Open();
        }

        [TearDown]
        public virtual void Teardown()
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}