# C# Bitcoin tutorial based on Mixin Network : Create a Bitcoin wallet
![cover](https://github.com/wenewzhang/mixin_labs-csharp-bot/raw/master/BItcoin_C#.jpg)
We have created a bot to [echo message](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README.md) and [echo Bitcoin](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README2.md).

# What you will learn from this chapter
1. How to create Bitcoin wallet
2. How to read Bitcoin balance
3. How to send Bitcoin with zero transaction fee and confirmed in 1 second
4. How to send Bitcoin to other wallet


Pre-request: You should have a Mixin Network account.
Create an account code like below:

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
The function in C# SDK create a RSA key pair automatically, then call Mixin Network to create an account. last the function return all account information.

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

Now you need to carefully keep the account information. You need these information to read asset balance and other content.
### Create Bitcoin wallet for the Mixin Network account
The Bitcoin  wallet is not generated automatically at same time when we create Mixin Network account. Read Bitcoin asset once to generate a Bitcoin wallet.
```csharp
MixinApi mixinApiNewUser = new MixinApi();
mixinApiNewUser.Init(UserIDNewUser, "", SessionIDNewUser, PinTokenNewUser, PrivateKeyNewUser);
Asset AssetBTC = mixinApiNewUser.ReadAsset(ASSET_ID_BTC);
Console.WriteLine("New User " + UserIDNewUser + " 's BTC balance is " + AssetBTC.balance);
Console.WriteLine("New User " + UserIDNewUser + " 's BTC address is " + AssetBTC.public_key);
```
You can found information about Bitcoin asset in the account. Public key is the Bitcoin deposit address. Full response of read  Bitcoin asset is
```bash
{"data":{"type":"asset","asset_id":"c6d0c728-2624-429b-8e0d-d9d19b6592fa",
"chain_id":"c6d0c728-2624-429b-8e0d-d9d19b6592fa","symbol":"BTC","name":"Bitcoin",
"icon_url":"https://images.mixin.one/HvYGJsV5TGeZ-X9Ek3FEQohQZ3fE9LBEBGcOcn4c4BNHovP4fW4YB97Dg5LcXoQ1hUjMEgjbl1DPlKg1TW7kK6XP=s128",
"balance":"0","public_key":"1EYt7hUP4yK2VfKqDtbVb3dzFtcRKzh8zN","account_name":"",
"account_tag":"","price_btc":"1","price_usd":"3776.98110465","change_btc":"0",
"change_usd":"-0.022213428553059168","asset_key":"c6d0c728-2624-429b-8e0d-d9d19b6592fa","confirmations":6,"capitalization":0}}
```
The API provide many information about Bitcoin asset.
* Deposit address:[public_key]
* Logo: [icon_url]
* Asset name:[name]
* Asset uuid in Mixin network: [asset_key]
* Price in USD from Coinmarketcap.com: [price_usd]
* Least confirmed blocks before deposit is accepted by Mixin network:[confirmations]


### Private key?
Where is Bitcoin private key? The private key is protected by multi signature inside Mixin Network so it is invisible for user. Bitcoin asset can only be withdraw to other address when user provide correct RSA private key signature, PIN code and Session key.

### Not only Bitcoin, but also Ethereum, EOS
The account not only contain a Bitcoin wallet, but also contains wallet for Ethereum, EOS, etc. Full blockchain support [list](https://mixin.one/network/chains). All ERC20 Token and EOS token are supported by the account.

Create other asset wallet is same as create Bitcoin wallet, just read the asset.
#### Mixin Network support cryptocurrencies (2019-02-19)

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

If you read EOS deposit address, the deposit address is composed of two parts: account_name and account tag. When you transfer EOS token to your account in Mixin network, you should fill both account name and memo. The memo content is value of 'account_tag'.
Result of read EOS asset is:
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

### Deposit Bitcoin and read balance
Now you can deposit Bitcoin into the deposit address.

This is maybe too expensive for this tutorial. There is a free and lightening fast solution to deposit Bitcoin: add the address in your Mixin messenger account withdrawal address and withdraw small amount Bitcoin from your account to the address. It is free and confirmed instantly because they are both on Mixin Network.

Now you can read Bitcoin balance of the account.
```csharp
Asset AssetBTC = mixinApiNewUser.ReadAsset(ASSET_ID_BTC);

```
### Send Bitcoin inside Mixin Network to enjoy instant confirmation and ZERO transaction fee
Any transaction happen between Mixin network account is free and is confirmed in 1 second.

#### Send Bitcoin to another Mixin Network account
We can send Bitcoin to our bot through Mixin Messenger, and then transfer Bitcoin from bot to new user.

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

Read bot's Bitcoin balance to confirm the transaction.
Caution: **UserIDNewUser,SessionIDNewUser,PrivateKeyNewUser** is for the New User!


### Send Bitcoin to another Bitcoin exchange or wallet
If you want to send Bitcoin to another exchange or wallet, you need to know the destination deposit address, then add the address in withdraw address list of the Mixin network account.

Pre-request: Withdrawal address is added and know the Bitcoin withdrawal fee

#### Add destination address to withdrawal address list
Call createAddress, the ID of address will be returned in result of API and is required soon.
```csharp
var addr = mixinApi.CreateAddress(ASSET_ID_BTC, BTC_WALLET_ADDR, "BTC withdraw", null, null, PinCode);
Console.WriteLine(addr);
```
The **14T129GTbXXPGXXvZzVaNLRFPeHXD1C25C** is a Bitcoin wallet address, Output like below,
The API result contains the withdrawal address ID, fee is 0.0034802 BTC.                                                   
```bash
{'data': {'type': 'address', 'address_id': '47998e2f-2761-45ce-9a6c-6f167b20c78b',
'asset_id': 'c6d0c728-2624-429b-8e0d-d9d19b6592fa',
'public_key': '14T129GTbXXPGXXvZzVaNLRFPeHXD1C25C', 'label': 'BTC',
'account_name': '', 'account_tag': '',
'fee': '0.0034802', 'reserve': '0', 'dust': '0.0001',
'updated_at': '2019-02-26T00:03:05.028140704Z'}}
```
If you want create a EOS address, call it like below:
- **eoswithmixin** eoswithmixin is account name.
- **d80363afcc466fbaf2daa7328ae2adfa** d80363afcc466fbaf2daa7328ae2adfa is account tag.
```csharp
var addr = mixinApi.CreateAddress(null, null, "EOS withdraw", "eoswithmixin", "d80363afcc466fbaf2daa7328ae2adfa", PinCode);
Console.WriteLine(addr);
```
#### Read withdraw fee anytime
```csharp
var addr = mixinApi.CreateAddress(ASSET_ID_BTC, BTC_WALLET_ADDR, "BTC withdraw", null, null, PinCode);
Console.WriteLine(addr);
Console.WriteLine(mixinApi.ReadAddress(addr.address_id));
```

#### Send Bitcoin to destination address
Submit the withdrawal request to Mixin Network, the "addr.address_id" is the address id it's return by CreateAddress
```csharp
 Console.WriteLine(mixinApi.Withdrawal(addr.address_id,AMOUNT,PinCode,System.Guid.NewGuid().ToString(), "Test withdraw"));
```
#### Confirm the transaction in blockchain explore

[Full source code](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/mixin_labs-csharp-bot/Program.cs)
