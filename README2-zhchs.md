# 基于Mixin Network的C#语言比特币开发教程 : 用 Mixin Messenger 机器人接受和发送比特币
在 [上一篇教程中](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README-zhchs.md), 我们创建了自动回复消息的机器人,当用户发送消息"Hello,World!"时，机器人会自动回复同一条消息!

按本篇教程后学习后完成后，你的机器人将会接受用户发送过来的加密货币，然后立即转回用户。
完整代码如下：
> Program.cs

```csharp
using System;
using System.Collections.Generic;
using MixinSdk;
using MixinSdk.Bean;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace echo_bot
{
    class Program
    {
        static void Main(string[] args)
        {
            MixinApi mixinApi = new MixinApi();
            mixinApi.Init(USRCONFIG.ClientId, USRCONFIG.ClientSecret, USRCONFIG.SessionId, USRCONFIG.PinToken, USRCONFIG.PrivateKey);

            mixinApi.WebSocketConnect(HandleOnRecivedMessage).Wait();

            mixinApi.StartRecive();

            do
            {
                var msg = Console.ReadLine();


            } while (true);

        }


        static void HandleOnRecivedMessage(object sender, EventArgs args, string message)
        {
            var incomingMessage = JsonConvert.DeserializeObject<RecvWebSocketMessage>(message);
            Console.WriteLine(incomingMessage);
            if ( (incomingMessage.action == "CREATE_MESSAGE") && (incomingMessage.data != null) ) {
                // Console.WriteLine(incomingMessage.data.conversation_id);
                MixinApi callback = (MixinApi)sender;
                //send ack for every Create Message!
                callback.SendMessageResponse(incomingMessage.data.message_id).Wait();
                if (incomingMessage.data.category == "PLAIN_TEXT") {
                  byte[] strOriginal = Convert.FromBase64String(incomingMessage.data.data);
                  string clearText = System.Text.Encoding.UTF8.GetString(strOriginal);
                  Console.WriteLine(clearText);
                  string thisMessageId = Guid.NewGuid().ToString();
                  System.Console.WriteLine("Send echo with message id:" + thisMessageId);
                  if (clearText == "a") {
                    AppCard appCard      = new AppCard();
                    appCard.title        = "Pay BTC 0.0001";
                    appCard.icon_url     = "https://images.mixin.one/HvYGJsV5TGeZ-X9Ek3FEQohQZ3fE9LBEBGcOcn4c4BNHovP4fW4YB97Dg5LcXoQ1hUjMEgjbl1DPlKg1TW7kK6XP=s128";
                    appCard.description  = "hi";
                    appCard.action       = "https://mixin.one/pay?recipient=" +
                              							 USRCONFIG.ClientId  + "&asset=" +
                              							 "c6d0c728-2624-429b-8e0d-d9d19b6592fa"   +
                              							 "&amount=" + "0.001" +
                              							 "&trace="  + System.Guid.NewGuid().ToString() +
                              							 "&memo=";
                    callback.SendAppCardMessage(incomingMessage.data.conversation_id,appCard);
                  } else if (clearText == "g") {
                    List<AppButton> appBtnList = new List<AppButton>();
                    string payLinkEOS = "https://mixin.one/pay?recipient=" +
                        							 USRCONFIG.ClientId  + "&asset=" +
                        							 "6cfe566e-4aad-470b-8c9a-2fd35b49c68d"   +
                        							 "&amount=" + "0.1" +
                        							 "&trace="  + System.Guid.NewGuid().ToString() +
                        							 "&memo=";
              		  string payLinkBTC = "https://mixin.one/pay?recipient=" +
                        							 USRCONFIG.ClientId  + "&asset=" +
                        							 "c6d0c728-2624-429b-8e0d-d9d19b6592fa"   +
                        							 "&amount=" + "0.001" +
                        							 "&trace="  + System.Guid.NewGuid().ToString() +
                        							 "&memo=";
                    AppButton btnBTC = new AppButton();
                    btnBTC.label     = "Pay BTC 0.001";
                    btnBTC.color     = "#0080FF";
                    btnBTC.action    = payLinkBTC;

                    AppButton btnEOS = new AppButton();
                    btnEOS.label     = "Pay EOS 0.1";
                    btnEOS.color     = "#8000FF";
                    btnEOS.action    = payLinkEOS;
                    appBtnList.Add(btnBTC);
                    appBtnList.Add(btnEOS);
              			callback.SendAppButtonGroupMessage(incomingMessage.data.conversation_id,appBtnList);
                  } else  callback.SendTextMessage(incomingMessage.data.conversation_id, clearText,thisMessageId);

                }
                if (incomingMessage.data.category == "SYSTEM_ACCOUNT_SNAPSHOT") {
                  byte[] strOriginal = Convert.FromBase64String(incomingMessage.data.data);
                  string clearText = System.Text.Encoding.UTF8.GetString(strOriginal);
                  Console.WriteLine(clearText);
                  Transfer trsInfo = JsonConvert.DeserializeObject<Transfer>(clearText);
                  Console.WriteLine(trsInfo.asset_id);
                  Console.WriteLine(trsInfo.opponent_id);
                  Console.WriteLine(trsInfo.amount);
                  if ( Int32.Parse(trsInfo.amount) > 0 ) {
                    Transfer reqInfo = callback.Transfer(trsInfo.asset_id,
                                                        trsInfo.opponent_id,
                                                        trsInfo.amount,
                                                        USRCONFIG.PinCode,
                                                        System.Guid.NewGuid().ToString(),
                                                        "");
                    Console.WriteLine(reqInfo);
                  }
                }
            }
            // Console.WriteLine(incomingMessage);
            if (incomingMessage.action == "ACKNOWLEDGE_MESSAGE_RECEIPT")
            {
                if (incomingMessage.data != null) {
                  System.Console.WriteLine("The message delivery status: " +
                                            incomingMessage.data.message_id + " "
                                            + incomingMessage.data.status);
                }
            }
            if (incomingMessage.action == "LIST_PENDING_MESSAGES")
            {
                System.Console.WriteLine("The bot is listening!");
            }
        }

    }
}


```
### 你好, 比特币!

