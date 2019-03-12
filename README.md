# C Sharp Bitcoin tutorial based on Mixin Network I : Create bot
![cover](https://github.com/wenewzhang/mixin_labs-csharp-bot/raw/master/BItcoin_C%23.jpg)
A Mixin messenger bot will be created in this tutorial. The bot is powered by C#, it echo message and Bitcoin from user.

Full Mixin network resource [index](https://github.com/awesome-mixin-network/index_of_Mixin_Network_resource)

## What you will learn from this tutorial
1. [How to create bot in Mixin messenger and reply message to user](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README.md) | [Chinese](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README-zhchs.md)
2. [How to receive Bitcoin and send Bitcoin in Mixin Messenger](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README2.md) | [Chinese](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README2-zhchs.md)
3. [How to create a Bitcoin wallet based on Mixin Network API](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README2.md) | [Chinese](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README3-zhchs.md)
## How to create bot in Mixin messenger and reply message to user

### C# installation:
Download the latest .Net Core [.Net Core](https://dotnet.microsoft.com/download) from here.

**macOS**

Download [dotnet-sdk-2.2](https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.104-macos-x64-installer) and double click the package to install it.

**Ubuntu**

Open a terminal and run the following commands:
```bash
wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-2.2
```

Check the installation, It's works right now!
```bash
$ dotnet --version
2.2.104
```


### Create C# project directory
Create a C# directory, for example: echo_bot, run **dotnet new console** to create Console Application.
macOS
```bash
mkdir echo_bot
cd echo_bot
dotnet new console
```

The create project output like below:
```bash
root@ubuntu:~/echo_bot# dotnet new console
Getting ready...
The template "Console Application" was created successfully.

Processing post-creation actions...
Running 'dotnet restore' on /root/echo_bot/echo_bot.csproj...
  Restoring packages for /root/echo_bot/echo_bot.csproj...
  Generating MSBuild file /root/echo_bot/obj/echo_bot.csproj.nuget.g.props.
  Generating MSBuild file /root/echo_bot/obj/echo_bot.csproj.nuget.g.targets.
  Restore completed in 404.88 ms for /root/echo_bot/echo_bot.csproj.

Restore succeeded.

root@ubuntu:~/echo_bot#
```

## Hello, world in C#
.Net create a hello-world application, Let's build and run it!
```bash
root@ubuntu:~/echo_bot# dotnet build
Microsoft (R) Build Engine version 15.9.20+g88f5fadfbe for .NET Core
Copyright (C) Microsoft Corporation. All rights reserved.

  Restoring packages for /root/echo_bot/echo_bot.csproj...
  Restore completed in 554.87 ms for /root/echo_bot/echo_bot.csproj.
  echo_bot -> /root/echo_bot/bin/Debug/netcoreapp2.2/echo_bot.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:05.96
root@ubuntu:~/echo_bot# dotnet bin/Debug/netcoreapp2.2/echo_bot.dll
Hello World!
root@ubuntu:~/echo_bot#
```

### Install the Mixin Network SDK for C#
This tutorial depends on Mixin CSharp SDK, you can use dotnet command add it!
```bash
$ dotnet add package MixinCSharpSdk
```

### Create your first app in Mixin Network developer dashboard
You need to create an app in dashboard. This [tutorial](https://mixin-network.gitbook.io/mixin-network/mixin-messenger-app/create-bot-account) can help you.

### Generate parameter of your app in dashboard
After app is created in dashboard, you still need to [generate parameter](https://mixin-network.gitbook.io/mixin-network/mixin-messenger-app/create-bot-account#generate-secure-parameter-for-your-app)
and write down required content, these content will be written into main.go file.

![mixin_network-keys](https://github.com/wenewzhang/mixin_labs-php-bot/blob/master/mixin_network-keys.jpg)
In the folder, create a file: config.cs  Copy the following content into it.
> config.cs

```csharp

namespace echo_bot
{
    static class USRCONFIG
    {
        public static string ClientId = "21042518-85c7-4903-bb19-f311813d1f51";
        public static string ClientSecret = "f14ccf35e52b7e888c2f5a2081bacbed93cb998a1d4e4ab353856a9d3a8beed9";
        public static string PinCode = "911424";
        public static string SessionId = "4267b63d-3daa-449e-bc13-970aa0357776";
        public static string PinToken = "gUUxpm3fPRVkKZNwA/gk10SHHDtR8LmxO+N6KbsZ/jymmwwVitUHKgLbk1NISdN8jBvsYJgF/5hbkxNnCJER5XAZ0Y35gsAxBOgcFN8otsV6F0FAm5TnWN8YYCqeFnXYJnqmI30IXJTAgMhliLj7iZsvyY/3htaHUUuN5pQ5F5s=";
        public static string PrivateKey =
        @"-----BEGIN RSA PRIVATE KEY-----
MIICXQIBAAKBgQCDXiWJRLe9BzPtXmcVe6acaFTY9Ogb4Hc2VHFjKFsp7QRVCytx
3KC/LRojTFViwwExaANTZQ6ectwpAxIvzeYeHDZCXCh6JRFIYK/ZuREmYPcPQEWD
s92Tv/4XTAdTH8l9UJ4VQY4zwqYMak237N9xEvowT0eR8lpeJG0jAjN97QIDAQAB
AoGADvORLB1hGCeQtmxvKRfIr7aEKak+HaYfi1RzD0kRjyUFwDQkPrJQrVGRzwCq
GzJ8mUXwUvaGgmwqOJS75ir2DL8KPz7UfgQnSsHDUwKqUzULgW6nd/3OdDTYWWaN
cDjbkEpsVchOpcdkywvZhhyGXszpM20Vr8emlBcFUOTfpTUCQQDVVjkeMcpRsImV
U3tPYyiuqADhBTcgPBb+Ownk/87jyKF1CZOPvJAebNmpfJP0RMxUVvT4B9/U/yxZ
WNLhLtCXAkEAnaOEuefUxGdE8/55dUTEb7xrr22mNqykJaax3zFK+hSFBrM3gUY5
fEETtHnl4gEdX4jCPybRVc1JSFY/GWoyGwJBAKoLti95JHkErEXYavuWYEEHLNwv
mgcZnoI6cOKVfEVYEEoHvhTeCkoWHVDZOd2EURIQ1eY18JYIZ0M4Z66R8DUCQCsK
iKTR3dA6eiM8qiEQw6nWgniFscpf3PnCx/Iu3U/m5mNr743GhM+eXSj7136b209I
YfEoQiPxRz8O/W+NBV0CQQDVPNqJlFD34MC9aQN42l3NV1hDsl1+nSkWkXSyhhNR
MpobtV1a7IgJGyt5HxBzgNlBNOayICRf0rRjvCdw6aTP
-----END RSA PRIVATE KEY-----";
    }
}

```
Replace the value with content generated in dashboard.

Full source code  of program.cs like below:
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


            mixinApi.WebSocketConnect(HandleOnRecivedMessage).Wait();

            mixinApi.StartRecive();

            do
            {
                var msg = Console.ReadLine();


            } while (true);

        }


        static void HandleOnRecivedMessage(object sender, EventArgs args, string message)
        {

            //System.Console.WriteLine(message);
            var incomingMessage = JsonConvert.DeserializeObject<RecvWebSocketMessage>(message);
            System.Console.WriteLine("incomingMessage");
            if ( (incomingMessage.action == "CREATE_MESSAGE") && (incomingMessage.data != null) ) {
                Console.WriteLine(incomingMessage.data.conversation_id);
                MixinApi callback = (MixinApi)sender;
                //send ack for every Create Message!
                callback.SendMessageResponse(incomingMessage.data.message_id).Wait();
                if (incomingMessage.data.category == "PLAIN_TEXT") {
                  byte[] strOriginal = Convert.FromBase64String(incomingMessage.data.data);
                  string clearText = System.Text.Encoding.UTF8.GetString(strOriginal);
                  Console.WriteLine(clearText);
                  callback.SendTextMessage(incomingMessage.data.conversation_id, clearText).Wait();
                }

            }
            Console.WriteLine(incomingMessage);
            if (incomingMessage.action == "ACKNOWLEDGE_MESSAGE_RECEIPT")
            {
                System.Console.WriteLine("This message has read already: " + incomingMessage.data.message_id);
            }
            if (incomingMessage.action == "LIST_PENDING_MESSAGES")
            {
                System.Console.WriteLine("The bot is listening!");
            }
        }

    }
}

```
### Build and run it
Issue **dotnet build** will generate an **echo_bot.dll** file, execute it.
```bash
dotnet build
dotnet bin/Debug/netcoreapp2.2/echo_bot.dll

root@ubuntu:~/echo_bot# dotnet bin/Debug/netcoreapp2.2/echo_bot.dll
incomingMessage
{"id":"e1ad0301-57ee-4c80-8695-09648f732c8b","action":"LIST_PENDING_MESSAGES","data":null}
The bot is listening!
```

Add the bot(for example, this bot id is 7000101639) as your friend in [Mixin Messenger](https://mixin.one/messenger) and send your messages.

![mixin_messenger](https://github.com/wenewzhang/mixin_labs-php-bot/raw/master/helloworld.jpeg)

### Source code explanation
Initial the Mixin Api with basic information.
```csharp
  MixinApi mixinApi = new MixinApi();
  mixinApi.Init(USRCONFIG.ClientId, USRCONFIG.ClientSecret,
                USRCONFIG.SessionId, USRCONFIG.PinToken,
                USRCONFIG.PrivateKey);
```

The code creates a websocket which connect to Mixin Network, and send **LISTPENDINGMESSAGES** to server, let it know the bot is online now, the server will reply all message to bot.
```csharp
  mixinApi.WebSocketConnect(HandleOnRecivedMessage).Wait();
  mixinApi.StartRecive();
```

The bot echo every text from user after received messages.
```csharp
if ( (incomingMessage.action == "CREATE_MESSAGE") && (incomingMessage.data != null) ) {
    Console.WriteLine(incomingMessage.data.conversation_id);
    MixinApi callback = (MixinApi)sender;
    //send ack for every Create Message!
    callback.SendMessageResponse(incomingMessage.data.message_id).Wait();
    if (incomingMessage.data.category == "PLAIN_TEXT") {
      byte[] strOriginal = Convert.FromBase64String(incomingMessage.data.data);
      string clearText = System.Text.Encoding.UTF8.GetString(strOriginal);
      Console.WriteLine(clearText);
      callback.SendTextMessage(incomingMessage.data.conversation_id, clearText).Wait();
    }

}
```
## Advanced usage
Mixin Messenger will send message status to you if you send message to Mixin messenger user:
- **DELIVERED**  just delivered to user.
- **READ**  user has read it.
```csharp
if (incomingMessage.action == "ACKNOWLEDGE_MESSAGE_RECEIPT")
{
    if (incomingMessage.data != null) {
      System.Console.WriteLine("The message delivery status: " +
                                incomingMessage.data.message_id + " "
                                + incomingMessage.data.status);
    }
}
```

Output like below:
```bash
Send echo with message id:cb686aac-e3bb-4530-8ed1-7fd580464bbd
The message delivery status: cb686aac-e3bb-4530-8ed1-7fd580464bbd DELIVERED
The message delivery status: cb686aac-e3bb-4530-8ed1-7fd580464bbd READ
```

Not only texts, images and other type message will be pushed to your bot. You can find more [details](https://developers.mixin.one/api/beta-mixin-message/websocket-messages/) about Messenger message.

### End
Now your bot worked. You can hack it.

Full code is [here](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/echo_bot/Program.cs)

## [How to receive Bitcoin and send Bitcoin in Mixin Messenger](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README2.md)
