using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestEmulator3000.Models
{
    public abstract class ChestModel
    {
        protected Random _rnd;
        protected int itemsToGive;
        public List<ChestItemModel> Items { get; set; }

        protected void resetRandom()
        {
            _rnd = new Random(Guid.NewGuid().GetHashCode());
        }
        public virtual ChestItemModel RollOne()
        {
            resetRandom();
            var n = this.Items.Count();
            var i = _rnd.Next(0, n);
            return this.Items[i];
        }
        public virtual List<ChestItemModel> RollAll()
        {
            resetRandom();
            List<ChestItemModel> result = new List<ChestItemModel>();
            for (int i = 0; i < itemsToGive; i++)
            {
                var card = this.Items[_rnd.Next(0, this.Items.Count - 1)];
                result.Add(card);
            }
            return result;
        }
    }
    public class QualityChest : ChestModel
    {
        public int shit = 0;
        public QualityChest(Dictionary<EnumGrade, int> gradeCounts)
        {
            this.itemsToGive = 1;
            this.Items = new List<ChestItemModel>();
            int id = 0;
            foreach (var grc in gradeCounts)
            {
                for (int i = 0; i < grc.Value; i++)
                {
                    this.Items.Add(new ChestItemModel(0, grc.Key, id));
                }
            }
        }
    }
    public class RadiantChestModel : ChestModel
    {
        bool boosted { get; set; }
        bool cardUniquePerChest { get; set; }
        public ChestModel qualityChest { get; set; }

        public EnumGrade GetQuality()
        {
            return qualityChest.RollOne().Grade;
        }

        public Dictionary<EnumGrade, List<ChestItemModel>> groupedCards;
        public RadiantChestModel(List<ChestItemModel> items, Dictionary<EnumGrade, int> gradeCounts, int itemCount = 4, bool cardUniquePerChest = true)
        {
            this.itemsToGive = itemCount;
            this.Items = items;
            this.groupedCards = items.GroupBy(x => x.Grade)
                .ToDictionary(x => x.Key, x => x.ToList());
            this.qualityChest = new QualityChest(gradeCounts);
            this.cardUniquePerChest = cardUniquePerChest;
        }

        public override List<ChestItemModel> RollAll()
        {
            resetRandom();
            List<ChestItemModel> result = new List<ChestItemModel>();
            for (int i = 0; i < itemsToGive; i++)
            {
                bool unique = false;
                ChestItemModel card = null;

                do
                {
                    var grade = GetQuality();
                    var itemPool = groupedCards[grade];
                    if (itemPool.Any())
                    {
                        var tCard = itemPool[_rnd.Next(0, itemPool.Count - 1)];
                        unique = !result.Any(x => x.Name == tCard.Name);

                        if (unique || !cardUniquePerChest)
                        {
                            card = tCard;
                        }
                    }
                    else
                    {
                        qualityChest.Items.RemoveAll(x => x.Grade == grade);
                    }
                }
                while (card == null && this.Items.Any());

                if (card != null)
                {
                    result.Add(card);
                    card.ApplyDupe();
                    if (card.IsMaxedOut)
                    {
                        this.Items.Remove(card);
                    }
                }
            }
            return result;
        }
    }
}
