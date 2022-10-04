using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class Thermostat
    {

        public string ThermostatID { get; set; }
        public int Temp { get; set; }
        public string TempUnit { get; set; } //C or F
        public string FanSetting { get; set; } //High, Med, Low
        public string CoreSetting { get; set; } // Heat, Cool, Off

    }
}
