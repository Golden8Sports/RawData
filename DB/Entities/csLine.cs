using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entities
{
    public class csLine
    {
        private int _minutesStopped;
        public int MinutesStopped
        {
            get { return _minutesStopped; }
            set { _minutesStopped = value; }
        }


        private int _minutesHeartBeat;
        public int MinutesHeartBeat
        {
            get { return _minutesHeartBeat; }
            set { _minutesHeartBeat = value; }
        }


        private string _lastDate;
        public String LastDate
        {
            get { return _lastDate; }
            set { _lastDate = value; }
        }

        public csLine() { }

        public csLine(string lastDate, int minstop, int heartBeat)
        {
            this._lastDate = lastDate;
            this._minutesStopped = minstop;
            this._minutesHeartBeat = heartBeat;
        }
    }
}
