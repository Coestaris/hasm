using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASMLib
{
    [Flags]
    public enum HASMMachineBannedFeatures
    {
        Preprocessor = 0x1,
        Keywords = 0x2,
        Comments = 0x4,
        MultipleInstructionsPerLine = 0x8,
    }
}
