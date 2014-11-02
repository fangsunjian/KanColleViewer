using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
    public class kcsapi_record
    {
        public string api_member_id { get; set; }
        public string api_nickname { get; set; }
        public string api_nickname_id { get; set; }
        public string api_cmt { get; set; }
        public string api_cmt_id { get; set; }
        public string api_photo_url { get; set; }
        public int api_level { get; set; }
        public int api_rank { get; set; }
        public int[] api_experience { get; set; }
        public Api_War api_war { get; set; }
        public Api_Mission api_mission { get; set; }
        public Api_Practice api_practice { get; set; }
        public int api_friend { get; set; }
        public int api_deck { get; set; }
        public int api_kdoc { get; set; }
        public int api_ndoc { get; set; }
        public long[] api_ship { get; set; }
        public long[] api_slotitem { get; set; }
        public int api_furniture { get; set; }
        public double[] api_complate { get; set; }
        public int api_large_dock { get; set; }
        public long api_material_max { get; set; }
    }

    public class Api_War
    {
        public int api_win { get; set; }
        public int api_lose { get; set; }
        public int api_rate { get; set; }
    }

    public class Api_Mission
    {
        public int api_count { get; set; }
        public int api_success { get; set; }
        public int api_rate { get; set; }
    }

    public class Api_Practice
    {
        public int api_win { get; set; }
        public int api_lose { get; set; }
        public int api_rate { get; set; }
    }
}
