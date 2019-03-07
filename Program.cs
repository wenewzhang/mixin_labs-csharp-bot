using System;

using System.Collections.Generic;
using MixinSdk;
using MixinSdk.Bean;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;

using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.TypeConversion;

namespace mixin_labs_csharp_bot
{
    class Program
    {
    public static string ClientId     = "ddbf6591-f908-46c3-8101-cd19df31aa93";
    public static string ClientSecret = "ac67c0d80f9736039379f6cf1ba62f0d4ce47195386bf8750cf1bd00a8b4ca8c";
    public static string PinCode      = "585128";
    public static string SessionId    = "8ea48956-e9de-4775-9a49-4192f20acfd6";
    public static string PinToken     = "IUgOe7lQyLuEjrAzYgovKycYU5e9y+S0cwpamLWpmNJ4mwBFeG9l2fswU1imsOkQnhO3jA6L8l/AGC07G2JyrzEfaPT9WKBCuhlLm3UVGOWwsxU4Pa5qkafO7iSgPH+cK9Me5qpQ4H18R+zXkYuQNDz3DxuxKYtSGA8tph9+S1w=";
    public static string PrivateKey   =
    @"-----BEGIN RSA PRIVATE KEY-----
MIICXAIBAAKBgQCVn+sAxgq4zdDZdkWria2JhvveIfAeYjK7A2hyEerhSBJKeuk6
kbDeyFcqEBVPtIDOt5oDr8YLSXQGvw69u+OKJeEXaxkD/oo+AOUu+YhTrRPpP9uA
gMzDYRMXhVlbaQ9lZsZKXxWxY5g2QE4vi2FFPt0qJ5ZnRA1Evpr7J+G4DQIDAQAB
AoGAMYkVI1dte0tgZm5amHTnSA2xWxQ/S7/U7ccuD/3Qli7nJ1NT3bkYJlmLSfiz
JYUr08RDMA9EcL4rtIQSXExVAwZpWXQwNFY2HZYgYVxpKpYEagolpqnZXPdIuxGX
HBME7/VAla3crC2ncw7lzvlRRigU9rjhb/MBweJ76Mp/470CQQDuu659Zasa06L9
1h0LClNpRZOvkXmQ2rS8vxcrKU9BtG1GKI7nY4YzKD8fXe+bEmPHjpgCetkhlgYL
MUkPGMfHAkEAoHJUCcYsCX/ulJkogVu2rFUHh/Z7R658vcUi9F+Hsrkx/J+J0Aqu
7nZmLO19hv0P0dCJps+JgRWZSihZrt7JiwJBAJjkche1KSQBLn3KxsbvUgQ1nyPt
0yFGMEJBT6FAz5WQ6/rmtr7SKnxQ5jw8eNujp2uCky/jZXPxFOXOJrAYerkCQEtm
z0W0oxdnzuh4vcdlIYkFgL+Nv0vlnWvVjGLJzkzYqbwuAacKjkE01TnB9l8M6HVT
Co2hNN68Fsj6A4Oh4ZcCQEBswM6RS/X6DmsyX0o/hac/7FWjrlJu0IWx8mit0cEE
MTvukq+k3M9xkAhWuvXAEOUcxrFYE0vPWQIJUzYwNqk=
-----END RSA PRIVATE KEY-----";

