using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestEmulator3000.Models
{
    public class ConfigModel
    {
        public int StartingLevel { get; set; }
        public int DesiredLevel { get; set; }

        public int CommonCountPerChar { get; set; }
        public int RareCountPerChar { get; set; }
        public int EpicCountPerChar { get; set; }
        public int LegendaryCountPerChar { get; set; }
        public int CosmeticsCount { get; set; }

        public int AttemptsCount { get; set; }
        public int ItemsPerchest { get; set; }
        public bool CardUniquePerChest { get; set; }
        public bool WantAllCards { get; set; }
        public bool WantAllCommonForChar { get; set; }
        public bool WantAllRareForChar { get; set; }
        public bool WantAllEpicForChar { get; set; }
        public bool WantAllLegendaryForChar { get; set; }
        public string CharacterName { get; set; }
        public int CharCount { get; set; }
    }
}
