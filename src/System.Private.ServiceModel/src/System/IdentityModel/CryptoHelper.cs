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
        
        internal static HashAlgorithm CreateHashAlgorithm(string s)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
    }
}

