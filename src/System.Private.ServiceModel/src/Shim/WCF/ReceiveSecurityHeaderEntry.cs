// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.ReceiveSecurityHeaderEntry
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

namespace System.ServiceModel.Security
{
  internal struct ReceiveSecurityHeaderEntry
  {
    internal ReceiveSecurityHeaderElementCategory elementCategory;
    internal object element;
    internal ReceiveSecurityHeaderBindingModes bindingMode;
    internal string id;
    internal string encryptedFormId;
    internal string encryptedFormWsuId;
    internal bool signed;
    internal bool encrypted;
    internal byte[] decryptedBuffer;
    internal TokenTracker supportingTokenTracker;
    internal bool doubleEncrypted;

    public bool MatchesId(string id, bool requiresEncryptedFormId)
    {
      if (this.doubleEncrypted)
      {
        if (!(this.encryptedFormId == id))
          return this.encryptedFormWsuId == id;
        return true;
      }
      if (requiresEncryptedFormId)
        return this.encryptedFormId == id;
      return this.id == id;
    }

    public void PreserveIdBeforeDecryption()
    {
      this.encryptedFormId = this.id;
    }

    public void SetElement(ReceiveSecurityHeaderElementCategory elementCategory, object element, ReceiveSecurityHeaderBindingModes bindingMode, string id, bool encrypted, byte[] decryptedBuffer, TokenTracker supportingTokenTracker)
    {
      this.elementCategory = elementCategory;
      this.element = element;
      this.bindingMode = bindingMode;
      this.encrypted = encrypted;
      this.decryptedBuffer = decryptedBuffer;
      this.supportingTokenTracker = supportingTokenTracker;
      this.id = id;
    }
  }
}
