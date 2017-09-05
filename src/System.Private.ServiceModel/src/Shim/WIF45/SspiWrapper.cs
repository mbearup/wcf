// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.SspiWrapper
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.ComponentModel;
using System.ServiceModel;
using System.Runtime.InteropServices;

namespace System.IdentityModel
{
  internal static class SspiWrapper
  {
#if !FEATURE_CORECLR
    private const int SECPKG_FLAG_NEGOTIABLE2 = 2097152;
    private static SecurityPackageInfoClass[] securityPackages;

    public static SecurityPackageInfoClass[] SecurityPackages
    {
      get
      {
        return SspiWrapper.securityPackages;
      }
      set
      {
        SspiWrapper.securityPackages = value;
      }
    }

    private static SecurityPackageInfoClass[] EnumerateSecurityPackages()
    {
      if (SspiWrapper.SecurityPackages != null)
        return SspiWrapper.SecurityPackages;
      int pkgnum = 0;
      SafeFreeContextBuffer pkgArray = (SafeFreeContextBuffer) null;
      try
      {
        int error = SafeFreeContextBuffer.EnumeratePackages(out pkgnum, out pkgArray);
        if (error != 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new Win32Exception(error));
        SecurityPackageInfoClass[] packageInfoClassArray = new SecurityPackageInfoClass[pkgnum];
        for (int index = 0; index < pkgnum; ++index)
          packageInfoClassArray[index] = new SecurityPackageInfoClass((SafeHandle) pkgArray, index);
        SspiWrapper.SecurityPackages = packageInfoClassArray;
      }
      finally
      {
        if (pkgArray != null)
          pkgArray.Close();
      }
      return SspiWrapper.SecurityPackages;
    }

    public static SecurityPackageInfoClass GetVerifyPackageInfo(string packageName)
    {
      SecurityPackageInfoClass[] packageInfoClassArray = SspiWrapper.EnumerateSecurityPackages();
      if (packageInfoClassArray != null)
      {
        for (int index = 0; index < packageInfoClassArray.Length; ++index)
        {
          if (string.Compare(packageInfoClassArray[index].Name, packageName, StringComparison.OrdinalIgnoreCase) == 0)
            return packageInfoClassArray[index];
        }
      }
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException(SR.GetString("SSPIPackageNotSupported", new object[1]{ (object) packageName })));
    }

    public static bool IsNegotiateExPackagePresent()
    {
      SecurityPackageInfoClass[] packageInfoClassArray = SspiWrapper.EnumerateSecurityPackages();
      if (packageInfoClassArray != null)
      {
        int num = 2097152;
        for (int index = 0; index < packageInfoClassArray.Length; ++index)
        {
          if ((packageInfoClassArray[index].Capabilities & num) != 0)
            return true;
        }
      }
      return false;
    }

    public static SafeFreeCredentials AcquireDefaultCredential(string package, CredentialUse intent, params string[] additionalPackages)
    {
      SafeFreeCredentials outCredential = (SafeFreeCredentials) null;
      AuthIdentityEx authIdentity = new AuthIdentityEx((string) null, (string) null, (string) null, additionalPackages);
      int error = SafeFreeCredentials.AcquireDefaultCredential(package, intent, ref authIdentity, out outCredential);
      if (error != 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new Win32Exception(error));
      return outCredential;
    }
#endif

    public static SafeFreeCredentials AcquireCredentialsHandle(string package, CredentialUse intent, ref AuthIdentityEx authdata)
    {
      SafeFreeCredentials outCredential = (SafeFreeCredentials) null;
      int error = SafeFreeCredentials.AcquireCredentialsHandle(package, intent, ref authdata, out outCredential);
      if (error != 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new Win32Exception(error));
      return outCredential;
    }
#if !FEATURE_CORECLR
    public static SafeFreeCredentials AcquireCredentialsHandle(string package, CredentialUse intent, SecureCredential scc)
    {
      SafeFreeCredentials outCredential = (SafeFreeCredentials) null;
      int error = SafeFreeCredentials.AcquireCredentialsHandle(package, intent, ref scc, out outCredential);
      if (error != 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new Win32Exception(error));
      return outCredential;
    }

    public static SafeFreeCredentials AcquireCredentialsHandle(string package, CredentialUse intent, ref IntPtr ppAuthIdentity)
    {
      SafeFreeCredentials outCredential = (SafeFreeCredentials) null;
      int error = SafeFreeCredentials.AcquireCredentialsHandle(package, intent, ref ppAuthIdentity, out outCredential);
      if (error != 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new Win32Exception(error));
      return outCredential;
    }

