using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            new hahab();
            Console.ReadKey();
        }
    }


    public class hahaa
    {
        static hahaa()
        {
            Console.WriteLine("static hahaa");
        }

        public hahaa()
        {
            Console.WriteLine("public hahaa");
        }
    }

    public class hahab : hahaa
    {
        static hahab()
        {
            Console.WriteLine("static hahab");
        }

        public hahab()
        {
            Console.WriteLine("public hahab");
        }
    }
}
