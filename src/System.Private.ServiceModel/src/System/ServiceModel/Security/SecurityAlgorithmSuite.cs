// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
    public abstract class SecurityAlgorithmSuite
    {
        private static SecurityAlgorithmSuite s_basic256;

        static public SecurityAlgorithmSuite Default
        {
            get
            {
                return Basic256;
            }
        }

        static public SecurityAlgorithmSuite Basic256
        {
            get
            {
                if (s_basic256 == null)
                    s_basic256 = new Basic256SecurityAlgorithmSuite();
                return s_basic256;
            }
        }

        public abstract string DefaultCanonicalizationAlgorithm { get; }
        public abstract string DefaultDigestAlgorithm { get; }
        public abstract string DefaultEncryptionAlgorithm { get; }
        public abstract int DefaultEncryptionKeyDerivationLength { get; }
        public abstract string DefaultSymmetricKeyWrapAlgorithm { get; }
        public abstract string DefaultAsymmetricKeyWrapAlgorithm { get; }
        public abstract string DefaultSymmetricSignatureAlgorithm { get; }
        public abstract string DefaultAsymmetricSignatureAlgorithm { get; }
        public abstract int DefaultSignatureKeyDerivationLength { get; }
        public abstract int DefaultSymmetricKeyLength { get; }

        public virtual XmlDictionaryString DefaultCanonicalizationAlgorithmDictionaryString { get { return null; } }
        public virtual XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return null; } }
        public virtual XmlDictionaryString DefaultEncryptionAlgorithmDictionaryString { get { return null; } }
        public virtual XmlDictionaryString DefaultSymmetricKeyWrapAlgorithmDictionaryString { get { return null; } }
        public virtual XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString { get { return null; } }
        public virtual XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return null; } }
        public virtual XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return null; } }

        protected SecurityAlgorithmSuite() { }

        public virtual bool IsCanonicalizationAlgorithmSupported(string algorithm) { return algorithm == DefaultCanonicalizationAlgorithm; }
        public virtual bool IsDigestAlgorithmSupported(string algorithm) { return algorithm == DefaultDigestAlgorithm; }
        public virtual bool IsEncryptionAlgorithmSupported(string algorithm) { return algorithm == DefaultEncryptionAlgorithm; }
        public virtual bool IsEncryptionKeyDerivationAlgorithmSupported(string algorithm) { return (algorithm == SecurityAlgorithms.Psha1KeyDerivation) || (algorithm == SecurityAlgorithms.Psha1KeyDerivationDec2005); }
        public virtual bool IsSymmetricKeyWrapAlgorithmSupported(string algorithm) { return algorithm == DefaultSymmetricKeyWrapAlgorithm; }
        public virtual bool IsAsymmetricKeyWrapAlgorithmSupported(string algorithm) { return algorithm == DefaultAsymmetricKeyWrapAlgorithm; }
        public virtual bool IsSymmetricSignatureAlgorithmSupported(string algorithm) { return algorithm == DefaultSymmetricSignatureAlgorithm; }
        public virtual bool IsAsymmetricSignatureAlgorithmSupported(string algorithm) { return algorithm == DefaultAsymmetricSignatureAlgorithm; }
        public virtual bool IsSignatureKeyDerivationAlgorithmSupported(string algorithm) { return (algorithm == SecurityAlgorithms.Psha1KeyDerivation) || (algorithm == SecurityAlgorithms.Psha1KeyDerivationDec2005); }
        public abstract bool IsSymmetricKeyLengthSupported(int length);
        public abstract bool IsAsymmetricKeyLengthSupported(int length);
        
        internal int GetSignatureKeyDerivationLength(SecurityToken token, SecureConversationVersion version)
        {
          if (token == null)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
          if (!SecurityUtils.IsSupportedAlgorithm(SecurityUtils.GetKeyDerivationAlgorithm(version), token))
            return 0;
          if (this.DefaultSignatureKeyDerivationLength % 8 != 0)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(SR.GetString("Psha1KeyLengthInvalid", new object[1]{ (object) this.DefaultSignatureKeyDerivationLength })));
          return this.DefaultSignatureKeyDerivationLength / 8;
        }
        
        internal void GetSignatureAlgorithmAndKey(SecurityToken token, out string signatureAlgorithm, out SecurityKey key, out XmlDictionaryString signatureAlgorithmDictionaryString)
        {
          ReadOnlyCollection<SecurityKey> securityKeys = token.SecurityKeys;
          if (securityKeys == null || securityKeys.Count == 0)
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SigningTokenHasNoKeys", new object[1]{ (object) token })));
          for (int index = 0; index < securityKeys.Count; ++index)
          {
            if (securityKeys[index].IsSupportedAlgorithm(this.DefaultSymmetricSignatureAlgorithm))
            {
              signatureAlgorithm = this.DefaultSymmetricSignatureAlgorithm;
              signatureAlgorithmDictionaryString = this.DefaultSymmetricSignatureAlgorithmDictionaryString;
              key = securityKeys[index];
              return;
            }
            if (securityKeys[index].IsSupportedAlgorithm(this.DefaultAsymmetricSignatureAlgorithm))
            {
              signatureAlgorithm = this.DefaultAsymmetricSignatureAlgorithm;
              signatureAlgorithmDictionaryString = this.DefaultAsymmetricSignatureAlgorithmDictionaryString;
              key = securityKeys[index];
              return;
            }
          }
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SigningTokenHasNoKeysSupportingTheAlgorithmSuite", (object) token, (object) this)));
        }
    }

    public class Basic256SecurityAlgorithmSuite : SecurityAlgorithmSuite
    {
        public Basic256SecurityAlgorithmSuite() : base() { }

        public override string DefaultCanonicalizationAlgorithm { get { return DefaultCanonicalizationAlgorithmDictionaryString.Value; } }
        public override string DefaultDigestAlgorithm { get { return DefaultDigestAlgorithmDictionaryString.Value; } }
        public override string DefaultEncryptionAlgorithm { get { return DefaultEncryptionAlgorithmDictionaryString.Value; } }
        public override int DefaultEncryptionKeyDerivationLength { get { return 256; } }
        public override string DefaultSymmetricKeyWrapAlgorithm { get { return DefaultSymmetricKeyWrapAlgorithmDictionaryString.Value; } }
        public override string DefaultAsymmetricKeyWrapAlgorithm { get { return DefaultAsymmetricKeyWrapAlgorithmDictionaryString.Value; } }
        public override string DefaultSymmetricSignatureAlgorithm { get { return DefaultSymmetricSignatureAlgorithmDictionaryString.Value; } }
        public override string DefaultAsymmetricSignatureAlgorithm { get { return DefaultAsymmetricSignatureAlgorithmDictionaryString.Value; } }
        public override int DefaultSignatureKeyDerivationLength { get { return 192; } }
        public override int DefaultSymmetricKeyLength { get { return 256; } }
        public override bool IsSymmetricKeyLengthSupported(int length) { return length == 256; }
        public override bool IsAsymmetricKeyLengthSupported(int length) { return length >= 1024 && length <= 4096; }

        public override XmlDictionaryString DefaultCanonicalizationAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.ExclusiveC14n; } }
        public override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha256Digest; } }
        public override XmlDictionaryString DefaultEncryptionAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Aes256Encryption; } }
        public override XmlDictionaryString DefaultSymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Aes256KeyWrap; } }
        public override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaOaepKeyWrap; } }
        public override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha256Signature; } }
        public override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha256Signature; } }

        public override string ToString()
        {
            return "Basic256";
        }
    }
}
