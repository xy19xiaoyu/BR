using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BRDB.Model;
using BRDB.Extend;

namespace BRDB.Helper
{
    public class hyHelper
    {
        public static List<hy> hys = new List<hy>();

        public static List<string> GetHyIds(string stripc)
        {
            string ipc = stripc.FormatIPC();
            if (hys.Count == 0) hys = IniHys();
            List<string> hyids = new List<string>();
            foreach (var hy in hys)
            {
                string id = hy.GetHyId(ipc);
                if (string.IsNullOrEmpty(id)) continue;
                hyids.Add(id);
            }

            return hyids.Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList<string>();
        }

        public static List<hy> IniHys()
        {
            List<hy> hys = new List<hy>();
            Dictionary<string, string> hyips = IniHyIPC();

            foreach (var hyipc in hyips)
            {
                hys.Add(new hy(hyipc.Value, hyipc.Key));
            }

            return hys;

        }
        public static Dictionary<string, string> IniHyIPC()
        {
            Dictionary<string, string> hys = new Dictionary<string, string>();
            hys.Add("1", "农业");
            hys.Add("2", "林业");
            hys.Add("3", "畜牧业");
            hys.Add("4", "渔业");
            hys.Add("5", "农、林、牧、渔服务业");
            hys.Add("6", "煤炭开采和洗选业");
            hys.Add("7", "石油和天然气开采业");
            hys.Add("8", "黑色金属矿采选业");
            hys.Add("9", "有色金属矿采选业");
            hys.Add("10", "非金属矿采选业");
            hys.Add("11", "开采辅助活动");
            hys.Add("12", "其他采矿业");
            hys.Add("13", "农副食品加工业");
            hys.Add("14", "食品制造业");
            hys.Add("15", "饮料制造业");
            hys.Add("16", "烟草制品业");
            hys.Add("17", "纺织业");
            hys.Add("18", "纺织服装、鞋、帽制造业");
            hys.Add("19", "皮革、毛皮、羽毛(绒)及其制品业");
            hys.Add("20", "木材加工及木、竹、藤、棕、草制品业");
            hys.Add("21", "家具制造业");
            hys.Add("22", "造纸及纸制品业");
            hys.Add("23", "印刷业和记录媒介的复制");
            hys.Add("24", "文教体育用品制造业");
            hys.Add("25", "精炼石油产品的制造");
            hys.Add("26", "化学原料及化学制晶制造业");
            hys.Add("27", "医药制造业");
            hys.Add("28", "化学纤维制造业");
            hys.Add("29", "橡胶和塑料制品业");
            hys.Add("30", "非全属矿物制品业");
            hys.Add("31", "黑色金属冶炼及压延加工业");
            hys.Add("32", "有色金属冶炼及压延加工业");
            hys.Add("33", "金属制品业");
            hys.Add("34", "通用设备制造业");
            hys.Add("35", "专用设备制造业");
            hys.Add("36", "汽车制造业");
            hys.Add("37", "火车、船舶、航天等运输设备制造业");
            hys.Add("38", "电气机械及器材制造业");
            hys.Add("39", "通信设备、计算机及其他电子设备制造业");
            hys.Add("40", "仪器仪表制造业");
            hys.Add("41", "其他制造业");
            hys.Add("42", "废弃资源综合利用业");
            hys.Add("43", "金属制品、机械和设备修理业");
            hys.Add("44", "电力、热力的生产和供应");
            hys.Add("45", "燃气生产和供应");
            hys.Add("46", "水的生产和供应");
            hys.Add("47", "房屋建筑业");
            hys.Add("48", "土木工程建筑业");
            hys.Add("49", "建筑安装业");
            hys.Add("50", "建筑装饰和其他建筑业");

            return hys;

        }



    }
}
