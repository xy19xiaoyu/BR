
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using xyExtensions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.Util;


namespace ST_2017
{
    public class ST_Base : ILGState
    {

        public StatConfig config { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string type { get; set; }
        public string DES { get; set; }
        public DataTable dt { get; set; }
        public ICellStyle dateStyle;
        public XSSFFont columnhead_font;
        public XSSFCellStyle column_headStyle;
        public XSSFCellStyle valueStyle;
        public XSSFCellStyle valueStyle_left;
        public ST_Base()
        {


        }

        public virtual bool Sate(string ztName)
        {
            throw new NotImplementedException("此方法必须强制重载！");
        }
        public void IniStryle(XSSFWorkbook xbook)
        {
            if (dateStyle == null)
            {

                dateStyle = xbook.CreateCellStyle();
                IDataFormat format = xbook.CreateDataFormat();
                dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                #region 字体

                XSSFFont columnhead_font = xbook.CreateFont() as XSSFFont;
                columnhead_font.FontHeightInPoints = 12;
                columnhead_font.Boldweight = (short)FontBoldWeight.Bold;

                XSSFFont value_font = xbook.CreateFont() as XSSFFont;
                value_font.FontHeightInPoints = 12;
                #endregion

                #region 样式

                column_headStyle = xbook.CreateCellStyle() as XSSFCellStyle;
                column_headStyle.Alignment = HorizontalAlignment.Center;
                column_headStyle.VerticalAlignment = VerticalAlignment.Center;
                column_headStyle.BorderTop = BorderStyle.Thin;
                column_headStyle.BorderLeft = BorderStyle.Thin;
                column_headStyle.BorderRight = BorderStyle.Thin;
                column_headStyle.BorderBottom = BorderStyle.Thin;
                column_headStyle.LeftBorderColor = HSSFColor.Black.Index;
                column_headStyle.TopBorderColor = HSSFColor.Black.Index;
                column_headStyle.RightBorderColor = HSSFColor.Black.Index;
                column_headStyle.TopBorderColor = HSSFColor.Black.Index;
                column_headStyle.SetFont(columnhead_font);

                valueStyle = xbook.CreateCellStyle() as XSSFCellStyle;
                valueStyle.Alignment = HorizontalAlignment.Center;
                valueStyle.VerticalAlignment = VerticalAlignment.Center;
                valueStyle.BorderTop = BorderStyle.Thin;
                valueStyle.BorderLeft = BorderStyle.Thin;
                valueStyle.BorderRight = BorderStyle.Thin;
                valueStyle.BorderBottom = BorderStyle.Thin;
                valueStyle.LeftBorderColor = HSSFColor.Black.Index;
                valueStyle.TopBorderColor = HSSFColor.Black.Index;
                valueStyle.RightBorderColor = HSSFColor.Black.Index;
                valueStyle.TopBorderColor = HSSFColor.Black.Index;
                valueStyle.SetFont(value_font);


                valueStyle_left = xbook.CreateCellStyle() as XSSFCellStyle;
                valueStyle_left.Alignment = HorizontalAlignment.Left;
                valueStyle_left.VerticalAlignment = VerticalAlignment.Center;
                valueStyle_left.BorderTop = BorderStyle.Thin;
                valueStyle_left.BorderLeft = BorderStyle.Thin;
                valueStyle_left.BorderRight = BorderStyle.Thin;
                valueStyle_left.BorderBottom = BorderStyle.Thin;
                valueStyle.LeftBorderColor = HSSFColor.Black.Index;
                valueStyle_left.TopBorderColor = HSSFColor.Black.Index;
                valueStyle_left.RightBorderColor = HSSFColor.Black.Index;
                valueStyle_left.TopBorderColor = HSSFColor.Black.Index;
                valueStyle_left.SetFont(value_font);
                #endregion
            }
        }
        public virtual bool OutPut2Worksheet(XSSFWorkbook xbook)
        {

            ISheet sheet = xbook.CreateSheet(this.Name);
            IniStryle(xbook);


            if (dt != null)
            {
                //取得列宽
                int[] arrColWidth = new int[dt.Columns.Count];
                foreach (DataColumn item in dt.Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                    arrColWidth[item.Ordinal] = arrColWidth[item.Ordinal] > 50 ? 50 : arrColWidth[item.Ordinal];
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        int intTemp = Encoding.GetEncoding(936).GetBytes(dt.Rows[i][j].ToString()).Length;
                        if (intTemp > arrColWidth[j] && intTemp < 50)
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }
                int rowIndex = 0;
                XSSFRow column_headerRow = sheet.CreateRow(rowIndex) as XSSFRow;
                column_headerRow.HeightInPoints = 21;
                foreach (DataColumn column in dt.Columns)
                {
                    column_headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                    column_headerRow.GetCell(column.Ordinal).CellStyle = column_headStyle;
                    sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 4) * 256);
                }
                rowIndex++;
                foreach (DataRow row in dt.Rows)
                {

                    #region 填充内容
                    IRow dataRow = sheet.CreateRow(rowIndex);
                    int columnIndex = 0;
                    foreach (DataColumn column in dt.Columns)
                    {
                        ICell newCell = dataRow.CreateCell(column.Ordinal);
                        newCell.CellStyle = valueStyle;
                        if (columnIndex == 0)
                        {
                            newCell.CellStyle = valueStyle_left;
                        }
                        else
                        {
                            newCell.CellStyle = valueStyle;
                        }
                        columnIndex++;
                        string drValue = row[column].ToString();

                        switch (column.DataType.ToString())
                        {
                            case "System.String"://字符串类型
                                newCell.SetCellValue(drValue);
                                break;
                            case "System.DateTime"://日期类型
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);

                                newCell.CellStyle = dateStyle;//格式化显示
                                break;
                            case "System.Boolean"://布尔型
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                newCell.SetCellValue(boolV);
                                break;
                            case "System.Int16"://整型
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                newCell.SetCellValue(intV);
                                break;
                            case "System.Decimal"://浮点型
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                newCell.SetCellValue(doubV);
                                break;
                            case "System.DBNull"://空值处理
                                newCell.SetCellValue("");
                                break;
                            default:
                                newCell.SetCellValue("");
                                break;
                        }

                    }
                    #endregion

                    rowIndex++;
                }

            }
            return true;
        }
        public string[,] getgen(Dictionary<string, int> ipc02, Dictionary<string, int> ipc13)
        {
            Dictionary<string, double> allgen = new Dictionary<string, double>();
            foreach (var x in ipc13)
            {
                string key = x.Key;
                int v13 = x.Value;
                int v02 = 0;

                if (ipc02.ContainsKey(key))
                {
                    v02 = ipc02[key];
                }
                double gen = 1.0 / 12;
                double zs = 0;
                if (v02 != 0)
                {
                    double num = Convert.ToInt32(v13) / v02;
                    zs = System.Math.Pow(num, gen) - 1;
                }
                else
                {
                    zs = 0;
                }
                allgen.Add(key, zs);
            }
            var xy = from y in allgen
                     orderby y.Value descending
                     select y;
            string[,] result = new string[xy.Count() + 1, 5];
            result[0, 0] = "排名";
            result[0, 2] = "平均增速";
            result[0, 1] = "IPC";
            int i = 1;
            foreach (var item in xy)
            {
                result[i, 0] = i.ToString();
                result[i, 1] = item.Key;
                result[i, 2] = item.Value.ToString();
                i++;
            }
            return result;
        }
        public string[,] getgen(Dictionary<string, List<string>> ipcall, int top)
        {
            Dictionary<string, double> allgen = new Dictionary<string, double>();
            foreach (var x in ipcall)
            {
                string key = x.Key;
                string first = x.Value.First();
                string last = x.Value.Last();

                string[] aryf = first.Split(',');
                string[] aryl = last.Split(',');
                int vf = Convert.ToInt32(aryf[0]);
                int yf = Convert.ToInt32(aryf[1]);
                int vl = Convert.ToInt32(aryl[0]);
                int yl = Convert.ToInt32(aryl[1]);
                int years = yl - yf;
                if (years != 0)
                {
                    double gen = 1.0 / years;
                    double num = Convert.ToDouble(vl) / vf;
                    double zs = System.Math.Pow(num, gen) - 1;
                    allgen.Add(key, zs);
                }
                else
                {
                    allgen.Add(key, -0.9999);
                }
            }
            var xy = from y in allgen
                     orderby y.Value descending
                     select y;

            int count = xy.Count();
            if (count >= top) count = top;
            string[,] result = new string[count + 1, 5];
            result[0, 0] = "排名";
            result[0, 1] = "IPC";
            result[0, 2] = "平均增速";
            result[0, 3] = "数量";
            result[0, 4] = "占比";
            int i = 1;
            foreach (var item in xy)
            {
                result[i, 0] = i.ToString();
                result[i, 1] = item.Key;
                result[i, 2] = item.Value.ToString("0.00%");
                i++;
                if (i > top) break;
            }
            return result;
        }
        public Dictionary<string, double> getgen(Dictionary<string, List<string>> zlall)
        {
            Dictionary<string, double> allgen = new Dictionary<string, double>();
            foreach (var x in zlall)
            {
                string key = x.Key;
                if (x.Value.Count == 0)
                {
                    allgen.Add(key, -0.9999);
                }
                else
                {
                    string first = x.Value.First();
                    string last = x.Value.Last();

                    string[] aryf = first.Split(',');
                    string[] aryl = last.Split(',');
                    int vf = Convert.ToInt32(aryf[0]);
                    int yf = Convert.ToInt32(aryf[1]);
                    int vl = Convert.ToInt32(aryl[0]);
                    int yl = Convert.ToInt32(aryl[1]);
                    int years = yl - yf;
                    if (years != 0)
                    {
                        double gen = 1.0 / years;
                        double num = Convert.ToDouble(vl) / vf;
                        double zs = System.Math.Pow(num, gen) - 1;
                        allgen.Add(key, zs);
                    }
                    else
                    {
                        allgen.Add(key, -0.9999);
                    }
                }
            }
            return allgen;

        }
        public double GetZZL(Dictionary<int, int> all)
        {
            if (all == null) return -0.9999d;
            if (all.Count == 0) return -0.9999d;
            double dzzl = 0d;
            int vf = all.First().Value;
            int yf = all.First().Key;
            int vl = all.Last().Value;
            int yl = all.Last().Value;
            int years = yl - yf;
            if (years != 0)
            {
                double gen = 1.0 / years;
                double num = Convert.ToDouble(vl) / vf;
                dzzl = System.Math.Pow(num, gen) - 1;

            }
            else
            {
                dzzl = -0.9999d;
            }
            return dzzl;
        }
        public string GetZZL1(Dictionary<int, int> all)
        {
            //do
            //{
            //    var first= all.First();
            //    if (first.Value == 0)
            //    {
            //        all.Remove(first.Key);
            //    }
            //    else
            //    {
            //        break;
            //    }
            //} while (true);
            if (all == null) return "N/A";
            if (all.Count == 0) return "N/A";

            double dzzl = 0d;
            if (all.Where(x => x.Value > 0).Count() <= 1) return "N/A";
            int vf = all.Where(x => x.Value > 0).First().Value;
            int yf = all.Where(x => x.Value > 0).First().Key;
            int vl = all.Where(x => x.Value > 0).Last().Value;
            int yl = all.Where(x => x.Value > 0).Last().Key;
            int years = yl - yf;
            if (years != 0)
            {
                double gen = 1.0 / years;
                double num = Convert.ToDouble(vl) / vf;
                dzzl = System.Math.Pow(num, gen) - 1;

            }
            else
            {
                return "N/A";
            }
            return dzzl.ToString();
        }
        public static string NumbertoString(int n)
        {
            string s = "";     // result  
            int r = 0;         // remainder  

            while (n != 0)
            {
                r = n % 26;
                char ch = ' ';
                if (r == 0)
                    ch = 'Z';
                else
                    ch = (char)(r - 1 + 'A');
                s = ch.ToString() + s;
                if (s[0] == 'Z')
                    n = n / 26 - 1;
                else
                    n /= 26;
            }
            return s;
        }

        public virtual void Dispose()
        {
            dt = null;
        }
        public virtual string GetFilter()
        {
            string where = "";
            if (!string.IsNullOrEmpty(config.GuoJia))
            {
                where += " and cn.guojia in(" + config.GuoJia + ")";
            }

            if (!string.IsNullOrEmpty(config.Sheng))
            {

                where += " and cn.sheng in(" + config.Sheng + ")";
            }
            if (!string.IsNullOrEmpty(config.Shi))
            {
                where += " and cn.Shi in (" + config.Shi + ")";
            }
            if (!string.IsNullOrEmpty(config.strQuXian))
            {
                where += " and cn.Xian in (" + config.strQuXian + ")";
            }
            if (!string.IsNullOrEmpty(config.strQuYu))
            {
                where += " and cn.QuYu in(" + config.strQuYu + ")";
            }

            if (!string.IsNullOrEmpty(config.zltype))
            {
                where += " and cn.type in(" + config.zltype + ")";
            }
            if (!string.IsNullOrEmpty(config.strYears))
            {
                where += " and years.y in(" + config.strYears + ")";
            }
            return where;
        }
        public virtual string GetFilter1()
        {
            string where = "";
            if (!string.IsNullOrEmpty(config.GuoJia))
            {
                where += " and cn.guojia in(" + config.GuoJia + ")";
            }

            if (!string.IsNullOrEmpty(config.Sheng))
            {

                where += " and cn.sheng in(" + config.Sheng + ")";
            }
            if (!string.IsNullOrEmpty(config.Shi))
            {
                where += " and cn.Shi in (" + config.Shi + ")";
            }
            if (!string.IsNullOrEmpty(config.strQuXian))
            {
                where += " and cn.Xian in (" + config.strQuXian + ")";
            }
            if (!string.IsNullOrEmpty(config.strQuYu))
            {
                where += " and cn.QuYu in(" + config.strQuYu + ")";
            }
            return where;
        }

        public virtual string GetFilter2()
        {
            string where = "";
            if (!string.IsNullOrEmpty(config.GuoJia))
            {
                where += " and cn.guojia in(" + config.GuoJia + ")";
            }

            if (!string.IsNullOrEmpty(config.Sheng))
            {
                where += " and cn.sheng in(" + config.Sheng + ")";
            }
            if (!string.IsNullOrEmpty(config.Shi))
            {
                where += " and cn.Shi in (" + config.Shi + ")";
            }
            if (!string.IsNullOrEmpty(config.strQuXian))
            {
                where += " and cn.Xian in (" + config.strQuXian + ")";
            }
            if (!string.IsNullOrEmpty(config.strQuYu))
            {
                where += " and cn.QuYu in(" + config.strQuYu + ")";
            }

            if (!string.IsNullOrEmpty(config.zltype))
            {
                where += " and cn.type in(" + config.zltype + ")";
            }

            if (!string.IsNullOrEmpty(config.strPas))
            {
                where += " and cn_pa.pa in(" + config.strPas + ")";
            }
            if (!string.IsNullOrEmpty(config.strYears))
            {
                where += " and years.y in(" + config.strYears + ")";
            }
            return where;
        }

        public virtual string GetFilter3()
        {
            string where = "";

            if (!string.IsNullOrEmpty(config.strYears))
            {
                where += " and years.y in(" + config.strYears + ")";
            }
            return where;
        }
        public virtual string GetFilter5_1()
        {
            string where = "";
            if (!string.IsNullOrEmpty(config.GuoJia))
            {
                where += " and cn.guojia in(" + config.GuoJia + ")";
            }

            if (!string.IsNullOrEmpty(config.Sheng))
            {
                where += " and cn.sheng in(" + config.Sheng + ")";
            }
            if (!string.IsNullOrEmpty(config.Shi))
            {
                where += " and cn.Shi in (" + config.Shi + ")";
            }
            if (!string.IsNullOrEmpty(config.strQuXian))
            {
                where += " and cn.Xian in (" + config.strQuXian + ")";
            }
            if (!string.IsNullOrEmpty(config.strQuYu))
            {
                where += " and cn.QuYu in(" + config.strQuYu + ")";
            }

            if (!string.IsNullOrEmpty(config.zltype))
            {
                where += " and cn.type in(" + config.zltype + ")";
            }

            if (!string.IsNullOrEmpty(config.strPas))
            {
                where += " and cn_pa.pa in(" + config.strPas + ")";
            }
            return where;
        }
        public virtual string GetFilter5()
        {
            string where = "";
            if (!string.IsNullOrEmpty(config.GuoJia))
            {
                where += " and cn.guojia in(" + config.GuoJia + ")";
            }

            if (!string.IsNullOrEmpty(config.Sheng))
            {
                where += " and cn.sheng in(" + config.Sheng + ")";
            }
            if (!string.IsNullOrEmpty(config.Shi))
            {
                where += " and cn.Shi in (" + config.Shi + ")";
            }
            if (!string.IsNullOrEmpty(config.strQuXian))
            {
                where += " and cn.Xian in (" + config.strQuXian + ")";
            }
            if (!string.IsNullOrEmpty(config.strQuYu))
            {
                where += " and cn.QuYu in(" + config.strQuYu + ")";
            }

            if (!string.IsNullOrEmpty(config.zltype))
            {
                where += " and cn.type in(" + config.zltype + ")";
            }

            if (!string.IsNullOrEmpty(config.strPas))
            {
                where += " and cn_pa.pa in(" + config.strPas + ")";
            }
            if (!string.IsNullOrEmpty(config.strYears))
            {
                where += " and cn.ady in(" + config.strYears + ")";
            }
            return where;
        }

        public string GetZZl(string sheng, string ipc, string ztname, string adys, out string zzl)
        {

            string sqlmin = "";

            sqlmin = string.Format("select ady, count(*) as 数量 from cn_zt as a,cn as b,cn_ipc as c where a.an = b.an  and a.an = c.an and a.ztname in ('{0}') and b.ady in({1}) and c.ipc3='{2}' and b.sheng in('{3}') group by b.ady order by b.ady", ztname, adys, ipc, sheng);


            DataTable adsums = DBA.SqlDbAccess.GetDataTable(CommandType.Text, sqlmin);
            List<int> sums = new List<int>();
            List<int> values = new List<int>();
            List<int> lstyear = new List<int>();

            for (int i = 2009; i <= 2013; i++)
            {
                var x = from tmp in adsums.AsEnumerable()
                        where tmp["ady"].ToString() == i.ToString()
                        select tmp["数量"].ToString();
                if (x.Count() > 0)
                {
                    sums.Add(Convert.ToInt32(x.First()));
                    lstyear.Add(i);
                    values.Add(Convert.ToInt32(x.First()));
                }
                else
                {
                    sums.Add(0);
                }
            }
            if (adsums.Rows.Count > 1)
            {
                zzl = "";
                int vf = values.First();
                int yf = lstyear.First();
                int vl = values.Last();
                int yl = lstyear.Last();
                int years = yl - yf;
                if (years != 0)
                {
                    double gen = 1.0 / years;
                    double num = Convert.ToDouble(vl) / vf;
                    double zs = System.Math.Pow(num, gen) - 1;
                    zzl = zs.ToString();
                }
                else
                {
                    zzl = "NA";
                }

            }
            else
            {
                zzl = "NA";
            }
            string strvalues = "";
            foreach (var s in sums)
            {
                strvalues += s + ",";
            }
            strvalues = strvalues.Trim(',');
            return strvalues;
        }
        public string GetZZl_IPC4(string sheng, string ipc, string ztname, string adys, out string zzl)
        {

            string sqlmin = "";

            sqlmin = string.Format("select ady, count(*) as 数量 from cn_zt as a,cn as b,cn_ipc as c where a.an = b.an  and a.an = c.an and a.ztname in ('{0}') and b.ady in({1}) and c.ipc4='{2}' and b.sheng in('{3}') group by b.ady order by b.ady", ztname, adys, ipc, sheng);


            DataTable adsums = DBA.SqlDbAccess.GetDataTable(CommandType.Text, sqlmin);
            List<int> sums = new List<int>();
            List<int> values = new List<int>();
            List<int> lstyear = new List<int>();

            for (int i = 2009; i <= 2013; i++)
            {
                var x = from tmp in adsums.AsEnumerable()
                        where tmp["ady"].ToString() == i.ToString()
                        select tmp["数量"].ToString();
                if (x.Count() > 0)
                {
                    sums.Add(Convert.ToInt32(x.First()));
                    lstyear.Add(i);
                    values.Add(Convert.ToInt32(x.First()));
                }
                else
                {
                    sums.Add(0);
                }
            }
            if (adsums.Rows.Count > 1)
            {
                zzl = "";
                int vf = values.First();
                int yf = lstyear.First();
                int vl = values.Last();
                int yl = lstyear.Last();
                int years = yl - yf;
                if (years != 0)
                {
                    double gen = 1.0 / years;
                    double num = Convert.ToDouble(vl) / vf;
                    double zs = System.Math.Pow(num, gen) - 1;
                    zzl = zs.ToString();
                }
                else
                {
                    zzl = "NA";
                }

            }
            else
            {
                zzl = "NA";
            }
            string strvalues = "";
            foreach (var s in sums)
            {
                strvalues += s + ",";
            }
            strvalues = strvalues.Trim(',');
            return strvalues;
        }
        private Dictionary<string, string> hys = new Dictionary<string, string>();
        public Dictionary<string, string> Hys
        {
            get
            {
                if (hys.Count == 0)
                {
                    hys.Add("1", "农业");
                    hys.Add("2", "林业");
                    hys.Add("3", "畜牧业");
                    hys.Add("4", "渔业");
                    hys.Add("5", "农、林、牧、渔服务业");
                    hys.Add("6", "煤炭开采和洗选业");
                    hys.Add("7", "石油和天然气开采业");
                    hys.Add("8", "黑色金属矿采选业");
                    hys.Add("9", "有色金属矿采选业");
                    hys.Add("10", "非金属矿采选业");
                    hys.Add("11", "开采辅助活动");
                    hys.Add("12", "其他采矿业");
                    hys.Add("13", "农副食品加工业");
                    hys.Add("14", "食品制造业");
                    hys.Add("15", "饮料制造业");
                    hys.Add("16", "烟草制品业");
                    hys.Add("17", "纺织业");
                    hys.Add("18", "纺织服装、鞋、帽制造业");
                    hys.Add("19", "皮革、毛皮、羽毛(绒)及其制品业");
                    hys.Add("20", "木材加工及木、竹、藤、棕、草制品业");
                    hys.Add("21", "家具制造业");
                    hys.Add("22", "造纸及纸制品业");
                    hys.Add("23", "印刷业和记录媒介的复制");
                    hys.Add("24", "文教体育用品制造业");
                    hys.Add("25", "精炼石油产品的制造");
                    hys.Add("26", "化学原料及化学制晶制造业");
                    hys.Add("27", "医药制造业");
                    hys.Add("28", "化学纤维制造业");
                    hys.Add("29", "橡胶和塑料制品业");
                    hys.Add("30", "非全属矿物制品业");
                    hys.Add("31", "黑色金属冶炼及压延加工业");
                    hys.Add("32", "有色金属冶炼及压延加工业");
                    hys.Add("33", "金属制品业");
                    hys.Add("34", "通用设备制造业");
                    hys.Add("35", "专用设备制造业");
                    hys.Add("36", "汽车制造业");
                    hys.Add("37", "火车、船舶、航天等运输设备制造业");
                    hys.Add("38", "电气机械及器材制造业");
                    hys.Add("39", "通信设备、计算机及其他电子设备制造业");
                    hys.Add("40", "仪器仪表制造业");
                    hys.Add("41", "其他制造业");
                    hys.Add("42", "废弃资源综合利用业");
                    hys.Add("43", "金属制品、机械和设备修理业");
                    hys.Add("44", "电力、热力的生产和供应");
                    hys.Add("45", "燃气生产和供应");
                    hys.Add("46", "水的生产和供应");
                    hys.Add("47", "房屋建筑业");
                    hys.Add("48", "土木工程建筑业");
                    hys.Add("49", "建筑安装业");
                    hys.Add("50", "建筑装饰和其他建筑业");

                }
                return hys;
            }
            set { hys = value; }
        }


        private Dictionary<string, string> dicHys = new Dictionary<string, string>();
        public Dictionary<string, string> DicHys
        {
            get
            {
                if (dicHys.Count == 0)
                {   
                    dicHys.Add("01.农业", "'91'");
                    dicHys.Add("02.林业", "'92'");
                    dicHys.Add("03.畜牧业", "'93'");
                    dicHys.Add("04.渔业", "'94'");
                    dicHys.Add("05.农、林、牧、渔服务业", "'95'");
                    dicHys.Add("06.煤炭开采和洗选业", "'4'");
                    dicHys.Add("07.石油和天然气开采业", "'8'");
                    dicHys.Add("08.黑色金属矿采选业", "'12'");
                    dicHys.Add("09.有色金属矿采选业", "'16'");
                    dicHys.Add("10.非金属矿采选业", "'20'");
                    dicHys.Add("11.开采辅助活动", "'21'");
                    dicHys.Add("12.其他采矿业", "'22'");
                    dicHys.Add("13.农副食品加工业", "'25'");
                    dicHys.Add("14.食品制造业", "'26'");
                    dicHys.Add("15.饮料制造业", "'30'");
                    dicHys.Add("16.烟草制品业", "'31'");
                    dicHys.Add("17.纺织业", "'32'");
                    dicHys.Add("18.纺织服装、鞋、帽制造业", "'33'");
                    dicHys.Add("19.皮革、毛皮、羽毛(绒)及其制品业", "'34'");
                    dicHys.Add("20.木材加工及木、竹、藤、棕、草制品业", "'35'");
                    dicHys.Add("21.家具制造业", "'36'");
                    dicHys.Add("22.造纸及纸制品业", "'37'");
                    dicHys.Add("23.印刷业和记录媒介的复制", "'38'");
                    dicHys.Add("24.文教体育用品制造业", "'41'");
                    dicHys.Add("25.精炼石油产品的制造", "'42'");
                    dicHys.Add("26.化学原料及化学制晶制造业", "'47'");
                    dicHys.Add("27.医药制造业", "'48'");
                    dicHys.Add("28.化学纤维制造业", "'49'");
                    dicHys.Add("29.橡胶和塑料制品业", "'195'");
                    dicHys.Add("30.非全属矿物制品业", "'52'");
                    dicHys.Add("31.黑色金属冶炼及压延加工业", "'53'");
                    dicHys.Add("32.有色金属冶炼及压延加工业", "'54'");
                    dicHys.Add("33.金属制品业", "'59'");
                    dicHys.Add("34.通用设备制造业", "'60'");
                    dicHys.Add("35.专用设备制造业", "'66'");
                    dicHys.Add("36.汽车制造业", "'69'");
                    dicHys.Add("37.火车、船舶、航天等运输设备制造业", "'73'");
                    dicHys.Add("38.电气机械及器材制造业", "'76'");
                    dicHys.Add("39.通信设备、计算机及其他电子设备制造业", "'81'");
                    dicHys.Add("40.仪器仪表制造业", "'84'");
                    dicHys.Add("41.其他制造业", "'96'");
                    dicHys.Add("42.废弃资源综合利用业", "'97'");
                    dicHys.Add("43.金属制品、机械和设备修理业", "'98'");
                    dicHys.Add("44.电力、热力的生产和供应", "'85'");
                    dicHys.Add("45.燃气生产和供应", "'89'");
                    dicHys.Add("46.水的生产和供应", "'90'");
                    dicHys.Add("47.房屋建筑业", "'99'");
                    dicHys.Add("48.土木工程建筑业", "'100'");
                    dicHys.Add("49.建筑安装业", "'101'");
                    dicHys.Add("50.建筑装饰和其他建筑业", "'102'");

                    dicHys.Add("工业", "'4','8','12','16','20','21','22','25','26','30','31','32','33','34','35','36','37','38','41','42','47','48','49','195','52','53','54','59','60','66','69','73','76','81','84','97','85','89','90'");
                    dicHys.Add("采矿业", "'4','8','12','16','20','21','22'");
                    dicHys.Add("制造业", "'25','26','30','31','32','33','34','35','36','37','38','41','42','47','48','49','195','50','51','52','53','54','59','60','66','69','73','76','81','84'");
                    dicHys.Add("生产和供应业", "'85','89','90'");
                    dicHys.Add("装备制造业", "'59','60','66','69','73','76','81','84'");                    

                }
                return dicHys;

            }
        }

        public string ztname { get; set; }

        public string zltype
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Sheng
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Shi
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string GuoJia
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string city
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

