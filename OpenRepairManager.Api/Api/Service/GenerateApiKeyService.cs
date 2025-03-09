using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenRepairManager.Api.Service
{
    public interface IApiKeyService
    {
        string GenerateApiKey();
    }

    internal class ApiKeyService : IApiKeyService
    {
        private const string _prefix = "ORM-";
        private const int _numberOfSecureBytesToGenerate = 32;
        private const int _lengthOfKey = 12;

        public string GenerateApiKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(_numberOfSecureBytesToGenerate);

            return string.Concat(_prefix, Convert.ToBase64String(bytes)
                .Replace("/", "")
                .Replace("+", "")
                .Replace("=", "")
                .AsSpan(0, _lengthOfKey - _prefix.Length));
        }
    }

}
