using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class FrozenItem
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public DateTime FrozenDate { get; set; }

        public int TargetId { get; set; }
    }
}
