using System;
using System.IO;
using System.Linq;
using Library.Extensions;

namespace Script
{
    public class dos2 : BaseClass
    {
        public dos2()
        {
            //var files = CheckPath("*.*", searchOption: SearchOption.AllDirectories).Select(p => "\"" + p + "\"").Join(" ");
            //var cmd = string.Format("{0} -k -s -q -i -o {1}", this.GetType().Name, files);
            //Console.WriteLine(cmd);
            //CmdReadLine(cmd).ForEach(p => Console.WriteLine(p));


            var files = CheckPath(null, searchOption: SearchOption.AllDirectories);

            files.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount).ForAll(file =>
            {
                var cmd = string.Format("{0} -k -s -q -i -o \"{1}\"", this.GetType().Name, file);
                //Console.WriteLine(cmd);
                CmdReadLine(cmd);
            });
        }
    }

    public class dos2unix : dos2
    {
    }

    public class unix2dos : dos2
    {
    }

    public class mac2unix : dos2
    {
    }

    public class unix2mac : dos2
    {
    }
}