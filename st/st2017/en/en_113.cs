﻿
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
    public class en_113 : ST_Base
    {

        public en_113()
        {
            this.Id = 113;
            this.Name = "国外前5国-主要IPC";
            this.type = "EN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            return true;
        }
        public override bool OutPut2Worksheet(XSSFWorkbook xbook)
        {

            string[] heads = new string[] { "国家", "IPC", "申请量" };

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
            foreach (var GJ in config.Top5Guojia)
            {
                #region 获取申请国申请人
                string pas = string.Format(@"
                    select
	                    top 50
	                    en_ipc.ipc4 as IPC,
	                    COUNT(distinct en.an) as 申请量
                    from
	                    en,
	                    en_ipc
                    where 
	                    en.pn = en_ipc.pn
	                    and en.i_c in('{0}')
	                    and {1}
                    group by
	                     en_ipc.ipc4 
                    order by 申请量 desc", GJ, GetFilter());
                DataTable dtpas = DBA.SqlDbAccess.GetDataTable(CommandType.Text, pas);
                #endregion
                #region 创建表格
                for (int j = 0; j < 50; j++)
                {
                    XSSFRow xls_row = sheet.CreateRow(rowIndex + j) as XSSFRow;
                    xls_row.CreateCell(0).SetCellValue(GJ);
                    xls_row.GetCell(0).CellStyle = valueStyle_left;
                    xls_row.CreateCell(1).SetCellValue("");
                    xls_row.GetCell(1).CellStyle = valueStyle_left;
                    xls_row.CreateCell(2).SetCellValue(0);
                    xls_row.GetCell(2).CellStyle = valueStyle;
                }
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex + 49, 0, 0));
                #endregion
                #region 赋值
                int tmpindex = 0;
                foreach (DataRow parow in dtpas.Rows)
                {
                    string pa = parow["IPC"].ToString();
                    string ancount = parow["申请量"].ToString();

                    XSSFRow xls_row = sheet.GetRow(rowIndex + tmpindex) as XSSFRow;
                    xls_row.GetCell(1).SetCellValue(pa);
                    xls_row.GetCell(2).SetCellValue(ancount);
                    tmpindex++;
                }
                #endregion
                Console.WriteLine(this.Name + "\t" + GJ);
                rowIndex += 50;
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
