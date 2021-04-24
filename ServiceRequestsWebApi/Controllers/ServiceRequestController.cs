using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ServiceRequestsWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestController : ControllerBase
    {
        private readonly ServiceRequestContext _context;

        public ServiceRequestController(ServiceRequestContext context)
        {
            _context = context;
        }

        // GET: api/ServiceRequest
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceRequest>>> GetserviceRequests()
        {
            return await _context.serviceRequests.ToListAsync();
        }

        // GET: api/ServiceRequest/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRequest>> GetServiceRequest(Guid id)
        {
            var serviceRequest = await _context.serviceRequests.FindAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return serviceRequest;
        }

        // PUT: api/ServiceRequest/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceRequest(Guid id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.id)
            {
                return BadRequest();
            }

            _context.Entry(serviceRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ServiceRequest
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ServiceRequest>> PostServiceRequest(ServiceRequest serviceRequest)
        {
            _context.serviceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServiceRequest", new { id = serviceRequest.id }, serviceRequest);
        }

        // DELETE: api/ServiceRequest/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceRequest>> DeleteServiceRequest(Guid id)
        {
            var serviceRequest = await _context.serviceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            _context.serviceRequests.Remove(serviceRequest);
            await _context.SaveChangesAsync();

            return serviceRequest;
        }

        private bool ServiceRequestExists(Guid id)
        {
            return _context.serviceRequests.Any(e => e.id == id);
        }
    }
}
