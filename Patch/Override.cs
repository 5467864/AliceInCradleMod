using evt;
using HarmonyLib;
using nel;
using UnityEngine;
using XX;
using static AliceInCradle.ConfigManage;

// ReSharper disable PossibleNullReferenceException
// ReSharper disable InconsistentNaming

namespace AliceInCradle.Patch
{
    internal class Override
    {
        [HarmonyPatch(typeof(ReelExecuter), "decideRotate")]
        [HarmonyPrefix]
        public static bool ReelExecuter_decideRotate_Prefix(ReelExecuter __instance, ref bool __result,
            bool randomise = false)
        {
            if (__instance.Acontent == null)
                __instance.initReelContent();
            if (__instance.content_id_dec >= 0)
            {
                __result = true;
                return false;
            }

            if (__instance.t_state < 0 || __instance.etype == ReelExecuter.ETYPE.ITEMKIND && __instance.t_state < 24)
            {
                __result = false;
                return false;
            }

            if (randomise)
            {
                __instance.content_id_dec = X.xors(__instance.Acontent.Length);
            }
            else
            {
                if (Input.GetKey(BestReel.Value))
                {
                    __instance.content_id_dec = __instance.etype switch
                    {
                        ReelExecuter.ETYPE.GRADE1 => 3,
                        ReelExecuter.ETYPE.GRADE2 => 2,
                        ReelExecuter.ETYPE.GRADE3 => 0,
                        ReelExecuter.ETYPE.COUNT_ADD1 => 1,
                        ReelExecuter.ETYPE.COUNT_ADD2 => 10,
                        ReelExecuter.ETYPE.COUNT_ADD3 => 1,
                        ReelExecuter.ETYPE.COUNT_MUL1 => 1,
                        ReelExecuter.ETYPE.ADD_MONEY => 2,
                        ReelExecuter.ETYPE.RANDOM => 0,
                        _ => X.IntR(__instance.content_id)
                    };
                }
                else
                {
                    __instance.content_id_dec = X.IntR(__instance.content_id);
                }
            }

            __instance.stt = ReelExecuter.ESTATE.OPEN;
            __instance.t_state = 0;
            __instance.Ui.playSnd("slot_stop");
            __instance.ATx[1].text_content = "";
            __instance.Ui.getEffect().PtcSTsetVar("w", __instance.reel_width_px)
                .PtcSTsetVar("h", __instance.reel_height_px * 2.0).PtcST("reel_decided", __instance);
            __instance.Ui.PadVib("reel_decide_0");
            if (__instance.etype == ReelExecuter.ETYPE.ITEMKIND)
            {
                var currentItemReel = __instance.Con.getCurrentItemReel();
                currentItemReel.touchObtainCountAll();
                var nelItemEntry = currentItemReel[__instance.content_id_dec % __instance.Acontent.Length];
                __instance.IKRow = new NelItemEntry(nelItemEntry.Data, nelItemEntry.count, nelItemEntry.grade);
            }

            __result = true;
            return false;
        }
    }
}