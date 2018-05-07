using FluentAssertions;
using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Types.Sql;
using Microsoft.Analytics.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ItWasMe.Azure.DataLake.Udo.Tests
{
	[TestClass]
	public class XmlExtractorTests
	{

		[TestMethod]
		public void should_parse_simple_xml()
		{
			const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?><contract schemeVersion=""1.0""><id>13354543</id><regNum>0176100000714000001</regNum><number>01-5-7349/14</number><publishDate>2014-01-11T10:59:49Z</publishDate><signDate>2013-12-31</signDate><versionNumber>0</versionNumber></contract>";
			var mappings =
				new SqlMap<string, string>(new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("id", "Id"),
					new KeyValuePair<string, string>("regNum", "RegistrationNumber")
				});

			var result = ExecuteExtract(xml, "contract", mappings, new USqlSchema(new USqlColumn<string>("Id"), new USqlColumn<string>("RegistrationNumber")));

			Assert.IsTrue(result[0].Get<string>("Id") == "13354543");
			Assert.IsTrue(result[0].Get<string>("RegistrationNumber") == "0176100000714000001");
		}
		[TestMethod]
		public void should_parse_xml_with_namespaces()
		{
			const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<export xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://zakupki.gov.ru/oos/export/1"" xmlns:oos=""http://zakupki.gov.ru/oos/types/1"">
  <contract schemeVersion=""1.0"">
    <oos:id>13354543</oos:id>
    <oos:regNum>0176100000714000001</oos:regNum>
    <oos:number>01-5-7349/14</oos:number>
    <oos:publishDate>2014-01-11T10:59:49Z</oos:publishDate>
    <oos:signDate>2013-12-31</oos:signDate>
    <oos:versionNumber>0</oos:versionNumber>
  </contract>
</export>";
			var mappings =
				new SqlMap<string, string>(new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("oos:id", "Id"),
					new KeyValuePair<string, string>("oos:regNum", "RegistrationNumber")
				});

			var nameSpaces = new SqlMap<string, string>(new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string,string>("oos","http://zakupki.gov.ru/oos/types/1" ),
				new KeyValuePair<string,string>("x","http://zakupki.gov.ru/oos/export/1" )
			});

			var result = ExecuteExtract(xml, "/x:export/x:contract", mappings,
				new USqlSchema(new USqlColumn<string>("Id"), new USqlColumn<string>("RegistrationNumber")), nameSpaces);

			Assert.IsTrue(result[0].Get<string>("Id") == "13354543");
			Assert.IsTrue(result[0].Get<string>("RegistrationNumber") == "0176100000714000001");
		}

		[TestMethod]
		[DeploymentItem(@"Input\contract.xml", "Input")]
		public void should_parse_real_xml()
		{
			var xml = File.ReadAllText(@"Input\contract.xml");
			var mappings =
				new SqlMap<string, string>(new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("oos:id", "Id"),
					new KeyValuePair<string, string>("oos:regNum", "RegistrationNumber"),
				});

			var nameSpaces = new SqlMap<string, string>(new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string,string>("oos","http://zakupki.gov.ru/oos/types/1" ),
				new KeyValuePair<string,string>("x","http://zakupki.gov.ru/oos/export/1" )
			});

			var result = ExecuteExtract(xml, "/x:export/x:contract", mappings,
				new USqlSchema(new USqlColumn<string>("Id"),
					new USqlColumn<string>("RegistrationNumber"),
					new USqlColumn<string>("Products"),
					new USqlColumn<string>("Suppliers")), nameSpaces);

			result[0].Get<string>("Id").Should().Be("13354543");
			result[0].Get<string>("RegistrationNumber").Should().Be("0176100000714000001");
		}

		[TestMethod]
		[DeploymentItem(@"Input\contractProducts.xml", "Input")]
		public void should_select_items_as_sql_map()
		{
			var xml = File.ReadAllText(@"Input\contractProducts.xml");
			var mappings =
				new SqlMap<string, string>(new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("id", "Id"),
					new KeyValuePair<string, string>("regNum", "RegistrationNumber"),
					new KeyValuePair<string, string>("products", "Products"),
				});

			var result = ExecuteExtract(xml, "//contract", mappings,
				new USqlSchema(new USqlColumn<string>("Id"),
					new USqlColumn<string>("RegistrationNumber"),
					new USqlColumn<SqlArray<string>>("Products")));

			result[0].Get<string>("Id").Should().Be("17386335");
			result[0].Get<string>("RegistrationNumber").Should().Be("0342100039714000078");
			result[0].Get<SqlArray<string>>("Products").Count.Should().BeGreaterThan(1);
		}

		private IList<IRow> ExecuteExtract(string xml, string rowPath, SqlMap<string, string> columnMappings, USqlSchema schema, SqlMap<string, string> namespaces = null)
		{
			using (var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
			{
				var reader = new USqlStreamReader(dataStream);
				var extractor = new XmlExtractor(rowPath, columnMappings, namespaces);
				return extractor.Extract(reader, new USqlRow(schema, null).AsUpdatable()).ToList();
			}
		}

	}
}
