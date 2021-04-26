using System.IO;
using System.Text.Json;
using ServiceRequestsWebApi;
using ServiceRequestsWebApi.Controllers;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ServiceRequestsTest
{
    public class ServiceRequestUnitTest : ServiceRequestControllerTest, IDisposable
    {
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
                Assert.Equal(sr, result.Value);
            }
        }

        [Fact]
        public void Post_ServiceRequest_Return_201()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();

                var controller = new ServiceRequestController(context);
                var result = (CreatedAtActionResult)controller.PostServiceRequest(sr).Result.Result;

                Assert.NotNull(result);
                Assert.NotNull(result.StatusCode);
                Assert.Equal(201, result.StatusCode);
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

        [Fact]
        public void GetAll_ServiceRequests_Return_ServiceRequest()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();

                var controller = new ServiceRequestController(context);
                _ = controller.PostServiceRequest(sr);
                ServiceRequest sr2 = GetServiceRequest();
                sr2.id = new Guid("dadd45b9-d13d-4fd1-9f29-514aa8459e0d");
                List<ServiceRequest> srs = new List<ServiceRequest>();
                srs.Add(sr);
                srs.Add(sr2);
                _ = controller.PostServiceRequest(sr2);
                var results = controller.GetserviceRequests();

                Assert.NotNull(results);
                Assert.NotNull(results.Result);
                Assert.Equal(srs, results.Result.Value);
            }
        }

        [Fact]
        public void GetAll_ServiceRequests_Return_NoContent()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();

                var controller = new ServiceRequestController(context);
                var contentStatus = (NoContentResult)controller.GetserviceRequests().Result.Result;

                Assert.NotNull(contentStatus);
                Assert.Equal(204, contentStatus.StatusCode);
            }
        }

        [Fact]
        public void Put_ServiceRequest_Return_ServiceRequest()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();
                sr.currentStatus = "InProgress";
                var controller = new ServiceRequestController(context);
                _ = controller.PutServiceRequest(sr.id, sr);
                var result = controller.GetServiceRequest(sr.id);

                Assert.NotNull(result);
                Assert.Equal(sr, result.Result.Value);
            }
        }

        [Fact]
        public void Put_ServiceRequest_Return_BadRequest()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();
                sr.currentStatus = "InProgress";
                var controller = new ServiceRequestController(context);
                var contentStatus  = (BadRequestObjectResult)controller.PutServiceRequest(new Guid("dadd45b9-d13d-4fd1-9f29-514aa8459e0d"), sr).Result;
                Assert.NotNull(contentStatus);
                Assert.Equal(400, contentStatus.StatusCode);
            }
        }

        [Fact]
        public void Put_ServiceRequest_Return_BadRequest_For_Id()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();
                sr.currentStatus = "InProgress";
                var controller = new ServiceRequestController(context);
                var contentStatus = (BadRequestObjectResult)controller.PutServiceRequest(new Guid("dadd45b9-d13d-4fd1-9f29-514aa8459e0d"), sr).Result;
                Assert.NotNull(contentStatus);
                Assert.Equal(400, contentStatus.StatusCode);
            }
        }

        [Fact]
        public void Put_ServiceRequest_Return_BadRequest_For_CurrentStatus()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();
                sr.currentStatus = "Invalid code";
                var controller = new ServiceRequestController(context);
                var contentStatus = (BadRequestObjectResult)controller.PutServiceRequest(sr.id, sr).Result;
                Assert.NotNull(contentStatus);
                Assert.Equal(400, contentStatus.StatusCode);
            }
        }

        [Fact]
        public void Put_ServiceRequest_Return_NotFound()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();
                sr.currentStatus = "InProgress";
                var controller = new ServiceRequestController(context);
                sr.id = new Guid("dadd45b9-d13d-4fd1-9f29-514aa8459e0d");
                var contentStatus = (NotFoundResult)controller.PutServiceRequest(sr.id, sr).Result;
                Assert.NotNull(contentStatus);
                Assert.Equal(404, contentStatus.StatusCode);
            }
        }

        [Fact]
        public void Delete_ServiceRequest_Request_Is_Deleted()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();
                var controller = new ServiceRequestController(context);
                _ = controller.PostServiceRequest(sr);
                var result = controller.DeleteServiceRequest(sr.id);
                var deletedItem = controller.GetServiceRequest(sr.id);
                Assert.NotNull(result);
                Assert.Equal(sr, result.Result.Value);
                Assert.Null(deletedItem.Result.Value);
            }
        }

        [Fact]
        public void Delete_ServiceRequest_Return_NotFound()
        {
            using (var context = new ServiceRequestContext())
            {
                ServiceRequest sr = GetServiceRequest();
                var controller = new ServiceRequestController(context);
                sr.id = new Guid("dadd45b9-d13d-4fd1-9f29-514aa8459e0d");
                var contentStatus = (NotFoundResult)controller.DeleteServiceRequest(sr.id).Result.Result;
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
    }
}
