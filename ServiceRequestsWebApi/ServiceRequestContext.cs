using Microsoft.EntityFrameworkCore;

namespace ServiceRequestsWebApi
{
    public class ServiceRequestContext : DbContext
    {
        public DbSet<ServiceRequest> serviceRequests { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=C:\servicerequest.db");
    }
}
