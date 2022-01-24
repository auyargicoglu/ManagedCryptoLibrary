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
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public byte[] Get()
        {
            var rng = new Random();
            WeatherForecast[] forecasts = new WeatherForecast[5];

            for(int index = 0; index < forecasts.Length; index++)
            {
                forecasts[index] = new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                };
            }

            // Instead of sending the typed data, convert it to byte array first
            // So it will be possible to alter the output's context and length
            // In real life projests, in order to mimimize the data to be sent to the client side
            // we prefer to use our serializers

            byte[] jsonArray = JsonSerializer.SerializeToUtf8Bytes(forecasts);

            // What are the crypto engine constructor arguments?
            // The first argument ownerSt is useful for managed crypto console logs
            // With the string given here, you may recognize a spesific crypto engine's log from the others
            // The second ownerSeed is used random number generator, if you are a programmer who is responsible
            // from system security, you are going to alter random number generations performed by this library
            // The third argument is the selected diffie-hellman, which will be used during key exchange
            // If you supply this argument as null, you will select default 4096 bits Diffe-Hellman which is named as 
            // "diffie-hellman-group16". But in this function we prefered 2048 bits version of it.

            CCryptoEngine cryptoEngine = new CCryptoEngine("Blazor Server Side Transient Crypto Engine",  (int)DateTime.Now.ToFileTimeUtc(), "diffie-hellman-group14");

            // Lets use a predetermined key here, later we will generate our key using DiffeHellman
            byte[] key = new byte[CCryptoEngine.AesKeySize];
            for (int i = 0; i < key.Length; i++)
                key[i] = (byte)(i * i + i + 0x43);

            // In our managed crypto library, we use counter mode which means every block is encrypted with an IV which is calculated
            // by incrementing the previous IV. If you send encrypted data over a lossy medium, and if you are able to store IV generated  
            // when the encrytion session begins, counter mode is very helpful. Do not forget that, counter mode cannot be used by very crypto algorithm. 

            cryptoEngine.GenerateIV(out byte[] iv);

            // Lets see what we are doing, are we really encrypting? First dump the data
            Console.WriteLine("IV:");
            Console.WriteLine(iv.DumpArray());
            Console.WriteLine("Data :");
            Console.WriteLine(jsonArray.DumpArray());

            cryptoEngine.AesEndecrypt(iv, key, jsonArray, 0, jsonArray.Length);

            // Now dump the encrypted data
            Console.WriteLine("Encryped Data :");
            Console.WriteLine(jsonArray.DumpArray());

            byte[] cryptedStream = new byte[iv.Length + jsonArray.Length];
            cryptedStream.CopyIn(0, iv, 0, CCryptoEngine.AesIvSize);
            cryptedStream.CopyIn(CCryptoEngine.AesIvSize, jsonArray, 0, jsonArray.Length);

            return cryptedStream;
        }
    }
}
