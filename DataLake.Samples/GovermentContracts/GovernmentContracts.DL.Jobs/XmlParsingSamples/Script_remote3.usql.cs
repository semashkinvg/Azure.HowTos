using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Local
{

	public static class SimpleOperations
	{
		public static string FormatDate(string value)
		{
			DateTime result;
			if (DateTime.TryParse(value, out result))
			{
				return result.ToString("yyyy-MM-dd");
			}

			return null;
		}
	}

}