    internal static int InitializeSecurityContext(SafeFreeCredentials credential, ref SafeDeleteContext context, string targetName, SspiContextFlags inFlags, Endianness datarep, SecurityBuffer inputBuffer, SecurityBuffer outputBuffer, ref SspiContextFlags outFlags)
    {
      return SafeDeleteContext.InitializeSecurityContext(credential, ref context, targetName, inFlags, datarep, inputBuffer, (SecurityBuffer[]) null, outputBuffer, ref outFlags);
    }

    internal static int InitializeSecurityContext(SafeFreeCredentials credential, ref SafeDeleteContext context, string targetName, SspiContextFlags inFlags, Endianness datarep, SecurityBuffer[] inputBuffers, SecurityBuffer outputBuffer, ref SspiContextFlags outFlags)
    {
      return SafeDeleteContext.InitializeSecurityContext(credential, ref context, targetName, inFlags, datarep, (SecurityBuffer) null, inputBuffers, outputBuffer, ref outFlags);
    }

    internal static int AcceptSecurityContext(SafeFreeCredentials credential, ref SafeDeleteContext refContext, SspiContextFlags inFlags, Endianness datarep, SecurityBuffer inputBuffer, SecurityBuffer outputBuffer, ref SspiContextFlags outFlags)
    {
      return SafeDeleteContext.AcceptSecurityContext(credential, ref refContext, inFlags, datarep, inputBuffer, (SecurityBuffer[]) null, outputBuffer, ref outFlags);
    }

    internal static int AcceptSecurityContext(SafeFreeCredentials credential, ref SafeDeleteContext refContext, SspiContextFlags inFlags, Endianness datarep, SecurityBuffer[] inputBuffers, SecurityBuffer outputBuffer, ref SspiContextFlags outFlags)
    {
      return SafeDeleteContext.AcceptSecurityContext(credential, ref refContext, inFlags, datarep, (SecurityBuffer) null, inputBuffers, outputBuffer, ref outFlags);
    }

    public static int QuerySecurityContextToken(SafeDeleteContext context, out SafeCloseHandle token)
    {
      return context.GetSecurityContextToken(out token);
    }

