using System;
using System.Text;
using SimpleMsgPack;
namespace bitcoint_wallet
{
    class Program
    {
        static void Main(string[] args)
        {
          // "815b0b1a-2764-3736-8faa-42d694fa620a"
          // php: gaFBxBCBWwsaJ2Q3No+qQtaU+mIK
          // c#:  xBCBWwsaJ2Q3No+qQtaU+mIK

         // "815b0b1a-2764-3736-8faa-42d694fa620a"
          //php: gaFBxBDG0McoJiRCm44N2dGbZZL6
          // C#: xBDG0McoJiRCm44N2dGbZZL6
            Console.WriteLine("c6d0c728-2624-429b-8e0d-d9d19b6592fa");
            Guid guid = new Guid("c6d0c728-2624-429b-8e0d-d9d19b6592fa");
            var gbytes = guid.ToByteArray();
            Array.Reverse(gbytes,0,4);
            Array.Reverse(gbytes,4,2);
            Array.Reverse(gbytes,6,2);
            foreach (var byt in gbytes)
            Console.Write("{0:X2} ", byt);

            MsgPack msgpack = new MsgPack();
            msgpack.SetAsBytes(gbytes);

            byte[] packData = msgpack.Encode2Bytes();
            Console.WriteLine(Convert.ToBase64String(packData));
        }
    }
}
