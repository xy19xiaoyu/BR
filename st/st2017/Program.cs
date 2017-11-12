using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using Newtonsoft.Json;
using xyExtensions;


namespace ST_2017
{
    class Program
    {
        private static string SavePath = System.Configuration.ConfigurationManager.AppSettings["SavePath"].ToString();
        private static log4net.ILog log = log4net.LogManager.GetLogger("st.14");

        static void Main(string[] args)
        {


            List<StatConfig> configs = new List<StatConfig>();
            configs.AddRange(ConfigCN());
            // configs.AddRange(ConfigEN());
            foreach (var config in configs)
            {
                StateManager st = new StateManager(config);
                st.State();
                st.Dispose();
                GC.Collect();
            }

        }
        public static List<StatConfig> ConfigCN()
        {
            List<string> gjs = new List<string>();
            gjs.Add("BH");
            gjs.Add("BT");
            gjs.Add("ID");
            gjs.Add("KH");
            gjs.Add("LA");
            gjs.Add("MV");
            gjs.Add("RS");
            gjs.Add("TJ");
            gjs.Add("TM");
            gjs.Add("AL");
            gjs.Add("BA");
            gjs.Add("NP");
            gjs.Add("BU");
            gjs.Add("AZ");
            gjs.Add("KG");
            gjs.Add("MN");
            gjs.Add("OM");
            gjs.Add("SY");
            gjs.Add("YE");
            gjs.Add("AM");
            gjs.Add("BD");
            gjs.Add("AF");
            gjs.Add("IQ");
            gjs.Add("GE");
            gjs.Add("UZ");
            gjs.Add("QA");
            gjs.Add("KW");
            gjs.Add("LB");
            gjs.Add("PK");
            gjs.Add("LT");
            gjs.Add("JO");
            gjs.Add("KZ");
            gjs.Add("LK");
            gjs.Add("RO");
            gjs.Add("BY");
            gjs.Add("IR");
            gjs.Add("EG");
            gjs.Add("LV");
            gjs.Add("EE");
            gjs.Add("VN");
            gjs.Add("BN");
            gjs.Add("BG");
            gjs.Add("SK");
            gjs.Add("HR");
            gjs.Add("PH");
            gjs.Add("UA");
            gjs.Add("SI");
            gjs.Add("TH");
            gjs.Add("PL");
            gjs.Add("HU");
            gjs.Add("TR");
            gjs.Add("SA");
            gjs.Add("CZ");
            gjs.Add("MY");
            gjs.Add("RU");
            gjs.Add("IN");
            gjs.Add("IL");
            gjs.Add("SG");

            List<StatConfig> configs = new List<StatConfig>();
            configs.Add(new StatConfig()
            {
                Type = "cn",
                Dir = "中国",
                FileName = "64国数据",
                GuoJias = gjs,
                Shengs = new List<string>(),
                Tables = new List<int>() { 1, 2, 3, 4, 5 },
                ZLTypes = new List<string>(),
                ZTNames = new List<string>(),
                Years = new List<string>()
            });

            foreach (string gj in gjs)
            {
                configs.Add(new StatConfig()
                {
                    Type = "cn",
                    Dir = "中国",
                    FileName = gj + "-IPC小类分布",
                    GuoJias = new List<string>() { gj },
                    Shengs = new List<string>(),
                    Tables = new List<int>() { 9 },
                    ZLTypes = new List<string>(),
                    ZTNames = new List<string>(),
                    Years = new List<string>()
                });
            }

            return configs;

        }

