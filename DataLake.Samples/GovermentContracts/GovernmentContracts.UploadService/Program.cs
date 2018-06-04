using System.IO;
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
		    var regionFolders =
		        Directory.GetDirectories(@"d:\Development\github\Azure.HowTos\DataLake.Samples\GovermentContracts\Data\");
		    foreach (var regionFolder in regionFolders)
		    {
                var dirInfo = new DirectoryInfo(regionFolder);
		        string inputRemoteFolder = $"/mynewfolder/{dirInfo.Name}";

		        if (!client.CheckExists(inputRemoteFolder))
		        {
		            client.CreateDirectory(inputRemoteFolder);
		        }

		        var result = client.BulkUpload(regionFolder, inputRemoteFolder, shouldOverwrite: IfExists.Overwrite);
            }
		}
	}
}
