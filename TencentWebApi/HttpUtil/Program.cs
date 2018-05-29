using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpUtil util = new HttpUtil();
            util.HttpGet("","");
        }
    }
}
