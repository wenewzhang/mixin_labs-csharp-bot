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
    public static string ClientId        = "ddbf6591-f908-46c3-8101-cd19df31aa93";
    public static string ClientSecret    = "ac67c0d80f9736039379f6cf1ba62f0d4ce47195386bf8750cf1bd00a8b4ca8c";
    public static string PinCode         = "585128";
    public static string SessionId       = "8ea48956-e9de-4775-9a49-4192f20acfd6";
    public static string PinToken        = "IUgOe7lQyLuEjrAzYgovKycYU5e9y+S0cwpamLWpmNJ4mwBFeG9l2fswU1imsOkQnhO3jA6L8l/AGC07G2JyrzEfaPT9WKBCuhlLm3UVGOWwsxU4Pa5qkafO7iSgPH+cK9Me5qpQ4H18R+zXkYuQNDz3DxuxKYtSGA8tph9+S1w=";
    public static string PrivateKey      =
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
    public static string MASTER_ID       = "37222956";
    public static string MASTER_UUID     = "0b4f49dc-8fb4-4539-9a89-fb3afc613747";
    public static string ASSET_ID_BTC    = "c6d0c728-2624-429b-8e0d-d9d19b6592fa";
    public static string ASSET_ID_EOS    = "6cfe566e-4aad-470b-8c9a-2fd35b49c68d";
    public static string BTC_WALLET_ADDR = "14T129GTbXXPGXXvZzVaNLRFPeHXD1C25C";
    public static string AMOUNT          = "0.001";
// # // Mixin Network support cryptocurrencies (2019-02-19)
// # // |EOS|6cfe566e-4aad-470b-8c9a-2fd35b49c68d
// # // |CNB|965e5c6e-434c-3fa9-b780-c50f43cd955c
// # // |BTC|c6d0c728-2624-429b-8e0d-d9d19b6592fa
// # // |ETC|2204c1ee-0ea2-4add-bb9a-b3719cfff93a
// # // |XRP|23dfb5a5-5d7b-48b6-905f-3970e3176e27
// # // |XEM|27921032-f73e-434e-955f-43d55672ee31
// # // |ETH|43d61dcd-e413-450d-80b8-101d5e903357
// # // |DASH|6472e7e3-75fd-48b6-b1dc-28d294ee1476
// # // |DOGE|6770a1e5-6086-44d5-b60f-545f9d9e8ffd
// # // |LTC|76c802a2-7c88-447f-a93e-c29c9e5dd9c8
// # // |SC|990c4c29-57e9-48f6-9819-7d986ea44985
// # // |ZEN|a2c5d22b-62a2-4c13-b3f0-013290dbac60
// # // |ZEC|c996abc9-d94e-4494-b1cf-2a3fd3ac5714
// # // |BCH|fd11b6e3-0b87-41f1-a41f-f0e9b49e5bf0
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
          	PromptMsg += "8: Withdraw bot's Bitcoin\n8: Withdraw bot's EOS\na: Verify Pin\nd: Create Address and Delete it\nr: Create Address and read it\n";
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

