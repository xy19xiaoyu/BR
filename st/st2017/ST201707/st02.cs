
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using xyExtensions;

namespace ST_2017.ST201707
{
    public class st02 : ST_Base
    {

        public st02()
        {
            this.Id = 2;
            this.Name = "(02)有效发明专利量";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            dt = new DataTable();
            dt.Columns.Add(new DataColumn("年代", typeof(string)));
            dt.Columns.Add(new DataColumn("有效发明专利量", typeof(string)));

            string joinzt = " join cn_zt on cn_zt.an = cn.an";
            string sql = @"
select
	count(cn.an) as 有效发明专利量
from 
	cn 
    join cn_lg on cn.an = cn_lg.an
    {2}
where cn.type = '发明专利' and cn.pdy <='{0}' and cn_lg.dead_year >{0} {1}";
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

                object count = DBA.SqlDbAccess.ExecuteScalar(CommandType.Text, exe_sql);
                DataRow row = dt.NewRow();
                row[0] = year;
                row[1] = count.to_i();
                dt.Rows.Add(row);
            }            
            return true;
        }

    }
}
