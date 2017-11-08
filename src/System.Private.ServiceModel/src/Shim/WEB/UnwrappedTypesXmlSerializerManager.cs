// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Dispatcher.UnwrappedTypesXmlSerializerManager
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace System.ServiceModel.Dispatcher
{
  internal class UnwrappedTypesXmlSerializerManager
  {
    private Dictionary<Type, XmlTypeMapping> allTypes;
    private XmlReflectionImporter importer;
    private Dictionary<object, IList<Type>> operationTypes;
    private bool serializersCreated;
    private Dictionary<Type, XmlSerializer> serializersMap;
    private object thisLock;

    public UnwrappedTypesXmlSerializerManager()
    {
      this.allTypes = new Dictionary<Type, XmlTypeMapping>();
      this.serializersMap = new Dictionary<Type, XmlSerializer>();
      this.operationTypes = new Dictionary<object, IList<Type>>();
      this.importer = new XmlReflectionImporter();
      this.thisLock = new object();
    }

    public UnwrappedTypesXmlSerializerManager.TypeSerializerPair[] GetOperationSerializers(object key)
    {
      if (key == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("key");
      lock (this.thisLock)
      {
        if (!this.serializersCreated)
        {
          this.BuildSerializers();
          this.serializersCreated = true;
        }
        List<UnwrappedTypesXmlSerializerManager.TypeSerializerPair> typeSerializerPairList = new List<UnwrappedTypesXmlSerializerManager.TypeSerializerPair>();
        IList<Type> operationType = this.operationTypes[key];
        for (int index = 0; index < operationType.Count; ++index)
          typeSerializerPairList.Add(new UnwrappedTypesXmlSerializerManager.TypeSerializerPair()
          {
            Type = operationType[index],
            Serializer = (XmlObjectSerializer) new UnwrappedTypesXmlSerializerManager.XmlSerializerXmlObjectSerializer(this.serializersMap[operationType[index]])
          });
        return typeSerializerPairList.ToArray();
      }
    }

    public void RegisterType(object key, IList<Type> types)
    {
      if (key == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("key");
      if (types == null)
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("types");
      lock (this.thisLock)
      {
        if (this.serializersCreated)
        {
          // ISSUE: reference to a compiler-generated method
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(SR2.GetString(SR2.XmlSerializersCreatedBeforeRegistration)));
        }
        for (int index = 0; index < types.Count; ++index)
        {
          if (!this.allTypes.ContainsKey(types[index]))
            this.allTypes.Add(types[index], this.importer.ImportTypeMapping(types[index]));
        }
        this.operationTypes.Add(key, types);
      }
    }

    private void BuildSerializers()
    {
      List<Type> typeList = new List<Type>();
      List<XmlMapping> xmlMappingList = new List<XmlMapping>();
      foreach (Type key in this.allTypes.Keys)
      {
        XmlTypeMapping allType = this.allTypes[key];
        typeList.Add(key);
        xmlMappingList.Add((XmlMapping) allType);
      }
      XmlSerializer[] xmlSerializerArray = XmlSerializer.FromMappings(xmlMappingList.ToArray());
      for (int index = 0; index < typeList.Count; ++index)
        this.serializersMap.Add(typeList[index], xmlSerializerArray[index]);
    }

    public struct TypeSerializerPair
    {
      public XmlObjectSerializer Serializer;
      public Type Type;
    }

    private class XmlSerializerXmlObjectSerializer : XmlObjectSerializer
    {
      private XmlSerializer serializer;

      public XmlSerializerXmlObjectSerializer(XmlSerializer serializer)
      {
        if (serializer == null)
          throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serializer");
        this.serializer = serializer;
      }

      public override bool IsStartObject(XmlDictionaryReader reader)
      {
        return this.serializer.CanDeserialize((XmlReader) reader);
      }

      public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
      {
        return this.serializer.Deserialize((XmlReader) reader);
      }

      public override void WriteEndObject(XmlDictionaryWriter writer)
      {
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
      }

      public override void WriteObject(XmlDictionaryWriter writer, object graph)
      {
        this.serializer.Serialize((XmlWriter) writer, graph);
      }

      public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
      {
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
      }

      public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
      {
        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
      }
    }
  }
}
