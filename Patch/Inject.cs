using System.Collections.Generic;
using System.Diagnostics;
using AliceInCradle.Mono;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using evt;
using m2d;
using nel;
using XX;
using static AliceInCradle.ConfigManage;
using Random = UnityEngine.Random;

// ReSharper disable InconsistentNaming

namespace AliceInCradle.Patch
{
    public class Inject
    {
        private static readonly ManualLogSource EVENT = Loader.EVENT;

        public static readonly Dictionary<string, ReelManager.ItemReelContainer[]> Reels = new();

        [HarmonyPatch(typeof(MosaicShower), "FnDrawMosaic")]
        [HarmonyPrefix] // 去马赛克
        private static bool MosaicShower_FnDrawMosaic_Prefix(ref bool __result)
        {
            if (!NoMosaic.Value) return true;
            __result = false;
            return false;
        }

        [HarmonyPatch(typeof(SceneTitleTemp), "Awake")]
        [HarmonyPostfix] // 注入 标题场景.预初始化
        private static void SceneTitleTemp_Awake_Postfix()
        {
            // EVENT.LogInfo("SceneTitleTemp_Awake");
            Loader.Plugin = new GameObject("PluginObject");
            Loader.Plugin.AddComponent(typeof(Plugin));
        }

        [HarmonyPatch(typeof(SceneGame), "Awake")]
        [HarmonyPostfix] // 注入 游戏场景.预初始化
        private static void SceneGame_Awake_Postfix()
        {
            // EVENT.LogInfo("SceneGame_Awake");
            Loader.Plugin = new GameObject("PluginObject");
            Loader.Plugin.AddComponent(typeof(Plugin));
        }

        [HarmonyPatch(typeof(NelItemManager), "newGame")]
        [HarmonyPostfix]
        private static void NelItemManager_newGame_Postfix(NelItemManager __instance)
        {
            Loader.StInventory = __instance.StInventory;
            Loader.StPrecious = __instance.StPrecious;
            Loader.StHouseInventory = __instance.StHouseInventory;
            Loader.StEnhancer = __instance.StEnhancer;
            Loader.ReelM = __instance.ReelM;
        }

        [HarmonyPatch(typeof(ReelManager), "getCurrentItemReel")]
        [HarmonyPrefix] // 注入 获取当前物品卷轴 替换返回值
        public static bool ReelManager_getCurrentItemReel_Prefix(ReelManager __instance,
            ref ReelManager.ItemReelContainer __result)
        {
            if (__instance.AStackIR.Count <= 0)
            {
                __result = null;
                return false;
            }

            if (!Reels.ContainsKey(__instance.AStackIR[0].tx_key))
            {
                var ir = __instance.AStackIR[0];
                var orig = new ReelManager.ItemReelContainer(ir.key, ir.ColSet);
                ir.AContent.ForEach(i =>
                    {
                        orig.AContent.Add(new NelItemEntry(i.Data, i.count, i.grade));
                        i.count = Random.Range(X.Mx(i.count - 1, 0) * 100, X.Mx(i.count, 1) * 100);
                    }
                );
                Reels.Add(ir.tx_key, new[] { orig, ir });
                EVENT.LogInfo("???ADD:" + ir.tx_key);
            }

            __result = EnhancedItemList.Value
                ? __instance.AStackIR[0]
                : Reels[__instance.AStackIR[0].tx_key][0];
            return false;
        }

        [HarmonyPatch(typeof(M2EventItem_ItemSupply), "getDataList")]
        [HarmonyPrefix]
        public static bool getDataList_Prefix(M2EventItem_ItemSupply __instance, ref bool is_reel,
            ref NelItemEntry[] __result)
        {
            var ir = __instance.IReel;
            if (!Reels.ContainsKey(ir.tx_key))
            {
                var orig = new ReelManager.ItemReelContainer(ir.key, ir.ColSet);
                ir.AContent.ForEach(i =>
                    {
                        orig.AContent.Add(new NelItemEntry(i.Data, i.count, i.grade));
                        i.count = Random.Range(X.Mx(i.count - 1, 0) * 100, X.Mx(i.count, 1) * 100);
                    }
                );
                Reels.Add(ir.tx_key, new[] { orig, ir });
                EVENT.LogInfo("ADD:" + ir.tx_key);
            }

            is_reel = __instance.LpCon.is_reel;
            __result = EnhancedItemList.Value
                ? __instance.IReel.ToArray()
                : Reels[__instance.IReel.tx_key][0].ToArray();
            return false;
        }

