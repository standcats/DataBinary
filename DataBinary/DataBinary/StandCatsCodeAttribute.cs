using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandCats
{
    public class StandCatsCodeAttribute : Attribute
    {
        public byte Code;
        public StandCatsCodeAttribute(byte Code)
        {
            this.Code = Code;
        }
    }
}
