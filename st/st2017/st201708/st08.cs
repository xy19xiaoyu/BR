
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
    public class st08 : ST_Base
    {

        public st08()
        {
            this.Id = 08;
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

            string[] years = new string[] { "2013", "2016" };
            #region 取得列宽
            //取得列宽

            string[] heads = new string[] { "维持年限", "发明占比" };

            #endregion
            #region 第一行
            int rowIndex = 0;
            XSSFRow head1 = sheet.CreateRow(rowIndex) as XSSFRow;
            head1.HeightInPoints = 21;
            int col_index = 1;
            foreach (var year in years)
            {
                head1.CreateCell(col_index + 0).SetCellValue(year);
                head1.CreateCell(col_index + 1).SetCellValue("");
                head1.GetCell(col_index + 0).CellStyle = column_headStyle;
                head1.GetCell(col_index + 1).CellStyle = column_headStyle;

                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex, col_index, col_index + 1));
                col_index += 2;
            }
            #endregion

            #region 第二行
            rowIndex = 1;
            XSSFRow head2 = sheet.CreateRow(rowIndex) as XSSFRow;
            head2.HeightInPoints = 21;
            col_index = 1;
            foreach (var year in years)
            {
                for (int i = 0; i < heads.Length; i++)
                {
                    head2.CreateCell(col_index).SetCellValue(heads[i]);
                    head2.GetCell(col_index).CellStyle = column_headStyle;
                    sheet.SetColumnWidth(col_index, 12 * 256);
                    col_index++;
                }
            }
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, 0, 0));
            head1.CreateCell(0).SetCellValue("行业");
            head1.GetCell(0).CellStyle = column_headStyle;
            head2.CreateCell(0).SetCellValue("");
            head2.GetCell(0).CellStyle = column_headStyle;
            sheet.SetColumnWidth(0, 52 * 256);
            #endregion
            rowIndex++;
            foreach (var hy in DicHys)
            {
                string sql = @"
select
		AVG(cn_lg.age_{2}) as 平均维持年限
	from 
		cn,
		cn_zt,
		cn_lg
	where 
		cn.an = cn_zt.an
		and cn.an = cn_lg.an
        and gdy <='{2}'
		and cn_zt.ztname in({0})
        {1}
";
                string zhuanlicount = @"
select * from (
    select
		    cn.type as 专利类型,
		    count(cn.an) as 专利数量
	    from 
		    cn,
		    cn_zt,
		    cn_lg
	    where 
		    cn.an = cn_zt.an
		    and cn.an = cn_lg.an
            and gdy <='{2}'
		    and cn_zt.ztname in({0})
            {1}
    group by cn.type) as a
PIVOT(sum(专利数量) for 专利类型 in([发明专利],[实用新型])) as table2
";
                List<double> values = new List<double>();

                XSSFRow val_row = sheet.CreateRow(rowIndex) as XSSFRow;
                val_row.HeightInPoints = 21;
                val_row.CreateCell(0).SetCellValue(hy.Key);
                val_row.GetCell(0).CellStyle = valueStyle_left;
                int column = 1;
                foreach (var year in years)
                {
                    string exe_sql = "";
                    exe_sql = string.Format(sql, hy.Value, GetFilter(), year);
                    DataTable tmpdt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
                    if (tmpdt.Rows.Count > 0)
                    {
                        val_row.CreateCell(column).SetCellValue(tmpdt.Rows[0]["平均维持年限"].ToString().to_i());
                    }
                    else
                    {
                        val_row.CreateCell(column).SetCellValue(0);
                    }
                    string exe_zl = string.Format(zhuanlicount, hy.Value, GetFilter(), year);
                    DataTable dtcout = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_zl);
                    if (dtcout.Rows.Count > 0)
                    {
                        int fm = dtcout.Rows[0]["发明专利"].ToString().to_i();
                        int xx = dtcout.Rows[0]["实用新型"].ToString().to_i();
                        double sum = fm + xx;
                        double fmpercent = 0;
                        if (sum != 0)
                        {
                            fmpercent = fm / sum;
                        }
                        val_row.CreateCell(column + 1).SetCellValue(fmpercent.ToString("0.00%"));

                    }
                    else
                    {
                        val_row.CreateCell(column + 1).SetCellValue("N/A");
                    }
                    val_row.GetCell(column).CellStyle = valueStyle;
                    val_row.GetCell(column + 1).CellStyle = valueStyle;
                    column += 2;
                }
                rowIndex++;
                Console.WriteLine(hy.Key.ToString());
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
