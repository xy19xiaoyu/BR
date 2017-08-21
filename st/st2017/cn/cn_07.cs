
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ST_2017.Interface;
using System.Data;

namespace ST_2017.cn
{
    public class cn_07 : ST_Base
    {

        public cn_07()
        {
            this.Id = 7;
            this.Name = "RS—IPC小类分布";
            this.type = "CN";
            this.DES = "";
        }
        public override bool Sate(string ztName)
        {
            Console.WriteLine("正在生成表：{0}\t{1}\t{2}\t{3}", ztName, this.config.FileName, this.Name, DateTime.Now.ToString());
            string sql = @"
	select 
		ipc4 as IPC小类,
		count(distinct cn.an) as 专利数量
	from 
		cn,
		cn_ipc
	where cn.an = cn_ipc.an
	and city='RS'
	group by 
		ipc4
	order by 专利数量 desc";

            dt = DBA.SqlDbAccess.GetDataTable(CommandType.Text, sql);
            return true;
        }

    }
}
