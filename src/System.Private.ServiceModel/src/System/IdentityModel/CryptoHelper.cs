// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.ServiceModel;

namespace System.IdentityModel
{
    internal static class CryptoHelper
    {
        private static readonly RandomNumberGenerator random = (RandomNumberGenerator) new RNGCryptoServiceProvider();
        private static byte[] emptyBuffer;
        
        internal static bool IsEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;
            for (int index = 0; index < a.Length; ++index)
            {
                if ((int) a[index] != (int) b[index])
                    return false;
            }
            return true;
        }

        internal static bool IsSymmetricAlgorithm(string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static byte[] UnwrapKey(byte[] wrappingKey, byte[] wrappedKey, string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static byte[] WrapKey(byte[] wrappingKey, byte[] keyToBeWrapped, string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static byte[] GenerateDerivedKey(byte[] key, string algorithm, byte[] label, byte[] nonce, int derivedKeySize, int position)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static int GetIVSize(string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static ICryptoTransform CreateDecryptor(byte[] key, byte[] iv, string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static ICryptoTransform CreateEncryptor(byte[] key, byte[] iv, string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static KeyedHashAlgorithm CreateKeyedHashAlgorithm(byte[] key, string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static SymmetricAlgorithm GetSymmetricAlgorithm(byte[] key, string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static bool IsAsymmetricAlgorithm(string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static bool IsSymmetricSupportedAlgorithm(string algorithm, int keySize)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static void FillRandomBytes(byte[] buffer)
        {
          CryptoHelper.random.GetBytes(buffer);
        }
        
        internal static HashAlgorithm NewSha1HashAlgorithm()
        {
          return CryptoHelper.CreateHashAlgorithm("http://www.w3.org/2000/09/xmldsig#sha1");
        }
        
        internal static byte[] EmptyBuffer
        {
          get
          {
            if (CryptoHelper.emptyBuffer == null)
              CryptoHelper.emptyBuffer = new byte[0];
            return CryptoHelper.emptyBuffer;
          }
        }
        
		internal static HashAlgorithm NewSha256HashAlgorithm()
		{
		  return CryptoHelper.CreateHashAlgorithm("http://www.w3.org/2001/04/xmlenc#sha256");
		}
		
        internal static HashAlgorithm CreateHashAlgorithm(string algorithm)
        {
          object algorithmFromConfig = CryptoHelper.GetAlgorithmFromConfig(algorithm);
          if (algorithmFromConfig != null)
          {
            HashAlgorithm hashAlgorithm = algorithmFromConfig as HashAlgorithm;
            if (hashAlgorithm != null)
              return hashAlgorithm;
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR.GetString("CustomCryptoAlgorithmIsNotValidHashAlgorithm", new object[1]{ (object) algorithm })));
          }
          if (!(algorithm == "SHA") && !(algorithm == "SHA1") && (!(algorithm == "System.Security.Cryptography.SHA1") && !(algorithm == "http://www.w3.org/2000/09/xmldsig#sha1")))
          {
            if (algorithm == "SHA256" || algorithm == "http://www.w3.org/2001/04/xmlenc#sha256")
            {
              if (SecurityUtils.RequiresFipsCompliance)
                return (HashAlgorithm) new SHA256CryptoServiceProvider();
              return (HashAlgorithm) new SHA256Managed();
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR.GetString("UnsupportedCryptoAlgorithm", new object[1]{ (object) algorithm })));
          }
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning((Exception) new InvalidOperationException(SR.GetString("UnsupportedCryptoAlgorithm", new object[1]{ (object) algorithm })));
        }
        
        internal static object GetAlgorithmFromConfig(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("algorithm"));
            }
            if (algorithm == "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256" || algorithm == "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256" || algorithm == "http://www.w3.org/2001/04/xmlenc#sha256")
            {
                Console.WriteLine("Returning Sha256Managed()");
                return (object) new SHA256Managed();           
            }
            throw new NotImplementedException("Unsupported algorithm " + algorithm);
        }
        
        internal static RandomNumberGenerator RandomNumberGenerator
        {
          get
          {
            if (CryptoHelper.random == null)
            {
                throw new ArgumentNullException("CryptoHelper.random");
            }
            return CryptoHelper.random;
          }
        }
        
        public static void GenerateRandomBytes(byte[] data)
        {
          CryptoHelper.RandomNumberGenerator.GetNonZeroBytes(data);
        }
        
        public static byte[] GenerateRandomBytes(int sizeInBits)
        {
          int length = sizeInBits / 8;
          if (sizeInBits <= 0)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("sizeInBits", SR.GetString("ID6033", new object[1]{ (object) sizeInBits })));
          if (length * 8 != sizeInBits)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("ID6002", new object[1]{ (object) sizeInBits }), "sizeInBits"));
          byte[] data = new byte[length];
          CryptoHelper.GenerateRandomBytes(data);
          return data;
        }
    }
}

