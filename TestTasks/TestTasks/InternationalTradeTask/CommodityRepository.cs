using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TestTasks.InternationalTradeTask.Models;

namespace TestTasks.InternationalTradeTask
{
    public class CommodityRepository
    {
        public double GetImportTariff(string commodityName)
        {
            foreach (var group in _allCommodityGroups)
            {
                var tariff = FindDeepestTariff(group, commodityName, true);
                if (tariff.HasValue)
                    return tariff.Value;
            }
            throw new ArgumentException("Commodity not found");
        }

        public double GetExportTariff(string commodityName)
        {
            foreach (var group in _allCommodityGroups)
            {
                var tariff = FindDeepestTariff(group, commodityName, false);
                if (tariff.HasValue)
                    return tariff.Value;
            }
            throw new ArgumentException("Commodity not found");
        }

        private FullySpecifiedCommodityGroup[] _allCommodityGroups = new FullySpecifiedCommodityGroup[]
        {
            new FullySpecifiedCommodityGroup("06", "Sugar, sugar preparations and honey", 0.05, 0)
            {
                SubGroups = new CommodityGroup[]
                {
                    new CommodityGroup("061", "Sugar and honey")
                    {
                        SubGroups = new CommodityGroup[]
                        {
                            new CommodityGroup("0611", "Raw sugar,beet & cane"),
                            new CommodityGroup("0612", "Refined sugar & other prod.of refining,no syrup"),
                            new CommodityGroup("0615", "Molasses", 0, 0),
                            new CommodityGroup("0616", "Natural honey", 0, 0),
                            new CommodityGroup("0619", "Sugars & syrups nes incl.art.honey & caramel"),
                        }
                    },
                    new CommodityGroup("062", "Sugar confy, sugar preps. Ex chocolate confy", 0, 0)
                }
            },
            new FullySpecifiedCommodityGroup("282", "Iron and steel scrap", 0, 0.1)
            {
                SubGroups = new CommodityGroup[]
                {
                    new CommodityGroup("28201", "Iron/steel scrap not sorted or graded"),
                    new CommodityGroup("28202", "Iron/steel scrap sorted or graded/cast iron"),
                    new CommodityGroup("28203", "Iron/steel scrap sort.or graded/tinned iron"),
                    new CommodityGroup("28204", "Rest of 282.0")
                }
            }
        };

        private double? FindDeepestTariff(ICommodityGroup commodity, string name, bool isImport)
        {
            double? tariff = null;
            int maxDepth = -1;

            void DFS(ICommodityGroup group, int depth, double? inheritedTariff)
            {
                if (group == null) return;

                double? currentTariff = isImport ? group.ImportTarif : group.ExportTarif;
                double? effectiveTariff = currentTariff ?? inheritedTariff;

                if (group.Name == name)
                {
                    if (depth > maxDepth)
                    {
                        maxDepth = depth;
                        tariff = effectiveTariff;
                    }
                }

                if (group.SubGroups != null)
                {
                    foreach (var subgroup in group.SubGroups)
                    {
                        DFS(subgroup, depth + 1, effectiveTariff);
                    }
                }
            }

            DFS(commodity, 0, null);
            return tariff;
        }


    }
}

