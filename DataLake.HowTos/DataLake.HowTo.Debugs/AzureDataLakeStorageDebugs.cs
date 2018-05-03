using FluentAssertions;
using Microsoft.Azure.DataLake.Store;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLake.HowTo.Debugs
{
	[TestClass]
	public class AzureDataLakeStorageDebugs
	{

		[TestMethod]
		public void authentificate_and_load_directory_info()
		{
			var creds = new ClientCredential(TestsConfiguration.ApplicationId, TestsConfiguration.Secret);
			var clientCreds = ApplicationTokenProvider.LoginSilentAsync(TestsConfiguration.Tenant, creds).GetAwaiter().GetResult();
			var client = AdlsClient.CreateClient(TestsConfiguration.AccountStorage, clientCreds);
			var result = client.GetDirectoryEntry("/mynewfolder");
			result.Should().NotBeNull();
		}
	}
}
