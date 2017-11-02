// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.ReceiveSecurityHeaderElementManager
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.Xml;

namespace System.ServiceModel.Security
{
  internal sealed class ReceiveSecurityHeaderElementManager : ISignatureReaderProvider
  {
    private const int InitialCapacity = 8;
    private readonly ReceiveSecurityHeader securityHeader;
    private ReceiveSecurityHeaderEntry[] elements;
    private int count;
    private readonly string[] headerIds;
    private string[] predecryptionHeaderIds;
    private string bodyId;
    private string bodyContentId;
    private bool isPrimaryTokenSigned;

    public int Count
    {
      get
      {
        return this.count;
      }
    }

    public bool IsPrimaryTokenSigned
    {
      get
      {
        return this.isPrimaryTokenSigned;
      }
      set
      {
        this.isPrimaryTokenSigned = value;
      }
    }

    public ReceiveSecurityHeaderElementManager(ReceiveSecurityHeader securityHeader)
    {
      this.securityHeader = securityHeader;
      this.elements = new ReceiveSecurityHeaderEntry[8];
      if (!securityHeader.RequireMessageProtection)
        return;
      this.headerIds = new string[securityHeader.ProcessedMessage.Headers.Count];
    }

    public void AppendElement(ReceiveSecurityHeaderElementCategory elementCategory, object element, ReceiveSecurityHeaderBindingModes bindingMode, string id, TokenTracker supportingTokenTracker)
    {
      if (id != null)
        this.VerifyIdUniquenessInSecurityHeader(id);
      this.EnsureCapacityToAdd();
      ReceiveSecurityHeaderEntry[] elements = this.elements;
      int count = this.count;
      this.count = count + 1;
      int index = count;
      elements[index].SetElement(elementCategory, element, bindingMode, id, false, (byte[]) null, supportingTokenTracker);
    }

    public void AppendSignature(SignedXml signedXml)
    {
      this.AppendElement(ReceiveSecurityHeaderElementCategory.Signature, (object) signedXml, ReceiveSecurityHeaderBindingModes.Unknown, signedXml.Id, (TokenTracker) null);
    }

    public void AppendReferenceList(ReferenceList referenceList)
    {
      this.AppendElement(ReceiveSecurityHeaderElementCategory.ReferenceList, (object) referenceList, ReceiveSecurityHeaderBindingModes.Unknown, (string) null, (TokenTracker) null);
    }

    public void AppendEncryptedData(EncryptedData encryptedData)
    {
      this.AppendElement(ReceiveSecurityHeaderElementCategory.EncryptedData, (object) encryptedData, ReceiveSecurityHeaderBindingModes.Unknown, encryptedData.Id, (TokenTracker) null);
    }

    public void AppendSignatureConfirmation(ISignatureValueSecurityElement signatureConfirmationElement)
    {
      this.AppendElement(ReceiveSecurityHeaderElementCategory.SignatureConfirmation, (object) signatureConfirmationElement, ReceiveSecurityHeaderBindingModes.Unknown, signatureConfirmationElement.Id, (TokenTracker) null);
    }

    public void AppendTimestamp(SecurityTimestamp timestamp)
    {
      this.AppendElement(ReceiveSecurityHeaderElementCategory.Timestamp, (object) timestamp, ReceiveSecurityHeaderBindingModes.Unknown, timestamp.Id, (TokenTracker) null);
    }

    public void AppendSecurityTokenReference(SecurityKeyIdentifierClause strClause, string strId)
    {
      if (string.IsNullOrEmpty(strId))
        return;
      this.VerifyIdUniquenessInSecurityHeader(strId);
      this.AppendElement(ReceiveSecurityHeaderElementCategory.SecurityTokenReference, (object) strClause, ReceiveSecurityHeaderBindingModes.Unknown, strId, (TokenTracker) null);
    }

    public void AppendToken(SecurityToken token, ReceiveSecurityHeaderBindingModes mode, TokenTracker supportingTokenTracker)
    {
      this.AppendElement(ReceiveSecurityHeaderElementCategory.Token, (object) token, mode, token.Id, supportingTokenTracker);
    }

