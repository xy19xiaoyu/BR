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
        #region wpi
        static void Main(string[] args)
        {

            //ExchangeBiblioWPI();
            //ExchangeIPCWPI();
            ExchangeENhy();


        }
        #endregion
        //static void Main(string[] args)
        //{
        //    #region docdb
        //    //ExchangeBiblio();
        //    //ExchangeIPC();
        //    //ExchangeENhy();
        //    #endregion
        //    //ExchangeCNhy();
        //    //ExchangeIPC();
        //}

        #region  wpi

        public static void ExchangeIPCWPI()
        {
            enDataContext en = new enDataContext();
            long maxid = en.Ipc.Max(x => x.ID);
            long loop = maxid / 1000;
            using (StreamWriter sw = new StreamWriter("d:\\en_ipc1.txt", false, Encoding.UTF8) { AutoFlush = true })
            {
                for (int i = 0; i < loop + 1; i++)
                {
                    if (i * 1000 > maxid) break;
                    en = new enDataContext();
                    int min = i * 1000;
                    int max = (i + 1) * 1000 - 1;
                    var ipcs = en.Ipc.Where(x => x.ID >= min && x.ID <= max);
                    int j = 0;
                    foreach (var ipc in ipcs)
                    {
                        string stripc = ipc.IPC1.Replace(" ", "").FormatIPC();
                        sw.WriteLine($"0|{ipc.PubID}|{stripc}|{stripc[0]}|{stripc.Left(3)}|{stripc.Left(4)}|{stripc.Left(7)}|{j}");
                        j++;

                    }
                    Console.WriteLine($"{i} /{loop + 1}  -{DateTime.Now}");
                }
            }


        }

        public static enDataContext en_pri = new enDataContext();
        public static string GetFistrPRCountry(string pubno)
        {
            try
            {
                return en_pri.Priority.Where(x => x.PubID == pubno && x.Sequence == 1).First().PriorityNo.Left(2);
            }
            catch (Exception)
            {
                return "";
            }

        }
        public static void ExchangeBiblioWPI()
        {
            enDataContext en = new enDataContext();
            long maxid = en.DocInfo.Max(x => x.ID);
            long loop = maxid / 1000;
            using (StreamWriter sw = new StreamWriter("D:\\en_pa1.txt", false, Encoding.ASCII),
                   sw_en = new StreamWriter("D:\\en1.txt", false, Encoding.ASCII),
                   sw_en_tiabs = new StreamWriter("D:\\en_biblio1.txt", false, Encoding.ASCII))
            {
                for (int i = 0; i < loop + 1; i++)
                {

                    if (i * 1000 > maxid) break;
                    en = new enDataContext();
                    int min = i * 1000;
                    int max = (i + 1) * 1000 - 1;

                    var docinfos = en.DocInfo.Where(x => x.ID >= min && x.ID <= max);
                    foreach (var doc in docinfos)
                    {
                        sw_en.WriteLine($"0|{doc.PubID}|{doc.AppNo}|{doc.PubID.Left(2)}|{GetFistrPRCountry(doc.PubID)}|{doc.AppDate.Left(4).to_i()}|{doc.PubDate.Left(4).to_i()}");
                        sw_en_tiabs.WriteLine($"0|{doc.PubID}|{doc.Title.Left(500).Replace("|", "")}|{doc.Abstract.Left(1000).Replace("|", "")}|{doc.Applicants.Left(500).Replace("|", "")}");
                        string[] pas = doc.Applicants.Split(";&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var strpa in pas)
                        {
                            string[] ary = strpa.Split('(');
                            string cpy = string.Empty;
                            if (ary.Length >= 2)
                            {
                                cpy = ary[1].Replace(")", "").Trim();
                            }
                            sw.WriteLine($"0|{doc.PubID.Left(20).Replace("|", "")}|{strpa.Trim().Left(100).Replace("|", "")}|{cpy}");
                        }

                    }
                    Console.WriteLine($"{i} /{loop + 1}  -{DateTime.Now}");
                }
            }

        }
        #endregion

        #region hy


        public static void ExchangeENhy()
        {
            enDataContext en = new enDataContext();
            int step = 1000;
            long maxid = en.en_ipc.Max(x => x.id);
            long loop = maxid / step;

            using (StreamWriter sw = new StreamWriter("d:\\en_zt.txt", false, Encoding.GetEncoding("utf-8"))) //还是bcp 文件导入快
            {
                for (int i = 0; i < loop + 1; i++)
                {
                    if (i * step > maxid) break;
                    en = new enDataContext();
                    int min = i * step;
                    int max = (i + 1) * step - 1;
                    List<en_ipc> ipcs = en.en_ipc.Where(x => x.id >= min && x.id <= max).ToList<en_ipc>();
                    foreach (var ipc in ipcs)
                    {
                        List<string> ids = hyHelper.GetHyIds(ipc.ipc);
                        foreach (var x in ids)
                        {
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
            cnDataContext cn = new cnDataContext();
            long maxid = cn.cn_ipc.Max(x => x.id);
            long loop = maxid / 1000;


            for (int i = 0; i < loop + 1; i++)
            {
                if (i * 1000 > maxid) break;
                cn = new cnDataContext();
                int min = i * 1000;
                int max = (i + 1) * 1000 - 1;
                List<cn_ipc> ipcs = cn.cn_ipc.Where(x => x.id >= min && x.id <= max).ToList<cn_ipc>();
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
        #endregion

        #region docdb


        public static void ExchangeBiblio()
        {
            enDataContext en = new enDataContext();
            long maxid = en.DocInfo.Max(x => x.ID);
            long loop = maxid / 1000;
            using (StreamWriter sw = new StreamWriter("D:\\en_pa.txt", false, Encoding.ASCII) { AutoFlush = true },
                sw_en = new StreamWriter("D:\\en.txt", false, Encoding.ASCII) { AutoFlush = true },
                sw_en_tiabs = new StreamWriter("D:\\en_biblio.txt", false, Encoding.ASCII) { AutoFlush = true })
            {
                for (int i = 0; i < loop + 1; i++)
                {
                    if (i * 1000 > maxid) break;
                    en = new enDataContext();
                    int min = i * 1000;
                    int max = (i + 1) * 1000 - 1;
                    var docinfos = en.DocInfo.Where(x => x.ID >= min && x.ID <= max);
                    foreach (var doc in docinfos)
                    {
                        sw_en.WriteLine($"0|{doc.PubID}|{doc.AppNo}|{doc.PubID.Left(2)}|{doc.ApplicantCountry.Left(2)}|{doc.AppDate.Left(4).to_i()}|{doc.PubDate.Left(4).to_i()}");
                        sw_en_tiabs.WriteLine($"0|{doc.PubID}|{doc.Title.Left(500).Replace("|", "")}|{doc.Abstract.Left(1000).Replace("|", "")}|{doc.Applicants.Left(500).Replace("|", "")}");
                        string[] pas = doc.Applicants.Split(";&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        foreach (var strpa in pas)
                        {
                            string[] ary = strpa.Split('(');
                            string cpy = string.Empty;
                            if (ary.Length >= 2)
                            {
                                cpy = ary[1].Replace(")", "").Trim();
                            }
                            sw.WriteLine($"0|{doc.PubID.Left(20).Replace("|", "")}|{strpa.Trim().Left(100).Replace("|", "")}|{cpy}");
                        }
                    }
                    Console.WriteLine($"{i} /{loop + 1}  -{DateTime.Now}");
                }
            }

        }
        #endregion


        #region fix i_c is null bug
        static void fixbug()
        {
            enDataContext en = new enDataContext();
            var ens = en.en.Where(x => x.i_c == "");
            int i = 0;
            foreach (var e in ens)
            {
                i++;
                var pr = en.Priority.Where(x => x.PubID == e.pn && x.Sequence == 1).FirstOrDefault();
                if (pr != null)
                {
                    string ic = pr.PriorityNo.Left(2);
                    e.i_c = ic;
                    if (i % 100 == 0)
                    {
                        en.SubmitChanges();
                    }
                    Console.WriteLine($"{i}\t{e.pn}\t{ic}");

                }
                else
                {
                    Console.WriteLine($"{i}\t{e.pn}");
                }


            }
            en.SubmitChanges();
        }
        #endregion


        public static void ExchangeIPC()
        {
            enDataContext en = new enDataContext();
            long maxid = en.Ipc.Max(x => x.ID);
            long loop = maxid / 1000;
            using (StreamWriter sw = new StreamWriter("d:\\en_ipc.txt", false, Encoding.UTF8) { AutoFlush = true })
            {
                for (int i = 0; i < loop + 1; i++)
                {
                    if (i * 1000 > maxid) break;
                    en = new enDataContext();
                    int min = i * 1000;
                    int max = (i + 1) * 1000 - 1;
                    var ipcs = en.Ipc.Where(x => x.ID >= min && x.ID <= max);
                    int j = 0;
                    foreach (var ipc in ipcs)
                    {
                        string stripc = ipc.IPC1.FormatIPC();
                        sw.WriteLine($"0|{ipc.PubID}|{stripc}|{stripc[0]}|{stripc.Left(3)}|{stripc.Left(4)}|{stripc.Left(7)}|{j}");
                        j++;

                    }
                    Console.WriteLine($"{i} /{loop + 1}  -{DateTime.Now}");
                }
            }


        }
    }

}
