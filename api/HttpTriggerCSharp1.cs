using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace Company.Function
{
    public static class HttpTriggerCSharp
    {
        const string hashKey = "E546C8DF278CD5931069B522E695D4F2";
        //The key sizes need to be 128, 192 or 256 bits. 
        //Since the code above reads bytes from a UTF8 string min key length is 16 characters, 24 and 32 chars lengths will also work. 
        //Anything else will throw an exception


        [FunctionName("NewGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request for new game.");

            string newWord = GetNewWord();
            string uncoded = "0::" + newWord;
            string code = await EncryptString(uncoded, hashKey);
            GalgjeAntwoord antwoord = new GalgjeAntwoord();
            antwoord.woord=new string('_',newWord.Length);
            antwoord.code = code;
            antwoord.uncoded=uncoded;
            return new OkObjectResult(antwoord);
        }


        private static string GetNewWord()
        {
            List<string> woorden = new List<string>();
            woorden.Add("behanglijm");
            woorden.Add("sekswerker");
            woorden.Add("antigeluid");
            woorden.Add("cappuccino");
            woorden.Add("duinvallei");
            woorden.Add("euromunten");
            woorden.Add("flinterdun");
            woorden.Add("genadeloos");
            woorden.Add("herfstwind");
            woorden.Add("inburgeren");
            woorden.Add("jeneverbes");
            woorden.Add("katjesdrop");
            woorden.Add("legpuzzels");
            woorden.Add("malariamug");
            woorden.Add("nylondraad");
            woorden.Add("ochtendzon");
            woorden.Add("pepernoten");
            woorden.Add("quizmaster");
            woorden.Add("reanimeren");
            woorden.Add("tarwebloem");
            woorden.Add("uilskuiken");
            woorden.Add("verbouwing");
            woorden.Add("webwinkels");
            woorden.Add("yoghurtijs");
            woorden.Add("zoldertrap");

            int index =  GetRandomKeuze(woorden.Count); 

            return woorden[index];
        }

        private static int GetRandomKeuze(int count)
        {
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            byte[] data = new byte[8];
            ulong value;
            do
            {
                rngCsp.GetBytes(data);
                value = BitConverter.ToUInt64(data, 0);
            } while (value == 0);
            return (int)(value % (ulong)count);
        }

        public static async Task<string> EncryptString(string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);
            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        string s = Convert.ToBase64String(result);
                        s = s.Split("=")[0]; // Remove any trailing '='s
                        s = s.Replace("+", "-"); // 62nd char of encoding
                        s = s.Replace("/", "_"); // 63rd char of encoding                            
                        return s;
                    }
                }
            }
      
        }
    }

}
