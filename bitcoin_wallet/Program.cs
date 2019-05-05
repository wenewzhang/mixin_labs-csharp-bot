using System;
using System.Text;
using MsgPack;
using MsgPack.Serialization;
using System.Linq;
using System.Net.Http;

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
using CsvHelper;
using CsvHelper.TypeConversion;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
namespace bitcoin_wallet
{
    public class ExchangeResult
    {
        public string C { get; set; }
        public string P { get; set; }
        public string F { get; set; }
        public string FA { get; set; }
        public string T { get; set; }
        public string R { get; set; }
        public string O { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class MarketInfo
    {
        public string code { get; set; }
        public string message { get; set; }
        public List<AssetInfo> data { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class AssetInfo
    {
        public string base_asset { get; set; }
        public string base_asset_symbol { get; set; }
        public string exchange_asset_symbol { get; set; }
        public string price  { get; set; }
        public string minimum_amount { get; set; }
        public string maximum_amount  { get; set; }
        public List<string> exchanges  { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

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
            Guid guid = new Guid("c6d0c728-2624-429b-8e0d-d9d19b6592fa");
            var gbytes = guid.ToByteArray();
            Array.Reverse(gbytes,0,4);
            Array.Reverse(gbytes,4,2);
            Array.Reverse(gbytes,6,2);
            // foreach (var byt in gbytes)
            // Console.Write("{0:X2} ", byt);

            var serializer = MessagePackSerializer.Get(gbytes.GetType());
            // Pack obj to stream.
            var stream = new MemoryStream();
            serializer.Pack(stream, gbytes);
            Console.WriteLine(Convert.ToBase64String(stream.ToArray()));


            string dataStr = "hqFDzQPooVCnMzkzOC42MqFGqTAuMDAwNzg3OKJGQcQQgVsLGidkNzaPqkLWlPpiCqFUoVKhT8QQGj2FYSbnSbuK4+2Fziu5Vw==";
            var myByteArray = Convert.FromBase64String(dataStr);
            var str = MessagePackSerializer.UnpackMessagePackObject(myByteArray);
            Console.WriteLine(str);

            MixinApi mixinApi = new MixinApi();
            mixinApi.Init(USRCONFIG.ClientId,
                          USRCONFIG.ClientSecret,
                          USRCONFIG.SessionId,
                          USRCONFIG.PinToken,
                          USRCONFIG.PrivateKey);
            string PromptMsg;
            PromptMsg  = "1: Create Bitcoin Wallet and update PIN\n2: Read Bitcoin balance & address \n3: Read USDT balance & address\n4: Read EOS balance & address\n";
            PromptMsg += "5: pay 0.0001 BTC buy USDT\n6: pay $1 USDT buy BTC\n7: Read Snapshots\n8: Fetch market price(USDT)\n9: Fetch market price(BTC)\n";
            PromptMsg += "v: Verify Wallet Pin\n";
            PromptMsg += "ab: Read Bot Assets \naw: Read Wallet Assets\n";
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
            if (cmd == "v" ) {
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
                      Console.WriteLine("\n\n======== Test Verify PIN ===========\n");
                      Console.WriteLine("PIN is " + PinNewUser + " user id is " + UserIDNewUser);
                      // Console.WriteLine(mixinApiNewUser.CreatePIN("", PinNewUser).ToString());
                      Console.WriteLine(mixinApiNewUser.VerifyPIN(PinNewUser.ToString()).ToString());
                  }
              }
            }
            if (cmd == "aw" ) {
                // Console.WriteLine(mixinApi.VerifyPIN(USRCONFIG.PinCode.ToString()).ToString());
                MixinApi mixinApiNewUser = GetWalletSDK();
                var assets = mixinApiNewUser.ReadAssets();
                string wuuid = GetWalletUUID();
                Console.WriteLine("Current wallet uuid is " + wuuid);
                foreach (Asset asset in assets)
                {
                  if (asset.symbol == "EOS") {
                   Console.WriteLine(asset.symbol + " Public Address is: " +
                                     asset.account_name + " " +
                                     asset.account_tag +
                                     " Balance is: " + asset.balance);
                 } else Console.WriteLine(asset.symbol + " Public Address is: " +
                                          asset.public_key + " Balance is: " +
                                          asset.balance);
                Console.WriteLine();
                }
            }
            if (cmd == "ab" ) {
                var assets = mixinApi.ReadAssets();
                foreach (Asset asset in assets)
                {
                  if (asset.symbol == "EOS") {
                   Console.WriteLine(asset.symbol + " Public Address is: " +
                                     asset.account_name + " " +
                                     asset.account_tag +
                                     " Balance is: " + asset.balance);
                 } else Console.WriteLine(asset.symbol + " Public Address is: " +
                                          asset.public_key + " Balance is: " +
                                          asset.balance);
                Console.WriteLine();
                }
            }
            if (cmd == "5" ) {
                var memo = TargetAssetID(USRCONFIG.ASSET_ID_USDT);
                Console.WriteLine(memo);
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
                      // Console.WriteLine(mixinApiNewUser.CreatePIN("", "123456").ToString());
                      Transfer reqInfo = mixinApiNewUser.Transfer(USRCONFIG.ASSET_ID_BTC,
                                              USRCONFIG.EXIN_BOT,
                                              "0.0001",
                                              PinNewUser.ToString(),
                                              System.Guid.NewGuid().ToString(),
                                              memo);
                      Console.WriteLine(reqInfo);
                    }
                }
            }
            if (cmd == "6" ) {
                var memo = TargetAssetID(USRCONFIG.ASSET_ID_BTC);
                Console.WriteLine(memo);
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
                      // Console.WriteLine(mixinApiNewUser.CreatePIN("", "123456").ToString());
                      Transfer reqInfo = mixinApiNewUser.Transfer(USRCONFIG.ASSET_ID_USDT,
                                              USRCONFIG.MASTER_UUID,
                                              "0.1078188",
                                              PinNewUser.ToString(),
                                              System.Guid.NewGuid().ToString(),
                                              memo);
                      Console.WriteLine(reqInfo);
                    }
                }
            }
            if (cmd == "7" ) {
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
                    // Console.WriteLine(mixinApiNewUser.CreatePIN("", "123456").ToString());
                    var snaps = mixinApiNewUser.NetworkSnapshots(10,"2019-03-26T01:49:52.462741863Z", "815b0b1a-2764-3736-8faa-42d694fa620a", "ASC",true);
                    // Console.WriteLine(snaps);
                    foreach (var sn in snaps)
                    {
                      if ( Convert.ToDouble(sn.amount) > 0 ) {

                        if ( sn.data != null ) {
                          var memoBytes = Convert.FromBase64String(sn.data);
                          var memoObj = MessagePackSerializer.UnpackMessagePackObject(memoBytes);
                          Console.WriteLine(memoObj.ToString());

                          var xR = JsonConvert.DeserializeObject<ExchangeResult>(memoObj.ToString());
                          Console.WriteLine(xR.C);
                          if (xR.C == "1000") {
                            Console.WriteLine("-----------Successfully--Exchange-------------");
                            Console.WriteLine("You got " + sn.amount.ToString() + " back!");
                            Console.WriteLine("Price is  " + xR.P + " Fee is " + xR.F +
                                              " Percent of fee: " +
                                              Convert.ToDouble(xR.F)/Convert.ToDouble(sn.amount)*100 + " %");
                            Console.WriteLine("Fee Asset uuid: " + HexStringToUUID(xR.FA));
                            Console.WriteLine("trace  uuid: " + HexStringToUUID(xR.O));
                            Console.WriteLine("----------end of snapshots query--------------");
                          }
                        }
                      }
                    }
                  }
              }
            }
            if (cmd == "8" ) {
              string jsonData = FetchMarketPrice("815b0b1a-2764-3736-8faa-42d694fa620a");
              var marketObj = JsonConvert.DeserializeObject<MarketInfo>(jsonData);
              foreach (AssetInfo value in marketObj.data)
              {
                  Console.WriteLine(value);
              }
            }
            if (cmd == "9" ) {
              string jsonData = FetchMarketPrice("c6d0c728-2624-429b-8e0d-d9d19b6592fa");
              var marketObj = JsonConvert.DeserializeObject<MarketInfo>(jsonData);
              foreach (AssetInfo value in marketObj.data)
              {
                  Console.WriteLine(value);
              }
            }
          } while (true);
        }

        public static string FetchMarketPrice(string asset_id)
        {
            return FetchMarketPriceAsync(asset_id).Result;
        }
        public static async Task<string> FetchMarketPriceAsync(string asset_id)
        {
          HttpClient client = new HttpClient();
          // Call asynchronous network methods in a try/catch block to handle exceptions
          try
          {
             HttpResponseMessage response = await client.GetAsync("https://exinone.com/exincore/markets?base_asset=" + asset_id);
             response.EnsureSuccessStatusCode();
             string responseBody = await response.Content.ReadAsStringAsync();
             // Above three lines can be replaced with new helper method below
             // string responseBody = await client.GetStringAsync(uri);
             Console.WriteLine(responseBody);
             return responseBody;
          }
          catch(HttpRequestException e)
          {
             Console.WriteLine("\nException Caught!");
             Console.WriteLine("Message :{0} ",e.Message);
          }
          return null;
        }

        public static string HexStringToUUID(string hex) {
          if (hex.Length == 34) {
            byte[] bytes = StringToByteArray(hex.Substring(2, 32));
            Array.Reverse(bytes,0,4);
            Array.Reverse(bytes,4,2);
            Array.Reverse(bytes,6,2);
            Guid asset_uuid = new Guid(bytes);
            return asset_uuid.ToString();
          } else return null;
        }
        public static byte[] StringToByteArray(string hex) {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        private static string TargetAssetID(string asset_id) {
          Guid guid = new Guid(asset_id);
          var gbytes = guid.ToByteArray();
          Array.Reverse(gbytes,0,4);
          Array.Reverse(gbytes,4,2);
          Array.Reverse(gbytes,6,2);
          var serializer = MessagePackSerializer.Get(gbytes.GetType());

          var stream = new MemoryStream();
          serializer.Pack(stream, gbytes);
          return Convert.ToBase64String(stream.ToArray());
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
      private static MixinApi GetWalletSDK() {
        using (TextReader fileReader = File.OpenText(@"mybitcoin_wallet.csv"))
        {
            var csv = new CsvReader(fileReader);
            csv.Configuration.HasHeaderRecord = false;
            if (csv.Read())
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
              return mixinApiNewUser;
            } else return null;
        }
      }
      private static string GetWalletUUID() {
        using (TextReader fileReader = File.OpenText(@"mybitcoin_wallet.csv"))
        {
            var csv = new CsvReader(fileReader);
            csv.Configuration.HasHeaderRecord = false;
            if (csv.Read())
            {
              string UserIDNewUser;
              csv.TryGetField<string>(3, out UserIDNewUser);
              return UserIDNewUser;
            } else return "";
        }
      }
    }
}
