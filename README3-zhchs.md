# 基于Mixin Network的 C# 语言比特币开发教程 : 创建一个比特币钱包
![cover](https://github.com/wenewzhang/mixin_labs-csharp-bot/raw/master/BItcoin_C#.jpg)
我们已经创建过一个[回复消息](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README.md)的机器人和一个能自动[支付比特币](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README2.md)的机器人.

## 通过本教程的学习，你可以学到如下内容
1. 如何创建一个比特币钱包.
2. 如何读取比特币钱包的余额.
3. 如何实现免手续费支付比特币并1秒到账
4. 如何将Mixin Network的比特币提现到你的冷钱包或第三方交易所.

前期准备：你要有一个Mixin Network账户。如果没有账户，创建一个.

```csharp
var kpgen = new RsaKeyPairGenerator();

kpgen.Init(new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), 1024));

var keyPair = kpgen.GenerateKeyPair();
AsymmetricKeyParameter privateKey = keyPair.Private;
AsymmetricKeyParameter publicKey = keyPair.Public;

SubjectPublicKeyInfo info = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
string pk = Convert.ToBase64String(info.GetDerEncoded());


var user = mixinApi.APPUser("Csharp" + (new Random().Next() % 100) + " Cat", pk);
Console.WriteLine(user);
```
上面的语句会在本地创建一个RSA密钥对，然后调用Mixin Network来创建帐号，最后输出帐号信息,格式如下：


```csharp
//Create User api include all account information
public class UserInfo
{
    public string type { get; set; }
    public string user_id { get; set; }
    public string identity_number { get; set; }
    public string full_name { get; set; }
    public string avatar_url { get; set; }
    public string relationship { get; set; }
    public DateTime mute_until { get; set; }
    public string created_at { get; set; }
    public bool is_verified { get; set; }
    public App app { get; set; }
    public string session_id { get; set; }
    public string phone { get; set; }
    public string pin_token { get; set; }
    public string invitation_code { get; set; }
    public string code_id { get; set; }
    public string code_url { get; set; }
    public bool has_pin { get; set; }
    public string receive_message_source { get; set; }
    public string accept_conversation_source { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

```

现在你需要小心保管好你的帐号信息，在读取该账户的比特币资产余额或者进行其他操作时，将需要用到这些信息.
### 给新建的帐号创建一个比特币钱包
新账号并不默认内置比特币钱包， 现在读一下比特币余额就可以创建一个比特币钱包。
```csharp
MixinApi mixinApiNewUser = new MixinApi();
mixinApiNewUser.Init(UserIDNewUser, "", SessionIDNewUser, PinTokenNewUser, PrivateKeyNewUser);
Asset AssetBTC = mixinApiNewUser.ReadAsset(ASSET_ID_BTC);
Console.WriteLine("New User " + UserIDNewUser + " 's BTC balance is " + AssetBTC.balance);
Console.WriteLine("New User " + UserIDNewUser + " 's BTC address is " + AssetBTC.public_key);
```

创建的帐号的比特币资产详细信息如下，其中public key就是比特币的钱包地址:
```bash
{"data":{"type":"asset","asset_id":"c6d0c728-2624-429b-8e0d-d9d19b6592fa",
"chain_id":"c6d0c728-2624-429b-8e0d-d9d19b6592fa","symbol":"BTC","name":"Bitcoin",
"icon_url":"https://images.mixin.one/HvYGJsV5TGeZ-X9Ek3FEQohQZ3fE9LBEBGcOcn4c4BNHovP4fW4YB97Dg5LcXoQ1hUjMEgjbl1DPlKg1TW7kK6XP=s128",
"balance":"0","public_key":"1EYt7hUP4yK2VfKqDtbVb3dzFtcRKzh8zN","account_name":"",
"account_tag":"","price_btc":"1","price_usd":"3776.98110465","change_btc":"0",
"change_usd":"-0.022213428553059168","asset_key":"c6d0c728-2624-429b-8e0d-d9d19b6592fa","confirmations":6,"capitalization":0}}
```
这个API能够提供若干与比特币有关的信息:
* 钱包地址:[public_key]
* Logo: [icon_url]
* 资产名字:[name]
* 资产在Mixin Network的uuid: [asset_key]
* 对美元的价格(Coinmarketcap.com提供): [price_usd]
* 存币时确认的区块数量:[confirmations]


### 比特币私钥呢？
比特币的私钥呢？这个私钥被Mixin Network通过多重签名保护，所以对用户来说是不可见的,比特币资产的提现和转账都需要用户提供正确的的RSA签名,PIN代码与会话密钥才能完成.

### 不只是比特币，还有以太坊，EOS等
这个帐号不只支持比特币，还支持以太坊，EOS等, 完整的区块链支持[列表](https://mixin.one/network/chains). 这个账户同时也支持所有的 ERC20 代币与 EOS 代币.

创建其它的币的钱包与创建比特币钱包过程一样，读对应的资产余额就可以.

#### Mixin Network 当前支持的加密货币 (2019-02-19)

|crypto |uuid in Mixin Network
|---|---
|EOS|6cfe566e-4aad-470b-8c9a-2fd35b49c68d
|CNB|965e5c6e-434c-3fa9-b780-c50f43cd955c
|BTC|c6d0c728-2624-429b-8e0d-d9d19b6592fa
|ETC|2204c1ee-0ea2-4add-bb9a-b3719cfff93a
|XRP|23dfb5a5-5d7b-48b6-905f-3970e3176e27
|XEM|27921032-f73e-434e-955f-43d55672ee31
|ETH|43d61dcd-e413-450d-80b8-101d5e903357
|DASH|6472e7e3-75fd-48b6-b1dc-28d294ee1476
|DOGE|6770a1e5-6086-44d5-b60f-545f9d9e8ffd
|LTC|76c802a2-7c88-447f-a93e-c29c9e5dd9c8
|SC|990c4c29-57e9-48f6-9819-7d986ea44985
|ZEN|a2c5d22b-62a2-4c13-b3f0-013290dbac60
|ZEC|c996abc9-d94e-4494-b1cf-2a3fd3ac5714
|BCH|fd11b6e3-0b87-41f1-a41f-f0e9b49e5bf0


EOS的存币地址与其它的币有些不同，它由两部分组成： account_name and account tag, 如果你向Mixin Network存入EOS，你需要填两项数据： account name 是**eoswithmixin**,备注里输入你的account_tag,比如**0aa2b00fad2c69059ca1b50de2b45569**.

EOS的资产余额返回结果如下:
```bash
{'data': {'type': 'asset', 'asset_id': '6cfe566e-4aad-470b-8c9a-2fd35b49c68d',
'chain_id': '6cfe566e-4aad-470b-8c9a-2fd35b49c68d',
'symbol': 'EOS', 'name': 'EOS',
'icon_url': 'https://images.mixin.one/a5dtG-IAg2IO0Zm4HxqJoQjfz-5nf1HWZ0teCyOnReMd3pmB8oEdSAXWvFHt2AJkJj5YgfyceTACjGmXnI-VyRo=s128',
'balance': '0', 'public_key': '',
'account_name': 'eoswithmixin', 'account_tag': '185b27f83d76dad3033ee437195aac11',
'price_btc': '0.00096903', 'price_usd': '3.8563221', 'change_btc': '0.00842757579765049',
'change_usd': '0.0066057628802373095', 'asset_key': 'eosio.token:EOS',
'confirmations': 64, 'capitalization': 0}}
```

### 存入比特币与读取比特币余额
现在，你可以向比特币的钱包存币了。

当然，在比特币网络里转币，手续费是相当贵的，费用的中位数在0.001BTC,按当前4000美元的价格，在4美元左右，有一个方便的办法，如果你有[Mixin Messenger](https://mixin.one/messenger)帐号，里面并且有比特币的话，可以直接提现比特币到新创建的帐号的比特币充值地址，它们在同一个Mixin Network网络内，手续费为0，而且1秒到账。

### Mixin Network网内免手续费的，并且即时确认
任何币在Mixin Network内部的交易，都是无手续费的，并且立刻到账。

#### Mixin Network帐号之间的比特币支付
通过Mixin Messenger，我们可以先转比特币给机器人，然后让机器人转币给新用户。

```csharp
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
```

下面的代码，可以读取比特币钱包余额.
读取Bitcoin的余额，来确认比特币是不是转成功了！
- 注意: **UserIDNewUser,SessionIDNewUser,PrivateKeyNewUser** 新创建的用户的！


### 如何将比特币存入你的冷钱包或者第三方交易所
如果你希望将币存入你的冷钱包或者第三方交易所, 先要得到冷钱包或者你在第三方交易所的钱包地址，然后将钱包地址提交到Mixin Network.

### **要点提示**:
- 1.提现是需要支付收续费的;
- 2.准备好比特币包地址!

#### 增加目的钱包地址到Mixin Network
调用CreateAddress API, 将会返回一个address_id,下一步的提现操作会用到这个id。
```csharp
var addr = mixinApi.CreateAddress(ASSET_ID_BTC, BTC_WALLET_ADDR, "BTC withdraw", null, null, PinCode);
Console.WriteLine(addr);
```
这里的 **14T129GTbXXPGXXvZzVaNLRFPeHXD1C25C** 就是一个比特币钱包地址, 如下所示，提现费用是0.0034802 BTC, address_id  是"345855b5-56a5-4f3b-ba9e-d99601ef86c1".                                                                                                                                                     
```bash
{'data': {'type': 'address', 'address_id': '47998e2f-2761-45ce-9a6c-6f167b20c78b',
'asset_id': 'c6d0c728-2624-429b-8e0d-d9d19b6592fa',
'public_key': '14T129GTbXXPGXXvZzVaNLRFPeHXD1C25C', 'label': 'BTC',
'account_name': '', 'account_tag': '',
'fee': '0.0034802', 'reserve': '0', 'dust': '0.0001',
'updated_at': '2019-02-26T00:03:05.028140704Z'}}
```
如果你操作的是EOS, 示例代码如下：
- **eoswithmixin** EOS帐号名称，比如Mixin的EOS帐号是eoswithmixin
- **d80363afcc466fbaf2daa7328ae2adfa** d80363afcc466fbaf2daa7328ae2adfa 钱包地址
```csharp
var addr = mixinApi.CreateAddress(null, null, "EOS withdraw", "eoswithmixin", "d80363afcc466fbaf2daa7328ae2adfa", PinCode);
Console.WriteLine(addr);
```
#### 创建提现地址成功后，你可以用 ReadAddress 读取最新的提现费。
```csharp
var addr = mixinApi.CreateAddress(ASSET_ID_BTC, BTC_WALLET_ADDR, "BTC withdraw", null, null, PinCode);
Console.WriteLine(addr);
Console.WriteLine(mixinApi.ReadAddress(addr.address_id));
```

#### 提交提现请求，Mixin Network会即时处理提现请求.
提交提现请求到Mixin Network, "addr.address_id" 就是CreateAddress创建的。

```csharp
 Console.WriteLine(mixinApi.Withdrawal(addr.address_id,AMOUNT,PinCode,System.Guid.NewGuid().ToString(), "Test withdraw"));
```
#### 可以通过[**blockchain explore**](https://www.blockchain.com/explorer)来查看提现进度.

[Full source code](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/mixin_labs-csharp-bot/Program.cs)
