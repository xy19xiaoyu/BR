
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using NPOI.XSSF.UserModel;
using xyExtensions;
using NPOI.SS.UserModel;

namespace ST_2017.en
{
    public class en_104 : ST_Base
    {

        public en_104()
        {
            this.Id = 104;
            this.Name = "近10年主要专利申请人";
            this.type = "EN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            return true;
        }
        public override bool OutPut2Worksheet(XSSFWorkbook xbook)
        {

            string[] heads = new string[] { "行业", "重点申请人", "申请量", "CPY" };

            Console.WriteLine("开始出表：{0} ", Name);


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
            sheet.SetColumnWidth(1, 40 * 256);
            #endregion
            rowIndex = 1;
            foreach (var hy in Hys)
            {
                #region 获取行业申请人类型
                string pas = string.Format(@"
                    select
	                    top 10
	                    en_pa.pa as 申请人,
	                    COUNT(distinct en.an) as 申请量,
                        en_pa.cpy as CPY
                    from
	                    en,
	                    en_pa,
	                    en_zt
                    where 
	                    en.pn = en_pa.pn
	                    and en.pn = en_zt.pn
	                    and en_zt.zt in({0})
                        and en.ady >=2007
	                    and {1}
                    group by
	                    en_pa.pa,en_pa.cpy
                    order by 申请量 desc", hy.Key, GetFilter());
                DataTable dtpas = DBA.SqlDbAccess.GetDataTable(CommandType.Text, pas);
                #endregion
                #region 创建表格
                for (int j = 0; j < 10; j++)
                {
                    XSSFRow xls_row = sheet.CreateRow(rowIndex + j) as XSSFRow;
                    xls_row.CreateCell(0).SetCellValue(Hy(hy.Key));
                    xls_row.GetCell(0).CellStyle = valueStyle_left;
                    xls_row.CreateCell(1).SetCellValue("");
                    xls_row.GetCell(1).CellStyle = valueStyle_left;
                    xls_row.CreateCell(2).SetCellValue(0);
                    xls_row.GetCell(2).CellStyle = valueStyle;
                    xls_row.CreateCell(3).SetCellValue("");
                    xls_row.GetCell(3).CellStyle = valueStyle;
                }

                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex + 9, 0, 0));
                #endregion
                #region 赋值
                int tmpindex = 0;
                foreach (DataRow parow in dtpas.Rows)
                {
                    string pa = parow["申请人"].ToString();
                    string ancount = parow["申请量"].ToString();
                    string cpy = parow["CPY"].ToString();

                    XSSFRow xls_row = sheet.GetRow(rowIndex + tmpindex) as XSSFRow;
                    xls_row.GetCell(1).SetCellValue(pa);
                    xls_row.GetCell(2).SetCellValue(ancount);
                    xls_row.GetCell(3).SetCellValue(cpy);
                    tmpindex++;
                }
                #endregion
                Console.WriteLine(this.Name + "\t" + hy.Value);
                rowIndex += 10;
            }
            Console.WriteLine("结束出表：{0} ", Name);
            return true;
        }

        public override string GetFilter()
        {
            return $" en.p_c in({config.GuoJia}) ";
        }
    }
}
