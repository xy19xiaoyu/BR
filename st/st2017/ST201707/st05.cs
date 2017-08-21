
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using xyExtensions;

namespace ST_2017.ST201707
{
    public class st05 : ST_Base
    {

        public st05()
        {
            this.Id = 5;
            this.Name = "(05)申请人类型-有效专利量";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            dt = new DataTable();
            dt.Columns.Add(new DataColumn("年代", typeof(string)));
            dt.Columns.Add(new DataColumn("个人", typeof(string)));
            dt.Columns.Add(new DataColumn("工矿企业", typeof(string)));
            dt.Columns.Add(new DataColumn("大专院校", typeof(string)));
            dt.Columns.Add(new DataColumn("科研单位", typeof(string)));
            dt.Columns.Add(new DataColumn("事业单位", typeof(string)));

            string joinzt = " join cn_zt on cn_zt.an = cn.an";
            string sql = @"
select * from (select
	cn_pa.pa_type as 申请人类型,
	count(distinct cn_pa.an) as 申请量
from 
	cn   	
    join cn_pa on cn.an = cn_pa.an 
    join cn_lg on cn.an = cn_lg.an
    {2}
where  cn.pdy <='{0}'  and cn_lg.dead_year >{0} {1}
group by cn_pa.pa_type) as a
PIVOT (sum(申请量) for 申请人类型 in([个人],[工矿企业],[大专院校],[科研单位],[事业单位])) as table2
";
            foreach (var year in config.Years)
            {
                string exe_sql = "";
                if (string.IsNullOrEmpty(config.ztname))
                {
                    exe_sql = string.Format(sql, year, GetFilter1(), "");
                }
                else
                {
                    exe_sql = string.Format(sql, year, GetFilter1(), joinzt);
                }


                DataTable tmpdt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
                DataRow row = dt.NewRow();
                row[0] = year;
                if (tmpdt.Rows.Count > 0)
                {
                    row["个人"] = tmpdt.Rows[0]["个人"].to_i();
                    row["工矿企业"] = tmpdt.Rows[0]["工矿企业"].to_i();
                    row["大专院校"] = tmpdt.Rows[0]["大专院校"].to_i();
                    row["科研单位"] = tmpdt.Rows[0]["科研单位"].to_i();
                    row["事业单位"] = tmpdt.Rows[0]["事业单位"].to_i();

                }
                else
                {
                    row["个人"] = 0;
                    row["工矿企业"] = 0;
                    row["大专院校"] = 0;
                    row["科研单位"] = 0;
                    row["事业单位"] = 0;
                }
                dt.Rows.Add(row);
            }                        
            return true;
        }

    }
}