    private static unsafe int QueryContextAttributes(SafeDeleteContext phContext, ContextAttribute attribute, byte[] buffer, Type handleType, out SafeHandle refHandle)
    {
      refHandle = (SafeHandle) null;
      if (handleType != (Type) null)
      {
        if (handleType == typeof (SafeFreeContextBuffer))
          refHandle = (SafeHandle) SafeFreeContextBuffer.CreateEmptyHandle();
        else if (handleType == typeof (SafeFreeCertContext))
          refHandle = (SafeHandle) new SafeFreeCertContext();
        else
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("handleType", SR.GetString("ValueMustBeOf2Types", (object) typeof (SafeFreeContextBuffer).ToString(), (object) typeof (SafeFreeCertContext).ToString())));
      }
      fixed (byte* buffer1 = buffer)
        return SafeFreeContextBuffer.QueryContextAttributes(phContext, attribute, buffer1, refHandle);
    }

    public static unsafe object QueryContextAttributes(SafeDeleteContext securityContext, ContextAttribute contextAttribute)
    {
      int length = IntPtr.Size;
      Type handleType = (Type) null;
      switch (contextAttribute)
      {
        case ContextAttribute.LocalCertificate:
          handleType = typeof (SafeFreeCertContext);
          goto case ContextAttribute.Flags;
        case ContextAttribute.ConnectionInfo:
          length = Marshal.SizeOf(typeof (SslConnectionInfo));
          goto case ContextAttribute.Flags;
        case ContextAttribute.Sizes:
          length = SecSizes.SizeOf;
          goto case ContextAttribute.Flags;
        case ContextAttribute.Names:
          handleType = typeof (SafeFreeContextBuffer);
          goto case ContextAttribute.Flags;
        case ContextAttribute.Lifespan:
          length = LifeSpan_Struct.Size;
          goto case ContextAttribute.Flags;
        case ContextAttribute.StreamSizes:
          length = StreamSizes.SizeOf;
          goto case ContextAttribute.Flags;
        case ContextAttribute.SessionKey:
          handleType = typeof (SafeFreeContextBuffer);
          length = SecPkgContext_SessionKey.Size;
          goto case ContextAttribute.Flags;
        case ContextAttribute.PackageInfo:
          handleType = typeof (SafeFreeContextBuffer);
          goto case ContextAttribute.Flags;
        case ContextAttribute.NegotiationInfo:
          handleType = typeof (SafeFreeContextBuffer);
          length = Marshal.SizeOf(typeof (NegotiationInfo));
          goto case ContextAttribute.Flags;
        case ContextAttribute.Flags:
          SafeHandle refHandle = (SafeHandle) null;
          object obj = (object) null;
          try
          {
            byte[] numArray = new byte[length];
            int error = SspiWrapper.QueryContextAttributes(securityContext, contextAttribute, numArray, handleType, out refHandle);
            if (error != 0)
              throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new Win32Exception(error));
            switch (contextAttribute)
            {
              case ContextAttribute.LocalCertificate:
              case ContextAttribute.RemoteCertificate:
                obj = (object) refHandle;
                refHandle = (SafeHandle) null;
                break;
              case ContextAttribute.ConnectionInfo:
                obj = (object) new SslConnectionInfo(numArray);
                break;
              case ContextAttribute.Sizes:
                obj = (object) new SecSizes(numArray);
                break;
              case ContextAttribute.Names:
                obj = (object) Marshal.PtrToStringUni(refHandle.DangerousGetHandle());
                break;
              case ContextAttribute.Lifespan:
                obj = (object) new LifeSpan(numArray);
                break;
              case ContextAttribute.StreamSizes:
                obj = (object) new StreamSizes(numArray);
                break;
              case ContextAttribute.SessionKey:
                fixed (byte* numPtr = numArray)
                {
                  obj = (object) new SecuritySessionKeyClass(refHandle, Marshal.ReadInt32(new IntPtr((void*) numPtr)));
                  break;
                }
              case ContextAttribute.PackageInfo:
                obj = (object) new SecurityPackageInfoClass(refHandle, 0);
                break;
              case ContextAttribute.NegotiationInfo:
                fixed (byte* numPtr = numArray)
                {
                  obj = (object) new NegotiationInfoClass(refHandle, Marshal.ReadInt32(new IntPtr((void*) numPtr), NegotiationInfo.NegotiationStateOffset));
                  break;
                }
              case ContextAttribute.Flags:
                fixed (byte* numPtr = numArray)
                {
                  obj = (object) Marshal.ReadInt32(new IntPtr((void*) numPtr));
                  break;
                }
            }
          }
          finally
          {
            if (refHandle != null)
              refHandle.Close();
          }
          return obj;
        case ContextAttribute.RemoteCertificate:
          handleType = typeof (SafeFreeCertContext);
          goto case ContextAttribute.Flags;
        default:
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidEnumArgumentException("contextAttribute", (int) contextAttribute, typeof (ContextAttribute)));
      }
    }

    public static int QuerySpecifiedTarget(SafeDeleteContext securityContext, out string specifiedTarget)
    {
      int size = IntPtr.Size;
      Type handleType = typeof (SafeFreeContextBuffer);
      SafeHandle refHandle = (SafeHandle) null;
      specifiedTarget = (string) null;
      int num;
      try
      {
        byte[] buffer = new byte[size];
        num = SspiWrapper.QueryContextAttributes(securityContext, ContextAttribute.SpecifiedTarget, buffer, handleType, out refHandle);
        if (num != 0)
          return num;
        specifiedTarget = Marshal.PtrToStringUni(refHandle.DangerousGetHandle());
      }
      finally
      {
        if (refHandle != null)
          refHandle.Close();
      }
      return num;
    }

    public static void ImpersonateSecurityContext(SafeDeleteContext context)
    {
      int error = SafeDeleteContext.ImpersonateSecurityContext(context);
      if (error != 0)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new Win32Exception(error));
    }

    public static unsafe int EncryptDecryptHelper(SafeDeleteContext context, SecurityBuffer[] input, uint sequenceNumber, bool encrypt, bool isGssBlob)
    {
      SecurityBufferDescriptor inputOutput = new SecurityBufferDescriptor(input.Length);
      SecurityBufferStruct[] securityBufferStructArray = new SecurityBufferStruct[input.Length];
      byte[][] numArray = new byte[input.Length][];
      fixed (SecurityBufferStruct* securityBufferStructPtr = securityBufferStructArray)
      {
        inputOutput.UnmanagedPointer = (void*) securityBufferStructPtr;
        GCHandle[] gcHandleArray = new GCHandle[input.Length];
        try
        {
          for (int index = 0; index < input.Length; ++index)
          {
            SecurityBuffer securityBuffer = input[index];
            securityBufferStructArray[index].count = securityBuffer.size;
            securityBufferStructArray[index].type = securityBuffer.type;
            if (securityBuffer.token == null || securityBuffer.token.Length == 0)
            {
              securityBufferStructArray[index].token = IntPtr.Zero;
            }
            else
            {
              gcHandleArray[index] = GCHandle.Alloc((object) securityBuffer.token, GCHandleType.Pinned);
              securityBufferStructArray[index].token = Marshal.UnsafeAddrOfPinnedArrayElement((Array) securityBuffer.token, securityBuffer.offset);
              numArray[index] = securityBuffer.token;
            }
          }
          int num = !encrypt ? SafeDeleteContext.DecryptMessage(context, inputOutput, sequenceNumber) : SafeDeleteContext.EncryptMessage(context, inputOutput, sequenceNumber);
          for (int index1 = 0; index1 < input.Length; ++index1)
          {
            SecurityBuffer securityBuffer = input[index1];
            securityBuffer.size = securityBufferStructArray[index1].count;
            securityBuffer.type = securityBufferStructArray[index1].type;
            if (securityBuffer.size == 0)
            {
              securityBuffer.offset = 0;
              securityBuffer.token = (byte[]) null;
            }
            else if (isGssBlob && !encrypt && securityBuffer.type == BufferType.Data)
            {
              securityBuffer.token = DiagnosticUtility.Utility.AllocateByteArray(securityBuffer.size);
              Marshal.Copy(securityBufferStructArray[index1].token, securityBuffer.token, 0, securityBuffer.size);
            }
            else
            {
              int index2 = 0;
              while (index2 < input.Length)
              {
                if (numArray[index2] != null)
                {
                  byte* numPtr = (byte*) (void*) Marshal.UnsafeAddrOfPinnedArrayElement((Array) numArray[index2], 0);
                  if ((void*) securityBufferStructArray[index1].token >= numPtr && (UIntPtr) checked ((IntPtr) (void*) securityBufferStructArray[index1].token + securityBuffer.size) <= (UIntPtr) checked ((IntPtr) numPtr + numArray[index2].Length))
                  {
                    securityBuffer.offset = checked ((int) ((byte*) (void*) securityBufferStructArray[index1].token - numPtr));
                    securityBuffer.token = numArray[index2];
                    break;
                  }
                }
                checked { ++index2; }
              }
              if (index2 >= input.Length)
              {
                securityBuffer.size = 0;
                securityBuffer.offset = 0;
                securityBuffer.token = (byte[]) null;
              }
              if (securityBuffer.offset < 0 || securityBuffer.offset > (securityBuffer.token == null ? 0 : securityBuffer.token.Length))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SspiWrapperEncryptDecryptAssert1", new object[1]{ (object) securityBuffer.offset })));
              if (securityBuffer.size < 0 || securityBuffer.size > (securityBuffer.token == null ? 0 : checked (securityBuffer.token.Length - securityBuffer.offset)))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR.GetString("SspiWrapperEncryptDecryptAssert2", new object[1]{ (object) securityBuffer.size })));
            }
          }
          return num;
        }
        finally
        {
          for (int index = 0; index < gcHandleArray.Length; ++index)
          {
            if (gcHandleArray[index].IsAllocated)
              gcHandleArray[index].Free();
          }
        }
      }
    }

    public static int EncryptMessage(SafeDeleteContext context, SecurityBuffer[] input, uint sequenceNumber)
    {
      return SspiWrapper.EncryptDecryptHelper(context, input, sequenceNumber, true, false);
    }

    public static int DecryptMessage(SafeDeleteContext context, SecurityBuffer[] input, uint sequenceNumber, bool isGssBlob)
    {
      return SspiWrapper.EncryptDecryptHelper(context, input, sequenceNumber, false, isGssBlob);
    }

    public static uint SspiPromptForCredential(string targetName, string packageName, out IntPtr ppAuthIdentity, ref bool saveCredentials)
    {
      return NativeMethods.SspiPromptForCredentials(targetName, ref new CREDUI_INFO() { cbSize = Marshal.SizeOf(typeof (CREDUI_INFO)), pszCaptionText = SR.GetString("SspiLoginPromptHeaderMessage"), pszMessageText = "" }, 0U, packageName, IntPtr.Zero, out ppAuthIdentity, ref saveCredentials, 0U);
    }

    public static bool IsSspiPromptingNeeded(uint ErrorOrNtStatus)
    {
      return NativeMethods.SspiIsPromptingNeeded(ErrorOrNtStatus);
    }
#endif
  }
}