//Update the pincode of New user
                  MixinApi mixinApiNewUser = new MixinApi();
                  mixinApiNewUser.Init(user.user_id, "", user.session_id, user.pin_token, pemText.ToString());
                  Console.WriteLine(mixinApiNewUser.CreatePIN("", "123456").ToString());
              }
            }
            if (cmd == "2" || cmd == "3") {
              using (TextReader fileReader = File.OpenText(@"new_users.csv"))
              {
                  var csv = new CsvReader(fileReader);
                  csv.Configuration.HasHeaderRecord = false;
                  while (csv.Read())
                  {
                      string UserIDNewUser;
                      csv.TryGetField<string>(0, out UserIDNewUser);
                      string PrivateKeyNewUser;
                      csv.TryGetField<string>(1, out PrivateKeyNewUser);
                      string PinTokenNewUser;
                      csv.TryGetField<string>(2, out PinTokenNewUser);
                      string SessionIDNewUser;
                      csv.TryGetField<string>(3, out SessionIDNewUser);

                      MixinApi mixinApiNewUser = new MixinApi();
                      mixinApiNewUser.Init(UserIDNewUser, "", SessionIDNewUser, PinTokenNewUser, PrivateKeyNewUser);
                      Asset AssetBTC = mixinApiNewUser.ReadAsset(ASSET_ID_BTC);
                      Console.WriteLine("New User " + UserIDNewUser + " 's BTC balance is " + AssetBTC.balance);
                      Console.WriteLine("New User " + UserIDNewUser + " 's BTC address is " + AssetBTC.public_key);
                  }
              }
            }
            if (cmd == "4" || cmd == "5") {
              using (TextReader fileReader = File.OpenText(@"new_users.csv"))
              {
                  var csv = new CsvReader(fileReader);
                  csv.Configuration.HasHeaderRecord = false;
                  while (csv.Read())
                  {
                      string UserIDNewUser;
                      csv.TryGetField<string>(0, out UserIDNewUser);
                      string PrivateKeyNewUser;
                      csv.TryGetField<string>(1, out PrivateKeyNewUser);
                      string PinTokenNewUser;
                      csv.TryGetField<string>(2, out PinTokenNewUser);
                      string SessionIDNewUser;
                      csv.TryGetField<string>(3, out SessionIDNewUser);

                      MixinApi mixinApiNewUser = new MixinApi();
                      mixinApiNewUser.Init(UserIDNewUser, "", SessionIDNewUser, PinTokenNewUser, PrivateKeyNewUser);
                      Asset  AssetEOS = mixinApiNewUser.ReadAsset(ASSET_ID_EOS);
                      Console.WriteLine("New User " + UserIDNewUser + " 's EOS balance is " + AssetEOS.balance);
                      Console.WriteLine("New User " + UserIDNewUser +
                                        " 's EOS address is " + AssetEOS.account_name + " " + AssetEOS.account_tag);
                  }
              }
            }
            if (cmd == "6") {
              using (TextReader fileReader = File.OpenText(@"new_users.csv"))
              {
                  var csv = new CsvReader(fileReader);
                  csv.Configuration.HasHeaderRecord = false;
                  while (csv.Read())
                  {
                      string UserIDNewUser;
                      csv.TryGetField<string>(0, out UserIDNewUser);
                      Transfer reqInfo = mixinApi.Transfer(ASSET_ID_BTC,
                                        UserIDNewUser,
                                        AMOUNT, PinCode,
                                        System.Guid.NewGuid().ToString(),
                                        "Test");
                      Console.WriteLine(reqInfo);
                  }
              }
            }
            if (cmd == "7") {
              using (TextReader fileReader = File.OpenText(@"new_users.csv"))
              {
                  var csv = new CsvReader(fileReader);
                  csv.Configuration.HasHeaderRecord = false;
                  while (csv.Read())
                  {
                    string UserIDNewUser;
                    csv.TryGetField<string>(0, out UserIDNewUser);
                    string PrivateKeyNewUser;
                    csv.TryGetField<string>(1, out PrivateKeyNewUser);
                    string PinTokenNewUser;
                    csv.TryGetField<string>(2, out PinTokenNewUser);
                    string SessionIDNewUser;
                    csv.TryGetField<string>(3, out SessionIDNewUser);

                    MixinApi mixinApiNewUser = new MixinApi();
                    mixinApiNewUser.Init(UserIDNewUser, "", SessionIDNewUser, PinTokenNewUser, PrivateKeyNewUser);
                    // Console.WriteLine(mixinApiNewUser.CreatePIN("", "123456").ToString());
                    Transfer reqInfo = mixinApiNewUser.Transfer(ASSET_ID_BTC,
                                            MASTER_UUID,
                                            AMOUNT,
                                            "123456",
                                            System.Guid.NewGuid().ToString(),
                                            "Test");
                    Console.WriteLine(reqInfo);
                  }
              }
            }
            if (cmd == "8") {
              var addr = mixinApi.CreateAddress(ASSET_ID_BTC, BTC_WALLET_ADDR, "BTC withdraw", null, null, PinCode);
              Console.WriteLine(addr);
              // Console.WriteLine(mixinApi.Withdrawal(addr.address_id,AMOUNT,PinCode,System.Guid.NewGuid().ToString(), "Test withdraw"));
            }
            if (cmd == "9") {
              var addr = mixinApi.CreateAddress(null, null, "EOS withdraw", "eoswithmixin", "d80363afcc466fbaf2daa7328ae2adfa", PinCode);
              Console.WriteLine(addr);
              // Console.WriteLine(mixinApi.Withdrawal(addr.address_id,AMOUNT,PinCode,System.Guid.NewGuid().ToString(), "Test withdraw"));
            }
            if (cmd == "q") { break;}
            if (cmd == "d") {
              var addr = mixinApi.CreateAddress(ASSET_ID_BTC, BTC_WALLET_ADDR, "BTC withdraw", null, null, PinCode);
              Console.WriteLine(addr);
              Console.WriteLine(mixinApi.DeleteAddress(PinCode, addr.address_id));
            }
            if (cmd == "r") {
              var addr = mixinApi.CreateAddress(ASSET_ID_BTC, BTC_WALLET_ADDR, "BTC withdraw", null, null, PinCode);
              Console.WriteLine(addr);
              Console.WriteLine(mixinApi.ReadAddress(addr.address_id));
            }
            if (cmd == "qs") {
              var assets = mixinApi.ReadAssets();
              foreach (var asset in assets)
              {
                 Console.WriteLine(asset.ToString());
                 Console.WriteLine();
              }
            }
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
