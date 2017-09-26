// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.Xml;

namespace System.ServiceModel.Security
{
    public abstract class SendSecurityHeader : SecurityHeader, IMessageHeaderWithSharedNamespace
    {
        private System.ServiceModel.Channels.BufferManager bufferManager;
		private static readonly string[] ids = new string[10]{ "_0", "_1", "_2", "_3", "_4", "_5", "_6", "_7", "_8", "_9" };
		private bool signThenEncrypt = true;
        private string idPrefix;
		private int idCounter;
		private bool hasSignedTokens;
		private bool shouldSignToHeader;
		private List<SecurityToken> basicTokens;
		private List<SecurityTokenParameters> basicSupportingTokenParameters;
		private List<SecurityTokenParameters> endorsingTokenParameters;
		private List<SecurityTokenParameters> signedEndorsingTokenParameters;
		private List<SecurityTokenParameters> signedTokenParameters;
        private SendSecurityHeaderElementContainer elementContainer;
		private bool hasEncryptedTokens;
		private MessagePartSpecification signatureParts;
		private MessagePartSpecification encryptionParts;
		private byte[] primarySignatureValue;

        protected SendSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            MessageDirection transferDirection)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, transferDirection)
        {
           this.elementContainer = new SendSecurityHeaderElementContainer();
        }

#region FROMWCF
		public bool HasSignedTokens
		{
		  get
		  {
			return this.hasSignedTokens;
		  }
		}
		
		public bool HasEncryptedTokens
		{
		  get
		  {
			return this.hasEncryptedTokens;
		  }
		}
		
		internal SendSecurityHeaderElementContainer ElementContainer
		{
		  get
		  {
			return this.elementContainer;
		  }
		}
		
		protected bool ShouldSignToHeader
		{
		  get
		  {
			return this.shouldSignToHeader;
		  }
		}
		
		internal byte[] PrimarySignatureValue
		{
		  get
		  {
			return this.primarySignatureValue;
		  }
		}
		
		public bool SignThenEncrypt
		{
		  get
		  {
			return this.signThenEncrypt;
		  }
		  set
		  {
			this.ThrowIfProcessingStarted();
			this.signThenEncrypt = value;
		  }
		}
		
		internal void StartSecurityApplication()
		{
#if FEATURE_CORECLR
		  throw new NotImplementedException("StartSecurityApplication not implemented in .NET Core");
#else
		  if (this.SignThenEncrypt)
		  {
			this.StartSignature();
			this.StartEncryption();
		  }
		  else
		  {
			this.StartEncryption();
			this.StartSignature();
		  }
#endif
		}
		
		private void StartSignature()
		{
		  if (this.elementContainer.SourceSigningToken == null)
			return;
#if FEATURE_CORECLR
		  throw new NotImplementedException("SecurityTokenParameters.CreateKeyIdentifierClause not implemented in .NET Core");
#else
		  SecurityKeyIdentifierClause identifierClause1 = this.signingTokenParameters.CreateKeyIdentifierClause(this.elementContainer.SourceSigningToken, this.GetTokenReferenceStyle(this.signingTokenParameters));
		  if (identifierClause1 == null)
			throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(System.ServiceModel.SR.GetString("TokenManagerCannotCreateTokenReference")), this.Message);
		  SecurityToken token;
		  SecurityKeyIdentifierClause identifierClause2;
		  if (this.signingTokenParameters.RequireDerivedKeys && !this.signingTokenParameters.HasAsymmetricKey)
		  {
			string derivationAlgorithm1 = this.AlgorithmSuite.GetSignatureKeyDerivationAlgorithm(this.elementContainer.SourceSigningToken, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
			string derivationAlgorithm2 = SecurityUtils.GetKeyDerivationAlgorithm(this.StandardsManager.MessageSecurityVersion.SecureConversationVersion);
			if (derivationAlgorithm1 == derivationAlgorithm2)
			{
			  token = this.elementContainer.DerivedSigningToken = (SecurityToken) new DerivedKeySecurityToken(-1, 0, this.AlgorithmSuite.GetSignatureKeyDerivationLength(this.elementContainer.SourceSigningToken, this.StandardsManager.MessageSecurityVersion.SecureConversationVersion), (string) null, 16, this.elementContainer.SourceSigningToken, identifierClause1, derivationAlgorithm1, this.GenerateId());
			  identifierClause2 = (SecurityKeyIdentifierClause) new LocalIdKeyIdentifierClause(token.Id, token.GetType());
			}
			else
			  throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(System.ServiceModel.SR.GetString("UnsupportedCryptoAlgorithm", new object[1]{ (object) derivationAlgorithm1 })));
		  }
		  else
		  {
			token = this.elementContainer.SourceSigningToken;
			identifierClause2 = identifierClause1;
		  }
		  SecurityKeyIdentifier identifier = new SecurityKeyIdentifier(new SecurityKeyIdentifierClause[1]{ identifierClause2 });
		  if (this.signatureConfirmationsToSend != null && this.signatureConfirmationsToSend.Count > 0)
		  {
			ISecurityElement[] confirmationElements = (ISecurityElement[]) this.CreateSignatureConfirmationElements(this.signatureConfirmationsToSend);
			for (int index = 0; index < confirmationElements.Length; ++index)
			  this.elementContainer.AddSignatureConfirmation(new SendSecurityHeaderElement(confirmationElements[index].Id, confirmationElements[index])
			  {
				MarkedForEncryption = this.signatureConfirmationsToSend.IsMarkedForEncryption
			  });
		  }
		  bool generateTargettablePrimarySignature = this.endorsingTokenParameters != null || this.signedEndorsingTokenParameters != null;
		  this.StartPrimarySignatureCore(token, identifier, this.signatureParts, generateTargettablePrimarySignature);
