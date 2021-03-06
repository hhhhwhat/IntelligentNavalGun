﻿using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Data.SQLite;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    class ConstructionRecords : RecordsGroup
    {
        public override string GroupName => "construction";

        internal ConstructionRecords(SQLiteConnection rpConnection) : base(rpConnection)
        {
            DisposableObjects.Add(ConstructionDock.NewConstruction.Subscribe(InsertRecord));
        }

        protected override void CreateTable()
        {
            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS construction(" +
                    "time INTEGER PRIMARY KEY NOT NULL, " +
                    "ship INTEGER NOT NULL, " +
                    "fuel INTEGER NOT NULL, " +
                    "bullet INTEGER NOT NULL, " +
                    "steel INTEGER NOT NULL, " +
                    "bauxite INTEGER NOT NULL, " +
                    "dev_material INTEGER NOT NULL, " +
                    "flagship INTEGER NOT NULL, " +
                    "hq_level INTEGER NOT NULL, " +
                    "is_lsc BOOLEAN NOT NULL," +
                    "empty_dock INTEGER)";

                rCommand.ExecuteNonQuery();
            }
        }

        internal void InsertRecord(ConstructionDock rpData)
        {
            var rPort = KanColleGame.Current.Port;
            var rShip = rpData.Ship;
            var rSecretaryShip = rPort.Fleets[1].Ships[0].Info;
            var rHeadquarterLevel = rPort.Admiral.Level;
            var rEmptyDockCount = !rpData.IsLargeShipConstruction.Value ? (int?)null : rPort.ConstructionDocks.Values.Count(r => r.State == ConstructionDockState.Idle);

            using (var rCommand = Connection.CreateCommand())
            {
                rCommand.CommandText = "INSERT INTO construction(time, ship, fuel, bullet, steel, bauxite, dev_material, flagship, hq_level, is_lsc, empty_dock) " +
                    "VALUES(strftime('%s', 'now'), @ship, @fuel, @bullet, @steel, @bauxite, @dev_material, @flagship, @hq_level, @is_lsc, @empty_dock);";
                rCommand.Parameters.AddWithValue("@ship", rShip.ID);
                rCommand.Parameters.AddWithValue("@fuel", rpData.FuelConsumption);
                rCommand.Parameters.AddWithValue("@bullet", rpData.BulletConsumption);
                rCommand.Parameters.AddWithValue("@steel", rpData.SteelConsumption);
                rCommand.Parameters.AddWithValue("@bauxite", rpData.BauxiteConsumption);
                rCommand.Parameters.AddWithValue("@dev_material", rpData.DevelopmentMaterialConsumption);
                rCommand.Parameters.AddWithValue("@flagship", rSecretaryShip.ID);
                rCommand.Parameters.AddWithValue("@hq_level", rHeadquarterLevel);
                rCommand.Parameters.AddWithValue("@is_lsc", rpData.IsLargeShipConstruction);
                rCommand.Parameters.AddWithValue("@empty_dock", rEmptyDockCount);

                rCommand.ExecuteNonQuery();
            }
        }
    }
}
