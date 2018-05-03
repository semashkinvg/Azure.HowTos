 # what's here?

this folder contatins simple HowTos built on MS Tests around scripting, automation and managment of Data Lake solution

# DataLake.HowTo.Debugs

This contains example of the code of calling Azure Data Lake API using number of nuget packages application in Azure Active Directory (AAD)->App registrations

## Configure

The APIs in tests use Service-to-Service authorization. So it's required to have registered app in AAD
In [appSettings.json](https://github.com/semashkinvg/Azure.HowTos/blob/master/DataLake.HowTos/DataLake.HowTo.Debugs/appSettings.json) set parameters as follows:

 - ApplicationId = AAD->Regestered Apps->YourApp->ApplicationId
 - Secret=AAD->Regestered Apps->YourApp->Settings->Keys->Key(value column)
 - Tenant=AAD->Properties->Directory ID
 - Account.Analytics=Name of your Data Lake Analytics service without domain
 - Account.Analytics=Name of your Data Lake Store service WITH domain


To give your application permission to do something in Data Lake Storage and Analytics it's required to add Application Service Principal to Access Control (IAM)

## Possible Issues&Solutions

Here are the list of issues with answers that i've faced and got solved 
1. Azure Search .NET SDK throws InvalidOperationException with SerializationBinder

  https://github.com/Azure/azure-sdk-for-net/issues/3441

2. The client '' with object id '' does not have authorization to perform action 'Microsoft.Authorization/permissions/read' over scope '/subscriptions/aaa/resourceGroups/aaaa/providers/Microsoft.DataLakeAnalytics/accounts/aaa/providers/Microsoft.Authorization'.

  Add Application Service Principal to Access Control (IAM)
 https://stackoverflow.com/questions/42134892/the-client-with-object-id-does-not-have-authorization-to-perform-action-microso
 
 3. The user is not authorized to perform this operation on storage.
 
  https://stackoverflow.com/a/45060604/3623784 
 




