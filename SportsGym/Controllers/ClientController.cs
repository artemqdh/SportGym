using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGym.Models.Entities;
using SportsGym.Services;

namespace SportsGym.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly PostgresConnection _db;

        public ClientController(PostgresConnection db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<List<Client>> GetClients()  ///< Get all clients
        {
            return await _db.Clients.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)  ///< Get client by ID
        {
            Client client = await _db.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)  ///< Add new client
        {
            _db.Clients.Add(client);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> PutClient(int id, Client updatedClient)  ///< Update client details
        {
            if (id != updatedClient.Id)
            {
                return BadRequest();
            }

            _db.Entry(updatedClient).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> DeleteClient(int id)  ///< Delete client
        {
            var client = await _db.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
