using System;
using ExcelDataReader;

namespace Cron_Testings.Helpers
{
	public class ExcelReaders
	{
		public ExcelReaders()
		{
            
        }

		public void readExcel()
		{
            using (var stream = File.Open("dbSiswa.xls", FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Choose one of either 1 or 2:

                    // 1. Use the reader methods
                    do
                    {
                        while (reader.Read())
                        {
                            // reader.GetDouble(0);
                        }
                    } while (reader.NextResult());

                    // 2. Use the AsDataSet extension method
                    var result = reader.AsDataSet();

                    Console.WriteLine(result);

                    // The result of each spreadsheet is in result.Tables
                }
            }
        }
	}
}

