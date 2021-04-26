using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ServiceRequestsWebApi;

namespace ServiceRequestsTest
{
    public class ServiceRequestControllerTest
    {
        protected ServiceRequestControllerTest(DbContextOptions<ServiceRequestContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }

        protected DbContextOptions<ServiceRequestContext> ContextOptions { get; }

        private void Seed()
        {
            using (var context = new ServiceRequestContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.SaveChanges();
            }
        }
    }
}
