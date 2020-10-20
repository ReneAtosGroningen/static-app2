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


        [FunctionName("PlayGame")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request");

            
            string inputLetter = req.Query["letter"]; 
            string inputCode = req.Query["code"]; 

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject("{" + requestBody + "}");
            // inputLetter = inputLetter ?? data?.letter;
            // inputCode = inputCode ?? data?.code;

        

            GalgjeAntwoord antwoord = new GalgjeAntwoord();

            if (string.IsNullOrWhiteSpace(inputLetter) &&
                string.IsNullOrWhiteSpace(inputCode))
            {
                // start new game
                string newWord = GetNewWord();
                string uncoded = "0::" + newWord;
                string code = EncryptString(uncoded, hashKey);
                antwoord.score=0;
                antwoord.gespeeldeLetters="";
                antwoord.woord=new string('_',newWord.Length);
                antwoord.code = code;
                antwoord.uncoded=data?.code;
            }
            else
            {         
                string decodedInput = DecodeHash(inputCode);
                if (decodedInput == "")
                {
                    return new RedirectResult("/valsspel.html");
                }
                else
                {
                    string[] parts = decodedInput.Split(':', StringSplitOptions.None);
                    int aantalFout = int.Parse(parts[0]);
                    string letters = parts[1] + inputLetter.Substring(0, 1);
                    string word = parts[2];
                    string maskedWord = "";
                    foreach (char c in word)
                    {
                        if (letters.Contains(c))
                        {
                            maskedWord += c;
                        }
                        else
                        {
                            maskedWord += "_";
                        }
                    }
                    if (!word.Contains(inputLetter.Substring(0, 1)))
                    {
                        aantalFout++;
                    }
                    string uncoded = $"{aantalFout}:{letters}:{word}";  
                    string code = EncryptString(uncoded, hashKey);

                    antwoord.score = aantalFout;
                    antwoord.gespeeldeLetters = letters;
                    antwoord.woord = maskedWord;
                    antwoord.code = code;
                    antwoord.uncoded = uncoded;
                }
                
            }
            return new OkObjectResult(antwoord);
        }

        private static string DecodeHash(string hash)
        {           
            try
            {
                return DecryptString(hash, hashKey);                
            }
            catch
            {              
                return "";
            }         
        }

        private static string DecryptString(string cipherText, string keyString)
        {
            // put back removed chars
            string s = cipherText;
            s = s.Replace("-", "+"); // 62nd char of encoding
            s = s.Replace("_", "/"); // 63rd char of encoding

            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
            }

            var fullCipher = Convert.FromBase64String(s);

            var iv = new byte[16];
            //var cipher = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length]; //changes here


            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            //Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length); // changes here
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                    return result;
                }
            }
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

        private static string EncryptString(string text, string keyString)
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
