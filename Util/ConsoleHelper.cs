using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

public static class ConsoleHelper
{
    //表示固定宽度 TrueType 字体的类型
    private const int FixedWidthTrueType = 54;

    //这是标准输出句柄的标识符。
    private const int StandardOutputHandle = -11;

    //声明了一个外部方法，用于获取标准输入、输出或错误设备的句柄。它使用 kernel32.dll 库。
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern IntPtr GetStdHandle(int nStdHandle);

    //声明了一个外部方法，用于设置当前控制台的字体，返回值为布尔型，表示是否成功。
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

    //声明了一个外部方法，用于获取当前控制台的字体，返回值为布尔型。
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);
    
    // 通过调用 GetStdHandle 方法获取当前控制台的输出句柄。
    private static readonly IntPtr ConsoleOutputHandle = GetStdHandle(StandardOutputHandle);

    // FontInfo 结构体用于封装字体信息，包括字体大小、名称、族和粗细。
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FontInfo
    {
        internal int cbSize;        // 结构体的大小（以字节为单位）
        internal int nFont;         // 系统控制台字体表中字体的索引
        internal short FontWidth;   // 字体宽度
        public short FontSize;      // 字体大小
        public int FontFamily;      // 字体间距和系列（如固定宽度 TrueType）
        public int FontWeight;      // 字体粗细。 粗细范围为 100 到 1000，按 100 的倍数表示。 例如，正常粗细为 400，而 700 为粗体。

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.wc, SizeConst = 32)]
        public string FontName;     // 字体的名称（如 Courier 或 Arial）
    }

    public static FontInfo[] SetCurrentFont(string font, short fontSize = 0)
    {
        // Console.WriteLine("Set Current Font: " + font);

        FontInfo before = new FontInfo
        {
            cbSize = Marshal.SizeOf<FontInfo>()
        };

        if (GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref before))
        {
            FontInfo set = new FontInfo
            {
                cbSize = Marshal.SizeOf<FontInfo>(),
                nFont = 0,
                FontSize = fontSize > 0 ? fontSize : before.FontSize,
                FontFamily = FixedWidthTrueType,
                FontWeight = 300,
                FontName = font,
            };

            // Get some settings from current font.
            if (!SetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref set))
            {
                var ex = Marshal.GetLastWin32Error();
                Console.WriteLine("Set error " + ex);
                throw new System.ComponentModel.Win32Exception(ex);
            }

            FontInfo after = new FontInfo
            {
                cbSize = Marshal.SizeOf<FontInfo>()
            };
            GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref after);

            return new[] { before, set, after };
        }
        var er = Marshal.GetLastWin32Error();
        Console.WriteLine("Get error " + er);
        throw new System.ComponentModel.Win32Exception(er);
    }
}