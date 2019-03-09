# 基于Mixin Network的Go语言比特币开发教程:创建机器人
![cover](https://github.com/wenewzhang/mixin_labs-csharp-bot/raw/master/Bitcoin_go.jpg)
[Mixin Network](https://mixin.one) 是一个免费的 极速的端对端加密数字货币交易系统.
在本章中，你可以按教程在Mixin Messenger中创建一个bot来接收用户消息, 学到如何给机器人转**比特币** 或者 让机器人给你转**比特币**.

[Mixin Network的开发资源汇编](https://github.com/awesome-mixin-network/index_of_Mixin_Network_resource)

## 课程简介
1. [创建一个接受消息的机器人](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/README-zhchs.md)
2. [机器人接受比特币并立即退还用户](#)
3. [创建比特币钱包](#)

## 创建一个接受消息的机器人

通过本教程，你将学会如何用 C# 创建一个机器人APP,让它能接受消息.

### C# 安装:
下载安装最新的 .Net Core SDK [.Net Core](https://dotnet.microsoft.com/download).

**macOS**

下载 [dotnet-sdk-2.2](https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.104-macos-x64-installer) 双击安装包然后安装.

**Ubuntu**

打开终端然后按下面的流程安装
```bash
wget -q https://packages.microsoft.com/config/ubuntu/16.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-2.2
```

执行一下 **dotnet --version** 检查是不是安装好了！
```bash
$ dotnet --version
2.2.104
```


### 创建 C# 目录
创建 C# 目录, 比如: echo_bot, 再执行 **dotnet new console** 创建一个 终端下的应用程序!
macOS
```bash
mkdir echo_bot
cd echo_bot
dotnet new console
```

执行输出内容如下：
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

## 你好, 世界

现在我们已经创建了一个“你好世界”的程序，编译然后运行!
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

### 安装 Mixin Network SDK for C#
本教程的程序代码依赖 Mixin CSharp SDK, 可以使用dotnet来安装此包。
```bash
$ dotnet add package MixinCSharpSdk
```

### 创建第一个机器人APP
按下面的提示，到[mixin.one](https://mixin.one)创建一个APP[tutorial](https://mixin-network.gitbook.io/mixin-network/mixin-messenger-app/create-bot-account).

### 生成相应的参数
记下这些[生成的参数](https://mixin-network.gitbook.io/mixin-network/mixin-messenger-app/create-bot-account#generate-secure-parameter-for-your-app)
config.cs中.

![mixin_network-keys](https://github.com/wenewzhang/mixin_labs-php-bot/raw/master/mixin_network-keys.jpg)
config.cs,将生成的参数，替换成你的！
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


Program.cs 的代码如下:

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
### 编译并运行
执行 **dotnet build** 将产生一个 **echo_bot.dll** 文件, 执行方法如下.
```bash
dotnet build
dotnet bin/Debug/netcoreapp2.2/echo_bot.dll

root@ubuntu:~/echo_bot# dotnet bin/Debug/netcoreapp2.2/echo_bot.dll
incomingMessage
{"id":"e1ad0301-57ee-4c80-8695-09648f732c8b","action":"LIST_PENDING_MESSAGES","data":null}
The bot is listening!
```

在手机安装 [Mixin Messenger](https://mixin.one/),增加机器人为好友,(比如这个机器人是7000101639) 然后发送消息给它,效果如下!

![mixin_messenger](https://github.com/wenewzhang/mixin_labs-php-bot/raw/master/helloworld.jpeg)

### 源代码解释
初始化 Mixin API
```csharp
  MixinApi mixinApi = new MixinApi();
  mixinApi.Init(USRCONFIG.ClientId, USRCONFIG.ClientSecret,
                USRCONFIG.SessionId, USRCONFIG.PinToken,
                USRCONFIG.PrivateKey);
```

WebSocket是建立在TCP基础之上的全双工通讯方式，连接到Mixin Network并发送"LISTPENDINGMESSAGES"消息，服务器以后会将收到的消息转发给此程序!
```csharp
  mixinApi.WebSocketConnect(HandleOnRecivedMessage).Wait();
  mixinApi.StartRecive();
```

当服务器给机器人推送消息的时候,机器人会原封不动的回复回去.
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

Mixin Messenger支持的消息类型很多，除了文本，还有图片，视频，语音等等，具体可到下面链接查看:  [WebSocket消息类型](https://developers.mixin.one/api/beta-mixin-message/websocket-messages/).

### 完成
现在你的机器人APP运行起来了，你打算如何改造你的机器人呢？

完整的代码[在这儿](https://github.com/wenewzhang/mixin_labs-csharp-bot/blob/master/echo_bot/Program.cs)
