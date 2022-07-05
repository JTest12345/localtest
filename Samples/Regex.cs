using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Samples
{
    class RegexSample
    {
        static void _Main(string[] args)
        {
            var arr = Regex.Split("codename123", @"(?<=[a-zA-Z])(?=\d)");

            Console.ReadKey();
        }
    }
}
