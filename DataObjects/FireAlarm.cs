using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    internal class FireAlarm
    {

        public string FPanelID { get; set; }
        public DateOnly TestLast { get; set; }
        public DateOnly TestNext { get; set; }

    }
}
