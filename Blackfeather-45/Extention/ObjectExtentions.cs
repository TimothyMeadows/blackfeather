using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Org.BouncyCastle.Asn1.X509.Qualified;

namespace Blackfeather.Extention
{
    public static class ObjectExtentions
    {
        public static bool IsNullOrDefault<T>(this object value)
        {
            return value == null || value.Equals(default(T));
        }
    }
}