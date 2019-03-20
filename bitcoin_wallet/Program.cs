using System;
using System.Text;
using ProGaudi.MsgPack.Light;
// using MessagePack;
namespace bitcoint_wallet
{
    class Program
    {
        static void Main(string[] args)
        {
          // "815b0b1a-2764-3736-8faa-42d694fa620a"
          // php: gaFBxBCBWwsaJ2Q3No+qQtaU+mIK
          // c#:  xBAaC1uBZCc2N4+qQtaU+mIK
            Console.WriteLine("Hello World!");
            Guid guid = new Guid("815b0b1a-2764-3736-8faa-42d694fa620a");
            var gbytes = guid.ToByteArray();
            foreach (var byt in gbytes)
            Console.Write("{0:X2} ", byt);
            Console.WriteLine("");
            var bytes = MsgPackSerializer.Serialize(gbytes);
            // var bytes = MessagePackSerializer.Serialize(Guid.NewGuid().ToByteArray());
            Console.WriteLine(Convert.ToBase64String(bytes));
            // var value = MsgPackSerializer.Deserialize<string>(bytes);
            // var value = MessagePackSerializer.Deserialize<string>(bytes);
            // Console.WriteLine(value);
        }
    }
}
