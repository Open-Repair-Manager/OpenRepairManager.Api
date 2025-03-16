using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenRepairManager.Api.Api.Attributes;
using OpenRepairManager.Api.Data;
using OpenRepairManager.Common.Models;
using OpenRepairManager.Common.Models.ApiModels;

namespace OpenRepairManager.Api.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class RepairItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RepairItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("/api/Stats/{slug}")]
        public async Task<ActionResult<IEnumerable<RepairItem>>> GetStats(string slug)
        {
            if (slug.ToLower() == "all")
            {
                return await _context.RepairItem
                    .Include(s => s.Session)
                    .ToListAsync();
            }
            else
            {
                return await _context.RepairItem
                    .Include(s => s.Session)
                    .Where(s => s.Session.SessionSlug.ToUpper() == slug.ToUpper())
                    .ToListAsync();                
            }

        }
        
        // GET: api/RepairItems
        [HttpGet("Session/{sessionSlug}")]
        public async Task<ActionResult<IEnumerable<RepairItem>>> GetRepairItem(string sessionSlug)
        {
            return await _context.RepairItem.Include(s => s.Session).Where(r => r.Session.SessionSlug.ToUpper() == sessionSlug.ToUpper()).ToListAsync();
        }

        [HttpGet("Session/{sessionSlug}/Completed")]
        public async Task<ActionResult<IEnumerable<RepairItem>>> GetCompletedRepairItem(string sessionSlug)
        {
            return await _context.RepairItem.Include(s => s.Session).Where(r => r.Session.SessionSlug.ToUpper() == sessionSlug.ToUpper()).ToListAsync();
        }

        // GET: api/RepairItems
        [HttpGet("Session/{sessionSlug}/{category}")]
        public async Task<ActionResult<IEnumerable<RepairItem>>> GetRepairItem(string sessionSlug, string category)
        {
            return await _context.RepairItem.Include(s => s.Session).Where(r => r.Session.SessionSlug.ToUpper() == sessionSlug.ToUpper()).Where(c => c.PartnerProductCategory.ToUpper() == category.ToUpper()).Where(w => !w.Completed).ToListAsync();
        }

        [HttpGet("Session/{sessionSlug}/Totals")]
        public async Task<ActionResult<IEnumerable<RepairItem>>> GetTotals(string sessionSlug)
        {
            return await _context.RepairItem.Include(s => s.Session).Where(r => r.Session.SessionSlug.ToUpper() == sessionSlug.ToUpper()).Where(w => !w.Completed).ToListAsync();
        }

        // GET: api/RepairItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RepairItem>> GetRepairItem(int id)
        {
            var repairItem = await _context.RepairItem.FindAsync(id);

            if (repairItem == null)
            {
                return NotFound();
            }

            return repairItem;
        }

        // PUT: api/RepairItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRepairItem(int id, RepairItem repairItem)
        {
            if (id != repairItem.ID)
            {
                return BadRequest();
            }

            _context.Entry(repairItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return StatusCode(201, new Response { Status = "Success", Message = $"Item Edited Successfully!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RepairItemExists(id))
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

        // POST: api/RepairItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RepairItem>> PostRepairItem(RepairItem repairItem)
        {
            repairItem.ItemGuid = Guid.NewGuid();
            _context.RepairItem.Add(repairItem);
            await _context.SaveChangesAsync();

            return StatusCode(201, new Response { Status = "Success", Message = $"Item Added Successfully!" });
        }

        [HttpPost("ReturningItem")]
        public async Task<IActionResult> ReturningRepairItem([FromBody] ReturningItemModel model)
        {
            var olditem = await _context.RepairItem.Where(s => s.ItemGuid == model.Guid).FirstOrDefaultAsync();
            if (olditem != null)
            {
                RepairItem repairItem = new RepairItem()
                {
                    PartnerProductCategory = olditem.PartnerProductCategory,
                    Completed = false,
                    RepairStatus = olditem.RepairStatus,
                    CustomerName = olditem.CustomerName,
                    ItemGuid = Guid.NewGuid(),
                    SessionID = model.SessionID,
                    ItemName = olditem.ItemName,
                    Problem = olditem.Problem,
                    ItemNotes = olditem.ItemNotes
                };
                _context.Add(repairItem);
                await _context.SaveChangesAsync();
                return StatusCode(201, new Response { Status = "Success", Message = $"Item Added Successfully!" });
            }
            else
            {
                return StatusCode(400, new Response { Status = "Error", Message = $"Item Not Found!" });
            }
        }

        private bool RepairItemExists(int id)
        {
            return _context.RepairItem.Any(e => e.ID == id);
        }
    }
}
