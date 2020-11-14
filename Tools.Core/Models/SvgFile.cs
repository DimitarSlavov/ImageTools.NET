using System.Collections.Generic;

namespace Tools.Core.Models
{
    public struct SvgFile
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public IEnumerable<SvgDetails> SvgDetailsCollection { get; set; }
    }
}
