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
using ST_2017.en;

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
                lstST.Add(new cn_09());
                #endregion

            }
            if (config.Type.ToUpper() == "EN")
            {
                #region CN
                lstST.Add(new en_101());
                lstST.Add(new en_102());
                lstST.Add(new en_103());
                lstST.Add(new en_104());
                lstST.Add(new en_105());
                lstST.Add(new en_106());
                lstST.Add(new en_107());
                lstST.Add(new en_108());
                lstST.Add(new en_109());
                lstST.Add(new en_110());
                lstST.Add(new en_111());
                lstST.Add(new en_112());
                lstST.Add(new en_113());
                lstST.Add(new en_114());
                lstST.Add(new en_115());                
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




        public void Dispose()
        {
            xbook = null;
            lstST.Clear();
            lstST = null;
        }
    }
}
