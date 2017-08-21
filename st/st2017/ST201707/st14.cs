
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using xyExtensions;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace ST_2017.ST201707
{
    public class st14 : ST_Base
    {

        public st14()
        {
            this.Id = 14;
            this.Name = "14-医药制造业IPC大类";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            return true;
        }
        public override bool OutPut2Worksheet(XSSFWorkbook xbook)
        {
            string[] heads = new string[] { "行业", "ipc", "有效专利量", "2002", "2008", "2012", "2016", "增速" };
            Dictionary<string, string> hys = new Dictionary<string, string>();
            hys.Add("48", "27.医药制造业");

            Console.WriteLine("开始出表：{0}\t{1} ", Name, ztname);

            #region 生成表单逻辑

            ISheet sheet = xbook.CreateSheet(this.Name);
            IniStryle(xbook);


            #region 第一行
            int rowIndex = 0;
            XSSFRow head2 = sheet.CreateRow(rowIndex) as XSSFRow;
            head2.HeightInPoints = 21;
            int col_index = 0;
            for (int i = 0; i < heads.Length; i++)
            {
                head2.CreateCell(col_index).SetCellValue(heads[i]);
                head2.GetCell(col_index).CellStyle = column_headStyle;
                sheet.SetColumnWidth(col_index, 16 * 256);
                col_index++;
            }
            sheet.SetColumnWidth(0, 52 * 256);
            #endregion
            rowIndex = 1;
            foreach (var hy in hys)
            {
                #region 获取行业IPC有效专利第一
                string top10ipc = string.Format(@"
                        select 
	                        top 10 
	                        cn_ipc.ipc3 ,
	                        count(distinct cn.an) 数量 
                        from 
	                        cn_zt,
	                        cn,
                            cn_ipc,
	                        cn_lg 
                        where 
                            cn_zt.an = cn.an     
	                        and cn_zt.an = cn_ipc.an  
	                        and cn_zt.an = cn_lg.an
	                        and cn_lg.ValidStatus in('有效','审中') 
	                        and cn_zt.ztname in('{0}') 
                            {1}
                        group by cn_ipc.ipc3
                        order by 数量 desc", hy.Key, GetFilter5());

                DataTable dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, top10ipc);
                #endregion
                #region 创建表格
                for (int i = 0; i < 10; i++)
                {
                    XSSFRow xls_row = sheet.CreateRow(rowIndex + i) as XSSFRow;
                    xls_row.CreateCell(0).SetCellValue(hy.Value.ToString());
                    xls_row.GetCell(0).CellStyle = valueStyle_left;
                    for (int j = 1; j < heads.Length; j++)
                    {
                        xls_row.CreateCell(j).SetCellValue(0);
                        xls_row.GetCell(j).CellStyle = valueStyle;
                    }
                }
                #endregion
                #region 合并单元格
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex + 9, 0, 0));
                #endregion
                #region 获取IPC的增长率
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    #region 获取某一IPC的专利数据
                    string ipc = row["ipc3"].ToString();
                    string sum = row["数量"].ToString();
                    XSSFRow xls_row = sheet.GetRow(rowIndex + i) as XSSFRow;
                    xls_row.GetCell(1).SetCellValue(ipc);
                    xls_row.GetCell(2).SetCellValue(sum);

                    string sqlady = @"
select 
    count(distinct cn.an) 数量 
from 
    cn_zt,
    cn,
    cn_ipc,
    cn_lg 
where 
    cn_zt.an = cn.an     
    and cn_zt.an = cn_ipc.an  
    and cn_zt.an = cn_lg.an
    and cn_zt.ztname = '{0}'   
    and cn_ipc.ipc3 = '{1}' 
    and cn.pdy <='{2}'  and cn_lg.dead_year >{2} {3}";
                    List<string> values = new List<string>();
                    Dictionary<int, int> dts = new Dictionary<int, int>();
                    foreach (var year in config.Years)
                    {
                        string exesql = string.Format(sqlady, hy.Key, ipc, year, GetFilter());
                        object obj_result = DBA.SqlDbAccess.ExecuteScalar(CommandType.Text, exesql);
                        dts.Add(year.to_i(), obj_result.to_i());
                        values.Add(obj_result.ToString());
                    }
                    string zzl = GetZZL1(dts);
                    values.Add(zzl);

                    for (int x = 0; x < values.Count; x++)
                    {
                        xls_row.GetCell(3 + x).SetCellValue(values[x]);
                    }
                    #endregion
                }
                #endregion
                rowIndex += 10;
                Console.WriteLine(this.Name + "\t" + hy.Value);
            }
            Console.WriteLine("结束出表：{0}\t{1} ", Name, ztname);
            #endregion

            return true;
        }
        public override string GetFilter()
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
        public override void Dispose()
        {
            dt = null;
        }
    }
}
