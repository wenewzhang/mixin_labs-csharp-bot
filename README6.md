# How to list any ERC20 token on decentralized market through C#
![cover](https://github.com/wenewzhang/mixin_labs-csharp-bot/raw/master/BItcoin_C%23.jpg)

OceanOne is introduced in [last chapter](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README5.md), you can trade Bitcoin. All kinds of crypto asset on Mixin Network can be listed on OceanOne.All ERC20 token and EOS token can be listed. Following example will show you how to list a ERC20 token.

There is a [ERC20 token](https://etherscan.io/token/0xc409b5696c5f9612e194a582e14c8cd41ecdbc67) called Benz. It is deposited into Mixin Network. You can search all transaction history from [Mixin Network browser](https://mixin.one/snapshots/2b9c216c-ef60-398d-a42a-eba1b298581d )

### Pre-request:
Deposit some coin to your wallet, and then use **getAssets** API fetch the asset UUID which Mixin Network gave it.

### Get the ERC-20 compliant coin UUID
The **getAssets** API return json data, for example:

- **asset_id** UUID of this coin
- **public_key** The wallet address for this coin
- **symbol**  Coin name, Eg: Benz.

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
The detail information of **getAssets** is output like below:
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
### Make the limit order
- **Limit Order to Buy**  at or below the market.
- **Limit Order to Sell**  at or above the market.

OceanOne support three base coin: USDT, XIN, BTC, that mean you can sell or buy it between USDT, XIN, BTC, so, you have there order: Benz/USDT, Benz/XIN, Benz/BTC, here show you how to make the sell order with USDT.

### Make the limit order to sell.

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

### Make the limit order to buy.
After the order commit, wait 1 minute to let the OceanOne exchange initialize it.
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
### Read orders book from Ocean.one
Now, check the orders-book.
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
### Command of make orders

Commands list of this source code:

- trb:Transfer ERC20 from Bot to Wallet
- trm:Transfer ERC20 from Wallet to Master
- o: Ocean.One Exchange

Make your choose(eg: q for Exit!):
- x:  Orders-Book of ERC20/USDT
- x1: Buy ERC20 pay USDT
- x2: Sell ERC20 get USDT
- c: Cancel the order
- q: Exit

[Full source code](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/bitcoin_wallet/Program.cs)
