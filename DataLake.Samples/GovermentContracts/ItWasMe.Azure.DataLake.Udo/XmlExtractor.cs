using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Types.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ItWasMe.Azure.DataLake.Udo
{

	[SqlUserDefinedExtractor(AtomicFileProcessing = true)]
	public class XmlExtractor : IExtractor
	{
		private readonly string _rowPath;

		private readonly SqlMap<string, string> _columnPaths;
		private readonly XmlNamespaceManager _namespaceManager;

		public XmlExtractor(string rowXPath, SqlMap<string, string> columnPaths) : this(rowXPath, columnPaths, null)
		{
		}
		public XmlExtractor(string rowXPath, SqlMap<string, string> columnPaths, SqlMap<string, string> namespaces)
		{
			_rowPath = rowXPath;
			_columnPaths = columnPaths;
			_namespaceManager = new XmlNamespaceManager(new NameTable());
			foreach (var @namespace in (namespaces ?? new SqlMap<string, string>()))
			{
				_namespaceManager.AddNamespace(@namespace.Key, @namespace.Value);
			}
		}

		public override IEnumerable<IRow> Extract(IUnstructuredReader input, IUpdatableRow output)
		{
			// Make sure that all requested columns are of type string
			IColumn wrongColumn = output.Schema.FirstOrDefault(col => col.Type != typeof(string) && col.Type != typeof(SqlArray<string>));
			if (wrongColumn != null)
			{
				throw new ArgumentException(string.Format("Column '{0}' must be of type 'string', not '{1}'", wrongColumn.Name, wrongColumn.Type.Name));
			}

			using (var reader = XmlReader.Create(input.BaseStream))
			{
				var xdocument = XDocument.Load(reader);

				foreach (var element in xdocument.XPathSelectElements(_rowPath, _namespaceManager))
				{
					foreach (var columnPath in _columnPaths)
					{
						var column = output.Schema.FirstOrDefault(a => a.Name == columnPath.Value);
						if (column == null)
						{
							throw new ArgumentException(string.Format("Column '{0}' hasn't been found", columnPath.Value));
						}

						if (column.Type == typeof(SqlArray<string>))
						{
							output.Set(columnPath.Value, new SqlArray<string>(
								element.XPathSelectElement(columnPath.Key, _namespaceManager)?.Elements().Select(a => a.ToString())));
						}
						else
						{
							output.Set(columnPath.Value, element.XPathSelectElement(columnPath.Key, _namespaceManager)?.Value);
						}
					}

					yield return output.AsReadOnly();
				}
			}

		}
	}
}