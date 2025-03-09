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
    public class SessionsController
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Session>>> GetSession()
        {
            return await _context.Session.OrderByDescending(s => s.SessionDate).ToListAsync();
        }
    }
}
