
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using xyExtensions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ST_2017.ST201708
{
    public class st12 : ST_Base
    {

        public st12()
        {
            this.Id = 12;
            this.Name = "行业-申请人2008-2016";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            return true;
        }
        public override bool OutPut2Worksheet(XSSFWorkbook xbook)
        {

            string[] heads = new string[] { "行业", "申请人类型", "申请量", "占比", "重点申请人", "申请量" };
            Dictionary<string, string> hys = new Dictionary<string, string>();

            hys.Add("48", "27.医药制造业");
            hys.Add("59','60','66','69','73','76','81','84", "装备制造业");
            hys.Add("59", "33.金属制品业");
            hys.Add("60", "34.通用设备制造业");
            hys.Add("66", "35.专用设备制造业");
            hys.Add("69", "36.汽车制造业");
            hys.Add("73", "37.火车、船舶、航天等运输设备制造业");
            hys.Add("76", "38.电气机械及器材制造业");
            hys.Add("81", "39.通信设备、计算机及其他电子设备制造业");
            hys.Add("84", "40.仪器仪表制造业");
            Console.WriteLine("开始出表：{0}\t{1} ", Name, ztname);



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
            sheet.SetColumnWidth(4, 40 * 256);
            #endregion
            rowIndex = 1;
            foreach (var hy in hys)
            {
                #region 获取行业申请人类型
                string patyp = string.Format(@"
select * from (
    select 
	    cn_pa.pa_type as 申请人类型,
	    count(distinct cn.an) as  申请量
    from 
	    cn,
	    cn_zt,
	    cn_pa
    where 
	    cn.an = cn_zt.an
	    and cn.an  = cn_pa.an
	    and cn.ady between '2008' and '2016'
	    and cn_zt.ztname in ('{0}') {1}
    group by cn_pa.pa_type
) as a
PIVOT (sum(申请量) for 申请人类型 in([工矿企业],[科研院所],[个人])) as table2
", hy.Key, GetFilter1());
                string[] types = new string[] { "工矿企业", "科研院所", "个人" };
                DataTable dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, patyp);
                #endregion
                #region 创建表格
                //head*10 ,0 ==hangy,1= type
                for (int i = 0; i < types.Length; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        XSSFRow xls_row = sheet.CreateRow(rowIndex + (i * 10) + j) as XSSFRow;
                        xls_row.CreateCell(0).SetCellValue(hy.Value.ToString());
                        xls_row.GetCell(0).CellStyle = valueStyle_left;
                        xls_row.CreateCell(1).SetCellValue(hy.Value.ToString());
                        xls_row.GetCell(1).CellStyle = valueStyle_left;
                        for (int col = 2; col <= 5; col++)
                        {
                            xls_row.CreateCell(col).SetCellValue("");
                            xls_row.GetCell(col).CellStyle = valueStyle_left;
                        }

                    }
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex + (i * 10), rowIndex + (i * 10) + 9, 1, 1));
                }
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex, rowIndex + (types.Length * 10) - 1, 0, 0));
                #endregion
                #region 获取某一申请人类型的重点申请人
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    List<int> valus = new List<int>();
                    foreach (string type in types)
                    {
                        valus.Add(row[type].ToString().to_i());
                    }
                    double sum = valus.Sum();
                    for (int i = 0; i < types.Length; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            XSSFRow xls_row = sheet.GetRow(rowIndex + (i * 10) + j) as XSSFRow;
                            xls_row.GetCell(1).SetCellValue(types[i]);
                            xls_row.GetCell(2).SetCellValue(valus[i]);
                            if (sum == 0)
                            {
                                xls_row.GetCell(3).SetCellValue(0);
                            }
                            else
                            {
                                xls_row.GetCell(3).SetCellValue(Math.Round(valus[i] / sum, 4).ToString("0.00%"));
                            }
                        }

                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex + i * 10, rowIndex + i * 10 + 9, 2, 2));
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIndex + i * 10, rowIndex + i * 10 + 9, 3, 3));
                    }

                    string sql = @"
select 
	top 10
	cn_pa.pa as 申请人,
	count(distinct cn.an) as  申请量
from 
	cn,
	cn_zt,
	cn_pa
where 
	cn.an = cn_zt.an
	and cn.an  = cn_pa.an
	and cn.ady between '2008' and '2016'
	and cn_zt.ztname in('{0}')
	and cn_pa.pa_type = '{1}' {2}
group by cn_pa.pa
order by 申请量 desc";
                    for (int i = 0; i < types.Length; i++)
                    {
                        string sql_pas = string.Format(sql, hy.Key, types[i], GetFilter1());
                        DataTable dtpas = DBA.SqlDbAccess.GetDataTable(CommandType.Text, sql_pas);

                        int tmpindex = 0;
                        foreach (DataRow parow in dtpas.Rows)
                        {
                            string pa = parow["申请人"].ToString();
                            string ancount = parow["申请量"].ToString();

                            XSSFRow xls_row = sheet.GetRow(rowIndex + (i * 10) + tmpindex) as XSSFRow;
                            xls_row.GetCell(4).SetCellValue(pa);
                            xls_row.GetCell(5).SetCellValue(ancount);
                            tmpindex++;
                        }
                    }
                    rowIndex += 30;

                }
                Console.WriteLine(this.Name + "\t" + hy.Value);
                #endregion

            }
            Console.WriteLine("结束出表：{0} ", Name);


            return true;
        }

        public override void Dispose()
        {
            dt = null;
        }
    }
}
