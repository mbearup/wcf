// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.Xml;

namespace System.ServiceModel.Security
{
    public abstract class SendSecurityHeader : SecurityHeader, IMessageHeaderWithSharedNamespace
    {
        private System.ServiceModel.Channels.BufferManager bufferManager;
        private string idPrefix;
        protected SendSecurityHeader(Message message, string actor, bool mustUnderstand, bool relay,
            SecurityStandardsManager standardsManager,
            SecurityAlgorithmSuite algorithmSuite,
            MessageDirection transferDirection)
            : base(message, actor, mustUnderstand, relay, standardsManager, algorithmSuite, transferDirection)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

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
          throw new NotImplementedException("SendSecurityHeader is not supported in .NET Core");
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

        public void AddTimestamp(TimeSpan timestampValidityDuration)
        {
          DateTime utcNow = DateTime.UtcNow;
          string id = this.RequireMessageProtection ? SecurityUtils.GenerateId() : this.GenerateId();
          this.AddTimestamp(new SecurityTimestamp(utcNow, utcNow + timestampValidityDuration, id));
        }

        internal void AddTimestamp(SecurityTimestamp timestamp)
        {
          this.ThrowIfProcessingStarted();
#if FEATURE_CORECLR
          throw new NotImplementedException("SecurityProtocol.elementContainer is not supported in .NET Core");
#else
          if (this.elementContainer.Timestamp != null)
            throw TraceUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("TimestampAlreadySetForSecurityHeader")), this.Message);
          if (timestamp == null)
            throw TraceUtility.ThrowHelperArgumentNull("timestamp", this.Message);
          this.elementContainer.Timestamp = timestamp;
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
