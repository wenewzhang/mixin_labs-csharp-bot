using System;
using ProGaudi.MsgPack.Light;
// using MsgPack.Serialization;
namespace bitcoint_wallet
{
    class Program
    {
        static void Main(string[] args)
        {
          // "815b0b1a-2764-3736-8faa-42d694fa620a"
          // gaFBxBCBWwsaJ2Q3No+qQtaU+mIK
            Console.WriteLine("Hello World!");
            var bytes = MsgPackSerializer.Serialize("815b0b1a-2764-3736-8faa-42d694fa620a");
            Console.WriteLine(Convert.ToBase64String(bytes));
            var value = MsgPackSerializer.Deserialize<string>(bytes);
            Console.WriteLine(value);
        }
    }
}
