using BRDB.Extend;
using BRDB.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRDB
{
    class Program
    {
        private static cnDataContext cn = new cnDataContext();
        private static enDataContext en = new enDataContext();
        static void Main(string[] args)
        {
            ExchangeCNhy();



            //ExchangeBiblio();
            //ExchangeIPC();
        }

        public static void ExchangeCNhy()
        {
            long maxid = cn.cn_ipc.Max(x => x.id);
            long loop = maxid / 1000;


            for (int i = 0; i < loop + 1; i++)
            {
                if (i * 1000 > maxid) break;
                List<cn_ipc> ipcs = cn.cn_ipc.Skip(i * 1000).Take(1000).ToList<cn_ipc>();
                foreach (var ipc in ipcs)
                {
                    List<string> ids = hyHelper.GetHyIds(ipc.ipc);
                    foreach (var x in ids)
                    {
                        cn.cn_zt.InsertOnSubmit(new cn_zt() { an = ipc.an, zt = x.to_i() });
                    }

                }
                Console.WriteLine($"{i} -{DateTime.Now}");
                cn.SubmitChanges();
            }
        }

        public static void ExchangeBiblio()
        {
            long maxid = en.DocInfo.Max(x => x.ID);
            long loop = maxid / 1000;

            for (int i = 0; i < loop + 1; i++)
            {
                if (i * 1000 > maxid) break;
                List<DocInfo> docinfos = en.DocInfo.Skip(i * 1000).Take(1000).ToList<DocInfo>();
                foreach (var doc in docinfos)
                {

                    var tmp_en = new en()
                    {
                        pn = doc.PubID,
                        an = doc.AppNo,
                        i_c = doc.ApplicantCountry.Left(2),
                        p_c = doc.PubID.Left(2),
                        ady = doc.AppDate.Left(4).to_i(),
                        pdy = doc.PubDate.Left(4).to_i()
                    };
                    en.en.InsertOnSubmit(tmp_en);


                    string[] pas = doc.Applicants.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var strpa in pas)
                    {
                        var pa = new en_pa()
                        {
                            pn = doc.PubDate,
                            pa = strpa
                        };
                        en.en_pa.InsertOnSubmit(pa);
                    }
                    //en.SubmitChanges();
                }
                Console.WriteLine($"{i} /{loop + 1}  -{DateTime.Now}");
                en.SubmitChanges();
            }
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
                Console.WriteLine($"{i} -{DateTime.Now}");
                en.SubmitChanges();
            }
        }
    }

}
