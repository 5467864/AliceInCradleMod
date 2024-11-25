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
        [HarmonyPatch(typeof(NelEvDebugListener), "initCategoryDangerousness")]
        [HarmonyPrefix]
        private static bool NelEvDebugListener_initCategoryDangerousness_Prefix(NelEvDebugListener __instance,
            Designer DsT, Designer DsL, Designer DsR)
        {
            DsT.Clear();
            DsL.Clear();
            DsR.Clear();
            NightController NC = __instance.NM2D.NightCon;
            DsL.add(new DsnDataInput
            {
                label = "当前地图",
                h = 24f,
                editable = false,
                bounds_w = DsL.use_w - 20f,
                size = 14,
                fnReturn = __instance.fnReturnNothing,
                def = ((__instance.NM2D.curMap != null) ? __instance.NM2D.curMap.key : "-")
            }).Br();
            __instance.addHr(DsL);
            DsL.add(new DsnDataInput
            {
                label = "危险",
                bounds_w = 240f,
                integer = true,
                h = 40f,
                size = 22,
                min = 0.0,
                max = 160.0,
                def = NC.getDangerMeterVal(true).ToString(),
                fnReturn = __instance.fnReturnNothing,
                fnChangedDelay = delegate(LabeledInputField Li)
                {
                    NC.applyDangerousFromEvent(X.NmI(Li.text, 0, true), true);
                    return true;
                }
            }).Br();
            DsL.add(new DsnDataInput
            {
                label = " └ D:果汁",
                bounds_w = 240f,
                integer = true,
                min = 0.0,
                max = 45.0,
                def = NC.getDangerAddedVal().ToString(),
                size = 14,
                fnChangedDelay = delegate(LabeledInputField Li)
                {
                    NC.setAdditionalDangerLevelManual(X.NmI(Li.text, 0, true));
                    return true;
                }
            }).Br();
            DsL.add(new DsnDataInput
            {
                label = "最大卷轴",
                bounds_w = 320f,
                integer = true,
                size = 22,
                h = 40f,
                min = 0.0,
                max = 99.0,
                def = NC.getBattleCount().ToString(),
                fnReturn = __instance.fnReturnNothing,
                fnChangedDelay = delegate(LabeledInputField Li)
                {
                    NC.setBattleCount(X.NmI(Li.text, 0, true));
                    return true;
                }
            }).Br();
            __instance.addHr(DsL);
            DsL.add(new DsnDataChecks
            {
                clms = 1,
                margin_h = 4,
                w = DsL.use_w,
                h = 28f,
                keys = new[] { "锁定危险性", "允许夜间快速旅行" },
                def = ((NC.debug_lock_dangerousness ? 1 : 0) | (NC.debug_allow_night_travel ? 2 : 0)),
                fnClick = __instance.fnChangeLock
            }).Br();
            int num = 7;
            string[] array = new string[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = "weather." + i;
            }

            __instance.BConWt = DsR.addButtonMultiT<aBtnNel>(new DsnDataButtonMulti
            {
                skin = "mini_dark",
                margin_h = 2f,
                margin_w = 2f,
                w = 32f,
                h = 32f,
                def = 0,
                titles = array,
                fnClick = __instance.fnClickWeather
            });
            __instance.fineWeatherBCon();
            DsR.Br();
            DsR.add(new DsnDataChecks
            {
                clms = 1,
                margin_h = 4,
                w = DsL.use_w,
                h = 28f,
                keys = new[] { "锁定天气" },
                def = (NC.debug_lock_weather ? 1 : 0),
                fnClick = __instance.fnChangeLock
            }).Br();
            return false;
        }

        [HarmonyPatch(typeof(EventLineDebugger), "prepareBasicPad")]
        [HarmonyPrefix]
        private static bool EventLineDebugger_prepareBasicPad_Prefix(EventLineDebugger __instance, Designer Ds,
            FnBtnBindings FnClick, bool add_playing_pad = false)
        {
            string[] array2;
            if (!add_playing_pad)
            {
                string[] array = new string[2];
                array[0] = "play";
                array2 = array;
                array[1] = "pause";
            }
            else
            {
                string[] array3 = new string[4];
                array3[0] = "play";
                array3[1] = "pause";
                array3[2] = "arrow_r";
                array2 = array3;
                array3[3] = "close";
            }

            string[] array4 = array2;
            Ds.addButtonMulti(new DsnDataButtonMulti
            {
                unselectable = 2,
                skin = "mini",
                name = "padbtn",
                titles = array4,
                w = 30f,
                h = 30f,
                margin_w = 4f,
                margin_h = 0f,
                clms = array4.Length,
                fnClick = (FnClick ?? EventLineDebugger.fnClickPadBtnBasic)
            }).Get(EventLineDebugger.is_pause ? 1 : 0).SetChecked(true);
            Ds.Br().addButton(new DsnDataButton
            {
                unselectable = 2,
                w = X.Mx(130f, Ds.use_w),
                h = 30f,
                title = "&&Debug_inject_if",
                name = "Debug_inject_if",
                skin = "checkbox_string",
                def = EventLineDebugger.use_if_inject,
                fnClick = (FnClick ?? EventLineDebugger.fnClickPadBtnBasic)
            });
            Ds.Br().addButton(new DsnDataButton
            {
                unselectable = 2,
                w = X.Mx(130f, Ds.use_w),
                h = 30f,
                title = "&&Debug_use_breakpoint",
                name = "Debug_use_breakpoint",
                skin = "checkbox_string",
                def = EventLineDebugger.use_breakpoint,
                fnClick = (FnClick ?? EventLineDebugger.fnClickPadBtnBasic)
            });
            Ds.Br().addSlider(new DsnDataSlider
            {
                unselectable = 2,
                name = "max_delay",
                title = "Speed",
                def = 60 - EventLineDebugger.max_delay,
                mn = 0f,
                mx = 60f,
                valintv = 5f,
                w = 90f,
                h = 30f,
                fnChanged = EventLineDebugger.fnChangedDelayMeter
            });
            Ds.addP(new DsnDataP
            {
                text = "[V]变量显示\n[F]GF显示",
                TxCol = MTRX.ColWhite,
                swidth = Ds.use_w - 4f,
                sheight = 30f,
                size = 14f,
                text_auto_wrap = true,
                alignx = ALIGN.CENTER
            });
            return false;
        }

        [HarmonyPatch(typeof(ReelExecuter), "decideRotate")]
        [HarmonyPrefix]
        public static bool ReelExecuter_decideRotate_Prefix(ReelExecuter __instance, ref bool __result, bool randomise = false)
        {
            if (__instance.Acontent == null)
                __instance.initReelContent();
            if (__instance.content_id_dec >= 0){
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
            __instance.Ui.getEffect().PtcSTsetVar("w", __instance.reel_width_px).PtcSTsetVar("h", __instance.reel_height_px * 2.0).PtcST("reel_decided", __instance);
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