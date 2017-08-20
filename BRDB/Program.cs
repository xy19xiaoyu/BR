using BRDB.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRDB
{
    class Program
    {
        private static enDataContext en = new enDataContext();
        static void Main(string[] args)
        {
            ExchangeIPC();

        }

        public static void ExchangeIPC()
        {
            long maxid = en.Ipc.Max(x => x.ID);
            long loop = maxid / 1000;

            for (int i = 0; i < loop + 1; i++)
            {
                if (i * 1000 > maxid) break;
                List<Ipc> ipcs = en.Ipc.Skip(i * 1000).Take(1000).ToList<Ipc>();
                foreach (var ipc in ipcs)
                {
                    string stripc = ipc.IPC1.FormatIPC();
                    var tmpipc = new en_ipc()
                    {
                        pn = ipc.PubID,
                        ipc = stripc,
                        ipc1 = stripc[0],
                        ipc3 = stripc.Left(3),
                        ipc4 = stripc.Left(4),
                        ipc7 = stripc.Left(7)
                    };
                    en.en_ipc.InsertOnSubmit(tmpipc);
                }
                Console.WriteLine(i);
                en.SubmitChanges();
            }
        }
    }

}
