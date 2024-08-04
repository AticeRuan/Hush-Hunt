using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HushHunt.Maui.Models
{
    public class GameModel
    {
        public HashSet<int> UsedImageNumbers { get; set; } = new HashSet<int>();
        public HashSet<(int, int)> UsedPositions { get; set; } = new HashSet<(int, int)>();
        public List<(Image MainGridImage, Image FlexLayoutImage)> TargetItems { get; set; } = new List<(Image, Image)>();
        public int LevelCount { get; set; } = 1;
        public int HintCount { get; set; } = 5;
    }
}
