# How to list bitcoin order through C#
![cover](https://github.com/wenewzhang/mixin_labs-csharp-bot/raw/master/BItcoin_C%23.jpg)

Exincore is introduced in [last chapter](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README4.md), you can exchange many crypto asset at market price and receive your asset in 1 seconds. If you want to trade asset at limited price, or trade asset is not supported by ExinCore now, OceanOne is the answer.
## Solution Two: List your order on Ocean.One exchange
[Ocean.one](https://github.com/mixinNetwork/ocean.one) is a decentralized exchange built on Mixin Network, it's almost the first time that a decentralized exchange gain the same user experience as a centralized one.

You can list any asset on OceanOne. Pay the asset you want to sell to OceanOne account, write your request in payment memo, OceanOne will list your order to market. It send asset to your wallet after your order is matched.

* No sign up required
* No deposit required
* No listing process.

### Pre-request:
You should  have created a bot based on Mixin Network. Create one by reading [C# Bitcoin tutorial](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README.md).

#### Install required packages
This tutorial dependent **MsgPack.Cli** and **mixin-csharp-sdk** , [chapter 4](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README4.md), assume them had installed before.

#### Deposit USDT or Bitcoin into your Mixin Network account and read balance
The Ocean.one can match any order. Here we exchange between USDT and Bitcoin, Check the wallet's balance & address before you make order.

- Check the address & balance, find it's Bitcoin wallet address.
- Deposit Bitcoin to this Bitcoin wallet address.
- Check Bitcoin balance after 100 minutes later.

**Omni USDT address is same as Bitcoin address**

```csharp
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
```

#### Read orders book from Ocean.one
How to check the coin's price? You need understand what is the base coin. If you want buy Bitcoin and sell USDT, the USDT is the base coin. If you want buy USDT and sell Bitcoin, the Bitcoin is the base coin.


```csharp
public class MarketInfoOcean
{
    public Omarket data { get; set; }
}
public class Omarket {
  public string market { get; set; }
  public string timestamp  { get; set; }
  public Side data { get; set; }
}
public class Side {
  public List<order> asks  { get; set; }
  public List<order> bids  { get; set; }
}
public class order {
  public string amount { get; set; }
  public string funds { get; set; }
  public string price { get; set; }
  public string side { get; set; }
}

if (cmdo == "1") {
  string jsonData = FetchOceanMarketPrice(USRCONFIG.XIN_ASSET_ID,USRCONFIG.ASSET_ID_USDT);
  // string jsonData = FetchMarketPrice("c6d0c728-2624-429b-8e0d-d9d19b6592fa");
  var marketObj = JsonConvert.DeserializeObject<MarketInfoOcean>(jsonData);
  Console.WriteLine("--Price--Amount---Funds---Side----");
  foreach (order value in marketObj.data.data.asks)
  {
      Console.WriteLine(value.price + " " + value.amount + " " + value.funds + " " + value.side);
  }
  foreach (order value in marketObj.data.data.bids)
  {
      Console.WriteLine(value.price + " " + value.amount + " " + value.funds + " " + value.side);
  }
}
public static string FetchOceanMarketPrice(string asset_id, string base_asset)
{
    return FetchOceanMarketPriceAsync(asset_id,base_asset).Result;
}
public static async Task<string> FetchOceanMarketPriceAsync(string asset_id, string base_asset)
{
  HttpClient client = new HttpClient();
  string baseUrl = "https://events.ocean.one/markets/" + asset_id + "-" + base_asset + "/book";
  try
  {
     HttpResponseMessage response = await client.GetAsync(baseUrl);
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
```

#### Create a memo to prepare order
The chapter two: [Echo Bitcoin](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README2.md) introduce transfer coins. But you need to let Ocean.one know which coin you want to buy.
- **Side** "B" or "A", "B" for buy, "A" for Sell.
- **AssetUuid** UUID of the asset you want to buy
- **Price** If Side is "B", Price is AssetUUID; if Side is "A", Price is the asset which transfer to Ocean.one.
```csharp
private static string GenerateOrderMemo(string Side, string AssetUuid, string Price) {
  Hashtable temp = new Hashtable();
  temp.Add("S","A");
  temp.Add("A",StringGuid2Bytes(AssetUuid));
  temp.Add("P",Price);
  temp.Add("T","L");
  var serializer3 = MessagePackSerializer.Get<Hashtable>();
  var stream3 = new MemoryStream();
  serializer3.Pack(stream3, temp);
  string memo = Convert.ToBase64String(stream3.ToArray());
  return memo;
}
```

The code show you how to buy XIN：
```csharp
public static string  OCEANONE_BOT   = "aaff5bef-42fb-4c9f-90e0-29f69176b7d4";
public static string  ASSET_ID_BTC   = "c6d0c728-2624-429b-8e0d-d9d19b6592fa";
public static string  ASSET_ID_EOS   = "6cfe566e-4aad-470b-8c9a-2fd35b49c68d";
public static string  ASSET_ID_USDT  = "815b0b1a-2764-3736-8faa-42d694fa620a";

public static string  XIN_ASSET_ID   = "c94ac88f-4671-3976-b60a-09064f1811e8";

if ( cmdo == "b1") {
  Console.WriteLine("Please input the price of XIN/USDT: ");
  var pinput = Console.ReadLine();
  Console.WriteLine("Please input the amount of USDT: ");
  var ainput = Console.ReadLine();

  string memo = GenerateOrderMemo("A",USRCONFIG.ASSET_ID_USDT,pinput);
  Console.WriteLine(memo);
  MixinApi mixinApiNewUser = GetWalletSDK();
  var assets = mixinApiNewUser.ReadAsset(USRCONFIG.ASSET_ID_USDT);
  Console.WriteLine(assets.balance);
  float balance = float.Parse(assets.balance);
  float amount  = float.Parse(ainput);
  if ( ( balance >= 1.0 ) && ( balance >= amount ) ) {
    Transfer reqInfo = mixinApiNewUser.Transfer(USRCONFIG.ASSET_ID_USDT,
                            USRCONFIG.OCEANONE_BOT,
                            ainput,
                            GetWalletPinCode(),
                            System.Guid.NewGuid().ToString(),
                            memo);
    Console.WriteLine(reqInfo);
    Console.WriteLine("Order id is " + reqInfo.trace_id);
  } else Console.WriteLine("Not enough USDT!");
}

```

#### Pay XIN to OceanOne with generated memo
Transfer XIN(XIN_ASSET_ID) to Ocean.one(OCEANONE_BOT), put you target asset uuid(USDT) in the memo.
```csharp
public static string  OCEANONE_BOT   = "aaff5bef-42fb-4c9f-90e0-29f69176b7d4";
public static string  ASSET_ID_BTC   = "c6d0c728-2624-429b-8e0d-d9d19b6592fa";
public static string  ASSET_ID_EOS   = "6cfe566e-4aad-470b-8c9a-2fd35b49c68d";
public static string  ASSET_ID_USDT  = "815b0b1a-2764-3736-8faa-42d694fa620a";

public static string  XIN_ASSET_ID   = "c94ac88f-4671-3976-b60a-09064f1811e8";

if ( cmdo == "s1") {
  Console.WriteLine("Please input the price of XIN/USDT: ");
  var pinput = Console.ReadLine();
  Console.WriteLine("Please input the amount of XIN: ");
  var ainput = Console.ReadLine();

  string memo = GenerateOrderMemo("A",USRCONFIG.ASSET_ID_USDT,pinput);
  Console.WriteLine(memo);
  // Console.WriteLine(Convert.ToBase64String(stream3.ToArray()));
  MixinApi mixinApiNewUser = GetWalletSDK();
  var assets = mixinApiNewUser.ReadAsset(USRCONFIG.XIN_ASSET_ID);
  float balance = float.Parse(assets.balance);
  float amount  = float.Parse(ainput);
  if ( ( balance >= 0 ) && ( balance >= amount ) ) {
    Transfer reqInfo = mixinApiNewUser.Transfer(USRCONFIG.XIN_ASSET_ID,
                            USRCONFIG.OCEANONE_BOT,
                            ainput,
                            GetWalletPinCode(),
                            System.Guid.NewGuid().ToString(),
                            memo);
    Console.WriteLine(reqInfo);
    Console.WriteLine("Order id is " + reqInfo.trace_id);
  } else Console.WriteLine("Not enough XIN!");
}
```

A success order output like below:
```bash
Please input the price of XIN/USDT:
110
Please input the amount of USDT:
1
hKFBxBDJSsiPRnE5drYKCQZPGBHooVShTKFToUKhUKMxMTA=
1
{"type":"transfer","snapshot_id":"fe04c667-3ad9-4f2b-b205-ba2cef3733ea",
"opponent_id":"aaff5bef-42fb-4c9f-90e0-29f69176b7d4",
"asset_id":"815b0b1a-2764-3736-8faa-42d694fa620a","amount":"-1",
"trace_id":"12cd76aa-e953-4897-bef0-18123a5e69dc",
"memo":"hKFBxBDJSsiPRnE5drYKCQZPGBHooVShTKFToUKhUKMxMTA=",
"created_at":"2019-05-06T06:43:13.488971627Z"}
Order id is 12cd76aa-e953-4897-bef0-18123a5e69dc
```

## Cancel the Order
To cancel order, just pay any amount of any asset to OceanOne, and write trace_id into memo. Ocean.one take the trace_id as the order id, for example, **12cd76aa-e953-4897-bef0-18123a5e69dc** is a order id,
We can use it to cancel the order.
```csharp
if ( cmdo == "c") {
  Console.WriteLine("Please input the Order id: ");
  var oinput = Console.ReadLine();
  Hashtable temp = new Hashtable();
  temp.Add("O",StringGuid2Bytes(oinput));
  var serializer3 = MessagePackSerializer.Get<Hashtable>();
  var stream3 = new MemoryStream();
  serializer3.Pack(stream3, temp);
  string memo = Convert.ToBase64String(stream3.ToArray());
  MixinApi mixinApiNewUser = GetWalletSDK();
  var assets = mixinApiNewUser.ReadAsset(USRCONFIG.CNB_ASSET_ID);
  Console.WriteLine(assets.balance);
  float balance = float.Parse(assets.balance);
  if (  balance >= 0  ) {
    Transfer reqInfo = mixinApiNewUser.Transfer(USRCONFIG.CNB_ASSET_ID,
                            USRCONFIG.OCEANONE_BOT,
                            "0.0000001",
                            GetWalletPinCode(),
                            System.Guid.NewGuid().ToString(),
                            memo);
    Console.WriteLine(reqInfo);
  } else Console.WriteLine("Not enough CNB!");
  }
```
#### Read Bitcoin balance
Check the wallet's balance.
```csharp
MixinApi mixinApiNewUser = GetWalletSDK();
var assets = mixinApiNewUser.ReadAssets();
```

## Source code usage
Build it and then run it.

- [x] **dotnet build** 编译项目.
- [x] **dotnet bin/Debug/netcoreapp2.2/bitcoin_wallet.dll** 运行它.


Commands list of this source code:

- 1: Create Bitcoin Wallet and update PIN
- 2: Read Bitcoin balance & address
- 3: Read USDT balance & address
- 4: Read EOS balance & address
- tub: Transfer USDT from Bot to Wallet
- tum: Transfer USDT from Wallet to Master
- tcb: Transfer CNB from Bot to Wallet
- tcm: Transfer CNB from Wallet to Master
- txb: Transfer XIN from Bot to Wallet
- txm: Transfer XIN from Wallet to Master
- 5: pay 0.0001 BTC buy USDT
- 6: pay $1 USDT buy BTC
- 7: Read Snapshots
- 8: Fetch market price(USDT)
- 9: Fetch market price(BTC)
- v: Verify Wallet Pin
- ab: Read Bot Assets
- aw: Read Wallet Assets
- o:  OceanOne Trading
- q: Exit
Make your choose:

Make your choose(eg: q for Exit!):

Make your choose:
o
- 1:  Fetch XIN/USDT orders
- s1: Sell XIN/USDT
- b1: Buy XIN/USDT
- 2:  Fetch ERC20(Benz)/USDT orders
- s2: Sell Benz/USDT
- b2: Buy Benz/USDT
- c: Cancel Order
- q:  Exit

[Full source code](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/bitcoin_wallet/Program.cs)
