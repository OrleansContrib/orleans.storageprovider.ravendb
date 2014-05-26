Orleans StorageProvider for RavenDB
===============================

This is a StorageProvider for RavenDB for the Microsoft Research Project Orleans. The storage provider allows to persist stateful grains into RavenDB. The provider offers the following options:

* Server Mode
* Embedded Mode
* InMemory

In order to use the provider you need to add the following provider to your configuration:

````
<Provider Type="Orleans.StorageProvider.RavenDB.RavenDBStorageProvider" Name="RavenDBStore" ConnectionStringName="RavenDB"/>
````
and this line to any grain that uses it
````
[StorageProvider(ProviderName = "RavenDBStore")]
````
In addition you need an application configuration file (app.config) with a connection string named after the ConnectionStringName in the provider configuration entry.
````
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="RavenDB" connectionString="YOUR CONNECTION STRING HERE"/>
  </connectionStrings>
</configuration>
````
All connection string officially supported by RavenDB are also supported by the provider. The mode (Server, Embedded, InMemory) is chosen based on the connection string according to the following strategy:

* When the connection string is empty the InMemory mode is used
* When the connection string is an Url the Server mode is used
* When the connection string is a DataDir the Embedded mode is used

Here are examples of such connection strings:

````
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="RavenDB" connectionString="Url = http://localhost:8080;Database=RavenDBStorageProviderTests"/>  <!-- Server Mode -->
    <add name="RavenDBLocal" connectionString="DataDir = ~\Data"/> <!-- Embedded Mode -->
  </connectionStrings>
</configuration>	

<Provider Type="Orleans.StorageProvider.RavenDB.RavenDBStorageProvider" Name="RavenDBStore" ConnectionStringName=""/> <!-- InMemory Mode -->

````