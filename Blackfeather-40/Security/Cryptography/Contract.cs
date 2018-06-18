using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Blackfeather.Data.Encoding;
using Blackfeather.Extention;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Blackfeather.Security.Cryptography
{
    /// <summary>
    /// Private, and public signable keys. This IS NOT Public Key Infrastructure but a weaker alternative without a key-server
    /// </summary>
    public class Contract
    {
        /// <summary>
        /// Serializable cryptology key pair
        /// </summary>
        [Serializable]
        public struct Signature
        {
            /// <summary>
            /// Proof of identity for the signature
            /// </summary>
            public string Identity;

            /// <summary>
            /// Public key that represents the identity (safe to share)
            /// </summary>
            public string Public;

            /// <summary>
            /// Hash to represents the above data has not been tampered with
            /// </summary>
            public string Hash;
        }

        /// <summary>
        /// Serializable cryptology key pair
        /// </summary>
        [Serializable]
        public struct UnsignedContract
        {
            /// <summary>
            /// Number of unique participants the contact requires to be completed 
            /// </summary>
            public int Participants;

            /// <summary>
            /// Private key, do not expose
            /// </summary>
            public string Private;

            /// <summary>
            /// Public key, safe to expose
            /// </summary>
            public string Public;

            /// <summary>
            /// Hash to represents the above data has not been tampered with
            /// </summary>
            public string Hash;

            /// <summary>
            /// DateTime when the contract expires
            /// </summary>
            public long Expires;
        }

        /// <summary>
        /// Serializable cryptology key pair
        /// </summary>
        [Serializable]
        public struct SignedContract
        {
            /// <summary>
            /// Number of unique participants the contact requires to be completed 
            /// </summary>
            public int Participants;

            /// <summary>
            /// Private key, do not expose
            /// </summary>
            public string Private;

            /// <summary>
            /// Public key, safe to expose
            /// </summary>
            public string Public;

            /// <summary>
            /// Hash to represents the above data has not been tampered with
            /// </summary>
            public string Hash;

            /// <summary>
            /// Hash to represents the above data has not been tampered with
            /// </summary>
            public List<Signature> Signatures;

            /// <summary>
            /// DateTime when the contract expires
            /// </summary>
            public long Expires;
        }

        public UnsignedContract Create(int participants, DateTime expires)
        {
            var secret = 2048.ToRandomBytes();
            var privateKey = secret.ToHex();
            var publicKey = privateKey.ToHmac(secret).ToHex();

            return new UnsignedContract()
            {
                Participants = participants,
                Private = privateKey,
                Public = publicKey,
                Hash = $"participants={participants}&private={privateKey}&public={publicKey}&expires={expires.ToBinary()}".ToHmac(secret).ToHex(),
                Expires = expires.ToBinary()
            };
        }

        public SignedContract Sign(UnsignedContract contract, string signature)
        {
            var suspected =
                $"participants={contract.Participants}&private={contract.Private}&public={contract.Public}&expires={contract.Expires}"
                    .ToHmac(contract.Private.FromHex()).ToHex();

            if (!suspected.Equals(contract.Hash))
                throw new InvalidDataException("Contract has been tampered with!");

            var expires = DateTime.FromBinary(contract.Expires);
            if (expires <= DateTime.UtcNow)
                throw new InvalidDataException("Contract has expired!");

            var signed = new SignedContract()
            {
                Participants = contract.Participants,
                Private = contract.Private,
                Public = contract.Public,
                Signatures = new List<Signature>(),
                Expires = contract.Expires
            };

            var signatureHash = signature.ToHmac(contract.Private).ToHex();
            signed.Signatures.Add(new Signature()
            {
                Identity = signature,
                Public = signatureHash,
                Hash = $"{signature}:{signatureHash}".ToHmac(contract.Private).ToHex()
            });

            signed.Hash =
                $"participants={signed.Participants}&private={signed.Private}&public={signed.Public}&signatures={string.Join(":", signed.Signatures.Select(t => t.Hash))}&expires={signed.Expires}"
                    .ToHmac(signed.Private.FromHex()).ToHex();

            return signed;
        }

        public SignedContract Sign(SignedContract contract, string signature)
        {
            var suspected =
                $"participants={contract.Participants}&private={contract.Private}&public={contract.Public}&signatures={string.Join(":", contract.Signatures.Select(t => t.Hash))}&expires={contract.Expires}"
                    .ToHmac(contract.Private.FromHex()).ToHex();

            if (!suspected.Equals(contract.Hash))
                throw new InvalidDataException("Contract has been tampered with!");

            if (contract.Signatures.Count + 1 > contract.Participants)
                throw new InvalidDataException("Contract already has maximum number of participants!");

            var expires = DateTime.FromBinary(contract.Expires);
            if (expires <= DateTime.UtcNow)
                throw new InvalidDataException("Contract has expired!");

            var signatureHash = signature.ToHmac(contract.Private).ToHex();
            contract.Signatures.Add(new Signature()
            {
                Identity = signature,
                Public = signatureHash,
                Hash = $"{signature}:{signatureHash}".ToHmac(contract.Private).ToHex()
            });

            contract.Hash =
                $"participants={contract.Participants}&private={contract.Private}&public={contract.Public}&signatures={string.Join(":", contract.Signatures.Select(t => t.Hash))}&expires={contract.Expires}"
                    .ToHmac(contract.Private.FromHex()).ToHex();
            return contract;
        }
    }
}
