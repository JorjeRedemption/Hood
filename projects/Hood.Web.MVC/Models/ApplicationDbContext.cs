﻿using Hood.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hood.Web.MVC
{
    public class ApplicationDbContext : HoodDbContext
    {
        public ApplicationDbContext(DbContextOptions<HoodDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            DbContextExtensions.ConfigureForHood(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            DbContextExtensions.CreateHoodModels(builder);
        }
    }
}
