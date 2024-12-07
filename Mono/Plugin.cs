using System.Linq;
using BepInEx.Logging;
using UnityEngine;
using nel;
using XX;
using static AliceInCradle.ConfigManage;

namespace AliceInCradle.Mono
{
    public class Plugin : MonoBehaviour
    {
        private static readonly ManualLogSource Event = Loader.EVENT;

        private void Awake()
        {
            Loader.Noel = GetNoel();
            gameObject.AddComponent<ModUI>();
        }

        private void Update()
        {
            if (IN.getKD(IMGUI.Value))
            {
                Loader.WindowDisplay = !Loader.WindowDisplay;
            }

            if (IN.getKD(UGUI.Value))
            {
                Active.Value = !ModUI.UI.activeSelf;
                ModUI.UI.SetActive(Active.Value);
            }
        }

        private void OnGUI()
        {
            if (Loader.WindowDisplay)
            {
                Loader.WindowRect = GUI.Window(114514, Loader.WindowRect, DoMyWindow, PluginInfo.Name);
            }
        }

        private void OnDestroy()
        {
            Loader.WindowDisplay = false;
            Event.LogMessage("当前场景生命周期结束");
        }

        private void OnApplicationQuit()
        {
            ConfigManage.Save();
            Event.LogWarning("退出游戏!");
        }

        private void DoMyWindow(int windowID)
        {
            GUILayout.BeginArea(new Rect(10f, 20f, 480f, 250f));

            NoMosaic.Value = GUILayout.Toggle(NoMosaic.Value, "无马赛克", GUILayout.Height(24f));
            NoHpDamage.Value = GUILayout.Toggle(NoHpDamage.Value, "取消HP损失", GUILayout.Height(24f));
            NoMpDamage.Value = GUILayout.Toggle(NoMpDamage.Value, "取消MP损失", GUILayout.Height(24f));
            if (GUILayout.Button("输出金币", GUILayout.Height(24f)))
            {
                Event.LogInfo(
                    $"GOLD: {CoinStorage.Acount[0],-6}" +
                    $"CRAFTS: {CoinStorage.Acount[1],-6}" +
                    $"JUICE: {CoinStorage.Acount[2],-6}");
            }

            GUILayout.EndArea();
            GUI.DragWindow();
        }

        private static PRNoel GetNoel()
        {
            return FindObjectsOfType<PRNoel>().FirstOrDefault();
        }
    }
}