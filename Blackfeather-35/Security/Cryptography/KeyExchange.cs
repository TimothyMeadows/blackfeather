using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Blackfeather.Security.Cryptography
{
	public class KeyExchange
	{
		/// <summary>
		/// Serializable cryptology key pair
		/// </summary>
		[Serializable]
		public struct KeyPair
		{
			/// <summary>
			/// Private key, do not expose
			/// </summary>
			public string Private;

			/// <summary>
			/// Public key, safe to expose
			/// </summary>
			public string Public;
		}

		private struct Commonality
		{
			public BigInteger G;
			public BigInteger P;
		}

		private Commonality _common = new Commonality()
		{
			G = new BigInteger("3"),
			// https://en.wikipedia.org/wiki/RSA_numbers#RSA-617
			// https://en.wikipedia.org/wiki/RSA_numbers#RSA-2048
			P = new BigInteger("25195908475657893494027183240048398571429282126204032027777137836043662020707595556264018525880784406918290641249515082189298559149176184502808489120072844992687392807287776735971418347270261896375014971824691165077613379859095700097330459748808428401797429100642458691817195118746121515172654632282216869987549182422433637259085141865462043576798423387184774447920739934236584823824281198163815010674810451660377306056201619676256133844143603833904414952634432190114657544454178424020924616515723350778707749817125772467962926386356373289912154831438167899885040445364023527381951378636564391212010397122822120720357")
		};

		/// <summary>
		/// Create a Diffie-Hellman key pair mixture.
		/// </summary>
		/// <returns>KeyPair mixture</returns>
		public KeyPair Mix()
		{
			var cipherKeyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("DiffieHellman");
			var diffieHellmanParameters = new DHParameters(_common.P, _common.G, null);

			KeyGenerationParameters diffieHellmanKeyGenerationParameters = new DHKeyGenerationParameters(new Org.BouncyCastle.Security.SecureRandom(), diffieHellmanParameters);
			cipherKeyPairGenerator.Init(diffieHellmanKeyGenerationParameters);

			var asymmetricKeyPair = cipherKeyPairGenerator.GenerateKeyPair();
			var agreement = AgreementUtilities.GetBasicAgreement("DiffieHellman");
			agreement.Init(asymmetricKeyPair.Private);

			return new KeyPair() { Private = ((DHPrivateKeyParameters)asymmetricKeyPair.Private).X.ToString(), Public = ((DHPublicKeyParameters)asymmetricKeyPair.Public).Y.ToString() };
		}

		/// <summary>
		/// Remix a Diffie-Hellman private key pair with another public key pair to form a shared secret
		/// </summary>
		/// <param name="mixture">Private key pair with another public key pair</param>
		/// <returns>Shared secret</returns>
		public string Remix(KeyPair mixture)
		{
			var agreement = AgreementUtilities.GetBasicAgreement("DiffieHellman");
			agreement.Init(new DHPrivateKeyParameters(new BigInteger(mixture.Private), new DHParameters(_common.P, _common.G, null)));

			return agreement.CalculateAgreement(new DHPublicKeyParameters(new BigInteger(mixture.Public), new DHParameters(_common.P, _common.G, null))).ToString();
		}
	}
}