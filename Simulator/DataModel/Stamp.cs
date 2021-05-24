using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Stamp
    {
        public Stamp(string NewInfo, DateTime WhenWasThat)
        {
            Data = NewInfo;
            TimeStamp = WhenWasThat;
        }

        public Stamp() { }

        public string Data { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
