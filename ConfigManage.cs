using AliceInCradle.Mono;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AliceInCradle
{
    public class ConfigManage
    {
        public static ConfigEntry<bool> NoMosaic;
        public static ConfigEntry<bool> NoHpDamage;
        public static ConfigEntry<bool> NoMpDamage;
        public static ConfigEntry<bool> EnhancedItemList;

        public static ConfigEntry<Key> UGUI;
        public static ConfigEntry<Key> IMGUI;
        public static ConfigEntry<Key> Debug;
        public static ConfigEntry<KeyCode> BestReel;

        public static ConfigEntry<bool> Active;
        public static ConfigEntry<Vector3> Pos;

        public static ConfigEntry<int> HpMultiply;
        public static ConfigEntry<int> MpMultiply;

        public static ConfigEntry<int> MaxCount;

        public ConfigManage(ConfigFile config)
        {
            Initialize(config);
        }

        private static void Initialize(ConfigFile config)
        {
            UGUI = config.Bind("按键绑定",
                "UGUI",
                Key.F6,
                "切换UGUI界面显示");
            IMGUI = config.Bind("按键绑定",
                "IMGUI",
                Key.F2,
                "切换IMGUI界面显示");
            Debug = config.Bind("按键绑定",
                "Debug",
                Key.F3,
                "重定义F7调试工具界面显示 (需要启用调试工具)");
            BestReel = config.Bind("按键绑定",
                "BestReel",
                KeyCode.RightShift,
                "最优转轮");

            NoMosaic = config.Bind("功能",
                "NoMosaic",
                true,
                "去除马赛克 (无法去除图片上的)");
            NoHpDamage = config.Bind("功能",
                "NoHpDamage",
                true,
                "不减HP");
            NoMpDamage = config.Bind("功能",
                "NoMpDamage",
                true,
                "不减MP");
            EnhancedItemList = config.Bind("功能",
                "EnhancedItemList",
                true,
                "增强宝箱物品表");
            HpMultiply = config.Bind("功能",
                "HpMultiply",
                1,
                "HP伤害倍增");
            MpMultiply = config.Bind("功能",
                "MpMultiply",
                1,
                "MP伤害倍增");
            MaxCount = config.Bind("功能",
                "MaxCount",
                999,
                "转轮获取物品上限");

            Active = config.Bind("界面",
                "Active",
                false,
                "UGUI显示"
            );
            Pos = config.Bind("界面",
                "Pos",
                default(Vector3),
                "UGUI坐标"
            );
        }

        public static void Save()
        {
            Active.Value = ModUI.UI.activeSelf;
            Pos.Value = ModUIDrag.Pos;
        }
    }
}