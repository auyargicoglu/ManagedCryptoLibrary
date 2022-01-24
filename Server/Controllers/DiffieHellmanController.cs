using BlazorTesterForManagedCryptoLibrary.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ManagedCryptoLibrary;

namespace BlazorTesterForManagedCryptoLibrary.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DiffieHellmanController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public DiffieHellmanController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("[action]")]
        public byte[] PerformDiffeHellman(string gBModPSt)
        {
            int numberOfBytes = gBModPSt.Length / 2;
            if (numberOfBytes == 0)
            {
                return null;
            }

            byte[] gBModP = new byte[numberOfBytes];
            for (int i = 0; i < numberOfBytes; i++)
                gBModP[i] = Convert.ToByte(gBModPSt.Substring(i * 2, 2), 16);

            CCryptoEngine cryptoEngine = new CCryptoEngine("Blazor Server Side Transient Crypto Engine", (int)DateTime.Now.ToFileTimeUtc(), "diffie-hellman-group1");
            Console.WriteLine("Crypto Engine created, now g Exp A ModP is calculating ...");

            cryptoEngine.StartDiffieHellman(out byte[] gAModP);
            Console.WriteLine("gAModP :");
            Console.WriteLine(gAModP.DumpArray());

            cryptoEngine.GenerateDiffieHellmanSessionKey(gBModP, out byte[] sessionKey);
            Console.WriteLine("sessionKey :");
            Console.WriteLine(sessionKey.DumpArray());

            return gAModP;
        }
    }
}
