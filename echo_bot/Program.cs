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
            Console.WriteLine("incomingMessage");
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
                  callback.SendTextMessage(incomingMessage.data.conversation_id, clearText,thisMessageId);
                }
                if (incomingMessage.data.category == "SYSTEM_ACCOUNT_SNAPSHOT") {
                  byte[] strOriginal = Convert.FromBase64String(incomingMessage.data.data);
                  string clearText = System.Text.Encoding.UTF8.GetString(strOriginal);
                  Console.WriteLine(clearText);
                  Transfer trsInfo = JsonConvert.DeserializeObject<Transfer>(clearText);
                  Console.WriteLine(trsInfo.asset_id);
                  Console.WriteLine(trsInfo.opponent_id);
                  Console.WriteLine(trsInfo.amount);

                  MixinApi mixinHttpsApi = new MixinApi();
                  mixinHttpsApi.Init(USRCONFIG.ClientId,
                                     USRCONFIG.ClientSecret,
                                     USRCONFIG.SessionId,
                                     USRCONFIG.PinToken,
                                     USRCONFIG.PrivateKey);

                  Transfer reqInfo = mixinHttpsApi.Transfer(trsInfo.asset_id,
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
                  System.Console.WriteLine("This message has read already: " + incomingMessage.data.message_id);
                }
            }
            if (incomingMessage.action == "LIST_PENDING_MESSAGES")
            {
                System.Console.WriteLine("The bot is listening!");
            }
        }

    }
}
