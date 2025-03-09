using OpenRepairManager.Api.Data;
using OpenRepairManager.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRepairManager.Api.Api.Service
{
    public class CheckKeyService
    {
        private readonly ApplicationDbContext _context;

        public CheckKeyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsKeyValid(string key)
        {
            if (key == null)
            {
                return false;
            }

            ApiKey apiKey = _context.ApiKey.Find(key);

            if (apiKey != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
