using System;
using System.Text;
using SimpleMsgPack;

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

namespace bitcoin_wallet
{
    class Program
    {
        static void Main(string[] args)
        {
          // "815b0b1a-2764-3736-8faa-42d694fa620a"
          // php: gaFBxBCBWwsaJ2Q3No+qQtaU+mIK
          // c#:      xBCBWwsaJ2Q3No+qQtaU+mIK

         // "c6d0c728-2624-429b-8e0d-d9d19b6592fa"
          //php: gaFBxBDG0McoJiRCm44N2dGbZZL6
          // C#:     xBDG0McoJiRCm44N2dGbZZL6
            Console.WriteLine("c6d0c728-2624-429b-8e0d-d9d19b6592fa");
            Guid guid = new Guid("c6d0c728-2624-429b-8e0d-d9d19b6592fa");
            var gbytes = guid.ToByteArray();
            Array.Reverse(gbytes,0,4);
            Array.Reverse(gbytes,4,2);
            Array.Reverse(gbytes,6,2);
            foreach (var byt in gbytes)
            Console.Write("{0:X2} ", byt);

            MsgPack msgpack = new MsgPack();
            msgpack.SetAsBytes(gbytes);

            byte[] packData = msgpack.Encode2Bytes();
            Console.WriteLine(Convert.ToBase64String(packData));

            MixinApi mixinApi = new MixinApi();
            mixinApi.Init(USRCONFIG.ClientId,
                          USRCONFIG.ClientSecret,
                          USRCONFIG.SessionId,
                          USRCONFIG.PinToken,
                          USRCONFIG.PrivateKey);
            string PromptMsg;
            PromptMsg  = "1: Create Bitcoin Wallet and update PIN\n2: Read Bitcoin balance & address \n3: Read USDT balance & address\n4: Read EOS balance & address\n";
            PromptMsg += "5: Read EOS address\n6: Transfer Bitcoin from bot to new account\n7: Transfer Bitcoin from new account to Master\n";
            PromptMsg += "8: Withdraw bot's Bitcoin\n8: Withdraw bot's EOS\na: Verify Pin\nd: Create Address and Delete it\nr: Create Address and read it\n";
            PromptMsg += "q: Exit \nMake your choose:";
            // Console.WriteLine(mixinApi.VerifyPIN(PinCode).ToString());
            do {
            Console.WriteLine(PromptMsg);
            var cmd = Console.ReadLine();
            if (cmd == "q") { break;}
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

              using (var writer = new StreamWriter("mybitcoin_wallet.csv",append: true))
              using (var csv = new CsvWriter(writer))
              {
                  //Write Private key to CSV
                  RsaPrivateCrtKeyParameters rsaParameters = (RsaPrivateCrtKeyParameters) privateKey;
                  RSACryptoServiceProvider priKey = new RSACryptoServiceProvider();
                  priKey.ImportParameters(DotNetUtilities.ToRSAParameters(rsaParameters));
                  TextWriter pemText = new StringWriter();
                  ExportPrivateKey(priKey, pemText);
                  csv.WriteField(pemText.ToString());

                  csv.WriteField(user.pin_token);
                  csv.WriteField(user.session_id);
                  csv.WriteField(user.user_id);
                  csv.WriteField("123456");
                  csv.NextRecord();
                  csv.Flush();

//Update the pincode of New user
                  MixinApi mixinApiNewUser = new MixinApi();
                  mixinApiNewUser.Init(user.user_id, "", user.session_id, user.pin_token, pemText.ToString());
                  Console.WriteLine(mixinApiNewUser.CreatePIN("", "123456").ToString());
              }
            }
            if (cmd == "2" ) {
              using (TextReader fileReader = File.OpenText(@"mybitcoin_wallet.csv"))
              {
                  var csv = new CsvReader(fileReader);
                  csv.Configuration.HasHeaderRecord = false;
                  while (csv.Read())
                  {
                      string PrivateKeyNewUser;
                      csv.TryGetField<string>(0, out PrivateKeyNewUser);
                      string PinTokenNewUser;
                      csv.TryGetField<string>(1, out PinTokenNewUser);
                      string SessionIDNewUser;
                      csv.TryGetField<string>(2, out SessionIDNewUser);
                      string UserIDNewUser;
                      csv.TryGetField<string>(3, out UserIDNewUser);
                      string PinNewUser;
                      csv.TryGetField<string>(4, out PinNewUser);
                      MixinApi mixinApiNewUser = new MixinApi();
                      mixinApiNewUser.Init(UserIDNewUser, "", SessionIDNewUser, PinTokenNewUser, PrivateKeyNewUser);
                      Asset AssetBTC = mixinApiNewUser.ReadAsset(USRCONFIG.ASSET_ID_BTC);
                      Console.WriteLine("New User " + UserIDNewUser + " 's BTC balance is " + AssetBTC.balance);
                      Console.WriteLine("New User " + UserIDNewUser + " 's BTC address is " + AssetBTC.public_key);
                  }
              }
            }
            if (cmd == "3" ) {
              using (TextReader fileReader = File.OpenText(@"mybitcoin_wallet.csv"))
              {
                  var csv = new CsvReader(fileReader);
                  csv.Configuration.HasHeaderRecord = false;
                  while (csv.Read())
                  {
                      string PrivateKeyNewUser;
                      csv.TryGetField<string>(0, out PrivateKeyNewUser);
                      string PinTokenNewUser;
                      csv.TryGetField<string>(1, out PinTokenNewUser);
                      string SessionIDNewUser;
                      csv.TryGetField<string>(2, out SessionIDNewUser);
                      string UserIDNewUser;
                      csv.TryGetField<string>(3, out UserIDNewUser);
                      string PinNewUser;
                      csv.TryGetField<string>(4, out PinNewUser);
                      MixinApi mixinApiNewUser = new MixinApi();
                      mixinApiNewUser.Init(UserIDNewUser, "", SessionIDNewUser, PinTokenNewUser, PrivateKeyNewUser);
                      Asset AssetInfo = mixinApiNewUser.ReadAsset(USRCONFIG.ASSET_ID_USDT);
                      Console.WriteLine("New User " + UserIDNewUser + " 's USDT balance is " + AssetInfo.balance);
                      Console.WriteLine("New User " + UserIDNewUser + " 's USDT address is " + AssetInfo.public_key);
                  }
              }
            }
            if (cmd == "4" ) {
              using (TextReader fileReader = File.OpenText(@"mybitcoin_wallet.csv"))
              {
                  var csv = new CsvReader(fileReader);
                  csv.Configuration.HasHeaderRecord = false;
                  while (csv.Read())
                  {
                      string PrivateKeyNewUser;
                      csv.TryGetField<string>(0, out PrivateKeyNewUser);
                      string PinTokenNewUser;
                      csv.TryGetField<string>(1, out PinTokenNewUser);
                      string SessionIDNewUser;
                      csv.TryGetField<string>(2, out SessionIDNewUser);
                      string UserIDNewUser;
                      csv.TryGetField<string>(3, out UserIDNewUser);
                      string PinNewUser;
                      csv.TryGetField<string>(4, out PinNewUser);
                      MixinApi mixinApiNewUser = new MixinApi();
                      mixinApiNewUser.Init(UserIDNewUser, "", SessionIDNewUser, PinTokenNewUser, PrivateKeyNewUser);
                      Asset AssetInfo = mixinApiNewUser.ReadAsset(USRCONFIG.ASSET_ID_EOS);
                      Console.WriteLine("New User " + UserIDNewUser + " 's EOS balance is " + AssetInfo.balance);
                      Console.WriteLine("New User " + UserIDNewUser + " 's EOS address is " + AssetInfo.account_name +
                                        " " + AssetInfo.account_tag);
                  }
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
