
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using xyExtensions;

namespace ST_2017.ST201708
{
    public class st07 : ST_Base
    {

        public st07()
        {
            this.Id = 07;
            this.Name = "专利维持年限";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}", this.config.FileName, this.Name, DateTime.Now.ToString());
            return true;
        }
        public override bool OutPut2Worksheet(NPOI.XSSF.UserModel.XSSFWorkbook xbook)
        {
            ISheet sheet = xbook.CreateSheet(this.Name);
            IniStryle(xbook);


            #region 取得列宽
            //取得列宽

            string[] heads = new string[] { "维持年限" };

            #endregion
            #region 第一行




            #endregion

            #region 第二行
            int rowIndex = 0;
            int col_index = 1;
            XSSFRow head2 = sheet.CreateRow(rowIndex) as XSSFRow;
            head2.HeightInPoints = 21;
            col_index = 1;
            for (int i = 0; i < heads.Length; i++)
            {
                head2.CreateCell(col_index).SetCellValue(heads[i]);
                head2.GetCell(col_index).CellStyle = column_headStyle;
                sheet.SetColumnWidth(col_index, 12 * 256);
                col_index++;
            }
            head2.CreateCell(0).SetCellValue("行业");
            head2.GetCell(0).CellStyle = column_headStyle;
            sheet.SetColumnWidth(0, 52 * 256);
            #endregion
            rowIndex++;
            foreach (var name in DicHys)
            {
                string sql = @"
select
		AVG(cn_lg.age) as 平均维持年限
	from 
		cn,
		cn_zt,
		cn_lg
	where 
		cn.an = cn_zt.an
		and cn.an = cn_lg.an
		and cn.gdy is not null
		and cn_zt.ztname in({0})
        {1}
";
                List<int> values = new List<int>();

                string exe_sql = "";
                exe_sql = string.Format(sql, name.Value, GetFilter());
                DataTable tmpdt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
                if (tmpdt.Rows.Count > 0)
                {
                    values.Add(tmpdt.Rows[0]["平均维持年限"].ToString().to_i());
                }
                else
                {
                    values.Add(0);
                }

                #region 数据行
                XSSFRow val_row = sheet.CreateRow(rowIndex) as XSSFRow;
                val_row.HeightInPoints = 21;
                val_row.CreateCell(0).SetCellValue(name.Key);
                val_row.GetCell(0).CellStyle = valueStyle_left;

                for (int i = 0; i < values.Count; i++)
                {
                    val_row.CreateCell(i + 1).SetCellValue(values[i]);
                    val_row.GetCell(i + 1).CellStyle = valueStyle;
                }
                #endregion
                rowIndex++;
                Console.WriteLine(name.Key);
            }

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
    }
}
