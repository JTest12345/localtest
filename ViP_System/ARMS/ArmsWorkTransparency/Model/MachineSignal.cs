using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsWorkTransparency.Model
{
    public class MachineSignal
    {
        public static Dictionary<string, bool> GetReadyList(int retryCt)
        {
            if (retryCt >= 10)
                throw new Exception("装置の運転信号の取得に失敗しました。試行回数：10回");

            Dictionary<string, bool> retv = new Dictionary<string, bool>();

            if (Properties.Settings.Default.UseMachineReady == false)
                return retv;

            StringCollection files = Properties.Settings.Default.MachineReadyFiles;
            foreach (string file in files)
            {
                try
                { 
                    string[] fileLines = File.ReadAllLines(file);

                    foreach (string line in fileLines)
                    {
                        string[] element = line.Split(',');
                        if (retv.Where(r => r.Key == element[1]).Count() == 0)
                        {
                            retv.Add(element[1], Convert.ToBoolean(int.Parse(element[2])));
                        }
                    }
                }
                catch (IOException)
                {
                    retryCt = retryCt + 1;
                    return GetReadyList(retryCt);
                }
            }

            return retv;
        }

        public static Dictionary<string, bool> GetReadyList()
        {
            return GetReadyList(0);
        }
    }
}
