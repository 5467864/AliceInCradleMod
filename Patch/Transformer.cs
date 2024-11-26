using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using nel;

namespace AliceInCradle.Patch
{
    internal class Transformer
    {
        [HarmonyPatch(typeof(ReelExecuter), "applyEffectToIK")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ReelExecuter_applyEffectToIK_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // 修改使用宝箱效果转轮的最高获取数量
            return new CodeMatcher(instructions)
                .MatchForward(false, 
                    new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_S && i.operand.ToString() == "99")
                )
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldc_I4,999))
                .InstructionEnumeration();
        }
    }
}