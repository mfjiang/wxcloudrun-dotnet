using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aspnetapp.Code.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("user_props")]
    public class UserProps
    {
        public long user_id { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string county { get; set; }
        public string address { get; set; }

    }
}
