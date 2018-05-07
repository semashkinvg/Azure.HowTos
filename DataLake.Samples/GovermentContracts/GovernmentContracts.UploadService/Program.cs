using Microsoft.Azure.DataLake.Store;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;

namespace GovernmentContracts.UploadService
{
	class Program
	{
		private static void Main(string[] args)
		{
			var creds = new ClientCredential(Configuration.ApplicationId, Configuration.Secret);
			var clientCreds = ApplicationTokenProvider.LoginSilentAsync(Configuration.Tenant, creds).GetAwaiter().GetResult();
			var client = AdlsClient.CreateClient(Configuration.AccountStorage, clientCreds);
			const string inputRemoteFolder = "/mynewfolder/Samarskaja_obl";
			const string localPart =
				@"d:\Development\github\Azure.HowTos\DataLake.Samples\GovermentContracts\Data\Samarskaja_obl\";
			if (!client.CheckExists(inputRemoteFolder))
			{
				client.CreateDirectory(inputRemoteFolder);
			}

			var result = client.BulkUpload(localPart, inputRemoteFolder, shouldOverwrite: IfExists.Overwrite);
		}
	}
}
