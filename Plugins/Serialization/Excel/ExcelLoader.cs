﻿
using System;
using System.Data;
using System.IO;
using ExcelDataReader;
namespace XIFramework
{
	public class ExcelLoader
	{
		private DataSet data;
		public string filePath;

		public ExcelLoader(string filePath, int headerRow)
		{
			this.filePath = filePath;
			using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
				{
					var result = reader.AsDataSet(CreateDataSetReading(headerRow));
					this.data = result;
				}
			}

			if (this.Sheets.Count < 1)
			{
				throw new Exception("空的Excel文件! filePath:" + this.filePath);
			}
			
		}

		public DataTableCollection Sheets
		{
			get
			{
				return data.Tables;
			}
		}

        private ExcelDataSetConfiguration CreateDataSetReading(int headerRow)
        {
			ExcelDataTableConfiguration tableConfig = new ExcelDataTableConfiguration()
			{
				UseHeaderRow = true,
			};

			return new ExcelDataSetConfiguration()
			{
				UseColumnDataType = true,
				ConfigureDataTable = v => tableConfig
			};
			
        }
    }
}
