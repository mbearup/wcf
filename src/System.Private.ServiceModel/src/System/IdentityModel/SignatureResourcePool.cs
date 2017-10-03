// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;
using System.Security.Cryptography;
using System.ServiceModel;
using System.IdentityModel.Tokens;
using System.Xml;
using System.Text;

namespace System.IdentityModel
{
    // for sequential use by one thread
    internal sealed class SignatureResourcePool
    {
        private const int BufferSize = 64;
#if !FEATURE_CORECLR
        private CanonicalizationDriver canonicalizationDriver;
#endif
        private HashStream hashStream;
        private HashAlgorithm hashAlgorithm;
        private XmlDictionaryWriter utf8Writer;
        private byte[] encodingBuffer;
        private char[] base64Buffer;

        public char[] TakeBase64Buffer()
        {
          if (this.base64Buffer == null)
            this.base64Buffer = new char[64];
          return this.base64Buffer;
        }

#if !FEATURE_CORECLR
        public CanonicalizationDriver TakeCanonicalizationDriver()
        {
          if (this.canonicalizationDriver == null)
            this.canonicalizationDriver = new CanonicalizationDriver();
          else
            this.canonicalizationDriver.Reset();
          return this.canonicalizationDriver;
        }
#endif

        public byte[] TakeEncodingBuffer()
        {
          if (this.encodingBuffer == null)
            this.encodingBuffer = new byte[64];
          return this.encodingBuffer;
        }
        
        public HashAlgorithm TakeHashAlgorithm(string algorithm)
        {
          if (this.hashAlgorithm == null)
          {
            if (string.IsNullOrEmpty(algorithm))
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SR.GetString("EmptyOrNullArgumentString", new object[1]{ (object) "algorithm" }));
            this.hashAlgorithm = CryptoHelper.CreateHashAlgorithm(algorithm);
          }
          else
            this.hashAlgorithm.Initialize();
          return this.hashAlgorithm;
        }

        public HashStream TakeHashStream(HashAlgorithm hash)
        {
          if (this.hashStream == null)
            this.hashStream = new HashStream(hash);
          else
            this.hashStream.Reset(hash);
          return this.hashStream;
        }

        public HashStream TakeHashStream(string algorithm)
        {
          return this.TakeHashStream(this.TakeHashAlgorithm(algorithm));
        }
    }
}
