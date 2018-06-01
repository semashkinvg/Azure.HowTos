<Query Kind="Statements" />

var resps = Directory.GetDirectories(@"d:\Projects\GOV_ZKP\Data\Unzips\").OrderBy(a=>a);
foreach (var republic in resps)
{
	var dirName = new DirectoryInfo(republic);
	var files = Directory.GetFiles(string.Format(republic), "contract_*_cleaned.xml", SearchOption.AllDirectories);
	var targetFolder = string.Format(@"d:\Development\github\Azure.HowTos\DataLake.Samples\GovermentContracts\Data\{0}\",dirName.Name);
	if (!Directory.Exists(targetFolder))
		Directory.CreateDirectory(targetFolder);

	var bigFilePathTemplate = Path.Combine(targetFolder, @"bigFile{0}.xml");

	var groups = files.Select((a, i) => new { Index = i, Path = a }).GroupBy(a => a.Index / 10000).ToList();
	int u = 0;
	foreach (var group in groups)
	{
		var bigFilepath = String.Format(bigFilePathTemplate, u);
		if (File.Exists(bigFilepath))
		{
			File.Delete(bigFilepath);
		}

		var regex = new Regex("[ ]+\\<");

		using (var bigFile = File.Create(bigFilepath))
		{
			using (var sw = new StreamWriter(bigFile))
			{
				sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
				sw.WriteLine(@"<export>");
				foreach (var file in group.Select(a => a.Path))
				{
					try
					{
						var doc = XDocument.Load(file);
						using (var reader = doc.CreateReader())
						{

							doc.Descendants().Where(a => a.Name.LocalName == "signature").Remove();
							//					foreach (var element in doc.Root.XPathSelectElements("//name"))
							//					{
							//						element.Value = element.Value.Trim();
							//						element.Value = element.Value.Length > 30 ? element.Value.Substring(0,30) : element.Value;
							//					}
							doc.Root.XPathSelectElements("//products/product/externalSid").Remove();
							doc.Root.XPathSelectElements("//products/product/priceRUR").Remove();
							doc.Root.XPathSelectElements("//products/product/sumRUR").Remove();
							doc.Root.XPathSelectElements("//products/product/drugPurchaseObjectInfo").Remove();
							foreach (var element in doc.Root.XPathSelectElements("//products/product/sid"))
								element.Name = "s";

							foreach (var element in doc.Root.XPathSelectElements("//products/product/OKPD/code"))
								element.Name = "c";
							foreach (var element in doc.Root.XPathSelectElements("//products/product/OKPD"))
								element.Name = "o";

							foreach (var element in doc.Root.XPathSelectElements("//products/product/OKPD2/code"))
								element.Name = "c";
							foreach (var element in doc.Root.XPathSelectElements("//products/product/OKPD2/name"))
								element.Name = "n";
							foreach (var element in doc.Root.XPathSelectElements("//products/product/OKPD2"))
								element.Name = "o2";

							foreach (var element in doc.Root.XPathSelectElements("//products/product/OKEI/code"))
								element.Name = "c";
							foreach (var element in doc.Root.XPathSelectElements("//products/product/OKEI/nationalCode"))
								element.Name = "nc";
							foreach (var element in doc.Root.XPathSelectElements("//products/product/OKEI"))
								element.Name = "ok";

							foreach (var element in doc.Root.XPathSelectElements("//product"))
								element.Name = "p";

							var elements = doc.Root.Descendants().Where(a => a.Name.LocalName == "contract");
							foreach (var element in elements)
							{
								//						var productsCheck = element.XPathSelectElement("//products");
								//						if (productsCheck != null && productsCheck.ToString().Length > 120000)
								//						{
								//							productsCheck.Dump();
								//						}

								sw.WriteLine(regex.Replace(element.ToString(), "<"));
							}
						}
					}
					catch (Exception ex)
					{
						ex.Dump();
					}
				}

				sw.WriteLine(@"</export>");
			}
		}


		var checkDoc = XDocument.Load(bigFilepath);

		//	using (var reader = checkDoc.CreateReader())
		//	{
		//		var products = checkDoc.XPathSelectElements("//products");
		//		foreach (var element in products)
		//		{
		//			if (element.ToString().Length > 128000)
		//			{
		//				element.Remove();
		//			}
		//		}
		//	}

		//	checkDoc.Save(bigFilepath);
		u++;
	}
}