在项目目录下编译并执行
- **dot build**  编译项目.
- **dotnet bin/Debug/netcoreapp2.0/echo_bot.dll** 运行机器人程序.
```bash
wenewzha:echo_bot wenewzhang$ dotnet build
Microsoft (R) Build Engine version 15.9.20+g88f5fadfbe for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

  Restore completed in 154.16 ms for /Users/wenewzhang/Documents/sl/mixin_labs-csharp-bot/echo_bot/echo_bot.csproj.
  Restore completed in 154.19 ms for /Users/wenewzhang/Documents/sl/mixin_labs-csharp-bot/mixin-csharp-sdk/mixin-csharp-sdk/mixin-csharp-sdk.csproj.
  mixin-csharp-sdk -> /Users/wenewzhang/Documents/sl/mixin_labs-csharp-bot/mixin-csharp-sdk/mixin-csharp-sdk/bin/Debug/netstandard2.0/mixin-csharp-sdk.dll
  Created package at /Users/wenewzhang/Documents/sl/mixin_labs-csharp-bot/mixin-csharp-sdk/mixin-csharp-sdk/bin/Debug/MixinCSharpSdk.0.1.0.nupkg.
  echo_bot -> /Users/wenewzhang/Documents/sl/mixin_labs-csharp-bot/echo_bot/bin/Debug/netcoreapp2.0/echo_bot.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:03.33
wenewzha:echo_bot wenewzhang$ dotnet bin/Debug/netcoreapp2.0/echo_bot.dll

{"id":"dd52dfe5-7f12-4c0e-bb44-d1865308e628","action":"LIST_PENDING_MESSAGES","data":null}
The bot is listening!
```

