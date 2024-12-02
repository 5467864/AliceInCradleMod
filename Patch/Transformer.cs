using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using evt;
using HarmonyLib;
using nel;

namespace AliceInCradle.Patch
{
    internal class Transformer
    {
        [HarmonyPatch(typeof(ReelExecuter), "applyEffectToIK")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ReelExecuter_applyEffectToIK_Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            // 修改使用宝箱效果转轮的最高获取数量
            return new CodeMatcher(instructions)
                .MatchForward(false,
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_S && i.operand.ToString() == "99")
                )
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldc_I4, 999))
                .InstructionEnumeration();
        }

        [HarmonyPatch(typeof(EvDebugger), "showSubMain")]
        [HarmonyPatch(typeof(EvDebugger), "fnClickDebugChecks")]
        [HarmonyPatch(typeof(NelEvDebugListener), "tabLTInit")]
        [HarmonyPatch(typeof(NelEvDebugListener), "initCategoryDangerousness")]
        [HarmonyPatch(typeof(NelEvDebugListener), "fnChangeLock")]
        [HarmonyPatch(typeof(NelEvDebugListener), "initCategoryEnemy")]
        [HarmonyPatch(typeof(NelEvDebugListener), "initCategoryItem")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> EvDebugger_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Dictionary<string, string> trans = new()
            {
                { "mighty", "强大" },
                { "nodamage", "无伤害" },
                { "weak", "虚弱" },
                { "allskill", "所有技能" },
                
                { "Danger", "危险" },
                { "HP/MP", "血量/魔力" },
                { "Item", "物品" },
                { "Recipe", "配方" },

                { "Current Map", "当前地图" },
                { " └ D:Juice", " └ D:果汁" },
                { "Reel max", "最大宝箱卷轴" },
                { "Lock Dangerousness", "锁定危险性" },
                { "Allow fast travel in night time", "允许夜间快速旅行" },
                { "Lock weather", "锁定天气" },
                
                { "Fine Columns", "Fine Columns" },
                { "Auto refine variables ", "Auto refine variables " },
                { "KILL", "自杀" },
                { "HP", "血量" },
                { "MP", "魔力" },
                { "Pos", "坐标" },
                
                { "Grade", "等级" },
                { "Get!", "获得!" },
                { "Money", "金钱" }
            };
            var codes = instructions.ToList();
            foreach (var code in codes.Where(code =>
                         code.opcode == OpCodes.Ldstr && trans.ContainsKey(code.operand.ToString())))
            {
                code.operand = trans[code.operand.ToString()];
            }

            return codes.AsEnumerable();
        }
    }
}