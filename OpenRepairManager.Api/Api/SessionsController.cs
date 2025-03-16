using Microsoft.AspNetCore.Mvc;
using OpenRepairManager.Api.Api.Attributes;
using OpenRepairManager.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRepairManager.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace OpenRepairManager.Api.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class SessionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Session>>> GetSession(int count, string orderBy)
        {
            if (orderBy == "desc")
            {
                if (count == 0)
                {
                    return await _context.Session.Include(l => l.Location).OrderByDescending(s => s.SessionDate).ToListAsync();
                }
                return await _context.Session.Include(l => l.Location).OrderByDescending(s => s.SessionDate).Take(count).ToListAsync();
            }
            else
            {
                if (count == 0)
                {
                    return await _context.Session.Include(l => l.Location).OrderBy(s => s.SessionDate).ToListAsync();
                }
                return await _context.Session.Include(l => l.Location).OrderBy(s => s.SessionDate).Take(count).ToListAsync();
            }
            
        }
    
        [HttpGet("/api/Session/View/{locationSlug}")]
        public async Task<ActionResult<Session>> GetSingleSession(string locationSlug)
        {
            var session = await _context.Session.Include(l => l.Location).Where(s => s.SessionSlug == locationSlug).FirstOrDefaultAsync();
            if (session == null)
            {
                return NotFound();
            }
            else
            {
                return session;
            }
            
        }
    }
}
