using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using fontConvert.Script;
using Library;
using Library.Extensions;
using Library.Helper;

namespace fontConvert
{
    public enum ConvertType
    {
        [Description("简体转换为繁体(Dll)")][TypeValue(typeof (ActionByDLL.ToTraditional))] ToTraditional_Dll = 1,
        [Description("繁体转换为简体(Dll)")][TypeValue(typeof (ActionByDLL.ToSimplified))] ToSimplified_Dll,
        [Description("简体转换为繁体(Array)")][TypeValue(typeof (ActionByArray.ToTraditional))] ToTraditional_Array,
        [Description("繁体转换为简体(Array)")][TypeValue(typeof (ActionByArray.ToSimplified))] ToSimplified_Array,
        //[TypeValue(typeof (ActionByVB.ToTraditional))] ToTraditional_VB,
        //[TypeValue(typeof (ActionByVB.ToSimplified))] ToSimplified_VB,
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Action<object> callAction = delegate(object o)
            {
                BaseActionBy baseActionFor = (BaseActionBy) o;
                baseActionFor.Open();
            };
            SystemConsole.Run<ConvertType>(callAction);
        }
    }


	internal class ProgramTest
	{
		private static void Main(string[] args)
		{
			List<string> keys = new List<string>();
			var path = Path.GetFullPath("SEGA_MatisseN v2-B.ttf");
			foreach (FontFamily fontFamily in Fonts.GetFontFamilies(path))
			{
				var typefaces = fontFamily.GetTypefaces();
				int index = 0;
				foreach (Typeface typeface in typefaces)
				{
					index++;
					Dictionary<object, object> result = new Dictionary<object, object>();
					if (typeface.TryGetGlyphTypeface(out GlyphTypeface glyphTypeface))
					{
						foreach (var item in glyphTypeface.CharacterToGlyphMap)
						{
							var key = item.Key.ToString("x8");
							byte[] arr = HexStringToByteArray(key);
							System.Text.UnicodeEncoding converter = new System.Text.UnicodeEncoding();
							string str = converter.GetString(arr);
							result[str] = item.Value;
						}
					}
					File.WriteAllText("test" + index + ".json", JsonHelper.ToJson(result, indentLevel: 2, isUnicode: false));
				}
			}
			File.WriteAllLines("test.txt", keys.ToArray());
		}

		public static byte[] HexStringToByteArray(string s)
		{
			s = s.Replace(" ", "");
			byte[] buffer = new byte[s.Length / 2];
			for (int i = 0; i < s.Length; i += 2)
			{
				buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
			}
			return buffer;
		}
	}
}
