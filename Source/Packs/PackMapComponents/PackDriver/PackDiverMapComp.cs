﻿using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
namespace AvaliMod
{
    public class AvaliPackDriver : MapComponent
    {
        private readonly bool enableDebug = LoadedModManager.GetMod<RimValiMod>().GetSettings<RimValiModSettings>().enableDebugMode;
        private readonly int maxSize = LoadedModManager.GetMod<RimValiMod>().GetSettings<RimValiModSettings>().maxPackSize;
        private readonly bool packsEnabled = LoadedModManager.GetMod<RimValiMod>().GetSettings<RimValiModSettings>().packsEnabled;
        private readonly bool checkOtherRaces = LoadedModManager.GetMod<RimValiMod>().GetSettings<RimValiModSettings>().checkOtherRaces;
        private readonly bool allowAllRaces = LoadedModManager.GetMod<RimValiMod>().GetSettings<RimValiModSettings>().allowAllRaces;
        private bool AddedRaces = false;
        private int onTick = 0;
        public AvaliPackDriver(Map map)
            : base(map)
        {

        }
        List<ThingDef> racesInPacks = new List<ThingDef>();
        public void AddRaces()
        {
            if (!AddedRaces)
            {
                racesInPacks.Add(AvaliDefs.RimVali);
                if (checkOtherRaces)
                {
                    foreach(ThingDef race in RimvaliPotentialPackRaces.potentialPackRaces)
                    {
                        racesInPacks.Add(race);
                    }
                }
                if (allowAllRaces)
                {
                    foreach(ThingDef race in RimvaliPotentialPackRaces.potentialRaces)
                    {
                        racesInPacks.Add(race);
                    }
                }
                AddedRaces = true;
            }
        }


        public void UpdatePacks()
        {
            IEnumerable<Pawn> pawnsOnMap = RimValiUtility.AllPawnsOfRaceOnMap(AvaliDefs.RimVali, map);
            foreach (Pawn pawn in pawnsOnMap)
            {
                if (racesInPacks.Contains(pawn.def))
                {
                    PackComp comp = pawn.TryGetComp<PackComp>();
                    if (!(comp == null))
                    {
                        PawnRelationDef relationDef = comp.Props.relation;
                        if (RimValiUtility.GetPackSize(pawn, relationDef) == 1)
                        {
                            if (enableDebug)
                            {
                                Log.Message("Attempting to make pack..");
                            }
                            RimValiUtility.MakePack(pawn, relationDef, racesInPacks, maxSize);
                        }
                    }
                }
            }
        }

        public override void MapComponentTick()
        {
            if (!AddedRaces)
            {
                AddRaces();
            }
            if (onTick == 120)
            {
                if (packsEnabled)
                { 
                    UpdatePacks();
                }
                onTick = 0;
            }
            else
            {
                onTick += 1;
            }
        }

    }
}