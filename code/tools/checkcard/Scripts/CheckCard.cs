using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library.Extensions;

namespace checkcard.Scripts
{

    public class CheckCard
    {
        public CheckCard()
        {
            var root = SystemConsole.GetInputStr("itemid:") + "\\";
            var xx = File.ReadAllLines("itemid.txt").ToList();
            xx = xx.Select(p => new List<string>()
            {
                string.Format("{0}", p.ToLower()),
            }).SelectMany(p => p).ToList();
            xx = xx.Where(p => !File.Exists(root + p + ".png")).ToList();

            File.WriteAllLines("checkItem.txt", xx);
        }
    }
}