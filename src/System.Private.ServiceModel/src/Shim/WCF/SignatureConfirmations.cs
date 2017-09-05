// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SignatureConfirmations
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

namespace System.ServiceModel.Security
{
  public class SignatureConfirmations
  {
    private SignatureConfirmations.SignatureConfirmation[] confirmations;
    private int length;
    private bool encrypted;

    public int Count
    {
      get
      {
        return this.length;
      }
    }

    public bool IsMarkedForEncryption
    {
      get
      {
        return this.encrypted;
      }
    }

    public SignatureConfirmations()
    {
      this.confirmations = new SignatureConfirmations.SignatureConfirmation[1];
      this.length = 0;
    }

    public void AddConfirmation(byte[] value, bool encrypted)
    {
      if (this.confirmations.Length == this.length)
      {
        SignatureConfirmations.SignatureConfirmation[] signatureConfirmationArray = new SignatureConfirmations.SignatureConfirmation[this.length * 2];
        Array.Copy((Array) this.confirmations, 0, (Array) signatureConfirmationArray, 0, this.length);
        this.confirmations = signatureConfirmationArray;
      }
      this.confirmations[this.length] = new SignatureConfirmations.SignatureConfirmation(value);
      this.length = this.length + 1;
      this.encrypted = this.encrypted | encrypted;
    }

    public void GetConfirmation(int index, out byte[] value, out bool encrypted)
    {
      if (index < 0 || index >= this.length)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("index", SR.GetString("ValueMustBeInRange", (object) 0, (object) this.length)));
      value = this.confirmations[index].value;
      encrypted = this.encrypted;
    }

    private struct SignatureConfirmation
    {
      public byte[] value;

      public SignatureConfirmation(byte[] value)
      {
        this.value = value;
      }
    }
  }
}
