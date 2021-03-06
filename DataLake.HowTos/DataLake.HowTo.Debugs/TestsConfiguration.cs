﻿using Microsoft.Extensions.Configuration;
using System.IO;

namespace DataLake.HowTo.Debugs
{
	public static class TestsConfiguration
	{
		public static readonly string ApplicationId;
		public static readonly string Secret;
		public static readonly string Tenant;
		public static readonly string AccountAnalytics;
		public static readonly string AccountStorage;

		static TestsConfiguration()
		{
			// it needs to get errors into tests output
			NLog.LogManager.LoadConfiguration("nlog.config");

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json");
			var configuration = builder.Build();

			ApplicationId = configuration["ApplicationId"];
			Secret = configuration["Secret"];
			Tenant = configuration["Tenant"];
			AccountAnalytics = configuration["Account:Analytics"];
			AccountStorage = configuration["Account:Storage"];
		}
	}
}
