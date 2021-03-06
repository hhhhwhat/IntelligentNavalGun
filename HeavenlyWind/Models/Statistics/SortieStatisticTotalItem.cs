﻿using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models.Statistics
{
    class SortieStatisticTotalItem : ModelBase
    {
        IEnumerable<SortieStatisticData> r_Items;

        public int Count => r_Items.Sum(r => r.Count);
        public int BossBattleCount => r_Items.Sum(r => r.BossBattleCount);

        public int FuelConsumption => r_Items.Sum(r => r.FuelConsumption);
        public int BulletConsumption => r_Items.Sum(r => r.BulletConsumption);
        public int SteelConsumption => r_Items.Sum(r => r.SteelConsumption);
        public int BauxiteConsumption => r_Items.Sum(r => r.BauxiteConsumption);
        public int BucketConsumption => r_Items.Sum(r => r.BucketConsumption);

        public double RankingPoint => r_Items.Sum(r => r.RankingPoint);

        public int SRankCount => r_Items.Sum(r => r.SRankCount);
        public int ARankCount => r_Items.Sum(r => r.ARankCount);
        public int BRankCount => r_Items.Sum(r => r.BRankCount);
        public int FailureRankCount => r_Items.Sum(r => r.FailureRankCount);

        public int BossSRankCount => r_Items.Sum(r => r.BossSRankCount);
        public int BossARankCount => r_Items.Sum(r => r.BossARankCount);
        public int BossBRankCount => r_Items.Sum(r => r.BossBRankCount);
        public int BossFailureRankCount => r_Items.Sum(r => r.BossFailureRankCount);

        public SortieStatisticTotalItem(IEnumerable<SortieStatisticData> rpItems)
        {
            r_Items = rpItems;
        }
    }
}
