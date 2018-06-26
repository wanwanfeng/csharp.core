using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Extensions;

namespace checkcard
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //do
            //{
            //    var root = SystemConsole.GetInputStr("cardid:") + "\\";
            //    var xx = File.ReadAllLines("cardid.txt").ToList();
            //    xx = xx.Select(p => new List<string>()
            //    {
            //        string.Format("card_{0}_c", p),
            //        string.Format("card_{0}_d", p),
            //        string.Format("card_{0}_f", p),
            //        string.Format("card_{0}_l", p),
            //        string.Format("card_{0}_m", p),
            //        string.Format("card_{0}_s", p),
            //        string.Format("card_{0}_v", p),
            //    }).SelectMany(p => p).ToList();
            //    xx = xx.Where(p => !File.Exists(root + p + ".png")).ToList();

            //    File.WriteAllLines("checkCard.txt", xx);

            //} while (SystemConsole.ContinueY());

            //do
            //{
            //    var root = SystemConsole.GetInputStr("pieceid:") + "\\";
            //    var xx = File.ReadAllLines("pieceid.txt").ToList();
            //    xx = xx.Select(p => new List<string>()
            //    {
            //        string.Format("memoria_{0}_c", p),
            //        string.Format("memoria_{0}_s", p)
            //    }).SelectMany(p => p).ToList();
            //    xx = xx.Where(p => !File.Exists(root + p + ".png")).ToList();

            //    File.WriteAllLines("checkMemoria.txt", xx);

            //} while (SystemConsole.ContinueY());

            do
            {
                var root = SystemConsole.GetInputStr("itemid:") + "\\";
                var xx = File.ReadAllLines("itemid.txt").ToList();
                xx = xx.Select(p => new List<string>()
                {
                    string.Format("{0}", p.ToLower()),
                }).SelectMany(p => p).ToList();
                xx = xx.Where(p => !File.Exists(root + p + ".png")).ToList();

                File.WriteAllLines("checkItem.txt", xx);

            } while (SystemConsole.ContinueY());
        }
    }
}
