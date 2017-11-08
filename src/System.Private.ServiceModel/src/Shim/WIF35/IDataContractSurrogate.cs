#region Assembly System.Runtime.Serialization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.2\System.Runtime.Serialization.dll
#endregion

using System.CodeDom;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Runtime.Serialization
{
    //
    // Summary:
    //     Provides the methods needed to substitute one type for another by the System.Runtime.Serialization.DataContractSerializer
    //     during serialization, deserialization, and export and import of XML schema documents
    //     (XSD).
    public interface IDataContractSurrogate
    {
        //
        // Summary:
        //     During schema export operations, inserts annotations into the schema for non-null
        //     return values.
        //
        // Parameters:
        //   memberInfo:
        //     A System.Reflection.MemberInfo that describes the member.
        //
        //   dataContractType:
        //     A System.Type.
        //
        // Returns:
        //     An object that represents the annotation to be inserted into the XML schema definition.
        object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType);
        //
        // Summary:
        //     During schema export operations, inserts annotations into the schema for non-null
        //     return values.
        //
        // Parameters:
        //   clrType:
        //     The CLR type to be replaced.
        //
        //   dataContractType:
        //     The data contract type to be annotated.
        //
        // Returns:
        //     An object that represents the annotation to be inserted into the XML schema definition.
        object GetCustomDataToExport(Type clrType, Type dataContractType);
        //
        // Summary:
        //     During serialization, deserialization, and schema import and export, returns
        //     a data contract type that substitutes the specified type.
        //
        // Parameters:
        //   type:
        //     The CLR type System.Type to substitute.
        //
        // Returns:
        //     The System.Type to substitute for the type value. This type must be serializable
        //     by the System.Runtime.Serialization.DataContractSerializer. For example, it must
        //     be marked with the System.Runtime.Serialization.DataContractAttribute attribute
        //     or other mechanisms that the serializer recognizes.
        Type GetDataContractType(Type type);
        //
        // Summary:
        //     During deserialization, returns an object that is a substitute for the specified
        //     object.
        //
        // Parameters:
        //   obj:
        //     The deserialized object to be substituted.
        //
        //   targetType:
        //     The System.Type that the substituted object should be assigned to.
        //
        // Returns:
        //     The substituted deserialized object. This object must be of a type that is serializable
        //     by the System.Runtime.Serialization.DataContractSerializer. For example, it must
        //     be marked with the System.Runtime.Serialization.DataContractAttribute attribute
        //     or other mechanisms that the serializer recognizes.
        object GetDeserializedObject(object obj, Type targetType);
        //
        // Summary:
        //     Sets the collection of known types to use for serialization and deserialization
        //     of the custom data objects.
        //
        // Parameters:
        //   customDataTypes:
        //     A System.Collections.ObjectModel.Collection`1 of System.Type to add known types
        //     to.
        void GetKnownCustomDataTypes(Collection<Type> customDataTypes);
        //
        // Summary:
        //     During serialization, returns an object that substitutes the specified object.
        //
        // Parameters:
        //   obj:
        //     The object to substitute.
        //
        //   targetType:
        //     The System.Type that the substituted object should be assigned to.
        //
        // Returns:
        //     The substituted object that will be serialized. The object must be serializable
        //     by the System.Runtime.Serialization.DataContractSerializer. For example, it must
        //     be marked with the System.Runtime.Serialization.DataContractAttribute attribute
        //     or other mechanisms that the serializer recognizes.
        object GetObjectToSerialize(object obj, Type targetType);
        //
        // Summary:
        //     During schema import, returns the type referenced by the schema.
        //
        // Parameters:
        //   typeName:
        //     The name of the type in schema.
        //
        //   typeNamespace:
        //     The namespace of the type in schema.
        //
        //   customData:
        //     The object that represents the annotation inserted into the XML schema definition,
        //     which is data that can be used for finding the referenced type.
        //
        // Returns:
        //     The System.Type to use for the referenced type.
        Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData);
    }
}
