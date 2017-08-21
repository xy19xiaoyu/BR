using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection; // 引用这个才能使用Missing字段

using System.IO;
using System.Diagnostics;
using System.Web;
using System.Threading;
using ST_2017.Interface;
using System.Data;
using xyExtensions;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using ST_2017.cn;

namespace ST_2017
{
    public class StateManager : IDisposable
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger("st.14");
        public static XSSFWorkbook xbook;
        private List<ILGState> lstST = new List<ILGState>();
        private static string SavePath = System.Configuration.ConfigurationManager.AppSettings["SavePath"].ToString();
        public string Subdir = "";
        private string dir = SavePath;
        private StatConfig config;
        private string filename;

        public StateManager(StatConfig config)
        {
            filename = config.Dir + "\\" + config.FileName;
            xbook = new XSSFWorkbook();

            this.config = config;
            if (config.Type.ToUpper() == "CN")
            {
                #region CN
                lstST.Add(new cn_01());
                lstST.Add(new cn_02());
                lstST.Add(new cn_03());
                lstST.Add(new cn_04());
                lstST.Add(new cn_05());
                lstST.Add(new cn_06());
                lstST.Add(new cn_07());
                lstST.Add(new cn_08());
                #endregion

            }




        }
        public bool State()
        {
            foreach (var x in lstST)
            {
                if (!config.Tables.Contains(x.Id))
                {
                    continue;
                }
                x.config = config;
                x.Sate(config.ZTNames.to_s(','));
                x.OutPut2Worksheet(xbook);
                x.Dispose();
            }
            Save();

            return true;
        }
        private bool Save()
        {
            string savepath;
            string path = "";
            if (dir == "")
            {
                path = SavePath;
            }
            else
            {
                path = dir;

            }
            savepath = path + filename + ".xlsx"; ;

            if (!Directory.Exists(Path.GetDirectoryName(savepath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savepath));
            }
            if (File.Exists(savepath))
            {
                File.Delete(savepath);
            }
            using (FileStream fs = new FileStream(savepath, FileMode.Create, FileAccess.Write))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    xbook.Write(ms);
                    ms.Flush();
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
            Console.WriteLine("OK");
            return true;
        }

        public static DataTable ReadDateTable(DataTable dt)
        {
            int pryear = 0;
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i]["年代"].ToString().Trim() == string.Empty)
                {
                    dt.Rows.RemoveAt(i);
                }
                else
                {
                    int year = Convert.ToInt32(dt.Rows[i]["年代"].ToString());
                    if (pryear == 0)
                    {
                        pryear = year;
                        continue;
                    }
                    for (int j = pryear + 1; j < year; j++)
                    {
                        DataRow tmprow = dt.NewRow();
                        tmprow[0] = j;
                        for (int m = 1; m < dt.Columns.Count; m++)
                        {
                            tmprow[m] = 0;
                        }
                        dt.Rows.InsertAt(tmprow, i + 1);
                    }
                    pryear = year;
                }
            }
            return dt;
        }

        public static DataTable AddSumRow(DataTable dt)
        {

            DataRow row1 = dt.NewRow();

            for (int j = 1; j < dt.Columns.Count; j++)
            {
                double count1 = 0;
                //if (dt.Columns[j].DataType == typeof(String)) continue;
                try
                {
                    foreach (DataRow tmp in dt.Rows)
                    {
                        double tmpd = 0d;
                        double.TryParse(tmp[j].ToString(), out tmpd);
                        count1 += tmpd;
                    }
                    row1[j] = count1;
                }
                catch (Exception e)
                {
                    row1[j] = null;
                }

            }
            row1[0] = "合计：";
            dt.Rows.Add(row1);
            return dt;
        }

        public static DataTable TopNumRow(DataTable dt, int TOPNum)
        {
            DataRow row = dt.NewRow();
            row[0] = "其它";
            double count = 0;
            for (int j = 1; j < dt.Columns.Count; j++)
            {
                try
                {
                    for (int i = dt.Rows.Count - 1; i >= TOPNum; i--)
                    {
                        count += Convert.ToDouble(dt.Rows[i][j].ToString());

                    }

                    row[j] = count;
                }
                catch (Exception e)
                {
                    row[j] = "";
                }
            }
            for (int i = dt.Rows.Count - 1; i >= TOPNum; i--)
            {

                dt.Rows.RemoveAt(i);
            }
            dt.Rows.Add(row);
            return dt;

        }



        public void Dispose()
        {
            xbook = null;
            lstST.Clear();
            lstST = null;
        }
    }
}
