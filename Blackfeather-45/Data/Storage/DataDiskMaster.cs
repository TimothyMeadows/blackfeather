using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackfeather.Data.Storage
{
    public class DataDiskMaster
    {
        public string Label { get; set; }
        public int Size { get; set; }
        public bool Indexed { get; set; }
    }
}
