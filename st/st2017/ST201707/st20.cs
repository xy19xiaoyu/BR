
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using xyExtensions;

namespace ST_2017.ST201707
{
    public class st20 : ST_Base
    {

        public st20()
        {
            this.Id = 20;
            this.Name = "(20)城市-申请量";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());

            dt = new DataTable();


            dt.Columns.Add(new DataColumn("城市"));
            string pivot = "";
            foreach (string head in config.Years)
            {
                dt.Columns.Add(new DataColumn(head, typeof(int)));
                pivot += string.Format("[{0}],", head);
            }
            pivot = pivot.Trim(',');


            foreach (var name in config.Shis)
            {
                string sql = @"
select * from (
    select
        cn.ady,
	    count(distinct cn.an) as 申请量
    from 
	    years  join cn on years.y = cn.ady 
        join cn_zt on cn_zt.an = cn.an
    where cn.shi='{0}' {1}
    group by cn.ady) as a
PIVOT (sum(申请量) for ady in({2})) as table2";
                string exe_sql = "";
                exe_sql = string.Format(sql, name, GetFilter3(), pivot);
                DataTable tmmdt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);

                if (tmmdt.Rows.Count > 0)
                {
                    DataRow row = dt.NewRow();
                    row[0] = name;
                    if (tmmdt.Rows.Count > 0)
                    {
                        foreach (string head in config.Years)
                        {
                            row[head] = tmmdt.Rows[0][head].ToString().to_i();
                        }

                    }
                    else
                    {
                        row[0] = name;
                        foreach (string head in config.Years)
                        {
                            row[head] = 0;
                        }
                    }
                    dt.Rows.Add(row);
                }
                else
                {
                    DataRow row = dt.NewRow();
                    row[0] = name;
                    foreach (string head in config.Years)
                    {
                        row[head] = 0;
                    }
                    dt.Rows.Add(row);
                }
                Console.WriteLine(name);

            }
            dt = StateManager.AddSumRow(dt);
            return true;
        }

    }
}
