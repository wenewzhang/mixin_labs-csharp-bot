# How to trade bitcoin through C#
![cover](https://github.com/wenewzhang/mixin_labs-csharp-bot/raw/master/BItcoin_C%23.jpg)

## Solution One: pay to ExinCore API
[Exincore](https://github.com/exinone/exincore) provide a commercial trading API on Mixin Network.

You pay USDT to ExinCore, ExinCore transfer Bitcoin to you on the fly with very low fee and fair price. Every transaction is anonymous to public but still can be verified on blockchain explorer. Only you and ExinCore know the details.

ExinCore don't know who you are because ExinCore only know your client's uuid.

### Pre-request:
You should  have created a bot based on Mixin Network. Create one by reading [C# Bitcoin tutorial](https://github.com/wenewzhang/mixin_labs-csharp-bot).

#### Install required packages
As you know, we introduce you the **mixin-csharp-sdk** in [chapter 1](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README.md), assume it has installed before, let's install **MsgPack.Cli** here.
```bash
  dotnet add package MixinCSharpSdk
  dotnet add package MsgPack.Cli --version 1.0.1
```
#### Deposit USDT or Bitcoin into your Mixin Network account and read balance
The ExinCore can exchange between Bitcoin, USDT, EOS, ETH etc. Here show you how to exchange between USDT and Bitcoin,
Check the wallet's balance & address before you make order.

- Check the address & balance, remember it Bitcoin wallet address.
- Deposit Bitcoin to this Bitcoin wallet address.
- Check Bitcoin balance after 100 minutes later.

**By the way, Bitcoin & USDT 's address are the same.**

```csharp
  MixinApi mixinApiNewUser = new MixinApi();
  mixinApiNewUser.Init(UserIDNewUser, "", SessionIDNewUser, PinTokenNewUser, PrivateKeyNewUser);
  Asset AssetBTC = mixinApiNewUser.ReadAsset(USRCONFIG.ASSET_ID_BTC);
  Console.WriteLine("New User " + UserIDNewUser + " 's BTC balance is " + AssetBTC.balance);
  Console.WriteLine("New User " + UserIDNewUser + " 's BTC address is " + AssetBTC.public_key);
```
#### Read market price
How to check the coin's price? You need understand what is the base coin. If you want buy Bitcoin and sell USDT, the USDT is the base coin. If you want buy USDT and sell Bitcoin, the Bitcoin is the base coin.
```csharp
string jsonData = FetchMarketPrice("815b0b1a-2764-3736-8faa-42d694fa620a");
var marketObj = JsonConvert.DeserializeObject<MarketInfo>(jsonData);
foreach (AssetInfo value in marketObj.data)
{
    Console.WriteLine(value);
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
  return "null";
}

```

#### Create a memo to prepare order
The chapter two: [Echo Bitcoin](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README2.md) introduce transfer coins. But you need to let ExinCore know which coin you want to buy. Just write your target asset into memo.
```csharp
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
```

#### Pay BTC to API gateway with generated memo
Transfer Bitcoin(BTC_ASSET_ID) to ExinCore(EXIN_BOT), put you target asset uuid in the memo, otherwise, ExinCore will refund you coin immediately!
```csharp
//config.cs
public static string EXIN_BOT     = "61103d28-3ac2-44a2-ae34-bd956070dab1";
// public static string EXIN_BOT     = "0b1a2027-4fd6-3aa0-b3a3-814778bb7a2e";
public static string MASTER_UUID  = "0b4f49dc-8fb4-4539-9a89-fb3afc613747";
public static string ASSET_ID_BTC = "c6d0c728-2624-429b-8e0d-d9d19b6592fa";
public static string ASSET_ID_EOS = "6cfe566e-4aad-470b-8c9a-2fd35b49c68d";
public static string ASSET_ID_USDT= "815b0b1a-2764-3736-8faa-42d694fa620a";

//Program.cs
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
```
If you want sell USDT buy BTC, call it like below:
```csharp
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
                                  USRCONFIG.EXIN_BOT,
                                  "1",
                                  PinNewUser.ToString(),
                                  System.Guid.NewGuid().ToString(),
                                  memo);
          Console.WriteLine(reqInfo);
        }
    }
}
```

The ExinCore should transfer the target coin to your bot, meanwhile, put the fee, order id, price etc. information in the memo, unpack the data like below.
- **NetworkSnapshots** Read snapshots of the user.
```csharp
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
```

If you coin exchange successful, console output like below:
```bash
-----------Successfully--Exchange-------------
You got 0.3923244 back!
Price is  3938.62 Fee is 0.0007878 Percent of fee: 0.200803212851406 %
Fee Asset uuid: 815b0b1a-2764-3736-8faa-42d694fa620a
trace  uuid: 1a3d8561-26e7-49bb-8ae3-ed85ce2bb957
----------end of snapshots query--------------
```

#### Read Bitcoin balance
Check the wallet's balance.
```csharp
  MixinApi mixinApiNewUser = new MixinApi();
  mixinApiNewUser.Init(UserIDNewUser, "", SessionIDNewUser, PinTokenNewUser, PrivateKeyNewUser);
  Asset AssetBTC = mixinApiNewUser.ReadAsset(USRCONFIG.ASSET_ID_BTC);
  Console.WriteLine("New User " + UserIDNewUser + " 's BTC balance is " + AssetBTC.balance);
  Console.WriteLine("New User " + UserIDNewUser + " 's BTC address is " + AssetBTC.public_key);
```

## Source code usage
Build it and then run it.

- **dotnet build** build project.
- **dotnet bin/Debug/netcoreapp2.2/bitcoin_wallet.dll** run it.

- 1: Create Bitcoin Wallet and update PIN
- 2: Read Bitcoin balance & address
- 3: Read USDT balance & address
- 4: Read EOS balance & address
- 5: pay 0.0001 BTC buy USDT
- 6: pay $1 USDT buy BTC
- 7: Read Snapshots
- 8: Fetch market price(USDT)
- 9: Fetch market price(BTC)
- v: Verify Wallet Pin
- q: Exit
Make your choose:

[Full source code](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/bitcoin_wallet/Program.cs)

## Solution Two: List your order on Ocean.One exchange
