using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackfeather.Data.Storage;

namespace TestPleaseDelete
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataDisk = new DataDisk("test.ddf");
            dataDisk.Format(new DataDiskFormat()
            {
                Indexed = true,
                Label = "Test Disk",
                Size = 1024
            });
        }
    }
}