#endif
		}
		
		internal abstract void ApplyBodySecurity(XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator);

		internal abstract void ApplySecurityAndWriteHeaders(MessageHeaders headers, XmlDictionaryWriter writer, IPrefixGenerator prefixGenerator);
		
		public Message SetupExecution()
		{
#if FEATURE_CORECLR
		  throw new NotImplementedException("SetupExecution not implemented in .NET Core");
#else
		  this.ThrowIfProcessingStarted();
		  this.SetProcessingStarted();
		  bool signBody = false;
		  if (this.elementContainer.SourceSigningToken != null)
		  {
			if (this.signatureParts == null)
			  throw TraceUtility.ThrowHelperError((Exception) new ArgumentNullException("SignatureParts"), this.Message);
			signBody = this.signatureParts.IsBodyIncluded;
		  }
		  bool encryptBody = false;
		  if (this.elementContainer.SourceEncryptionToken != null)
		  {
			if (this.encryptionParts == null)
			  throw TraceUtility.ThrowHelperError((Exception) new ArgumentNullException("EncryptionParts"), this.Message);
			encryptBody = this.encryptionParts.IsBodyIncluded;
		  }
		  SecurityAppliedMessage securityAppliedMessage = new SecurityAppliedMessage(this.Message, this, signBody, encryptBody);
		  this.Message = (Message) securityAppliedMessage;
		  return (Message) securityAppliedMessage;
#endif
		}
		
		public void RemoveSignatureEncryptionIfAppropriate()
		{
#if FEATURE_CORECLR
		  throw new NotImplementedException("SetupExecution not implemented in .NET Core");
#else
		  if (!this.SignThenEncrypt || !this.EncryptPrimarySignature || this.SecurityAppliedMessage.BodyProtectionMode == MessagePartProtectionMode.SignThenEncrypt || this.basicSupportingTokenParameters != null && this.basicSupportingTokenParameters.Count != 0 || (this.signatureConfirmationsToSend != null && this.signatureConfirmationsToSend.Count != 0 && this.signatureConfirmationsToSend.IsMarkedForEncryption || this.HasSignedEncryptedMessagePart))
			return;
		  this.encryptSignature = false;
#endif
		}
		
		internal void CompleteSecurityApplication()
		{
#if FEATURE_CORECLR
          throw new NotImplementedException("CompleteSecurityApplication is not supported in .NET Core");
#endif
		}
