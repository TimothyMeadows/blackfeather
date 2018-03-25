using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackfeather.Security.Cryptography;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var contract = new Contract();
            var unsigned = contract.Create(2, DateTime.UtcNow.AddYears(1));
            var signed1 = contract.Sign(unsigned, "caw caw caw");
            var signed2 = contract.Sign(signed1, "meeeeeh");

            return;
        }
    }
}
