﻿using Silk.DataAccess.Data;
using Silk.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silk.DataAccess.Repository
{

    
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
        }

        //unitofwork implemented becoz Save is a global method used by all Repositories in future
        //so it should not be placed inside individual repository implementations
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