#endregion

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedNamespace
        {
            get { return XD.UtilityDictionary.Namespace; }
        }

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedPrefix
        {
            get { return XD.UtilityDictionary.Prefix; }
        }

        public override string Name
        {
            get { return this.StandardsManager.SecurityVersion.HeaderName.Value; }
        }

        public override string Namespace
        {
            get { return this.StandardsManager.SecurityVersion.HeaderNamespace.Value; }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
        
#region FromWCF
        public System.ServiceModel.Channels.BufferManager StreamBufferManager
        {
          get
          {
            if (this.bufferManager == null)
              this.bufferManager = System.ServiceModel.Channels.BufferManager.CreateBufferManager(0L, int.MaxValue);
            return this.bufferManager;
          }
          set
          {
            this.bufferManager = value;
          }
        }

        public string GenerateId()
        {
		  int idCounter = this.idCounter;
		  this.idCounter = idCounter + 1;
		  int index = idCounter;
		  if (this.idPrefix != null)
			return this.idPrefix + (object) index;
		  if (index < SendSecurityHeader.ids.Length)
			return SendSecurityHeader.ids[index];
		  return "_" + (object) index;
        }

        public string IdPrefix
        {
           get
           {
             return this.idPrefix;
           }
          set
          {
            this.ThrowIfProcessingStarted();
            this.idPrefix = string.IsNullOrEmpty(value) || value == "_" ? (string) null : value;
          }
        }

		private void AddParameters(ref List<SecurityTokenParameters> list, SecurityTokenParameters item)
		{
		  if (list == null)
			list = new List<SecurityTokenParameters>();
		  list.Add(item);
		}
		
        public void AddTimestamp(TimeSpan timestampValidityDuration)
        {
          DateTime utcNow = DateTime.UtcNow;
          string id = this.RequireMessageProtection ? SecurityUtils.GenerateId() : this.GenerateId();
          this.AddTimestamp(new SecurityTimestamp(utcNow, utcNow + timestampValidityDuration, id));
        }

        internal void AddTimestamp(SecurityTimestamp timestamp)
        {
          this.ThrowIfProcessingStarted();
          if (this.elementContainer.Timestamp != null)
            throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("TimestampAlreadySetForSecurityHeader")), this.Message);
          if (timestamp == null)
            throw TraceUtility.ThrowHelperArgumentNull("timestamp", this.Message);
          this.elementContainer.Timestamp = timestamp;
        }
		
		public void AddBasicSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
		{
		  if (token == null)
			throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
		  if (parameters == null)
			throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
		  this.ThrowIfProcessingStarted();
		  this.elementContainer.AddBasicSupportingToken(new SendSecurityHeaderElement(token.Id, (ISecurityElement) new TokenElement(token, this.StandardsManager))
		  {
			MarkedForEncryption = true
		  });
		  this.hasEncryptedTokens = true;
		  this.hasSignedTokens = true;
		  this.AddParameters(ref this.basicSupportingTokenParameters, parameters);
		  if (this.basicTokens == null)
			this.basicTokens = new List<SecurityToken>();
		  this.basicTokens.Add(token);
		}

		public void AddEndorsingSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
		{
		  if (token == null)
			throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
		  if (parameters == null)
			throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
		  this.ThrowIfProcessingStarted();
		  this.elementContainer.AddEndorsingSupportingToken(token);
		  if (!(token is ProviderBackedSecurityToken))
	      {
#if FEATURE_CORECLR
            throw new NotImplementedException("AsymmetricSecurityKey is not supported");
#else
			this.shouldSignToHeader = ((this.shouldSignToHeader ? 1 : 0) | (this.RequireMessageProtection ? 0 : (SecurityUtils.GetSecurityKey<AsymmetricSecurityKey>(token) != null ? 1 : 0))) != 0;
#endif
		  }
		  this.AddParameters(ref this.endorsingTokenParameters, parameters);
		}
		
		public void AddSignedSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
		{
		  if (token == null)
			throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
		  if (parameters == null)
			throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
		  this.ThrowIfProcessingStarted();
		  this.elementContainer.AddSignedSupportingToken(token);
		  this.hasSignedTokens = true;
		  this.AddParameters(ref this.signedTokenParameters, parameters);
		}
		
		public void AddSignedEndorsingSupportingToken(SecurityToken token, SecurityTokenParameters parameters)
		{
		  if (token == null)
			throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
		  if (parameters == null)
			throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameters");
		  this.ThrowIfProcessingStarted();
		  this.elementContainer.AddSignedEndorsingSupportingToken(token);
		  this.hasSignedTokens = true;
#if FEATURE_CORECLR
          throw new NotImplementedException("AsymmetricSecurityKey is not supported");
#else
		  this.shouldSignToHeader = ((this.shouldSignToHeader ? 1 : 0) | (this.RequireMessageProtection ? 0 : (SecurityUtils.GetSecurityKey<AsymmetricSecurityKey>(token) != null ? 1 : 0))) != 0;
		  this.AddParameters(ref this.signedEndorsingTokenParameters, parameters);
#endif
		}
        // Trim for testing...
        /*public MessagePartSpecification SignatureParts
        {
          get
          {
            return this.signatureParts;
          }
          set
          {
            this.ThrowIfProcessingStarted();
            if (value == null)
              throw TraceUtility.ThrowHelperError((Exception) new ArgumentNullException("value"), this.Message);
            if (!value.IsReadOnly)
              throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(System.ServiceModel.SR.GetString("MessagePartSpecificationMustBeImmutable")), this.Message);
            this.signatureParts = value;
          }
        }
        
        public MessagePartSpecification EncryptionParts
        {
          get
          {
            return this.encryptionParts;
          }
          set
          {
            this.ThrowIfProcessingStarted();
            if (value == null)
              throw TraceUtility.ThrowHelperError((Exception) new ArgumentNullException("value"), this.Message);
            if (!value.IsReadOnly)
              throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(System.ServiceModel.SR.GetString("MessagePartSpecificationMustBeImmutable")), this.Message);
            this.encryptionParts = value;
          }
        }*/
#endregion
    }
}
