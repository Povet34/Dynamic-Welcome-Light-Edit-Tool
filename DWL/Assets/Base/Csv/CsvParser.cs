using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Neofect.BodyChecker.Language
{
	public class CsvParser
	{
		private List<List<string>> rows = new List<List<string>>();

		public void ReadStream(string stream)
		{
			StringReader reader = new StringReader(stream);

			rows.Clear();
			string strRow;
			
			while ((strRow = reader.ReadLine()) != null)
            {
				if (string.IsNullOrEmpty(strRow)) return;

				StringBuilder item = new StringBuilder();
				var innerRowList = new List<string>();
				bool isInQuote = false;

				for(int nIndex = 0; nIndex < strRow.Length; nIndex++)
                {
					var character = strRow[nIndex];

					if (character == '\"')
                    {
						if (nIndex - 1 >= 0 && strRow[nIndex - 1] == '\\')
							item.Append(character);
						else
                        {
							if (isInQuote)
								isInQuote = false;
							else
								isInQuote = true;
						}
					}
					else if(character == ',')
                    {
						if(isInQuote)
                        {
							item.Append(character);
                        }
						else
                        {
							item = item.Replace("\\\"", "\"");
							innerRowList.Add(item.ToString());

							// ** StringBuilder is not has clear...
							// http://stackoverflow.com/questions/1709471/best-way-to-clear-contents-of-nets-stringbuilder
							item.Length = 0;
							item.Capacity = 0;
						}
					}
					else
                    {
						item.Append(character);
					}
                }
				item = item.Replace("\\\"", "\"");
				innerRowList.Add(item.ToString());

				rows.Add(innerRowList);
			}

			reader.Dispose();
		}

		public List<string> GetRow(int index)
		{
			if (0 > index || rows.Count <= index)
			{
				throw new System.IndexOutOfRangeException();
			}
			return rows[index];
		}


		public int GetRowCount()
		{
			return rows.Count;
		}
	}
}
