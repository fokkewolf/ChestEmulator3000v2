using ChestEmulator3000.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestEmulator3000
{
    class Program
    {
        static int commonCountPerChar = 8;
        static int rareCountPerChar = 5;
        static int epicCountPerChar = 3;
        static int legendaryCountPerChar = 4;
        static int CharCount = 34;

        public static Int32 GetIntValueFromConsole()
        {
            Int32 result = 0;
            bool succ = false;
            while (!succ)
            {
                succ = Int32.TryParse(Console.ReadLine(), out result);
            }
            return result;
        }
        public static Boolean GetBooleanValueFromConsole()
        {
            Boolean result = false;
            bool succ = false;
            while (!succ)
            {
                var str = Console.ReadLine();
                if (str == "y" || str == "Y" || str == "1")
                {
                    succ = true;
                    result = true;
                }
                else if (str == "n" || str == "N" || str =="0")
                {
                    succ = true;
                    result = false;
                }
                else
                {
                    succ = Boolean.TryParse(str, out result);
                }
            }
            return result;
        }

        public static  Dictionary<EnumGrade, int> getQualityRatesDictionary()
        {
            var gradeCounts =new Dictionary<EnumGrade, int>();
            
            gradeCounts.Add(EnumGrade.Common, 48);
            gradeCounts.Add(EnumGrade.Rare, 24);
            gradeCounts.Add(EnumGrade.Epic, 7);
            gradeCounts.Add(EnumGrade.Legendary, 1);
            gradeCounts.Add(EnumGrade.Cosmetic, 19);

            return gradeCounts;
        }
        public static List<ChestItemModel> GetRadiantChestContentsAndWantedItems(
            out List<ChestItemModel> personWantedCards,
            int startingLevel,
            int commonCount,
            int rareCount,
            int epicCount, 
            int legendaryCount,
            int? cosmeticsCount,
            bool wantAllCards = false,
            bool wantAllCommonForCharacter = true,
            bool wantAllRareForCharacter = true,
            bool wantAllEpicForCharacter = true,
            bool wantAllLegendaryForCharacter = true,
            string characterName = "Ash")
        {
            #region Fill Chest
            //Indexers used to make something like a unique card ID
            var iCommon = 0;
            var iRare = 0;
            var iEpic = 0;
            var iLegendary = 0;

            personWantedCards = new List<ChestItemModel>();
            List<ChestItemModel> RadiantChestItems = new List<ChestItemModel>();
            do
            {
                bool wanted = false;
                if (iRare < rareCount)
                {
                    iRare++;
                    wanted = wantAllCards || (wantAllRareForCharacter && iRare <= rareCountPerChar);
                    var newRareCard = new ChestItemModel(startingLevel, EnumGrade.Rare, iRare, false, wanted ? characterName : "");
                    RadiantChestItems.Add(newRareCard);
                    if (wanted)
                    {
                        personWantedCards.Add(newRareCard);
                    }
                }
                if (iEpic < epicCount)
                {
                    iEpic++;
                    wanted = wantAllCards || (wantAllEpicForCharacter && iEpic <= epicCountPerChar);
                    var newEpicCard = new ChestItemModel(startingLevel, EnumGrade.Epic, iEpic, false, wanted ? characterName : "");
                    RadiantChestItems.Add(newEpicCard);
                    if (wanted)
                    {
                        personWantedCards.Add(newEpicCard);
                    }
                }
                if (iLegendary < legendaryCount)
                {
                    iLegendary++;
                    wanted = wantAllCards || (wantAllLegendaryForCharacter && iLegendary <= legendaryCountPerChar);
                    var newLegendaryCard = new ChestItemModel(startingLevel, EnumGrade.Legendary, iLegendary, false, wanted ? characterName : "");
                    RadiantChestItems.Add(newLegendaryCard);
                    if (wanted)
                    {
                        personWantedCards.Add(newLegendaryCard);
                    }
                }

                iCommon++;
                wanted = wantAllCards || (wantAllCommonForCharacter && iCommon <= commonCountPerChar);
                var newCommonCard = new ChestItemModel(startingLevel, EnumGrade.Common, iCommon, false, wanted ? characterName : "");
                RadiantChestItems.Add(newCommonCard);
                if (wanted)
                {
                    personWantedCards.Add(newCommonCard);
                }
            } while (iCommon != commonCount);

            //At this point I assume radiant chest has an amount of skins / voicepacks and shit equal to the count of cards.
            var totalCards = RadiantChestItems.Count();
            for (int i = 1; i <= 306; i++)
            {
                RadiantChestItems.Add(new ChestItemModel(0, EnumGrade.Cosmetic, i, false));
            }
            #endregion
            return RadiantChestItems;
        }

        public static int getAverageNeededChestCounts(List<ChestItemModel> personWantedCards, RadiantChestModel chest, int attemptCount, int desiredLevel)
        {
            var personWantedCardsDictionary = personWantedCards.ToDictionary(x => x.Name, x => x);

            int totalChests = 0;
            #region Opening chests
            //Duh
            for (int i = 0; i < attemptCount; i++)
            {
                int chestCount = 0;
                int fuckingHell = 0;
                //Open chests until all desired cards are open
                while (!personWantedCards.All(c => c.Level >= desiredLevel))
                {
                    var fuck = chest.RollAll();
                    chestCount++;
                    //Check received stuff, see if any good dupes
                    fuck.ForEach(f =>
                    {
                        if (!personWantedCardsDictionary.ContainsKey(f.Name))
                        {
                            fuckingHell++;
                        }
                        else
                        {
                            //Wooooo fucking hooo!
                            var personCard = personWantedCardsDictionary[f.Name];
                            personCard.ApplyDupe();
                        }
                        //Console.WriteLine(chestCount.ToString() + " " + personWantedCards.Count(x => x.Level != 5));
                    });
                }

                totalChests += chestCount;
                Console.WriteLine(String.Format("Attempt {0}, ChestCount {1}, Unnecessary Dupes = {2}", i, chestCount, fuckingHell));
                personWantedCards.ForEach(c => c.Reset());
            }
            #endregion
            return totalChests / attemptCount;
        }

        static bool workFlow()
        {
            Console.WriteLine("StartingLevel: ");
            int startingLevel = GetIntValueFromConsole();
            Console.WriteLine("DesiredLevel: ");
            int desiredLevel = GetIntValueFromConsole();
            Console.WriteLine("AttemptsCount: ");
            int attemptCount = GetIntValueFromConsole();
            Console.WriteLine("Items in chest: ");
            int itemsInChest = GetIntValueFromConsole();
            Console.WriteLine("CardUniquePerChest: ");
            bool cardUniquePerChest = GetBooleanValueFromConsole();

            Console.WriteLine("WantAllCards: ");
            bool wantAllCards = GetBooleanValueFromConsole();

            bool wantAllCommonForChar = wantAllCards;
            bool wantAllRareForChar = wantAllCards;
            bool wantAllEpicForChar = wantAllCards;
            bool wantAllLegendaryForChar = wantAllCards;

            if (!wantAllCards)
            {
                Console.WriteLine("WantAllCommonForChar: ");
                wantAllCommonForChar = GetBooleanValueFromConsole();
                Console.WriteLine("WantAllRareForChar: ");
                wantAllRareForChar = GetBooleanValueFromConsole();
                Console.WriteLine("WantAllEpicForChar: ");
                wantAllEpicForChar = GetBooleanValueFromConsole();
                Console.WriteLine("WantAllLegendaryForChar: ");
                wantAllLegendaryForChar = GetBooleanValueFromConsole();
            }
            Console.WriteLine("Character Name, Lol: ");
            string charName = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(charName)) charName = "Ash";

            //For convenience so that we don't need to delve into chest filling code to affect quantities
            var commonCount = commonCountPerChar * CharCount;
            var rareCount = rareCountPerChar * CharCount;
            var epicCount = epicCountPerChar * CharCount;
            var legendaryCount = legendaryCountPerChar * CharCount;
            var cosmeticsCount = 300;

            List<ChestItemModel> personWantedCards = new List<ChestItemModel>();
            var RadiantChestItems = GetRadiantChestContentsAndWantedItems(
                out personWantedCards, 
                startingLevel, 
                commonCount, 
                rareCount, 
                epicCount, 
                legendaryCount, 
                cosmeticsCount,
                wantAllCards,
                wantAllCommonForChar,
                wantAllRareForChar,
                wantAllEpicForChar,
                wantAllLegendaryForChar,
                charName);
            RadiantChestModel chest = new RadiantChestModel(RadiantChestItems, getQualityRatesDictionary(), itemsInChest, cardUniquePerChest);


            var avgChestsNeeded = getAverageNeededChestCounts(personWantedCards, chest, attemptCount, desiredLevel);

            Console.WriteLine(String.Format("Average Chests Needed - {0}", avgChestsNeeded));

            Console.WriteLine("Again? y/n");
            bool again = GetBooleanValueFromConsole();
             
            return again;
        }

        static void Main(string[] args)
        {

            while (workFlow())
            {
                Console.WriteLine("\n");
            }
        }
    }
}
