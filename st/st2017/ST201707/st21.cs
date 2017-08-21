
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using xyExtensions;

namespace ST_2017.ST201707
{
    public class st21 : ST_Base
    {

        public st21()
        {
            this.Id = 21;
            this.Name = "(21)城市-有效专利量";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());

            dt = new DataTable();


            dt.Columns.Add(new DataColumn("城市"));
            foreach (string head in config.Years)
            {
                dt.Columns.Add(new DataColumn(head, typeof(int)));
            }

            foreach (var name in config.Shis)
            {
                string sql = @"

    select
	    count(distinct cn.an) as 申请量
    from 
	    cn 
        join cn_lg on cn.an = cn_lg.an 
    where cn.shi='{0}'  and cn.pdy <='{1}'  and cn_lg.dead_year >{1}";
                DataRow row = dt.NewRow();
                row[0] = name;
                foreach (var year in config.Years)
                {
                    string exe_sql = "";
                    exe_sql = string.Format(sql, name, year);
                    object sum = DBA.SqlDbAccess.ExecuteScalar(CommandType.Text, exe_sql);
                    row[year] = sum.to_i();
                }
                dt.Rows.Add(row);
                Console.WriteLine(name);
            }
            dt = StateManager.AddSumRow(dt);
            return true;
        }

    }
}
