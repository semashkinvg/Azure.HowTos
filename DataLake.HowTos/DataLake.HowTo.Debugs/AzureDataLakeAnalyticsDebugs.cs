using FluentAssertions;
using Microsoft.Azure.Management.DataLake.Analytics;
using Microsoft.Azure.Management.DataLake.Analytics.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;

namespace DataLake.HowTo.Debugs
{
	/// <summary>
	/// issues the i've got to set up this tests:
	/// 
	/// </summary>
	[TestClass]
	public class AzureDataLakeAnalyticsDebugs
	{
		private ServiceClientCredentials _serviceClientCredentials;
		private readonly string _script = @"@a  = 
    SELECT * FROM 
        (VALUES
            (""Contoso"", 1500.0),
            (""Woodgrove"", 2700.0)
        ) AS 
              D( customer, amount );
OUTPUT @a
    TO ""/data.csv""
    USING Outputters.Csv();";
		private readonly string _script_with_parameters = @"
DECLARE EXTERNAL @val double = 1200.00;
@a  = 
    SELECT * FROM 
        (VALUES
            (""Contoso"", @val),
            (""Woodgrove"", 2700.0)
        ) AS 
              D( customer, amount );
OUTPUT @a
    TO ""/data.csv""
    USING Outputters.Csv();";

		[TestInitialize]
		public void SetUp()
		{

			var creds = new ClientCredential(TestsConfiguration.ApplicationId, TestsConfiguration.Secret);
			_serviceClientCredentials = ApplicationTokenProvider.LoginSilentAsync(TestsConfiguration.Tenant, creds).GetAwaiter().GetResult();
		}

		[TestMethod]
		public void create_job()
		{
			var jobClient = new DataLakeAnalyticsJobManagementClient(_serviceClientCredentials);
			var ji = new JobInformation("a job create_job", JobType.USql, new USqlJobProperties(_script));
			var jobInfo = jobClient.Job.CreateWithHttpMessagesAsync(TestsConfiguration.AccountAnalytics, Guid.NewGuid(), ji).GetAwaiter().GetResult();

			jobInfo.Response.IsSuccessStatusCode.Should().BeTrue();
		}

		[TestMethod]
		public void create_job_using_extension()
		{
			var jobClient = new DataLakeAnalyticsJobManagementClient(_serviceClientCredentials);
			var ji = new JobInformation("a job create_job_using_extension", JobType.USql, new USqlJobProperties(_script), Guid.Empty);
			var jobInfo = jobClient.Job.Create(TestsConfiguration.AccountAnalytics, Guid.NewGuid(), ji);
			jobInfo.Should().NotBeNull();
		}

		[TestMethod]
		public void create_job_and_wait()
		{
			var jobClient = new DataLakeAnalyticsJobManagementClient(_serviceClientCredentials);
			var ji = new JobInformation("a job create_job_and_wait", JobType.USql, new USqlJobProperties(_script), Guid.Empty);
			var jobInfo = jobClient.Job.Create(TestsConfiguration.AccountAnalytics, Guid.NewGuid(), ji);
			bool done = false;
			do
			{
				Thread.Sleep(TimeSpan.FromSeconds(3));
				var jobWaitInfo = jobClient.Job.Get(TestsConfiguration.AccountAnalytics, jobInfo.JobId.Value);

				done = jobWaitInfo.State == JobState.Ended;
				Debug.WriteLine($"State: {jobWaitInfo.State?.ToString() ?? "null"}");
			} while (!done);
		}

		[TestMethod, Ignore("no actual implementation")]
		public void create_job_and_pass_parameters()
		{
			var jobClient = new DataLakeAnalyticsJobManagementClient(_serviceClientCredentials);
			var ji = new JobInformation("a job", JobType.USql, new USqlJobProperties(_script_with_parameters));
			// TODO: how to pass parameters marked as external without injection into the script?
			//var jobInfo = jobClient.Job.Create(TestsConfiguration.AccountAnalytics, Guid.NewGuid(), ji);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobId">Set id of executed job within your account</param>
		[TestMethod]
		[DataRow("33b5d125-c534-4eeb-8a4a-5047b3e74f58")]
		public void schedule_job_and_get_status(string jobId)
		{
			var jobClient = new DataLakeAnalyticsJobManagementClient(_serviceClientCredentials);
			var stats = jobClient.Job.GetStatisticsWithHttpMessagesAsync(TestsConfiguration.AccountAnalytics, Guid.Parse(jobId)).GetAwaiter().GetResult();
			stats.Response.IsSuccessStatusCode.Should().BeTrue();
		}
	}
}
