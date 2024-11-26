using System.IO;
using System.Linq;
using System.Reflection;
using AliceInCradle.Mono;
using AliceInCradle.Patch;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;
using UnityEngine;
using nel;

// ReSharper disable InconsistentNaming

namespace AliceInCradle
{
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    public class Loader : BaseUnityPlugin
    {
        public static readonly ManualLogSource EVENT = new ("EVENT");
        private readonly ManualLogSource LOG = new ("AIC");

        public static ConfigManage ConfigManage;
        public static GameObject Plugin;
        public static ModUI UIObject;
        
        public static ItemStorage StInventory;
        public static ItemStorage StPrecious;
        public static ItemStorage StHouseInventory;
        public static ItemStorage StEnhancer;
        
        public static ReelManager ReelM;
        
        public static PRNoel Noel;
        
        public static bool WindowDisplay;
        public static Rect WindowRect;

        private Harmony _harmony;
        private void Awake()
        {
            BepInEx.Logging.Logger.Sources.Add(LOG);
            BepInEx.Logging.Logger.Sources.Add(EVENT);

            LOG.LogMessage("开始初始化！");
            
            HarmonyFileLog.Enabled = true;
            
            Plugin = new GameObject("PluginObject");
            ConfigManage = new ConfigManage(Config);
            
            // 修改控制台字体为黑体
            ConsoleHelper.SetCurrentFont("simhei", 16);

            if (_harmony == null)
            {
                _harmony = Harmony.CreateAndPatchAll(typeof(Inject));
                _harmony.PatchAll(typeof(Override));
                _harmony.PatchAll(typeof(Transformer));
            }
            if (_harmony.GetPatchedMethods().Any())
            {
                LOG.LogMessage("已修补游戏！");
            }
            
            // 通过反射加载嵌入dll的ModUI
            var assembly = Assembly.GetExecutingAssembly();
            ModUI.UIBundle = AssetBundle.LoadFromStream(assembly.GetManifestResourceStream("AliceInCradle.ModUI"));
            ReleaseFile(assembly);

            LOG.LogMessage("完成初始化！");
        }

        private void OnDestroy()
        {
            // 是的，Loader的生命周期就这点
            WindowRect = new Rect(300f, 300f, 500f, 300f);
            LOG.LogMessage("对象销毁!");
        }

        private static void ReleaseFile(Assembly assembly)
        {
            // Mono.ModUI.PadRightEx 的 Encoding.GetEncoding("UTF-8") 需要;
            if (File.Exists(Paths.PluginPath + "\\I18N.dll") &&
                File.Exists(Paths.PluginPath + "\\I18N.CJK.dll")) return;
            var stream = assembly.GetManifestResourceStream("AliceInCradle.api.I18N.dll");
            if (stream is not null)
            {
                var length = (int)stream.Length;
                var bs = new byte[length];
                var read = stream.Read(bs, 0, length);
                if (read != 0)
                {
                    File.WriteAllBytes(Paths.PluginPath+ "\\I18N.dll", bs);
                }
                stream.Close();
            }
            stream = assembly.GetManifestResourceStream("AliceInCradle.api.I18N.CJK.dll");
            if (stream is not null)
            {
                var length = (int)stream.Length;
                var bs = new byte[length];
                var read = stream.Read(bs, 0, length);
                if (read != 0)
                {
                    File.WriteAllBytes(Paths.PluginPath+ "\\I18N.CJK.dll", bs);
                }
                stream.Close();
            }
        }
    }

    public static class PluginInfo
    {
        public const string Guid = "AIC_Plugin";
        public const string Name = "AIC_Mod";
        public const string Version = "1.0.0";
    }
}