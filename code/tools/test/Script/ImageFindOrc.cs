#define imageOrc

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;
using Library.LitJson;
using LitJson;

namespace Script
{
    /// <summary>
    /// 图片查找形成excel格式并复制出来
    /// </summary>
    public class ImageFindOrc : BaseClass
    {
#if imageOrc
        private static string APP_ID = "11074564";
        private static string API_KEY = "sYyHWbN8eXXoxGfe15w5yWgy";
        private static string SECRET_KEY = "1suG7480xKHWft3dmROtgbrraNMg5YU3";
        public static Baidu.Aip.Ocr.Ocr client;

        static ImageFindOrc()
        {
            client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
        }
#endif

        Dictionary<string, string> dic = new Dictionary<string, string>();

        public ImageFindOrc()
        {
            var eList = ".png|.jpg|.bmp|.psd|.tga|.tif|.dds".AsStringArray('|').ToList();
            var res = Directory.GetFiles(root, "*", SearchOption.AllDirectories)
                //.Where(p => p.Contains("cocos_Data"))
                //.Where(p => p.Contains("Resources"))
                .Where(p => eList.Contains(Path.GetExtension(p)))
                .Select(p => p.Replace("\\", "/"))
                .ToList();
            res.Sort();
            if (res.Count == 0) return;
            RunList(res);
            WriteAllLines(dic);
        }

        public override void RunListOne(string re)
        {
#if imageOrc
            var imageOrc = File.ReadAllBytes(re);
            // 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
            var str = client.GeneralBasic(imageOrc).ToString();
            Console.WriteLine(str);
            var result = JsonMapper.ToObject(str);
            if (result.Keys.Contains("error_code"))
            {
                Console.ReadKey();
                return;
            }
            if (result["words_result_num"].AsInt() <= 0)
                return;
#endif
            dic[re.Replace(root, "")] = GetExcelCell(re);
        }


#if imageOrc
        public void GeneralBasicDemo()
        {
            var image = File.ReadAllBytes("图片文件路径");
            // 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.GeneralBasic(image);
            Console.WriteLine(result);
            // 如果有可选参数
            var options = new Dictionary<string, object>
            {
                {"language_type", "CHN_ENG"},
                {"detect_direction", "true"},
                {"detect_language", "true"},
                {"probability", "true"}
            };
            // 带参数调用通用文字识别, 图片参数为本地图片
            result = client.GeneralBasic(image, options);
            Console.WriteLine(result);
        }

        public void GeneralBasicUrlDemo()
        {
            var url = "https//www.x.com/sample.jpg";

            // 调用通用文字识别, 图片参数为远程url图片，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.GeneralBasicUrl(url);
            Console.WriteLine(result);
            // 如果有可选参数
            var options = new Dictionary<string, object>
            {
                {"language_type", "CHN_ENG"},
                {"detect_direction", "true"},
                {"detect_language", "true"},
                {"probability", "true"}
            };
            // 带参数调用通用文字识别, 图片参数为远程url图片
            result = client.GeneralBasicUrl(url, options);
            Console.WriteLine(result);
        }
        #endif
    }
}