        static void Main(string[] args)
        {
            MixinApi mixinApi = new MixinApi();
            mixinApi.Init(ClientId,
                          ClientSecret,
                          SessionId,
                          PinToken,
                          PrivateKey);
            string PromptMsg;
            PromptMsg  = "1: Create user and update PIN\n2: Read Bitcoin balance \n3: Read Bitcoin Address\n4: Read EOS balance\n";
          	PromptMsg += "5: Read EOS address\n6: Transfer Bitcoin from bot to new account\n7: Transfer Bitcoin from new account to Master\n";
          	PromptMsg += "8: Withdraw bot's Bitcoin\na: Verify Pin\nd: Create Address and Delete it\nr: Create Address and read it\n";
          	PromptMsg += "q: Exit \nMake your choose:";
            // Console.WriteLine(mixinApi.VerifyPIN(PinCode).ToString());
            do {
            Console.WriteLine(PromptMsg);
            var cmd = Console.ReadLine();
            if (cmd == "1") {
              var kpgen = new RsaKeyPairGenerator();

              kpgen.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 1024));

              var keyPair = kpgen.GenerateKeyPair();
              AsymmetricKeyParameter privateKey = keyPair.Private;
              AsymmetricKeyParameter publicKey = keyPair.Public;

              SubjectPublicKeyInfo info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
              string pk = Convert.ToBase64String(info.GetDerEncoded());


              var user = mixinApi.APPUser("Csharp" + (new Random().Next() % 100) + " Cat", pk);
              Console.WriteLine(user);
              using (var writer = new StreamWriter("new_users.csv",append: true))
              using (var csv = new CsvWriter(writer))
              {
                  csv.WriteField(user.user_id);

//Write Private key to CSV
                  RsaPrivateCrtKeyParameters rsaParameters = (RsaPrivateCrtKeyParameters) privateKey;
                  RSACryptoServiceProvider priKey = new RSACryptoServiceProvider();
                  priKey.ImportParameters(DotNetUtilities.ToRSAParameters(rsaParameters));
                  TextWriter pemText = new StringWriter();
                  ExportPrivateKey(priKey, pemText);
                  csv.WriteField(pemText.ToString());

                  csv.WriteField(user.pin_token);
                  csv.WriteField(user.session_id);
                  csv.NextRecord();
                  csv.Flush();
              }
            }
            if (cmd == "q") { break; }
            if (cmd == "s") {
              var u = mixinApi.SearchUser("37222956");
              Console.WriteLine(u);
              Console.WriteLine(u.user_id);
              Console.WriteLine(u.full_name);

              using (var writer = new StreamWriter("new_users.csv"))
              using (var csv = new CsvWriter(writer))
              {
                  csv.WriteField(u.user_id);
                  csv.WriteField(u.full_name);
                  csv.NextRecord();
                  csv.Flush();
              }
            }
          } while (true);
        }
        private static void ExportPrivateKey(RSACryptoServiceProvider csp, TextWriter outputStream)
        {
            if (csp.PublicOnly) throw new ArgumentException("CSP does not contain a private key", "csp");
            var parameters = csp.ExportParameters(true);
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write((byte)0x30); // SEQUENCE
                using (var innerStream = new MemoryStream())
                {
                    var innerWriter = new BinaryWriter(innerStream);
                    EncodeIntegerBigEndian(innerWriter, new byte[] { 0x00 }); // Version
                    EncodeIntegerBigEndian(innerWriter, parameters.Modulus);
                    EncodeIntegerBigEndian(innerWriter, parameters.Exponent);
                    EncodeIntegerBigEndian(innerWriter, parameters.D);
                    EncodeIntegerBigEndian(innerWriter, parameters.P);
                    EncodeIntegerBigEndian(innerWriter, parameters.Q);
                    EncodeIntegerBigEndian(innerWriter, parameters.DP);
                    EncodeIntegerBigEndian(innerWriter, parameters.DQ);
                    EncodeIntegerBigEndian(innerWriter, parameters.InverseQ);
                    var length = (int)innerStream.Length;
                    EncodeLength(writer, length);
                    writer.Write(innerStream.GetBuffer(), 0, length);
                }

                var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();
                outputStream.WriteLine("-----BEGIN RSA PRIVATE KEY-----");
                // Output as Base64 with lines chopped at 64 characters
                for (var i = 0; i < base64.Length; i += 64)
                {
                    outputStream.WriteLine(base64, i, Math.Min(64, base64.Length - i));
                }
                outputStream.WriteLine("-----END RSA PRIVATE KEY-----");
            }
        }

        private static void EncodeLength(BinaryWriter stream, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
            if (length < 0x80)
            {
                // Short form
                stream.Write((byte)length);
            }
            else
            {
                // Long form
                var temp = length;
                var bytesRequired = 0;
                while (temp > 0)
                {
                    temp >>= 8;
                    bytesRequired++;
                }
                stream.Write((byte)(bytesRequired | 0x80));
                for (var i = bytesRequired - 1; i >= 0; i--)
                {
                    stream.Write((byte)(length >> (8 * i) & 0xff));
                }
            }
        }

        private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
        {
            stream.Write((byte)0x02); // INTEGER
            var prefixZeros = 0;
            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] != 0) break;
                prefixZeros++;
            }
            if (value.Length - prefixZeros == 0)
            {
                EncodeLength(stream, 1);
                stream.Write((byte)0);
            }
            else
            {
                if (forceUnsigned && value[prefixZeros] > 0x7f)
                {
                    // Add a prefix zero to force unsigned if the MSB is 1
                    EncodeLength(stream, value.Length - prefixZeros + 1);
                    stream.Write((byte)0);
                }
                else
                {
                    EncodeLength(stream, value.Length - prefixZeros);
                }
                for (var i = prefixZeros; i < value.Length; i++)
                {
                    stream.Write(value[i]);
                }
            }
        }
    }
}
