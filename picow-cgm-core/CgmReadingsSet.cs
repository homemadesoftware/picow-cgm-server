using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace picow_cgm_core
{
    public class CgmReadingsSet
    {
        public ReadingItem[]? items { get; set; }

        public class ReadingItem
        {
            public DateTime dateTime { get; set; }
            public float convertedReading { get; set; }
        }
    }
}
