using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using nel;
using XX;
using AliceInCradle.Patch;
using static AliceInCradle.Loader;

namespace AliceInCradle.Mono
{
    public class ModUI : MonoBehaviour
    {
        // ReSharper disable once InconsistentNaming
        public static AssetBundle UIBundle;
        public static GameObject UI;
        private GameObject _ui;
        private Text _text1;
        private Text _text2;
        private Text _text3;
        private Toggle _toggle1;
        private Toggle _toggle2;
        private Toggle _toggle3;

        private void Awake()
        {
            if (!_ui)
            {
                _ui = UIBundle.LoadAsset<GameObject>("ModUI");
            }

            // 设置是否显示
            _ui.SetActive(ConfigManage.Active.Value);
            // 实例化
            UI = Instantiate(_ui);
            // UI功能绑定
            BindUI(UI);
        }

        public void Start()
        {
            _text1 = UI.transform.Find("BG/Text1").GetComponent<Text>();
            _text2 = UI.transform.Find("BG/Text2").GetComponent<Text>();
            _text3 = UI.transform.Find("BG/Text3").GetComponent<Text>();
            _toggle1 = UI.transform.Find("BG/Toggle1").GetComponent<Toggle>();
            _toggle2 = UI.transform.Find("BG/Toggle2").GetComponent<Toggle>();
            _toggle3 = UI.transform.Find("BG/Toggle3").GetComponent<Toggle>();
        }

        private void FixedUpdate()
        {
            UpdateToggle();
            if (Noel is null) return;
            _text1.text = $"血量 {Noel.hp}/{Noel.maxhp}";
            _text2.text = $"魔力 {Noel.mp}/{Noel.maxmp}";
            _text3.text = $"HP: {ConfigManage.HpMultiply.Value} MP: {ConfigManage.MpMultiply.Value}";
        }

        private void UpdateToggle()
        {
            _toggle1.isOn = ConfigManage.NoHpDamage.Value;
            _toggle2.isOn = ConfigManage.NoMpDamage.Value;
            _toggle3.isOn = ConfigManage.EnhancedItemList.Value;
        }

