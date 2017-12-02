using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestEmulator3000.Models
{
    public class ChestItemModel
    {
        public bool IsMaxedOut
        {
            get
            {
                return this.Level == 5;
            }
        }

        public EnumGrade Grade { get; set; }
        public string Name { get; set; }

        public bool Wanted { get; set; }

        public ChestItemModel()
        {

        }
        public ChestItemModel(
            int initLevel,
            EnumGrade grade, int i, bool wanted = true, string cardNameOptional = "")
        {
            this.Grade = grade;
            this.Name = String.Format("{0}_{1}_{2}", grade.ToString(), cardNameOptional, i.ToString());
            this.Wanted = wanted;

            this.InitialLevel = initLevel;
            this.Level = initLevel;
            this.CurrentDupes = 0;
        }

        #region progression stuff
        public int InitialLevel { get; set; }
        public int Level { get; set; }
        public int CurrentDupes { get; set; }

        public int GetDupesToLvlUp()
        {
            switch (Level)
            {
                case 1:
                    switch (Grade)
                    {
                        case EnumGrade.Common:
                            return 4;
                        case EnumGrade.Rare:
                            return 3;
                        case EnumGrade.Epic:
                            return 2;
                        case EnumGrade.Legendary:
                            return 1;
                        default: return 0;
                    }
                    break;
                case 2:
                    switch (Grade)
                    {
                        case EnumGrade.Common:
                            return 8;
                        case EnumGrade.Rare:
                            return 5;
                        case EnumGrade.Epic:
                            return 3;
                        case EnumGrade.Legendary:
                            return 2;
                        default: return 0;
                    }
                    break;
                case 3:
                    switch (Grade)
                    {
                        case EnumGrade.Common:
                            return 14;
                        case EnumGrade.Rare:
                            return 8;
                        case EnumGrade.Epic:
                            return 4;
                        case EnumGrade.Legendary:
                            return 3;
                        default: return 0;
                    }
                case 4:
                    switch (Grade)
                    {
                        case EnumGrade.Common:
                            return 16;
                        case EnumGrade.Rare:
                            return 12;
                        case EnumGrade.Epic:
                            return 5;
                        case EnumGrade.Legendary:
                            return 4;
                        default: return 0;
                    }
                default: return 0;
            }
        }
        public void ApplyDupe()
        {
            CurrentDupes++;
            if (Grade != EnumGrade.Cosmetic && CurrentDupes >= GetDupesToLvlUp())
            {
                LvlUp();
            }
        }
        public void LvlUp()
        {
            if (IsMaxedOut) return;
            Level++;
            CurrentDupes = 0;
        }
        public void Reset()
        {
            Level = InitialLevel;
            CurrentDupes = 0;
        }
        #endregion
    }
}
