using Insurance.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Tests
{
    public class ConnectionFactory: IDisposable
    {
        private bool disposed = false;

        public InsuranceDbContext CreateSQLiteContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            //We're using SQLite in memory database for testing purposes, so we need to open the connection and create the database to ensure it isn't disposed before we use it
            connection.Open();

            var option = new DbContextOptionsBuilder<InsuranceDbContext>().UseSqlite(connection).Options;

            var context = new InsuranceDbContext(option);

            context.Database.EnsureDeleted();
            context.Database.Migrate();

            return context;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
