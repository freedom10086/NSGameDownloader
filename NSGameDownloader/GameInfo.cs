using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSGameDownloader
{
    public class GameInfo
    {
        [PrimaryKey]
        public string tid { get; set; }
        public string cnName { get; set; }
        public string jpName { get; set; }
        public string usName { get; set; }
        public string hkName { get; set; }
        public string version { get; set; }
        public string releaseDate { get; set; }
        public string category { get; set; }
        public string publisher { get; set; }
        public string iconUrl { get; set; }
        public string bannerUrl { get; set; }
        public string intro { get; set; }
        public string description { get; set; }
        public string languages { get; set; }
        public long size { get; set; }
        public bool haveCn { get; set; }
        public bool star { get; set; }
    }
}
