using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    [Serializable]
    public class OperatorRequestObject
    {
        private string shortcode;
        public string Shortcode
        {
            get{ return shortcode;}
            set{ shortcode = value;}
        }
    }
}
