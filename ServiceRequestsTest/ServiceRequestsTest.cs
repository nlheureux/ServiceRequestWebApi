using System.IO;
using System.Text.Json;
using Moq;
using ServiceRequestsWebApi;
using ServiceRequestsWebApi.Controllers;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc;

namespace ServiceRequestsTest
{
    public class ServiceRequestUnitTest : ServiceRequestControllerTest, IDisposable
    {
        // Test Cases: Create, read, update, delete servicerequest object
        private readonly DbConnection _connection;

        public ServiceRequestUnitTest()
            : base(
                new DbContextOptionsBuilder<ServiceRequestContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        [Fact]
        public void Post_ServiceRequest_Return_ServiceRequest()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();

                var controller = new ServiceRequestController(context);
                var result = (CreatedAtActionResult)controller.PostServiceRequest(sr).Result.Result;

                Assert.NotNull(result);
                Assert.NotNull(result.Value);
                Assert.NotNull(result.StatusCode);
                Assert.Equal(201, result.StatusCode);
                Assert.Equal(sr, result.Value);
            }
        }

        [Fact]
        public void Post_ServiceRequest_Return_BadRequest()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();
                sr.currentStatus = "invalid status";

                var controller = new ServiceRequestController(context);
                var content = (BadRequestObjectResult)controller.PostServiceRequest(sr).Result.Result;

                Assert.NotNull(content);
                Assert.NotNull(content.StatusCode);
                Assert.Equal(400, content.StatusCode);
            }
        }

        [Fact]
        public void Get_ServiceRequest_Return_ServiceRequest()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();

                var controller = new ServiceRequestController(context);
                var result = controller.PostServiceRequest(sr);
                result = controller.GetServiceRequest(sr.id);

                Assert.NotNull(result);
                Assert.NotNull(result.Result);
                Assert.Equal(sr, result.Result.Value);
            }
        }

        [Fact]
        public void Get_ServiceRequest_Return_NotFound()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();
                var controller = new ServiceRequestController(context);
                _ = controller.PostServiceRequest(sr);
                var contentStatus = (NotFoundResult)controller.GetServiceRequest(new Guid("dadd45b9-d13d-4fd1-9f29-514aa8459e0d")).Result.Result;
                Assert.NotNull(contentStatus);
                Assert.Equal(404, contentStatus.StatusCode);
            }
        }

        private ServiceRequest GetServiceRequest()
        {
            string jsonString = File.ReadAllText("/Users/nicklheureux/Documents/ServiceRequestWebApi/ServiceRequestsWebApi/servicerequest.json");
            ServiceRequest serviceRequests = JsonSerializer.Deserialize<ServiceRequest>(jsonString);

            return serviceRequests;
        }

        public void Dispose() => _connection.Dispose();

        // TODO - Write code to get service request
        // Then to update
        // then to delete it
    }
}
