
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;

namespace ST_2017.cn
{
    public class cn_02 : ST_Base
    {

        public cn_02()
        {
            this.Id = 1;
            this.Name = "逐年专利申请情况";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            string sql = @"
select * from (select
 ady as 申请年,
 city as 国家
,count(*)  申请量
from cn
group by ady, city) as a1
PIVOT(sum(申请量) for 国家 in([ID],[RS],[UA])) as table2";

            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, sql);
            return true;
        }

    }
}
