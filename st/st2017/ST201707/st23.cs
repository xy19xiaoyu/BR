
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using xyExtensions;

namespace ST_2017.ST201707
{
    public class st23 : ST_Base
    {

        public st23()
        {
            this.Id = 23;
            this.Name = "(23)城市-专利许可";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            return true;
        }


        public override bool OutPut2Worksheet(NPOI.XSSF.UserModel.XSSFWorkbook xbook)
        {
            ISheet sheet = xbook.CreateSheet(this.Name);
            IniStryle(xbook);

            #region 取得列宽
            //取得列宽
            int[] arrColWidth = new int[10];
            string[] heads = new string[] { "发明专利", "实用新型" };
            for (int i = 0; i < heads.Length; i++)
            {
                arrColWidth[i] = Encoding.GetEncoding(936).GetBytes(heads[i]).Length;
                arrColWidth[i] = arrColWidth[i] > 50 ? 50 : arrColWidth[i];
            }
            #endregion

            #region 第一行
            int rowIndex = 0;
            XSSFRow head1 = sheet.CreateRow(rowIndex) as XSSFRow;
            head1.HeightInPoints = 21;
            int col_index = 1;
            foreach (var year in config.Years)
            {
                head1.CreateCell(col_index).SetCellValue(year);
                head1.CreateCell(col_index + 1).SetCellValue("");
                head1.GetCell(col_index).CellStyle = column_headStyle;
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
            foreach (var year in config.Years)
            {
                for (int i = 0; i < heads.Length; i++)
                {
                    head2.CreateCell(col_index).SetCellValue(heads[i]);
                    head2.GetCell(col_index).CellStyle = column_headStyle;
                    sheet.SetColumnWidth(col_index, 16 * 256);
                    col_index++;
                }
            }
            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, 0, 0));
            head1.CreateCell(0).SetCellValue("城市");
            head1.GetCell(0).CellStyle = column_headStyle;
            head2.CreateCell(0).SetCellValue("");
            head2.GetCell(0).CellStyle = column_headStyle;
            sheet.SetColumnWidth(0, 52 * 256);
            #endregion

            rowIndex++;
            foreach (var name in config.Shis)
            {
                string sql = @"
                    with tmp as (
                    select
	                    cn.ady,
                        cn.type as 专利类型,
	                    count(zl_xk.an) as 专利许可量
                    from 
	                    years join cn on years.y = cn.ady 
	                    join cn_zt on cn_zt.an = cn.an
                        join zl_xk on cn.an = zl_xk.an  
                    where cn.shi= '{0}' {1}
                    group by cn.ady,cn.type)
                    select ady,ISNULL([发明专利],0) as [发明专利],ISNULL([实用新型],0) as [实用新型] from tmp
                    PIVOT (sum(专利许可量) for 专利类型 in([发明专利],[实用新型])) as table2
                    order by ady;";

                string exe_sql = string.Format(sql, name, GetFilter3());

                DataTable tmpdt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
                List<int> values = new List<int>();

                if (tmpdt.Rows.Count > 0)
                {
                    foreach (var year in config.Years)
                    {
                        var result = from row in tmpdt.AsEnumerable()
                                     where row["ady"].ToString() == year.ToString()
                                     select row;
                        if (result.Count() == 1)
                        {
                            DataRow first = result.First();
                            values.Add(first["发明专利"].ToString().to_i());
                            values.Add(first["实用新型"].ToString().to_i());
                        }
                        else
                        {
                            values.Add(0);
                            values.Add(0);
                        }
                    }
                }
                else
                {
                    foreach (var year in config.Years)
                    {
                        values.Add(0);
                        values.Add(0);
                    }
                }
                #region 数据行
                XSSFRow val_row = sheet.CreateRow(rowIndex) as XSSFRow;
                val_row.HeightInPoints = 21;
                val_row.CreateCell(0).SetCellValue(name);
                val_row.GetCell(0).CellStyle = valueStyle_left;

                for (int i = 0; i < values.Count; i++)
                {
                    val_row.CreateCell(i + 1).SetCellValue(values[i]);
                    val_row.GetCell(i + 1).CellStyle = valueStyle;
                }
                #endregion
                rowIndex++;
                Console.WriteLine(name);
            }

            return true;

        }


    }
}
