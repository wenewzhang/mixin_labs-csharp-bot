# 通过 C# 买卖Bitcoin
![cover](https://github.com/wenewzhang/mixin_labs-go-bot/raw/master/Bitcoin_go.jpg)

## 方案一: 通过ExinCore API进行币币交易
[Exincore](https://github.com/exinone/exincore) 提供了基于Mixin Network的币币交易API.

你可以支付USDT给ExinCore, ExinCore会以最低的价格，最优惠的交易费将你购买的比特币转给你, 每一币交易都是匿名的，并且可以在区块链上进行验证，交易的细节只有你与ExinCore知道！

ExinCore 也不知道你是谁，它只知道你的UUID.

### 预备知识:
你先需要创建一个机器人, 方法在 [教程一](https://github.com/wenewzhang/mixin_labs-php-bot/blob/master/README-zhchs.md).

#### 安装依赖包
正如教程一里我们介绍过的， 我们需要依赖 **mixin-sdk-go**, 你应该先安装过它了， 这儿我们再安装 **uuid, msgpack** 两个软件包.
```bash
  dotnet add package MsgPack.Cli --version 1.0.1
```
#### 充币到 Mixin Network, 并读出它的余额.
ExinCore可以进行BTC, USDT, EOS, ETH 等等交易， 这儿演示如果用 USDT购买BTC 或者 用BTC购买USDT, 交易前，先检查一下钱包地址！
完整的步骤如下:
- 检查比特币或USDT的余额，钱包地址。并记下钱包地址。
- 从第三方交易所或者你的冷钱包中，将币充到上述钱包地址。
- 再检查一下币的余额，看到帐与否。(比特币的到帐时间是5个区块的高度，约100分钟)。

请注意，比特币与USDT的地址是一样的。

```csharp
  MixinApi mixinApiNewUser = new MixinApi();
  mixinApiNewUser.Init(UserIDNewUser, "", SessionIDNewUser, PinTokenNewUser, PrivateKeyNewUser);
  Asset AssetBTC = mixinApiNewUser.ReadAsset(USRCONFIG.ASSET_ID_BTC);
  Console.WriteLine("New User " + UserIDNewUser + " 's BTC balance is " + AssetBTC.balance);
  Console.WriteLine("New User " + UserIDNewUser + " 's BTC address is " + AssetBTC.public_key);
```

#### 查询ExinCore市场的价格信息
如果来查询ExinCore市场的价格信息呢？你要先了解你交易的基础币是什么，如果你想买比特币，卖出USDT,那么基础货币就是USDT;如果你想买USDT,卖出比特币，那么基础货币就是比特币.
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

#### 交易前，创建一个Memo!
在第二章里,[基于Mixin Network的PHP比特币开发教程: 机器人接受比特币并立即退还用户](https://github.com/wenewzhang/mixin_labs-php-bot/blob/master/README2-zhchs.md), 我们学习过退还用户比特币，在这里，我们除了给ExinCore支付币外，还要告诉他我们想购买的币是什么，即将想购买的币存到memo里。
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

#### 币币交易的完整流程
转币给ExinCore时，将memo写入你希望购买的币，否则，ExinCore会直接退币给你！
如果你想卖出比特币买入USDT,调用方式如下：

```csharp
//config.cs
public static string EXIN_BOT     = "61103d28-3ac2-44a2-ae34-bd956070dab1";
// public static string EXIN_BOT     = "0b1a2027-4fd6-3aa0-b3a3-814778bb7a2e";
public static string MASTER_UUID  = "0b4f49dc-8fb4-4539-9a89-fb3afc613747";
public static string ASSET_ID_BTC = "c6d0c728-2624-429b-8e0d-d9d19b6592fa";
public static string ASSET_ID_EOS = "6cfe566e-4aad-470b-8c9a-2fd35b49c68d";
public static string ASSET_ID_USDT= "815b0b1a-2764-3736-8faa-42d694fa620a";

//Program.cs
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

如果你想卖出USDT买入比特币,调用方式如下：

```go
packUuid, _ := uuid.FromString(mixin.GetAssetId("BTC"))
pack, _ := msgpack.Marshal(OrderAction{A: packUuid})
memo := base64.StdEncoding.EncodeToString(pack)
// fmt.Println(memo)
priKey, pToken, sID, userID, uPIN := GetWalletInfo()
bt, err := mixin.Transfer(EXIN_BOT,"0.0001",mixin.GetAssetId("USDT"),memo,
                         messenger.UuidNewV4().String(),
                         uPIN,pToken,userID,sID,priKey)
if err != nil {
        log.Fatal(err)
}
fmt.Println(string(bt))
```

交易完成后，Exincore会将你需要的币转到你的帐上，同样，会在memo里，记录成交价格，交易费用等信息！你只需要按下面的方式解开即可！
- **NetworkSnapshots** 读取钱包的交易记录。
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

一次成功的交易如下：
```bash
---------------Successful----Exchange-------------
You got  0.3981012
815b0b1a-2764-3736-8faa-42d694fa620a  price: 3996.8  Fee: 0.0007994
```

#### 读取币的余额
通过读取币的余额，来确认交易情况！
```csharp
  MixinApi mixinApiNewUser = new MixinApi();
  mixinApiNewUser.Init(UserIDNewUser, "", SessionIDNewUser, PinTokenNewUser, PrivateKeyNewUser);
  Asset AssetBTC = mixinApiNewUser.ReadAsset(USRCONFIG.ASSET_ID_BTC);
  Console.WriteLine("New User " + UserIDNewUser + " 's BTC balance is " + AssetBTC.balance);
  Console.WriteLine("New User " + UserIDNewUser + " 's BTC address is " + AssetBTC.public_key);
```

## 源代码执行
编译执行，即可开始交易了.

- **dotnet build** 编译项目.
- **dotnet bin/Debug/netcoreapp2.2/bitcoin_wallet.dll** 运行它.

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

[完整代码](https://github.com/wenewzhang/mixin_labs-go-bot/blob/master/coin_exchange/coin_exchange.go)

## Solution Two: List your order on Ocean.One exchange