开发者可以通过消息面板，给机器人转比特币，当机器人收到比特币后，马上返还给用户！
![transfer and tokens](https://github.com/wenewzhang/mixin_network-nodejs-bot2/raw/master/transfer-any-tokens.jpg)

事实上，用户可以发送任意的币种给机器人，它都能马上返还！
![pay-link](https://github.com/wenewzhang/mixin_network-nodejs-bot2/raw/master/Pay_and_refund_quickly.jpg)

## 源代码解释
```csharp
if (incomingMessage.data.category == "SYSTEM_ACCOUNT_SNAPSHOT") {
	byte[] strOriginal = Convert.FromBase64String(incomingMessage.data.data);
	string clearText = System.Text.Encoding.UTF8.GetString(strOriginal);
	Transfer trsInfo = JsonConvert.DeserializeObject<Transfer>(clearText);
if ( Int32.Parse(trsInfo.amount) > 0 ) {
	Transfer reqInfo = callback.Transfer(trsInfo.asset_id,
																			trsInfo.opponent_id,
																			trsInfo.amount,
																			USRCONFIG.PinCode,
																			System.Guid.NewGuid().ToString(),
																			"");
	Console.WriteLine(reqInfo);
}
```
如果机器人收到币，trsInfo.amount 大于零；如果机器人支付币给用户，接收到的消息是一样的，唯一不同的是,trsInfo.amount是一个负数.
最后一步，调用SDK的 callback.Transfer 将币返还用户！

## 高级用法
#### APP_BUTTON_GROUP
在一些应用场景，比如：有一个交易所想提供换币服务，将比特币换成以太坊，EOS,比特币现金等,
你想显示给用户一组按钮，它们分别代表不同的币与不同的数量,APP_BUTTON_GROUP可以帮你做到这一点.
```csharp
  List<AppButton> appBtnList = new List<AppButton>();
  string payLinkEOS = "https://mixin.one/pay?recipient=" +
                     USRCONFIG.ClientId  + "&asset=" +
                     "6cfe566e-4aad-470b-8c9a-2fd35b49c68d"   +
                     "&amount=" + "0.1" +
                     "&trace="  + System.Guid.NewGuid().ToString() +
                     "&memo=";
  string payLinkBTC = "https://mixin.one/pay?recipient=" +
                     USRCONFIG.ClientId  + "&asset=" +
                     "c6d0c728-2624-429b-8e0d-d9d19b6592fa"   +
                     "&amount=" + "0.001" +
                     "&trace="  + System.Guid.NewGuid().ToString() +
                     "&memo=";
  AppButton btnBTC = new AppButton();
  btnBTC.label     = "Pay BTC 0.001";
  btnBTC.color     = "#0080FF";
  btnBTC.action    = payLinkBTC;

  AppButton btnEOS = new AppButton();
  btnEOS.label     = "Pay EOS 0.1";
  btnEOS.color     = "#8000FF";
  btnEOS.action    = payLinkEOS;
  appBtnList.Add(btnBTC);
  appBtnList.Add(btnEOS);
  callback.SendAppButtonGroupMessage(incomingMessage.data.conversation_id,appBtnList);
```
这里演示给用户BTC与EOS两种，你还可以增加更多的按钮.

#### APP_CARD
如果你觉得一组按钮太单调了，可以试一下APP_CARD,它提供一个图标的链接
```csharp
  AppCard appCard      = new AppCard();
  appCard.title        = "Pay BTC 0.0001";
  appCard.icon_url     = "https://images.mixin.one/HvYGJsV5TGeZ-X9Ek3FEQohQZ3fE9LBEBGcOcn4c4BNHovP4fW4YB97Dg5LcXoQ1hUjMEgjbl1DPlKg1TW7kK6XP=s128";
  appCard.description  = "hi";
  appCard.action       = "https://mixin.one/pay?recipient=" +
                           USRCONFIG.ClientId  + "&asset=" +
                           "c6d0c728-2624-429b-8e0d-d9d19b6592fa"   +
                           "&amount=" + "0.001" +
                           "&trace="  + System.Guid.NewGuid().ToString() +
                           "&memo=";
  callback.SendAppCardMessage(incomingMessage.data.conversation_id,appCard);
```
![APP_CARD](https://github.com/wenewzhang/mixin_labs-python-bot/raw/master/app_card.jpg)

Full code is [here](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/echo_bot/Program.cs)
