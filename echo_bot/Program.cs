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
