<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="ff377002-863a-4438-98ff-4b52c04d8e02" namespace="ObjectQL.ORM.ConfigSection" xmlSchemaNamespace="urn:ObjectQL.ORM.ConfigSection" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="ObjectQLSection" namespace="ObjectQL" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="objectQL">
      <elementProperties>
        <elementProperty name="drivers" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="drivers" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/ff377002-863a-4438-98ff-4b52c04d8e02/DriverAccessCollection" />
          </type>
        </elementProperty>
        <elementProperty name="containers" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="containers" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/ff377002-863a-4438-98ff-4b52c04d8e02/MapContainerCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="DriverAccessCollection" namespace="ObjectQL" xmlItemName="add" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/ff377002-863a-4438-98ff-4b52c04d8e02/DriverAccess" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="DriverAccess" namespace="ObjectQL">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/ff377002-863a-4438-98ff-4b52c04d8e02/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Provider" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="provider" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/ff377002-863a-4438-98ff-4b52c04d8e02/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElement name="MapRegisterSetting" namespace="ObjectQL">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/ff377002-863a-4438-98ff-4b52c04d8e02/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="MapContainer" namespace="ObjectQL" xmlItemName="register" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <attributeProperties>
        <attributeProperty name="ConnectionKey" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="connectionKey" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/ff377002-863a-4438-98ff-4b52c04d8e02/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Schema" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="schema" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/ff377002-863a-4438-98ff-4b52c04d8e02/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="BuildingPolicy" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="buildingPolicy" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/ff377002-863a-4438-98ff-4b52c04d8e02/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <itemType>
        <configurationElementMoniker name="/ff377002-863a-4438-98ff-4b52c04d8e02/MapRegisterSetting" />
      </itemType>
    </configurationElementCollection>
    <configurationElementCollection name="MapContainerCollection" xmlItemName="mapContainer" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementCollectionMoniker name="/ff377002-863a-4438-98ff-4b52c04d8e02/MapContainer" />
      </itemType>
    </configurationElementCollection>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>