    public void EnsureAllRequiredSecurityHeaderTargetsWereProtected()
    {
      for (int index = 0; index < this.count; ++index)
      {
        ReceiveSecurityHeaderEntry element;
        this.GetElementEntry(index, out element);
        if (!element.signed)
        {
          switch (element.elementCategory)
          {
            case ReceiveSecurityHeaderElementCategory.SignatureConfirmation:
            case ReceiveSecurityHeaderElementCategory.Timestamp:
              throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("RequiredSecurityHeaderElementNotSigned", (object) element.elementCategory, (object) element.id)));
            case ReceiveSecurityHeaderElementCategory.Token:
              switch (element.bindingMode)
              {
                case ReceiveSecurityHeaderBindingModes.Signed:
                case ReceiveSecurityHeaderBindingModes.SignedEndorsing:
                case ReceiveSecurityHeaderBindingModes.Basic:
                  throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("RequiredSecurityTokenNotSigned", element.element, (object) element.bindingMode)));
              }
              break;
          }
        }
        if (!element.encrypted && element.elementCategory == ReceiveSecurityHeaderElementCategory.Token && element.bindingMode == ReceiveSecurityHeaderBindingModes.Basic)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("RequiredSecurityTokenNotEncrypted", element.element, (object) element.bindingMode)));
      }
    }

    private void EnsureCapacityToAdd()
    {
      if (this.count != this.elements.Length)
        return;
      ReceiveSecurityHeaderEntry[] securityHeaderEntryArray = new ReceiveSecurityHeaderEntry[this.elements.Length * 2];
      Array.Copy((Array) this.elements, 0, (Array) securityHeaderEntryArray, 0, this.count);
      this.elements = securityHeaderEntryArray;
    }

    public object GetElement(int index)
    {
      return this.elements[index].element;
    }

    public T GetElement<T>(int index) where T : class
    {
      return (T) this.elements[index].element;
    }

    public void GetElementEntry(int index, out ReceiveSecurityHeaderEntry element)
    {
      element = this.elements[index];
    }

    public ReceiveSecurityHeaderElementCategory GetElementCategory(int index)
    {
      return this.elements[index].elementCategory;
    }

    public void GetPrimarySignature(out XmlDictionaryReader reader, out string id)
    {
      for (int index = 0; index < this.count; ++index)
      {
        ReceiveSecurityHeaderEntry element;
        this.GetElementEntry(index, out element);
        if (element.elementCategory == ReceiveSecurityHeaderElementCategory.Signature && element.bindingMode == ReceiveSecurityHeaderBindingModes.Primary)
        {
          reader = this.GetReader(index, false);
          id = element.id;
          return;
        }
      }
      reader = (XmlDictionaryReader) null;
      id = (string) null;
    }

    internal XmlDictionaryReader GetReader(int index, bool requiresEncryptedFormReader)
    {
      if (!requiresEncryptedFormReader)
      {
        byte[] decryptedBuffer = this.elements[index].decryptedBuffer;
        if (decryptedBuffer != null)
          return this.securityHeader.CreateDecryptedReader(decryptedBuffer);
      }
      XmlDictionaryReader securityHeaderReader = this.securityHeader.CreateSecurityHeaderReader();
      securityHeaderReader.ReadStartElement();
      for (int index1 = 0; securityHeaderReader.IsStartElement() && index1 < index; ++index1)
        securityHeaderReader.Skip();
      return securityHeaderReader;
    }

    public XmlDictionaryReader GetSignatureVerificationReader(string id, bool requiresEncryptedFormReaderIfDecrypted)
    {
      for (int index = 0; index < this.count; ++index)
      {
        ReceiveSecurityHeaderEntry element;
        this.GetElementEntry(index, out element);
        bool flag1 = element.encrypted & requiresEncryptedFormReaderIfDecrypted;
        bool flag2 = element.bindingMode == ReceiveSecurityHeaderBindingModes.Signed || element.bindingMode == ReceiveSecurityHeaderBindingModes.SignedEndorsing;
        if (element.MatchesId(id, flag1))
        {
          this.SetSigned(index);
          if (!this.IsPrimaryTokenSigned)
            this.IsPrimaryTokenSigned = element.bindingMode == ReceiveSecurityHeaderBindingModes.Primary && element.elementCategory == ReceiveSecurityHeaderElementCategory.Token;
          return this.GetReader(index, flag1);
        }
        if (element.MatchesId(id, flag2))
        {
          this.SetSigned(index);
          if (!this.IsPrimaryTokenSigned)
            this.IsPrimaryTokenSigned = element.bindingMode == ReceiveSecurityHeaderBindingModes.Primary && element.elementCategory == ReceiveSecurityHeaderElementCategory.Token;
          return this.GetReader(index, flag2);
        }
      }
      return (XmlDictionaryReader) null;
    }

    private void OnDuplicateId(string id)
    {
      throw TraceUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("DuplicateIdInMessageToBeVerified", new object[1]{ (object) id })), (Message) this.securityHeader.SecurityVerifiedMessage);
    }

    public void SetBindingMode(int index, ReceiveSecurityHeaderBindingModes bindingMode)
    {
      this.elements[index].bindingMode = bindingMode;
    }

    public void SetElement(int index, object element)
    {
      this.elements[index].element = element;
    }

    public void ReplaceHeaderEntry(int index, ReceiveSecurityHeaderEntry element)
    {
      this.elements[index] = element;
    }

    public void SetElementAfterDecryption(int index, ReceiveSecurityHeaderElementCategory elementCategory, object element, ReceiveSecurityHeaderBindingModes bindingMode, string id, byte[] decryptedBuffer, TokenTracker supportingTokenTracker)
    {
      if (id != null)
        this.VerifyIdUniquenessInSecurityHeader(id);
      this.elements[index].PreserveIdBeforeDecryption();
      this.elements[index].SetElement(elementCategory, element, bindingMode, id, true, decryptedBuffer, supportingTokenTracker);
    }

    public void SetSignatureAfterDecryption(int index, SignedXml signedXml, byte[] decryptedBuffer)
    {
      this.SetElementAfterDecryption(index, ReceiveSecurityHeaderElementCategory.Signature, (object) signedXml, ReceiveSecurityHeaderBindingModes.Unknown, signedXml.Id, decryptedBuffer, (TokenTracker) null);
    }

    public void SetSignatureConfirmationAfterDecryption(int index, ISignatureValueSecurityElement signatureConfirmationElement, byte[] decryptedBuffer)
    {
      this.SetElementAfterDecryption(index, ReceiveSecurityHeaderElementCategory.SignatureConfirmation, (object) signatureConfirmationElement, ReceiveSecurityHeaderBindingModes.Unknown, signatureConfirmationElement.Id, decryptedBuffer, (TokenTracker) null);
    }

    internal void SetSigned(int index)
    {
      this.elements[index].signed = true;
      if (this.elements[index].supportingTokenTracker == null)
        return;
      this.elements[index].supportingTokenTracker.IsSigned = true;
    }

    public void SetTimestampSigned(string id)
    {
      for (int index = 0; index < this.count; ++index)
      {
        if (this.elements[index].elementCategory == ReceiveSecurityHeaderElementCategory.Timestamp && this.elements[index].id == id)
          this.SetSigned(index);
      }
    }

    public void SetTokenAfterDecryption(int index, SecurityToken token, ReceiveSecurityHeaderBindingModes mode, byte[] decryptedBuffer, TokenTracker supportingTokenTracker)
    {
      this.SetElementAfterDecryption(index, ReceiveSecurityHeaderElementCategory.Token, (object) token, mode, token.Id, decryptedBuffer, supportingTokenTracker);
    }

    internal bool TryGetTokenElementIndexFromStrId(string strId, out int index)
    {
      index = -1;
      SecurityKeyIdentifierClause keyIdentifierClause = (SecurityKeyIdentifierClause) null;
      for (int index1 = 0; index1 < this.Count; ++index1)
      {
        if (this.GetElementCategory(index1) == ReceiveSecurityHeaderElementCategory.SecurityTokenReference)
        {
          keyIdentifierClause = this.GetElement(index1) as SecurityKeyIdentifierClause;
          if (keyIdentifierClause.Id == strId)
            break;
        }
      }
      if (keyIdentifierClause == null)
        return false;
      for (int index1 = 0; index1 < this.Count; ++index1)
      {
        if (this.GetElementCategory(index1) == ReceiveSecurityHeaderElementCategory.Token && (this.GetElement(index1) as SecurityToken).MatchesKeyIdentifierClause(keyIdentifierClause))
        {
          index = index1;
          return true;
        }
      }
      return false;
    }

    public void VerifyUniquenessAndSetBodyId(string id)
    {
      if (id == null)
        return;
      this.VerifyIdUniquenessInSecurityHeader(id);
      this.VerifyIdUniquenessInMessageHeadersAndBody(id, this.headerIds.Length);
      this.bodyId = id;
    }

    public void VerifyUniquenessAndSetBodyContentId(string id)
    {
      if (id == null)
        return;
      this.VerifyIdUniquenessInSecurityHeader(id);
      this.VerifyIdUniquenessInMessageHeadersAndBody(id, this.headerIds.Length);
      this.bodyContentId = id;
    }

    public void VerifyUniquenessAndSetDecryptedHeaderId(string id, int headerIndex)
    {
      if (id == null)
        return;
      this.VerifyIdUniquenessInSecurityHeader(id);
      this.VerifyIdUniquenessInMessageHeadersAndBody(id, headerIndex);
      if (this.predecryptionHeaderIds == null)
        this.predecryptionHeaderIds = new string[this.headerIds.Length];
      this.predecryptionHeaderIds[headerIndex] = this.headerIds[headerIndex];
      this.headerIds[headerIndex] = id;
    }

    public void VerifyUniquenessAndSetHeaderId(string id, int headerIndex)
    {
      if (id == null)
        return;
      this.VerifyIdUniquenessInSecurityHeader(id);
      this.VerifyIdUniquenessInMessageHeadersAndBody(id, headerIndex);
      this.headerIds[headerIndex] = id;
    }

    private void VerifyIdUniquenessInHeaderIdTable(string id, int headerCount, string[] headerIdTable)
    {
      for (int index = 0; index < headerCount; ++index)
      {
        if (headerIdTable[index] == id)
          this.OnDuplicateId(id);
      }
    }

    private void VerifyIdUniquenessInSecurityHeader(string id)
    {
      for (int index = 0; index < this.count; ++index)
      {
        if (this.elements[index].id == id || this.elements[index].encryptedFormId == id)
          this.OnDuplicateId(id);
      }
    }

    private void VerifyIdUniquenessInMessageHeadersAndBody(string id, int headerCount)
    {
      this.VerifyIdUniquenessInHeaderIdTable(id, headerCount, this.headerIds);
      if (this.predecryptionHeaderIds != null)
        this.VerifyIdUniquenessInHeaderIdTable(id, headerCount, this.predecryptionHeaderIds);
      if (!(this.bodyId == id) && !(this.bodyContentId == id))
        return;
      this.OnDuplicateId(id);
    }

    XmlDictionaryReader ISignatureReaderProvider.GetReader(object callbackContext)
    {
      return this.GetReader((int) callbackContext, false);
    }

    public void VerifySignatureConfirmationWasFound()
    {
      for (int index = 0; index < this.count; ++index)
      {
        ReceiveSecurityHeaderEntry element;
        this.GetElementEntry(index, out element);
        if (element.elementCategory == ReceiveSecurityHeaderElementCategory.SignatureConfirmation)
          return;
      }
      throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new MessageSecurityException(SR.GetString("SignatureConfirmationWasExpected")));
    }
  }
}
