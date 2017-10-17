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
            //configs.AddRange(Config1());
            configs.AddRange(ConfigEN());
            foreach (var config in configs)
            {
                StateManager st = new StateManager(config);
                st.State();
                st.Dispose();
                GC.Collect();
            }

        }
        public static List<StatConfig> Config1()
        {
            List<StatConfig> configs = new List<StatConfig>();
            StatConfig c = new StatConfig()
            {
                Type = "cn",
                Dir = "中国",
                FileName = "三国数据",
                GuoJias = new List<string>(),
                Shengs = new List<string>(),
                Tables = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 },
                ZLTypes = new List<string>(),
                ZTNames = new List<string>(),
                Years = new List<string>()
            };

            configs.Add(c);

            return configs;

        }

        public static List<StatConfig> ConfigEN()
        {
            List<StatConfig> configs = new List<StatConfig>();
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

            configs.Add(new StatConfig()
            {
                Type = "EN",
                Dir = "国外",
                FileName = "ID",
                GuoJias = new List<string>() { "ID" },
                Shengs = new List<string>(),
                Tables = new List<int>() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115 },
                ZLTypes = new List<string>(),
                ZTNames = new List<string>(),
                Years = new List<string>() { "1997", "1998", "1999", "2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009", "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017" },
                Top5Guojia = new List<string>() { "EP", "DE", "CN", "US", "JP" }

            });


            //            string top3guojia = @"select top 3 i_c,count(*) as tt from 
            //en 
            //where P_c = '{0}'  and i_c not in ('US','JP','{0}','')
            //group by i_c
            //order by  tt desc
            //";
            //            // string[] guojias = new string[] { "MN", "RU", "KZ", "KG", "TJ", "UZ", "MY", "SG", "PH", "PL", "CZ", "CS", "HU", "SI", "HR", "RO", "BG", "YU", "ME", "BA", "EE", "LT", "LV", "BY", "MD", "TR", "SA", "JO", "IL", "AM", "GE", "EG" };
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