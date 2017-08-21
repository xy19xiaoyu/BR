using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BRDB.Extend;

namespace BRDB.Model
{
    public class hy
    {
        public Dictionary<string, string> ipc3 = new Dictionary<string, string>();
        public Dictionary<string, string> ipc4 = new Dictionary<string, string>();
        public Dictionary<string, string> ipc7 = new Dictionary<string, string>();
        public Dictionary<string, string> ipc = new Dictionary<string, string>();
        public string name { get; set; }
        public string id { get; set; }
        private string bashpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hys");
        public hy(string name, string id)
        {
            this.name = name;
            this.id = id;

            using (StreamReader sr = new StreamReader($"{bashpath}\\{id.PadLeft(2, '0')}.{name}.txt"))
            {
                while (!sr.EndOfStream)
                {
                    string rl = sr.ReadLine().Trim();
                    if (string.IsNullOrEmpty(rl)) continue;
                    string stripc = rl.Trim('*');

                    switch (stripc.Length)
                    {
                        case 3:
                            if (ipc3.ContainsKey(stripc)) continue;
                            ipc3.Add(stripc, id);
                            break;
                        case 4:
                            if (ipc4.ContainsKey(stripc)) continue;
                            ipc4.Add(stripc, id);
                            break;
                        case 5:
                        case 6:
                        case 7:
                            string stripc7 = stripc.FormatIPC().Left(7);
                            if (ipc7.ContainsKey(stripc7)) continue;
                            ipc7.Add(stripc7, id);
                            break;
                        default:
                            string tmpipc = stripc.FormatIPC();
                            if (ipc.ContainsKey(tmpipc)) continue;
                            ipc.Add(tmpipc, id);
                            break;
                    }

                }
            }
        }

        public string GetHyId(string stripc)
        {
            if (ipc.ContainsKey(stripc)) return id;
            if (ipc3.ContainsKey(stripc.Left(3))) return id;
            if (ipc4.ContainsKey(stripc.Left(4))) return id;
            if (ipc7.ContainsKey(stripc.Left(7))) return id;
            return string.Empty;
        }

        public override string ToString()
        {
            return $"{id},{name}";
        }


    }
}
