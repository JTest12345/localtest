using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace Samples
{
    class Yaml2Cs
    {
        static void Main(string[] args)
        {
            string yamlPath = @"C:\Oskas\oskas.yaml";
            var input = new StreamReader(yamlPath, Encoding.UTF8);
            var yaml = new YamlStream();
            yaml.Load(input);

            var rootNode = yaml.Documents[0].RootNode;

            var systemconf = (YamlMappingNode)rootNode["systemconf"];

            var debugmode = systemconf["debug"];
            Console.WriteLine($"debugmode : {debugmode}");


            var fileconf = (YamlMappingNode)rootNode["fileconf"];
            foreach (var c in fileconf.Children)
            {
                Console.WriteLine($"{c.Key} :");
                var value = c.Value.ToString().Replace("[", "").Replace("]", "");
                var valuearr = value.Split(',');
                foreach (var val in valuearr)
                {
                    Console.WriteLine(val);
                }
            }

            var mqtt = (YamlMappingNode)rootNode["mqtt"];
            foreach (var c in mqtt.Children)
            {
                Console.WriteLine($"{c.Key} : {c.Value}");
            }

            //string yamlPath = @"C:\Oskas\magcup\magcup.yaml";
            //var input = new StreamReader(yamlPath, Encoding.UTF8);
            //var yaml = new YamlStream();
            //yaml.Load(input);

            //var rootNode = yaml.Documents[0].RootNode;

            //var systemconf = (YamlMappingNode)rootNode["systemconf"];
            //foreach (var c in systemconf.Children)
            //{
            //    Console.WriteLine($"{c.Key} : {c.Value}");
            //}

            //var fileconf = (YamlMappingNode)rootNode["fileconf"];
            //foreach (var c in fileconf.Children)
            //{
            //    Console.WriteLine($"{c.Key} :");
            //    var value = c.Value.ToString().Replace("[", "").Replace("]", "");
            //    var valuearr = value.Split(',');
            //    foreach (var val in valuearr)
            //    {
            //        Console.WriteLine(val);
            //    }
            //}

            //var ffetch = (YamlMappingNode)rootNode["ffetch"];
            //foreach (var c in ffetch.Children)
            //{
            //    Console.WriteLine($"{c.Key} : {c.Value}");
            //}

            //var mqtt = (YamlMappingNode)rootNode["mqtt"];
            //foreach (var c in mqtt.Children)
            //{
            //    Console.WriteLine($"{c.Key} : {c.Value}");
            //}

            Console.ReadKey();
        }
    }
}
