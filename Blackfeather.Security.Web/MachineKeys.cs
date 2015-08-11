using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Blackfeather.Extension.Arrays;

namespace Blackfeather.Security.Web
{
    public class MachineKeys
    {
        public const int MACHINE_KEYS_SIZE = 88;
        public const int MACHINE_VALIDATION_KEY_SIZE = 64;
        public const int MACHINE_DECRYPTION_KEY_SIZE = 24;

        public static byte[] GetMachineKeys()
        {
            var autogenKeysFieldInfo = typeof(HttpRuntime).GetField("s_autogenKeys", BindingFlags.NonPublic | BindingFlags.Static);
            if (autogenKeysFieldInfo != null)
            {
                return (byte[])autogenKeysFieldInfo.GetValue(null);
            }

            throw new CryptographicException("Unable to locate machine keys!");
        }

        public static byte[] GetMachineDecryptionKey(byte[] machineKeys = null)
        {
            if (string.IsNullOrEmpty(HttpRuntime.AppDomainAppVirtualPath))
            {
                throw new HttpException("Unable to locate HttpRuntime.AppDomainAppVirtualPath!");
            }

            var machineKeyCollection = machineKeys ?? GetMachineKeys();
            var virtualPathHash =
                    StringComparer.InvariantCultureIgnoreCase.GetHashCode(HttpRuntime.AppDomainAppVirtualPath);

            var machineDecryptionKey = machineKeyCollection.Slice(MACHINE_VALIDATION_KEY_SIZE,
                    MACHINE_KEYS_SIZE);

            machineDecryptionKey[0] = (byte)(virtualPathHash & 0xFF);
            machineDecryptionKey[1] = (byte)((virtualPathHash & 0xFF00) >> 8);
            machineDecryptionKey[2] = (byte)((virtualPathHash & 0xFF0000) >> 16);
            machineDecryptionKey[3] = (byte)((virtualPathHash & 0xFF000000) >> 24);

            return machineDecryptionKey;
        }

        public static byte[] GetMachineValidationKey(byte[] machineKeys = null)
        {
            if (string.IsNullOrEmpty(HttpRuntime.AppDomainAppVirtualPath))
            {
                throw new HttpException("Unable to locate HttpRuntime.AppDomainAppVirtualPath!");
            }

            var machineKeyCollection = machineKeys ?? GetMachineKeys();
            var virtualPathHash =
                    StringComparer.InvariantCultureIgnoreCase.GetHashCode(HttpRuntime.AppDomainAppVirtualPath);

            var machineValidationKey = machineKeyCollection.Slice(0, MACHINE_VALIDATION_KEY_SIZE);

            machineValidationKey[0] = (byte)(virtualPathHash & 0xFF);
            machineValidationKey[1] = (byte)((virtualPathHash & 0xFF00) >> 8);
            machineValidationKey[2] = (byte)((virtualPathHash & 0xFF0000) >> 16);
            machineValidationKey[3] = (byte)((virtualPathHash & 0xFF000000) >> 24);

            return machineValidationKey;
        }
    }
}
