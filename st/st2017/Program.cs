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
            //    Tables = new List<int>() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113 },
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
            //    Tables = new List<int>() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113 },
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
                Tables = new List<int>() { 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113 },
                ZLTypes = new List<string>(),
                ZTNames = new List<string>(),
                Years = new List<string>() { "1997", "1998", "1999", "2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009", "2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017" },
                Top5Guojia = new List<string>() { "EP", "DE", "CN", "US", "JP" }
            });

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
//static void Main(string[] args)
//        {
//            string sql = "select cn_lg.id,cn_lg.age,cn_lg.dead_year,cn_lg.ValidStatus,cn.ady from cn_lg,cn where cn_lg.an = cn.an and cn_lg.id between {0} and {1}";

//            for (int i = 0; i < 11133448; i += 1000)
//            {
//                string exe_sql = string.Format(sql, i, i + 999);
//                DataTable dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);

//                for (int j = 0; j < dt.Rows.Count; j++)
//                {
//                    string id = dt.Rows[j]["id"].ToString();
//                    int ady = dt.Rows[j]["ady"].ToString().to_i();
//                    int deadyear = dt.Rows[j]["dead_year"].ToString().to_i();
//                    string st = dt.Rows[j]["ValidStatus"].ToString();
//                    int age = 0;
//                    int age13 = 0;
//                    int age16 = 0;

//                    if (st == "有效")
//                    {
//                        age = deadyear - ady;
//                    }
//                    else
//                    {
//                        age = dt.Rows[j]["age"].to_i();
//                    }

//                    if (deadyear > 2013)
//                    {
//                        age13 = 2013 - ady;
//                    }
//                    else
//                    {
//                        age13 = age;
//                    }
//                    if (deadyear > 2016)
//                    {
//                        age16 = 2016 - ady;
//                    }
//                    else
//                    {
//                        age16 = age;
//                    }

//                    string update_sql = string.Format("update cn_lg set age={0},age_2013={1},age_2016={2} where id={3}", age, age13, age16, id);
//                    DBA.SqlDbAccess.ExecNoQuery(CommandType.Text, update_sql);

//                    if (j % 100 == 0)
//                    {
//                        Console.WriteLine(i + j);
//                    }

//                }

//            }
//        }