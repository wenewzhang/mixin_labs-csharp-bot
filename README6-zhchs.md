# 在OceanOne上挂单买卖任意ERC20 token
![cover](https://github.com/wenewzhang/mixin_labs-csharp-bot/raw/master/BItcoin_C%23.jpg)

在[上一课](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README5.md)中，我们介绍了如何在OceanOne交易比特币。OceanOne支持交易任何Mixin Network上的token，包括所有的ERC20和EOS token，不需要任何手续和费用，直接挂单即可。下面介绍如何将将一个ERC20 token挂上OceanOne交易。掌握了ERC20的代币买卖之后，你就可以用同样的方法买卖任何EOS以及其他Mixin Network上的token

此处我们用一个叫做Benz的[ERC20 token](https://etherscan.io/token/0xc409b5696c5f9612e194a582e14c8cd41ecdbc67)为例。这个token已经被充值进Mixin Network，你可以在[区块链浏览器](https://mixin.one/snapshots/2b9c216c-ef60-398d-a42a-eba1b298581d )看到这个token在Mixin Network内部的总数和交易
### 预备知识:
先将Ben币存入你的钱包，然后使用**getAssets** API读取它的UUID.

### 取得该币的UUID
调用 **getAssets** API 会返回json数据, 如:

- **asset_id** 币的UUID.
- **public_key** 该币的当前钱包的地址.
- **symbol**  币的名称. 如: Benz.

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
调用 **getAssets** API的完整输出如下:
```bash
Make your choose:
aw
Current wallet uuid is 85d5609d-d93b-3c96-96f6-58357c5d99eb
Benz Public Address is: 0x5fD0F147830a186545e6020F58fEc0c4B39065D4 Balance is: 1

EOS Public Address is: eoswithmixin 30399f17622cb2bfc57efd3393144bf9 Balance is: 0

USDT Public Address is: 1JvQ98N5Y8TvDbXr8eA8DHsNZqEuGbzzng Balance is: 1

BTC Public Address is: 1JvQ98N5Y8TvDbXr8eA8DHsNZqEuGbzzng Balance is: 0

XIN Public Address is: 0x5fD0F147830a186545e6020F58fEc0c4B39065D4 Balance is: 0.01
```
### 限价挂单
- **挂限价买单**  低于或者等于市场价的单.
- **挂限价卖单**  高于或者是等于市场价的单.

OceanOne支持三种基类价格: USDT, XIN, BTC, 即: Benz/USDT, Benz/XIN, Benz/BTC, 这儿示范Benz/USDT.

### 限价挂卖单.
新币挂单后,需要等一分钟左右，等OceanOne来初始化新币的相关数据.

```csharp
public static string  ERC20_BENZ     = "2b9c216c-ef60-398d-a42a-eba1b298581d";
if ( cmdo == "s2") {
  Console.WriteLine("Please input the price of ERC20/USDT: ");
  var pinput = Console.ReadLine();
  Console.WriteLine("Please input the amount of ERC20: ");
  var ainput = Console.ReadLine();

  string memo = GenerateOrderMemo("A",USRCONFIG.ASSET_ID_USDT,pinput);
  Console.WriteLine(memo);
  // Console.WriteLine(Convert.ToBase64String(stream3.ToArray()));
  MixinApi mixinApiNewUser = GetWalletSDK();
  var assets = mixinApiNewUser.ReadAsset(USRCONFIG.ERC20_BENZ);
  float balance = float.Parse(assets.balance);
  float amount  = float.Parse(ainput);
  if ( ( balance >= 0 ) && ( balance >= amount ) ) {
    Transfer reqInfo = mixinApiNewUser.Transfer(USRCONFIG.ERC20_BENZ,
                            USRCONFIG.OCEANONE_BOT,
                            ainput,
                            GetWalletPinCode(),
                            System.Guid.NewGuid().ToString(),
                            memo);
    Console.WriteLine(reqInfo);
    Console.WriteLine("Order id is " + reqInfo.trace_id);
  } else Console.WriteLine("Not enough ERC20_BENZ!");
}
```

### 限价挂买单.
新币挂单后,需要等一分钟左右，等OceanOne来初始化新币的相关数据.

```csharp
if ( cmdo == "b2") {
  Console.WriteLine("Please input the price of ERC20_BENZ/USDT: ");
  var pinput = Console.ReadLine();
  Console.WriteLine("Please input the amount of USDT: ");
  var ainput = Console.ReadLine();

  string memo = GenerateOrderMemo("B",USRCONFIG.ERC20_BENZ,pinput);
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
### 读取币的价格列表
读取币的价格列表，来确认挂单是否成功!

```csharp
if (cmdo == "2") {
  string jsonData = FetchOceanMarketPrice(USRCONFIG.ERC20_BENZ,USRCONFIG.ASSET_ID_USDT);
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
### ERC20相关的操作指令

Commands list of this source code:

- teb:Transfer ERC20 from Bot to Wallet
- tem:Transfer ERC20 from Wallet to Master
- o: Ocean.One Exchange

Make your choose:
- 1:  Fetch XIN/USDT orders
- s1: Sell XIN/USDT
- b1: Buy XIN/USDT
- 2:  Fetch ERC20(Benz)/USDT orders
- s2: Sell Benz/USDT
- b2: Buy Benz/USDT
- c: Cancel Order
- q:  Exit

[完整的代码](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/bitcoin_wallet/Program.cs)
