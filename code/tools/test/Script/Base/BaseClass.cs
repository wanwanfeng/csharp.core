﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library;
using Library.Excel;
using Library.Extensions;
using Library.Helper;

namespace Script
{
    public class BaseClass : BaseSystemConsole
    {
        public void WriteAllLines(Dictionary<string, string> dic, string path)
        {
            //直接写入excel
            var listTable = new ListTable()
            {
                Rows = dic.Select(p => new List<object>() {p.Key, p.Value}).ToList(),
                Columns = new List<string>() {"key", "value"}
            };
            ExcelUtils.ExportToExcel(listTable.ToDataTable(), Path.ChangeExtension(path, ".xlsx"));
        }

        public void CreateDirectory(string name)
        {
            var path = GetType().Name + "/" + name;
            DirectoryHelper.CreateDirectory(path);
        }

        public string GetExcelCell(string path)
        {
			try
			{
				using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
					int width = image.Width;
					int height = image.Height;


					int max = Math.Max(width, height);
					if (max == width)
					{
						if (max > 350)
						{
							height = 350 * height / width;
							width = 350;
						}
					}
					else
					{
						if (max > 350)
						{
							width = 350 * width / height;
							height = 350;
						}
					}
					return string.Format("<table><img src='{0}' width='{1}' height='{2}'>", path, width, height);
				}
			}
			catch (Exception)
			{
				return "";
			}
        }

		protected List<string> GetPaths(string extension = ".txt")
		{
			extension = Path.GetExtension(extension);

			var path = SystemConsole.GetInputStr("请拖入选定（文件夹或文件）:");

			List<string> res = new List<string>();
			if (Directory.Exists(path))
			{
				var newPath = Path.ChangeExtension(path, extension);
				if (File.Exists(newPath))
					res.Add(newPath);
				else
					res.AddRange(Directory.GetFiles(path, "*" + extension, SearchOption.AllDirectories));
			}
			else
			{
				if (File.Exists(path) && Path.GetExtension(path) == extension)
					res.Add(path);
			}
			return res;
		}
	}
}