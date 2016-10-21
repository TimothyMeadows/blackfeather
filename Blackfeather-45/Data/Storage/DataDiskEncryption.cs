using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Blackfeather.Data.Storage
{
    public class DataDiskEncryption
    {
        public string Uid { get; set; }
        public string Password { get; set; }
        public byte[] Salt { get; set; }
    }
}
