using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace BannerlordTweaks.Patches
{
    [HarmonyPatch(typeof(MobileParty), "DailyTick")]
    public class MobilePartyDailyTickPatch
    {
        static void Postfix(MobileParty __instance)
        {
            var hasBoth = false;
            if (__instance.IsActive && __instance.HasPerk(DefaultPerks.Leadership.CombatTips) && __instance.HasPerk(DefaultPerks.Leadership.CombatTips))
            {
                hasBoth = true;
            }
            if (__instance.IsActive && __instance.HasPerk(DefaultPerks.Leadership.CombatTips))
            {
                for (int i = 0; i < __instance.MemberRoster.Count; i++)
                {
                    TroopRosterElement troopElement = __instance.MemberRoster.GetElementCopyAtIndex(i);
                    int totalTroopXp = Settings.Instance.TroopPerkCombatTipsAmount * troopElement.Number;

                    //Remove the default added xp
                    totalTroopXp -= Campaign.Current.Models.PartyTrainingModel.GetTroopPerksXp(DefaultPerks.Leadership.CombatTips);

                    __instance.Party.MemberRoster.AddXpToTroopAtIndex(totalTroopXp, i);

                }
            }
            if (__instance.IsActive && __instance.HasPerk(DefaultPerks.Leadership.RaiseTheMeek))
            {
                for (int i = 0; i < __instance.MemberRoster.Count; i++)
                {
                    TroopRosterElement troopElement2 = __instance.MemberRoster.GetElementCopyAtIndex(i);
                    if (troopElement2.Character.Tier < 4)
                    {
                        int defaultSingleTroopPerksXp2 = Campaign.Current.Models.PartyTrainingModel.GetTroopPerksXp(DefaultPerks.Leadership.RaiseTheMeek);
                        int totalTroopXp2 = Settings.Instance.TroopPerkRaiseTheMeekAmount * troopElement2.Number;
                        if (!hasBoth)
                        {
                            //Remove the default added xp only if we haven't removed it for CombatTips, native doesn't support both
                            //even if its technically possible as party can have multiple leaders. It only applies CombatTips instead.
                            totalTroopXp2 -= Campaign.Current.Models.PartyTrainingModel.GetTroopPerksXp(DefaultPerks.Leadership.RaiseTheMeek);
                        }
                        __instance.Party.MemberRoster.AddXpToTroopAtIndex(totalTroopXp2, i);
                    }

                }
            }

            //Need to re-run the check for upgrades for npcs, as this is a Postfix
            if (__instance.IsActive && __instance.MapEvent == null && __instance != MobileParty.MainParty)
            {
                Campaign.Current.PartyUpgrader.UpgradeReadyTroops(__instance.Party);
            }
        }

        static bool Prepare()
        {
            return Settings.Instance.TroopPerkXpMultipliedByStackEnabled;
        }
    }
}
