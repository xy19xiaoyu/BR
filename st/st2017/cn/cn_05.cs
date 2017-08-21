
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;

namespace ST_2017.cn
{
    public class cn_05 : ST_Base
    {

        public cn_05()
        {
            this.Id = 5;
            this.Name = "行业分布";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            string sql = @"
select zt,'' as 行业,ID,RS,UA from (
	select 
		zt ,
		city as 国家,
		count(distinct cn.an) as 专利数量
	from 
		cn_zt left join cn on  cn.an = cn_zt.an
	group by 
		cn_zt.zt,city
	) AS a
PIVOT(sum(专利数量) for 国家 in([ID],[RS],[UA])) as table2";

            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, sql);

            foreach (DataRow row in dt.Rows)
            {
                row["行业"] = $"{row[0].ToString().PadLeft(2, '0')}.{Hys[row[0].ToString()]}";
            }
            dt.Columns.RemoveAt(0);
            return true;
        }

    }
}
