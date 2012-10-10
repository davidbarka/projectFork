using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForkliftManager
{
    [Serializable]
    class ServiceHistory
    {
        private string ServiceYear { get; set; }
        private string ServiceMonth { get; set; }
        private int ServiceHours { get; set; }

    }
}
