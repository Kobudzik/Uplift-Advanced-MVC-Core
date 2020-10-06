using System;
using System.Collections.Generic;
using System.Text;
using Uplift.Models;

namespace Uplift.DataAccess.Data.Repository.IRepository
{
    public class SQLUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public SQLUnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category= new CategoryRepository(_db);
            Frequency= new FrequencyRepository(_db);
            Service= new ServiceRepository(_db);
            OrderHeader= new OrderHeaderRepository(_db);
            OrderDetails=new OrderDetailsRepository(_db);

        }

        public ICategoryRepository Category { get; private set; }
        public IFrequencyRepository Frequency { get; private set; }
        public IServiceRepository Service { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }


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
