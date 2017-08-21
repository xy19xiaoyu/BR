using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.XSSF.UserModel;

namespace ST_2017.Interface
{
    public interface ILGState : IDisposable 
    {
       StatConfig config { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        int Id
        {
            get;
        }

        /// <summary>
        /// 表名
        /// </summary>
        string Name
        {
            get;
        }
        string type
        {
            get;
        }
        /// <summary>
        /// 描述
        /// </summary>
        string DES
        {
            get;

        }
        string ztname
        {
            get;
            set;
        }
        bool Sate(string ztName);
        string zltype
        {
            get;
            set;
        }
        string Sheng
        {
            get;
            set;
        }
        string Shi
        {
            get;
            set;
        }
        string GuoJia
        {
            get;
            set;
        }
       /// <summary>
       /// 将统计结果输出指ExcelApp的一个Sheet中
       /// </summary>
       /// <param name="xApp"></param>
       /// <returns></returns>
        bool OutPut2Worksheet(XSSFWorkbook xbook);

        string city { get; set; }
    }
}
