# ManagedCryptoLibrary
Pure C# implemented managed crypto library for Blazor - WebAssembly projects which supoorts ECC, AES, DiffieHellman and SHA512. The codes are ported from CycloneCRYPTO Open C library, and tested.  

For simplicity, the project is built for a fixed configuration as it is used in our online voting project. Indeed, I only used a closed set of options like ECC 384, AES 256 Counter Mode, SHA 512 bits and DiffieHElmman (1024, 2048 and 4096bits). This project is not supposed to be used for C# Desktop applications, (if you wish ofcourse you may use accepting performance penalities.) but for such cases I strongly recommend you to use CycloneCRYPTO native codes and make calls your C# codes, or you may prepare a CLR C++ encapsulation project instead of using PInvoke calls. 

But for web assembly case, which is a very limited single thread platform, you may either directly compile native to WASM or port C native code to C#. I chose the second option and my solution worked well. If you have any questions you may send me email; my email address is auyargicoglu@yahoo.com.tr    
