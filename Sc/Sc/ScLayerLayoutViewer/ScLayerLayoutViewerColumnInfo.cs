using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc
{
    public class ScLayerLayoutViewerColumnInfo
    {
        public string dataName;
        public string displayName;
        public bool   isHideName;
        public bool isHideColoum;
        public float  length;

        public ScLayerLayoutViewerColumnInfo(string dataName, string displayName, bool isHideName, bool isHideColoum, float length)
        {
            this.dataName = dataName;
            this.displayName = displayName;
            this.isHideName = isHideName;
            this.isHideColoum = isHideColoum;
            this.length = length;
        }

    }
}
