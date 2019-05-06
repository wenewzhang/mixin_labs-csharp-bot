# 通过 C# 买卖Bitcoin
![cover](https://github.com/wenewzhang/mixin_labs-csharp-bot/raw/master/BItcoin_C%23.jpg)
上一章介绍了[Exincore](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README4-zhchs.md)，你可以1秒完成资产的市价买卖。如果你想限定价格买卖，或者买卖一些exincore不支持的资产，你需要OceanOne。
## 方案二: 挂单Ocean.One交易所
[Ocean.one](https://github.com/mixinNetwork/ocean.one)是基于Mixin Network的去中心化交易所，它性能一流。
你可以在OceanOne上交易任何资产，只需要将你的币转给OceanOne, 将交易信息写在交易的memo里，OceanOne会在市场里列出你的交易需求，
交易成功后，会将目标币转入到你的MixinNetwork帐上，它有三大特点与优势：
- 不需要在OceanOne注册
- 不需要存币到交易所
- 支持所有Mixin Network上能够转账的资产，所有的ERC20 EOS代币。

### 预备知识:
你先需要创建一个机器人, 方法在 [教程一](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README-zhchs.md).

#### 安装依赖包
我们需要依赖 **MsgPack.Cli** and **mixin-csharp-sdk** ,[第四章](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README4-zhchs.md) 已经做过介绍, 你应该先安装过它了.


#### 充币到 Mixin Network, 并读出它的余额.
此处演示用 USDT购买BTC 或者 用BTC购买USDT。交易前，先检查一下钱包地址。
完整的步骤如下:
- 检查比特币或USDT的余额，钱包地址。并记下钱包地址。
- 从第三方交易所或者你的冷钱包中，将币充到上述钱包地址。
- 再检查一下币的余额，看到帐与否。(比特币的到帐时间是5个区块的高度，约100分钟)。

比特币与USDT的充值地址是一样的。

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

#### 取得Ocean.one的市场价格信息
如何来查询Ocean.one市场的价格信息呢？你要先了解你交易的基础币是什么，如果你想买比特币，卖出USDT,那么基础货币就是USDT;如果你想买USDT,卖出比特币，那么基础货币就是比特币.

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

#### 交易前，创建一个Memo!
在第二章里,[基于Mixin Network的 C# 比特币开发教程: 机器人接受比特币并立即退还用户](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README2-zhchs.md), 我们学习过转帐，这儿我们介绍如何告诉Ocean.one，我们给它转帐的目的是什么，信息全部放在memo里.
- **Side** 方向,"B" 或者 "A", "B"是购买, "A"是出售.
- **AssetUuid** 目标虚拟资产的UUID.
- **Price** 价格，如果操作方向是"B", 价格就是AssetUUID的价格; 如果操作方向是"B", 价格就是转给Ocean.one币的价格.

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

买入XIN的代码如下：

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

#### 出售XIN的例子
转打算出售的XIN给Ocean.one(OCEANONE_BOT),将你打算换回来的目标虚拟资产的UUID放入memo.

```csharp
if ( cmdo == "s1") {
  Console.WriteLine("Please input the price of XIN/USDT: ");
  var pinput = Console.ReadLine();
  Console.WriteLine("Please input the amount of XIN: ");
  var ainput = Console.ReadLine();
  string memo = GenerateOrderMemo("B",USRCONFIG.XIN_ASSET_ID,pinput);
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

一个成功的挂单如下：

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

#### 取消挂单
Ocean.one将trace_id当做订单，比如上面的例子， **12cd76aa-e953-4897-bef0-18123a5e69dc** 就是订单号，我们用他来取消订单。

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

#### 通过读取资产余额，来确认到帐情况
```csharp
MixinApi mixinApiNewUser = GetWalletSDK();
var assets = mixinApiNewUser.ReadAssets();
```

## 源代码执行
编译执行，即可开始交易了.

## 源代码执行
编译执行，即可开始交易了.

- [x] **dotnet build** 编译项目.
- [x] **dotnet bin/Debug/netcoreapp2.2/bitcoin_wallet.dll** 运行它.

本代码执行时的命令列表:

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

[完整代码](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/bitcoin_wallet/Program.cs)
