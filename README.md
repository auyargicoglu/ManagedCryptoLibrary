# ManagedCryptoLibrary

Pure C# implemented managed crypto library for Blazor - WebAssembly projects which supoorts ECC, AES, DiffieHellman and SHA512. The codes are ported from CycloneCRYPTO Open C library, and tested.  

For simplicity, the project is built for a fixed configuration as it is used in our online voting project. Indeed, we only used a closed set of options like ECC 384, AES 256 Counter Mode, SHA 512 bits and DiffieHElmman (1024, 2048 and 4096bits). As many software engineers know, for some software certification issues like DO-178, unused code should be removed. This project is not targetting to be used inside C# Desktop applications, (if you wish you may use it, native is faster) but for such cases I strongly recommend you to use CycloneCRYPTO native codes and make calls from your C# codes, or you may prepare a CLR C++ encapsulation project instead of using PInvoke calls. 

But for web assembly case, which is a very limited single thread platform, you may either directly compile native to WASM or port C native code to C#. I chose the second option and my solution worked well. If you have any questions you may send me email; my email address is auyargicoglu@yahoo.com.tr    

In the project there exist Blazor client and server test projects. In these project, it is shown that how this crypto library could be integrated to an existing Blazor project. If you worked on it, you would realize that, the most important drawback is usage of synchronous - blocking functions. The speed of AES, ECC sign and verification may be acceptable, bu Diffie-Hellman key generation is slow. Also the generation of ECC keys needs time, but generation of ECC keys is expected to be called in rare scenarios.  

In the future, I plan to add async functions API. 


# AES Example

# Server Side:

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

# Client Side:

protected override async Task OnInitializedAsync()
    {
        byte[] cryptedStream = await Http.GetFromJsonAsync<byte[]>("WeatherForecast");

        CCryptoEngine cryptoEngine = new CCryptoEngine("Blazor Client Side Transient Crypto Engine", (int)DateTime.Now.ToFileTimeUtc(), "diffie-hellman-group14");

        // Lets use a predetermined key here, later we will generate our key using DiffeHellman
        byte[] key = new byte[CCryptoEngine.AesKeySize];
        for (int i = 0; i < key.Length; i++)
            key[i] = (byte)(i * i + i + 0x43);


        byte[] iv = cryptedStream.SubArray(0, CCryptoEngine.AesIvSize);
        byte[] jsonArray = cryptedStream.SubArray(CCryptoEngine.AesIvSize, cryptedStream.Length - CCryptoEngine.AesIvSize);

        Console.WriteLine("IV:");
        Console.WriteLine(iv.DumpArray());
        Console.WriteLine("Data :");
        Console.WriteLine(jsonArray.DumpArray());

        cryptoEngine.AesEndecrypt(iv, key, jsonArray, 0, jsonArray.Length);

        Console.WriteLine("Decrypted Data :");
        Console.WriteLine(jsonArray.DumpArray());

        forecasts = JsonSerializer.Deserialize<WeatherForecast[]>(jsonArray);
    }


# ECC Example (Only client side):

    private CCryptoEngine engine = null;

    private string eccPrivateKeySt = null;

    private string eccPublicKeySt = null;

    private string message = "";

    private string signatureSt = "";

    private byte[] eccPrivateKey = null;

    private byte[] eccPublicKey = null;

    private byte[] signature = null;

    private bool generatingKeys = false;

    private bool signing = false;

    private bool verifying = false;

    private int verified = 0;

    protected override void OnInitialized()
    {
        engine = new CCryptoEngine("ClientSide GenerateECKeys.razor", (int)DateTime.Now.ToFileTimeUtc(), null);
    }

    private async Task GenerateECCKeys()
    {
        generatingKeys = true;
        StateHasChanged();
        await Task.Delay(100);

        engine.GenerateECKeys(out eccPrivateKey, out eccPublicKey);
        generatingKeys = false;

        eccPrivateKeySt = eccPrivateKey.DumpArray();
        eccPublicKeySt = eccPublicKey.DumpArray();

        StateHasChanged();
    }

    private async Task Sign()
    {
        signing = true;
        StateHasChanged();
        await Task.Delay(100);

        byte[] data = JsonSerializer.SerializeToUtf8Bytes(message);
        signature = new byte[CCryptoEngine.EccSignatureSize];

        engine.Sign(eccPrivateKey, data, ref signature);
        signing = false;

        signatureSt = signature.DumpArray();

        StateHasChanged();
        await Task.Delay(100);
    }

    private async Task Verify()
    {
        verifying = true;
        StateHasChanged();
        await Task.Delay(100);

        byte[] data = JsonSerializer.SerializeToUtf8Bytes(message);

        bool verificationResult = engine.VerifySignature(eccPublicKey, data, 0, data.Length, signature);

        verifying = false;
        if (verificationResult == false)
            verified = -1;
        else
            verified = 1;

        StateHasChanged();
    }

# Diffie Hellman Example

# Server Side 

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

# Client Side

    private CCryptoEngine engine = null;

    private byte[] sessionKey = null;

    private string sessionKeySt = "";

    private bool keyGenerating = false;

    protected override void OnInitialized()
    {
        engine = new CCryptoEngine("Blazor Client Side Transient Crypto Engine", (int)DateTime.Now.ToFileTimeUtc(), "diffie-hellman-group1");
    }

    protected async Task GenerateSessionKey()
    {
        keyGenerating = true;
        StateHasChanged();
        await Task.Delay(100);

        engine.StartDiffieHellman(out byte[] gAModP);
        Console.WriteLine("gAModP :");
        Console.WriteLine(gAModP.DumpArray());

        string gAModPSt = "";
        for (int i = 0; i < gAModP.Length; i++)
            gAModPSt += gAModP[i].ToString("X2");

        string navigationSt = $"/DiffieHellman/PerformDiffeHellman?gBModPSt={gAModPSt}";
        byte[] gBModP = await Http.GetFromJsonAsync<byte[]>(navigationSt);

        engine.GenerateDiffieHellmanSessionKey(gBModP, out sessionKey);
        keyGenerating = false;

        sessionKeySt = sessionKey.DumpArray();
        Console.WriteLine("sessionKey :");
        Console.WriteLine(sessionKeySt);

        StateHasChanged();
    }

