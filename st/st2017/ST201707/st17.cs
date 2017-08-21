
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using xyExtensions;

namespace ST_2017.ST201707
{
    public class st17 : ST_Base
    {

        public st17()
        {
            this.Id = 17;
            this.Name = "(17)行业-有效专利量";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());

            dt = new DataTable();

            dt.Columns.Add(new DataColumn("行业"));
            foreach (string head in config.Years)
            {
                dt.Columns.Add(new DataColumn(head, typeof(int)));
            }
            foreach (var name in config.ZTNames)
            {
                string sql = @"
    select
        count(distinct cn.an) as 有效专利量
    from 
		cn 
        join cn_zt on cn_zt.an = cn.an
        join cn_lg on cn.an = cn_lg.an
        {3}
    where cn_zt.ztname = '{0}' and cn.pdy <='{1}'  and cn_lg.dead_year >{1} {2}";
                DataRow row = dt.NewRow();
                row[0] = Hys[name.ToString()];
                foreach (var year in config.Years)
                {
                    string exe_sql = "";
                    if (string.IsNullOrEmpty(config.strPas))
                    {
                        exe_sql = string.Format(sql, name, year, GetFilter(), "");
                    }
                    else
                    {
                        exe_sql = string.Format(sql, name, year, GetFilter(), " join cn_pa on cn.an = cn_pa.an ");
                    }
                    object objsum = DBA.SqlDbAccess.ExecuteScalar(CommandType.Text, exe_sql);

                    row[year] = objsum;

                }
                dt.Rows.Add(row);
                Console.WriteLine(Hys[name.ToString()]);

            }
            dt = StateManager.AddSumRow(dt);
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
