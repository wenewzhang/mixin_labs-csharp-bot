# C# Bitcoin tutorial based on Mixin Network: Receive and send Bitcoin in Mixin Messenger
In [the previous chapter](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README.md), we created our first app, when user sends "Hello,world!", the bot reply the same message.



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
            Console.WriteLine("======== Mixin C# SDK Test ========= \n");
            MixinApi mixinApi = new MixinApi();
            mixinApi.Init(USRCONFIG.ClientId, USRCONFIG.ClientSecret, USRCONFIG.SessionId, USRCONFIG.PinToken, USRCONFIG.PrivateKey);

            Console.WriteLine("======== Initiation Finished ========= \n");

            Console.WriteLine("\n\n======== Test Read Profile ===========\n");
            Console.WriteLine(mixinApi.ReadProfile());

            Console.WriteLine("\n\n======== Test Verify PIN ===========\n");
            Console.WriteLine(mixinApi.VerifyPIN(USRCONFIG.PinCode).ToString());

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

                  Transfer reqInfo = callback.Transfer(trsInfo.asset_id,
                                                      trsInfo.opponent_id,
                                                      trsInfo.amount,
                                                      USRCONFIG.PinCode,
                                                      System.Guid.NewGuid().ToString(),
                                                      "");
                  Console.WriteLine(reqInfo);
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
### Hello Bitcoin!
Execute **dotnet build** in the project directory to build the project, and then execute **dotnet bin/Debug/netcoreapp2.0/echo_bot.dll** make it run.
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
Developer can send Bitcoin to their bots in message panel. The bot receive the Bitcoin and then send back immediately.
![transfer and tokens](https://github.com/wenewzhang/mixin_network-nodejs-bot2/blob/master/transfer-any-tokens.jpg)

User can pay 0.001 Bitcoin to bot by click the button and the 0.001 Bitcoin will be refunded in 1 second,In fact, user can pay any coin either.
![pay-link](https://github.com/wenewzhang/mixin_network-nodejs-bot2/blob/master/Pay_and_refund_quickly.jpg)

## Source code summary
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
* trsInfo.amount is negative if bot sends Bitcoin to user successfully.
* trsInfo.amount is positive if bot receive Bitcoin from user.
Call callback.Transfer to refund the coins back to user.

## Advanced usage
#### APP_BUTTON_GROUP
In some payment scenario, for example:
The coin exchange provides coin-exchange service which transfer BTC to EOS ETH, BCH etc,
you want show the clients many pay links with different amount, APP_BUTTON_GROUP can help you here.
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
Here show clients two buttons for EOS and BTC, you can add more buttons in this way.

#### APP_CARD
Maybe a group of buttons too simple for you, try a pay link which show a icon: APP_CARD.
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
![APP_CARD](https://github.com/wenewzhang/mixin_labs-python-bot/blob/master/app_card.jpg)

Full code is [here](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/echo_bot/Program.cs)