        private static void BindUI(GameObject ui)
        {
            // var bg = ui.transform.Find("BG").GetComponent<Image>();
            // var title = ui.transform.Find("BG/Title").GetComponent<Text>();
            var btn1 = ui.transform.Find("BG/Btn1").GetComponent<Button>();
            var btn2 = ui.transform.Find("BG/Btn2").GetComponent<Button>();
            var btn3 = ui.transform.Find("BG/Btn3").GetComponent<Button>();
            var btn4 = ui.transform.Find("BG/Btn4").GetComponent<Button>();
            var btn5 = ui.transform.Find("BG/Btn5").GetComponent<Button>();
            var btn6 = ui.transform.Find("BG/Btn6").GetComponent<Button>();
            var toggle1 = ui.transform.Find("BG/Toggle1").GetComponent<Toggle>();
            var toggle2 = ui.transform.Find("BG/Toggle2").GetComponent<Toggle>();
            var toggle3 = ui.transform.Find("BG/Toggle3").GetComponent<Toggle>();
            var slider1 = ui.transform.Find("BG/Slider1").GetComponent<Slider>();
            var slider2 = ui.transform.Find("BG/Slider2").GetComponent<Slider>();

            btn1.transform.Find("Text").GetComponent<Text>().text = "输出金币";
            btn1.onClick.AddListener(() =>
            {
                EVENT.LogInfo(
                    $"GOLD: {CoinStorage.Acount[0],-9}" +
                    $"CRAFTS: {CoinStorage.Acount[1],-9}" +
                    $"JUICE: {CoinStorage.Acount[2],-9}");
            });
            btn2.transform.Find("Text").GetComponent<Text>().text = "输出背包物品";
            btn2.onClick.AddListener(() =>
            {
                if (StInventory is null) return;
                StInventory.row_max = 99;
                foreach (var row in StInventory.ARow)
                {
                    EVENT.LogInfo(
                        $"{PadRightEx(row.Data.getLocalizedName(0), 20)}" +
                        $"序号: {row.Info.newer,-4} " +
                        $"数量: {row.Info.total,-4} " +
                        $"堆叠: {StInventory.getItemStockable(row.Data),-4} " +
                        $"({TX.join(",", row.Info.Agrade)})");
                }
            });
            btn3.transform.Find("Text").GetComponent<Text>().text = "无限存储";
            btn3.onClick.AddListener(() =>
            {
                if (StInventory is null) return;
                StInventory.infinit_stockable = true;
            });
            btn4.transform.Find("Text").GetComponent<Text>().text = "输出物品表缓存";
            btn4.onClick.AddListener(() =>
            {
                foreach (var key in Inject.Reels.Keys)
                {
                    EVENT.LogInfo($"物品表: {key}");
                }
            });
            btn5.transform.Find("Text").GetComponent<Text>().text = "随机物品表";
            btn5.onClick.AddListener(() =>
            {
                foreach (var entity in Inject.Reels)
                {
                    var orig = entity.Value[0].AContent;
                    var rand = entity.Value[1].AContent;
                    for (var i = 0; i < orig.Count; i++)
                    {
                        rand[i].count = Random.Range(X.Mx(orig[i].count - 1, 0) * 100, X.Mx(orig[i].count, 1) * 100);
                    }
                }
            });
            btn6.transform.Find("Text").GetComponent<Text>().text = "转轮排序";
            btn6.onClick.AddListener(() => { ReelM.AReel.Sort(CompareByName); });

            toggle1.GetComponent<RectTransform>().sizeDelta += new Vector2(36, 0);
            toggle1.GetComponent<RectTransform>().localPosition += new Vector3(18, 0, 0);
            toggle1.transform.Find("Background").GetComponent<RectTransform>().sizeDelta += new Vector2(36, 0);
            toggle1.transform.Find("Label").GetComponent<Text>().text = "取消HP损失";
            toggle1.onValueChanged.AddListener(value => { ConfigManage.NoHpDamage.Value = value; });
            toggle2.GetComponent<RectTransform>().sizeDelta += new Vector2(36, 0);
            toggle2.GetComponent<RectTransform>().localPosition += new Vector3(18, 0, 0);
            toggle2.transform.Find("Background").GetComponent<RectTransform>().sizeDelta += new Vector2(36, 0);
            toggle2.transform.Find("Label").GetComponent<Text>().text = "取消MP损失";
            toggle2.onValueChanged.AddListener(value => { ConfigManage.NoMpDamage.Value = value; });
            toggle3.GetComponent<RectTransform>().sizeDelta += new Vector2(36, 0);
            toggle3.GetComponent<RectTransform>().localPosition += new Vector3(18, 0, 0);
            toggle3.transform.Find("Background").GetComponent<RectTransform>().sizeDelta += new Vector2(36, 0);
            toggle3.transform.Find("Label").GetComponent<Text>().text = "替换宝箱列表";
            toggle3.onValueChanged.AddListener(value => { ConfigManage.EnhancedItemList.Value = value; });

            slider1.onValueChanged.AddListener(value => { ConfigManage.HpMultiply.Value = X.IntC(value); });
            slider2.onValueChanged.AddListener(value => { ConfigManage.MpMultiply.Value = X.IntC(value); });

            toggle1.isOn = ConfigManage.NoHpDamage.Value;
            toggle2.isOn = ConfigManage.NoMpDamage.Value;
            toggle3.isOn = ConfigManage.EnhancedItemList.Value;
            slider1.value = ConfigManage.HpMultiply.Value;
            slider2.value = ConfigManage.MpMultiply.Value;

            ui.AddComponent<ModUIDrag>();
        }

        private static int CompareByName(ReelExecuter executor1, ReelExecuter executor2)
        {
            // 先 加法 再 乘法 后 随机
            return string.CompareOrdinal(executor1.etype.ToString(), executor2.etype.ToString());
        }

        private static string PadRightEx(string str, int totalByteCount)
        {
            // 去除 不可见的控制字符和未使用的代码点 例如 (\u0008 BS) WTF 为什么会有这个!
            var s = Regex.Replace(str, @"[\p{C}]", "");
            var coding = Encoding.GetEncoding("UTF-8");
            var count = s.ToCharArray().Count(ch => coding.GetByteCount(ch.ToString()) > 1);
            return count > totalByteCount ? s.PadRight(count + 2) : s.PadRight(totalByteCount - count);
        }
    }
}