﻿using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Internal;
using Sakuno.KanColle.Amatsukaze.Models.Records;
using Sakuno.KanColle.Amatsukaze.ViewModels.Records.Primitives;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Records
{
    class ExpeditionHistoryViewModel : HistoryViewModelWithTimeFilter<ExpeditionRecord>
    {
        protected override string LoadCommandText => "SELECT * FROM expedition WHERE time >= {0} AND time < {1};";

        public FilterKeyCollection<ExpeditionInfo> Expeditions { get; } = new FilterKeyCollection<ExpeditionInfo>(ExpeditionInfo.Dummy, (x, y) => x.ID - y.ID);

        bool r_SuccessfulRecordsOnly;
        public bool SuccessfulRecordsOnly
        {
            get { return r_SuccessfulRecordsOnly; }
            set
            {
                if (r_SuccessfulRecordsOnly != value)
                {
                    r_SuccessfulRecordsOnly = value;
                    OnPropertyChanged(nameof(SuccessfulRecordsOnly));

                    Refresh();
                }
            }
        }

        ExpeditionInfo r_Expedition = ExpeditionInfo.Dummy;
        public ExpeditionInfo SelectedExpedition
        {
            get { return r_Expedition; }
            set
            {
                if (r_Expedition != value)
                {
                    r_Expedition = value;
                    OnPropertyChanged(nameof(SelectedExpedition));

                    Refresh();
                }
            }
        }

        public override void OnInitialized()
        {
            var rMasterInfo = KanColleGame.Current.MasterInfo;

            using (var rCommand = CreateCommand())
            {
                rCommand.CommandText = "SELECT DISTINCT expedition FROM expedition ORDER BY expedition;";
                using (var rReader = rCommand.ExecuteReader())
                {
                    var rExpeditions = new List<ExpeditionInfo>(rReader.VisibleFieldCount);

                    while (rReader.Read())
                    {
                        ExpeditionInfo rExpedition;
                        if (rMasterInfo.Expeditions.TryGetValue(rReader.GetInt32(0), out rExpedition))
                            rExpeditions.Add(rExpedition);
                    }

                    Expeditions.Update(rExpeditions);
                }
            }
        }

        public override bool Filter(ExpeditionRecord rpItem) =>
            (!r_SuccessfulRecordsOnly || rpItem.Result != ExpeditionResult.Failure) &&
            (r_Expedition == ExpeditionInfo.Dummy || rpItem.Expedition == r_Expedition);

        protected override ExpeditionRecord CreateRecordFromReader(SQLiteDataReader rpReader) => new ExpeditionRecord(rpReader);

        protected override bool TableFilter(string rpTable) => rpTable == "main.expedition";

        protected override void OnRecordInsert(string rpTable, long rpRowID)
        {
            base.OnRecordInsert(rpTable, rpRowID);

            Expeditions.AddIfAbsent(LastInsertedRecord.Expedition);
        }

        protected override void PrepareCommandOnRecordInsert(SQLiteCommand rpCommand, string rpTable, long rpRowID)
        {
            if (SelectedTimeSpan.Type == TimeSpanType.Custom && !IsInTimeSpan(rpRowID))
                return;

            rpCommand.CommandText = "SELECT * FROM expedition WHERE time = @time LIMIT 1;";
            rpCommand.Parameters.AddWithValue("@time", rpRowID);
        }
    }
}
