using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackfeather.Data.Storage
{
    public class DataDiskFile
    {
        public int Id { get; set; }
        public string Uid { get; set; }
        public string Parent { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public long Created { get; set; }
        public long Modified { get; set; }
        public long Accessed { get; set; }
    }
}
