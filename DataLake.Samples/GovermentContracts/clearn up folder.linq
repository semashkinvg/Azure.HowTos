<Query Kind="Statements" />

var files = Directory.GetFiles(@"d:\Projects\GOV_ZKP\Data\Unzips\Samarskaja_obl\","contractProcedure_*.xml");
foreach (var file in files)
{
	File.Delete(file);
}