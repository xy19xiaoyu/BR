using BRDB.Extend;
using BRDB.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BRDB
{
    class Program
    {
        private static cnDataContext cn = new cnDataContext();
        private static enDataContext en = new enDataContext();
        static void Main(string[] args)
        {
            CMDHelper.RunCMD("bcp", "");
            //ExchangeBiblioWPI();
            //ExchangeIPCWPI();
            //ExchangeENhy();


            //ExchangeCNhy();
            //ExchangeBiblio();
            //ExchangeIPC();
        }

        public static void ExchangeENhy()
        {
            int step = 1000;
            long maxid = en.en_ipc.Max(x => x.id);
            long loop = maxid / step;

            using (StreamWriter sw = new StreamWriter("d:\\en_zt.txt", false, Encoding.GetEncoding("utf-8"))) //还是bcp 文件导入快
            {
                for (int i = 0; i < loop + 1; i++)
                {
                    if (i * step > maxid) break;
                    List<en_ipc> ipcs = en.en_ipc.Skip(i * step).Take(step).ToList<en_ipc>();
                    List<en_zt> zts = new List<en_zt>();
                    foreach (var ipc in ipcs)
                    {
                        List<string> ids = hyHelper.GetHyIds(ipc.ipc);
                        foreach (var x in ids)
                        {
                            zts.Add(new en_zt() { pn = ipc.pn, zt = x.to_i() });
                            sw.WriteLine($"0|{ipc.pn}|{x.to_i()}");
                        }

                    }
                    Console.WriteLine($"{i} /{loop + 1} -{DateTime.Now}");
                    //en.en_zt.InsertAllOnSubmit(zts);               
                    //en.SubmitChanges();
                }
            }

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
                Console.WriteLine($"{i} /{loop + 1}  -{DateTime.Now}");
                cn.SubmitChanges();
            }
        }

        public static string GetFistrPRCountry(string pubno)
        {
            try
            {
                return en.Priority_Dwpi.Where(x => x.PubID == pubno && x.Sequence ==1).First().PriorityNo.Left(2);
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        //bcp ExtractData.dbo.en_pa in d:\en_pa.txt -Usa -Psa@123456 -c -t"|"
        public static void ExchangeBiblioWPI()
        {
            long maxid = en.DocInfo_Dwpi.Max(x => x.ID);
            long loop = maxid / 1000;
            using (StreamWriter sw = new StreamWriter("D:\\en_pa.txt", false, Encoding.GetEncoding("utf-8")),
                   sw_en = new StreamWriter("D:\\en.txt", false, Encoding.GetEncoding("utf-8")),
                   sw_en_tiabs = new StreamWriter("D:\\en_tiabs.txt", false, Encoding.GetEncoding("utf-8")))
            {
                for (int i = 0; i < loop + 1; i++)
                {
                    if (i * 1000 > maxid) break;
                    List<DocInfo_Dwpi> docinfos = en.DocInfo_Dwpi.Skip(i * 1000).Take(1000).ToList<DocInfo_Dwpi>();
                    foreach (var doc in docinfos)
                    {
<<<<<<< HEAD
                        sw_en.WriteLine($"0|{doc.PubID}|{doc.AppNo}|{GetFistrPRCountry(doc.PubID)}|{doc.PubID.Left(2)}|{doc.AppDate.Left(4).to_i()}|{doc.PubDate.Left(4).to_i()}");
                        sw_en_tiabs.WriteLine($"0|{doc.PubID}|{doc.Title.Left(500).Replace("|", "")}|{doc.Abstract.Left(1000).Replace("|", "")}");
                        string[] pas = doc.Applicants.Split(";&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var strpa in pas)
                        {
                            var pa = new en_pa()
                            {
                                pn = doc.PubNo,
                                pa = strpa
                            };
                            sw.WriteLine($"0|{doc.PubID.Left(20).Replace("|", "")}|{strpa.Left(100).Replace("”", "").Replace("“", "").Replace("Č", "").Replace("\n", "").Replace("\r", "").Replace("|", "")}");
                            // en.en_pa.InsertOnSubmit(pa);
                        }
=======

                        var tmp_en = new en()
                        {
                            pn = doc.PubID,
                            an = doc.AppNo,
                            i_c = GetFistrPRCountry(doc.PubID),
                            p_c = doc.PubID.Left(2),
                            ady = doc.AppDate.Left(4).to_i(),
                            pdy = doc.PubDate.Left(4).to_i(),
                            ti = doc.Title.Left(500)
                        };
                        en.en.InsertOnSubmit(tmp_en);


                        //string[] pas = doc.Applicants.Split(";&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        //foreach (var strpa in pas)
                        //{
                        //    var pa = new en_pa()
                        //    {
                        //        pn = doc.PubNo,
                        //        pa = strpa
                        //    };
                        //    en.en_pa.InsertOnSubmit(pa);
                        //}

                        //string[] pas = doc.Applicants.Split(";&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        //foreach (var strpa in pas)
                        //{
                        //    var pa = new en_pa()
                        //    {
                        //        pn = doc.PubNo,
                        //        pa = strpa
                        //    };
                        //    sw.WriteLine($"0|{doc.PubID.Left(20).Replace("|", "")}|{strpa.Left(100).Replace("”", "").Replace("“", "").Replace("Č", "").Replace("\n", "").Replace("\r", "").Replace("|", "")}");
                        //    // en.en_pa.InsertOnSubmit(pa);
                        //}
>>>>>>> x
                    }
                    en.SubmitChanges();
                    Console.WriteLine($"{i} /{loop + 1}  -{DateTime.Now}");
                }
            }

        }
        public static void ExchangeBiblio()
        {
            long maxid = en.DocInfo.Max(x => x.ID);
            long loop = maxid / 1000;
            using (StreamWriter sw = new StreamWriter("D:\\en_pa.txt", false, Encoding.GetEncoding("utf-8")))
            {
                for (int i = 0; i < loop + 1; i++)
                {
                    if (i * 1000 > maxid) break;
                    List<DocInfo> docinfos = en.DocInfo.Skip(i * 1000).Take(1000).ToList<DocInfo>();
                    foreach (var doc in docinfos)
                    {

                        //var tmp_en = new en()
                        //{
                        //    pn = doc.PubID,
                        //    an = doc.AppNo,
                        //    i_c = doc.ApplicantCountry.Left(2),
                        //    p_c = doc.PubID.Left(2),
                        //    ady = doc.AppDate.Left(4).to_i(),
                        //    pdy = doc.PubDate.Left(4).to_i()
                        //};
                        //en.en.InsertOnSubmit(tmp_en);


                        string[] pas = doc.Applicants.Split(";&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var strpa in pas)
                        {
                            var pa = new en_pa()
                            {
                                pn = doc.PubNo,
                                pa = strpa
                            };
                            sw.WriteLine($"0|{doc.PubID.Left(20).Replace("|", "")}|{strpa.Left(100).Replace("”", "").Replace("“", "").Replace("Č", "").Replace("\n", "").Replace("\r", "").Replace("|", "")}");
                            // en.en_pa.InsertOnSubmit(pa);
                        }
                        //en.SubmitChanges();
                    }
                    Console.WriteLine($"{i} /{loop + 1}  -{DateTime.Now}");
                    en.SubmitChanges();
                }
            }

        }


        public static void ExchangeIPCWPI()
        {
            long maxid = en.Ipc_Dwpi.Max(x => x.ID);
            long loop = maxid / 1000;

            for (int i = 0; i < loop + 1; i++)
            {
                if (i * 1000 > maxid) break;
                List<Ipc_Dwpi> ipcs = en.Ipc_Dwpi.Skip(i * 1000).Take(1000).ToList<Ipc_Dwpi>();
                foreach (var ipc in ipcs)
                {
                    string stripc = ipc.IPC.Replace(" ", "").FormatIPC();
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
                Console.WriteLine($"{i} /{loop + 1}  -{DateTime.Now}");
                en.SubmitChanges();
            }
        }
    }

}
