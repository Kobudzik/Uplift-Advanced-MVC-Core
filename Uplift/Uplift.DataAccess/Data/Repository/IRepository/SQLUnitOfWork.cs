using System;
using System.Collections.Generic;
using System.Text;

namespace Uplift.DataAccess.Data.Repository.IRepository
{
    public class SQLUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public SQLUnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category= new CategoryRepository(_db);
        }

        public ICategoryRepository Category { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