        [HarmonyPatch(typeof(NelItem), "fnGetDetailItemReel")]
        [HarmonyPrefix] // 注入 获取物品卷轴详细信息 替换返回值
        public static bool NelItem_fnGetDetailItemReel_Prefix(NelItem Itm, int grade, string def, ref string __result)
        {
            var text = TX.slice(Itm.key, "itemreelC_".Length);
            var ir = ReelManager.GetIR(text);
            if (ir != null)
            {
                if (!Reels.ContainsKey(ir.tx_key))
                {
                    var orig = new ReelManager.ItemReelContainer(ir.key, ir.ColSet);
                    ir.AContent.ForEach(i =>
                        {
                            orig.AContent.Add(new NelItemEntry(i.Data, i.count, i.grade));
                            i.count = Random.Range(X.Mx(i.count - 1, 0) * 100, X.Mx(i.count, 1) * 100);
                        }
                    );
                    Reels.Add(ir.tx_key, new[] { orig, ir });
                    EVENT.LogInfo("ADD:" + ir.tx_key);
                }

                __result = EnhancedItemList.Value ? ir.listupItems("／") : Reels[ir.tx_key][0].listupItems("／");
                return false;
            }

            __result = "<ERROR> No Specfic ItemReel:\n" + text;
            return false;
        }

        [HarmonyPatch(typeof(EV), "FixedUpdate")]
        [HarmonyPrefix]
        private static bool EV_FixedUpdate_Prefix(EV __instance)
        {
            __instance.runEvInner(1f);
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            if ((EV.debug & EV.EVDEBUG.ALLOC_CONSOLE) != 0 && IN.getKD(ConfigManage.Debug.Value))
            {
                EV.Dbg.changeActivate(!EV.Dbg.isActive() && !EV.Dbg.isELActive());
            }

            return false;
        }

        [HarmonyPatch(typeof(M2Attackable), "applyHpDamage")]
        [HarmonyPrefix]
        private static bool M2Attackable_applyHpDamage_Prefix(int val)
        {
            // 如果 NoHpDamage==false 则 执行原函数
            if (!NoHpDamage.Value) return true;
            var sfs = new StackTrace(true).GetFrames();
            if (sfs != null)
            {
                // 如果 ==applyHpDamageSimple 则 跳过原函数
                return !sfs[2].GetMethod().Name.Equals("applyHpDamageSimple");
            }

            return true;
        }

        [HarmonyPatch(typeof(PR), "applyDamage", typeof(NelAttackInfo), typeof(bool))]
        [HarmonyPrefix]
        private static bool PR_applyDamage_Prefix(ref int __result)
        {
            if (!NoHpDamage.Value) return true;
            __result = 0;
            return false;
        }

        [HarmonyPatch(typeof(M2Attackable), "applyMpDamage")]
        [HarmonyPrefix]
        private static bool M2Attackable_applyMpDamage_Prefix(int val)
        {
            // 如果 NoMpDamage==false 则 执行原函数
            if (!NoMpDamage.Value) return true;
            var sfs = new StackTrace(true).GetFrames();
            if (sfs == null) return true;
            // 如果 ==PR 则 跳过原函数
            return sfs[2].GetMethod().DeclaringType?.Name != "PR";
        }

        [HarmonyPatch(typeof(PR), "getCastableMp")]
        [HarmonyPrefix]
        private static bool PR_getCastableMp_Prefix(PR __instance, ref float __result)
        {
            if (!NoMpDamage.Value) return true;
            __result = __instance.mp;
            return false;
        }

        [HarmonyPatch(typeof(M2PrSkill), "AtkMul")]
        [HarmonyPrefix]
        private static void M2PrSkill_AtkMul_Prefix(NelAttackInfo Atk, ref float hpdmg, ref float mpdmg)
        {
            hpdmg *= HpMultiply.Value;
            mpdmg *= MpMultiply.Value;
            // EVENT.LogInfo($"M2PrSkill_AtkMul_Prefix: {hpdmg}, {mpdmg}");
        }

        [HarmonyPatch(typeof(PR), "applyDamage", typeof(NelAttackInfo), typeof(bool), typeof(string), typeof(bool),
            typeof(bool))]
        [HarmonyPrefix]
        private static bool PR_applyDamage_Prefix(NelAttackInfo Atk)
        {
            if (!NoHpDamage.Value) return true;
            switch (Atk.ndmg)
            {
                case NDMG.MAPDAMAGE:
                case NDMG.MAPDAMAGE_LAVA:
                case NDMG.MAPDAMAGE_THUNDER:
                case NDMG.MAPDAMAGE_THUNDER_A:
                    return false;
                default:
                    EVENT.LogInfo("DMG:" + Atk.ndmg);
                    return true;
            }
        }
    }
}