using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;

namespace checkcard
{

    public class CheckCard
    {
        public CheckCard()
        {
            var root = SystemConsole.GetInputStr("itemid:") + "\\";
            var xx = File.ReadAllLines("itemid.txt").ToList();
            xx = xx.AsParallel().Select(p => new List<string>()
            {
                string.Format("{0}", p.ToLower()),
            }).SelectMany(p => p).Where(p => !File.Exists(root + p + ".png")).ToList();

            File.WriteAllLines("checkItem.txt", xx);
        }
    }
}