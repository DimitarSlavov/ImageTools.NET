namespace Tools.Core.Models
{
    public class ImageDetails : ImageDetailsShort
    {        
        public string Name { get; set; }

        public string Type { get; set; }

        public long Size { get; set; }

        public int Quality { get; set; }
    }
}
