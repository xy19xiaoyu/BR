
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using xyExtensions;

namespace ST_2017.ST201707
{
    public class st06 : ST_Base
    {

        public st06()
        {
            this.Id = 6;
            this.Name = "(06)行业-申请量";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());

            dt = new DataTable();


            dt.Columns.Add(new DataColumn("行业"));
            string pivot = "";
            foreach (string head in config.Years)
            {
                dt.Columns.Add(new DataColumn(head, typeof(int)));
                pivot += string.Format("[{0}],", head);
            }
            pivot = pivot.Trim(',');


            foreach (var name in config.ZTNames)
            {
                string sql = @"
select * from (
    select
        cn.ady,
	    count(distinct cn.an) as 申请量
    from 
	    years left join cn on years.y = cn.ady 
        left join cn_zt on cn_zt.an = cn.an
        {2}
    where cn_zt.ztname = {0} {1}
    group by cn.ady) as a
PIVOT (sum(申请量) for ady in({3})) as table2";
                string exe_sql = "";
                if (string.IsNullOrEmpty(config.strPas))
                {
                    exe_sql = string.Format(sql, name, GetFilter2(), "", pivot);
                }
                else
                {
                    exe_sql = string.Format(sql, name, GetFilter2(), " left join cn_pa on cn.an = cn_pa.an ", pivot);
                }


                DataTable tmmdt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
                DataRow row = dt.NewRow();
                row[0] = Hys[name.ToString()];
                if (tmmdt.Rows.Count > 0)
                {
                   
                    if (tmmdt.Rows.Count > 0)
                    {
                        foreach (string head in config.Years)
                        {
                            row[head] = tmmdt.Rows[0][head].ToString().to_i();
                        }

                    }
                    else
                    {
                        foreach (string head in config.Years)
                        {
                            row[head] = 0;
                        }
                    }
                   
                }
                else
                {
                    foreach (string head in config.Years)
                    {
                        row[head] = 0;
                    }
                }
                dt.Rows.Add(row);
                Console.WriteLine(Hys[name.ToString()]);

            }
            dt = StateManager.AddSumRow(dt);
            return true;
        }

    }
}