        public static List<StatConfig> ConfigEN()
        {
            List<StatConfig> configs = new List<StatConfig>();
            List<string> gjs = new List<string>();
            gjs.Add("AL");
            gjs.Add("AM");
            gjs.Add("BA");
            gjs.Add("BG");
            gjs.Add("BY");
            gjs.Add("CZ");
            gjs.Add("EE");
            gjs.Add("EG");
            gjs.Add("GE");
            gjs.Add("HR");
            gjs.Add("HU");
            gjs.Add("ID");
            gjs.Add("IL");
            gjs.Add("JO");
            gjs.Add("KG");
            gjs.Add("KZ");
            gjs.Add("LT");
            gjs.Add("LV");
            gjs.Add("MN");
            gjs.Add("MY");
            gjs.Add("PH");
            gjs.Add("PL");
            gjs.Add("RO");
            gjs.Add("RS");
            gjs.Add("RU");
            gjs.Add("SA");
            gjs.Add("SG");
            gjs.Add("SI");
            gjs.Add("TH");
            gjs.Add("TJ");
            gjs.Add("TM");
            gjs.Add("TR");
            gjs.Add("UA");
            gjs.Add("UZ");
            gjs.Add("VN");
            gjs = new List<string>() { "IN" };
            foreach (var gj in gjs)
            {
                configs.Add(new StatConfig()
                {
                    Type = "EN",
                    Dir = "国外",
                    FileName = gj,
                    GuoJias = new List<string>() { gj },
                    Shengs = new List<string>(),
                    Tables = new List<int>() { 114, 115 },
                    ZLTypes = new List<string>(),
                    ZTNames = new List<string>(),
                    Years = new List<string>() { "1997", "1998", "1999", "2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009", "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017" },
                    Top5Guojia = new List<string>()
                });

            }

            //configs.Add(new StatConfig()
            //{
            //    Type = "EN",
            //    Dir = "国外",
            //    FileName = "UA",
            //    GuoJias = new List<string>() { "UA" },
            //    Shengs = new List<string>(),
            //    Tables = new List<int>() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115 },
            //    ZLTypes = new List<string>(),
            //    ZTNames = new List<string>(),
            //    Years = new List<string>() { "1997", "1998", "1999", "2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009", "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017" },
            //    Top5Guojia = new List<string>() { "DE", "FR", "CH", "US", "JP" }
            //});

            //configs.Add(new StatConfig()
            //{
            //    Type = "EN",
            //    Dir = "国外",
            //    FileName = "RS",
            //    GuoJias = new List<string>() { "RS" },
            //    Shengs = new List<string>(),
            //    Tables = new List<int>() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115 },
            //    ZLTypes = new List<string>(),
            //    ZTNames = new List<string>(),
            //    Years = new List<string>() { "1997", "1998", "1999", "2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009", "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017" },
            //    Top5Guojia = new List<string>() { "DE", "CH", "FR", "US", "JP" }

            //});

            //configs.Add(new StatConfig()
            //{
            //    Type = "EN",
            //    Dir = "国外",
            //    FileName = "ID",
            //    GuoJias = new List<string>() { "ID" },
            //    Shengs = new List<string>(),
            //    Tables = new List<int>() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115 },
            //    ZLTypes = new List<string>(),
            //    ZTNames = new List<string>(),
            //    Years = new List<string>() { "1997", "1998", "1999", "2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009", "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017" },
            //    Top5Guojia = new List<string>() { "EP", "DE", "CN", "US", "JP" }

            //});



            //            string top3guojia = @"select top 3 i_c,count(*) as tt from 
            //en 
            //where P_c = '{0}'  and i_c not in ('US','JP','{0}','')
            //group by i_c
            //order by  tt desc
            //";
            //            // string[] guojias = new string[] {"IN","MN", "RU", "KZ", "KG", "TJ", "UZ", "MY", "SG", "PH", "PL", "CZ", "CS", "HU", "SI", "HR", "RO", "BG", "YU", "ME", "BA", "EE", "LT", "LV", "BY", "MD", "TR", "SA", "JO", "IL", "AM", "GE", "EG" };
            //            string[] guojias = new string[] { "IN" };
            //            foreach (var guojia in guojias)
            //            {
            //                DataTable top3gj = DBA.SqlDbAccess.GetDataTable(CommandType.Text, string.Format(top3guojia, guojia));
            //                List<string> lstGuoJia = new List<string>();

            //                if (top3gj.Rows.Count > 0)
            //                {
            //                    foreach (DataRow row in top3gj.Rows)
            //                    {
            //                        lstGuoJia.Add(row["i_c"].ToString());
            //                    }
            //                }
            //                lstGuoJia.Add("US");
            //                lstGuoJia.Add("JP");

            //                configs.Add(new StatConfig()
            //                {
            //                    Type = "EN",
            //                    Dir = "国外",
            //                    FileName = guojia,
            //                    GuoJias = new List<string>() { guojia },
            //                    Shengs = new List<string>(),
            //                    Tables = new List<int>() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113 },
            //                    ZLTypes = new List<string>(),
            //                    ZTNames = new List<string>(),
            //                    Years = new List<string>() { "1997", "1998", "1999", "2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009", "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017" },
            //                    Top5Guojia = lstGuoJia
            //                });
            //            }





            return configs;

        }



    }

    public class SSConfig
    {
        public string name;
        public List<string> shis;
        public string cfgName;
        public List<string> lstpa;
    }

}