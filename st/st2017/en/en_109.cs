
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;
using NPOI.XSSF.UserModel;

namespace ST_2017.en
{
    public class en_109 : ST_Base
    {

        public en_109()
        {
            this.Id = 109;
            this.Name = "中国-IPC小类TOP50";
            this.type = "EN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());

            string sql = @"
select
	top 50
	en_ipc.ipc4 as IPC,
	count(distinct an)  申请量
from 
	en,
	en_ipc
where 
	en.pn = en_ipc.pn 
	and en.i_c = 'CN'
    and {0}
group by 
	en_ipc.ipc4
order by  
	申请量 desc
";

            string exe_sql = string.Format(sql, GetFilter());
            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, exe_sql);
            return true;
        }

        public override string GetFilter()
        {
            return $" en.p_c in({config.GuoJias}) ";
        }
    }
}
