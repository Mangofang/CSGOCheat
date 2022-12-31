using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace CSGOCheat3._0
{
    class Program
    {
        struct Player
        {
            public static int[] Address = new int[20];
            public static float[] LocatX = new float[20];
            public static float[] LocatY = new float[20];
            public static float[] LocatZ = new float[20];
            public static float[] Head = new float[20];
            public static int[] Heal = new int[20];
            public static int[] Camp = new int[20];
        }
        public static float clipCoords_x;
        public static float clipCoords_y;
        public static float clipCoords_z;
        public static float clipCoords_w;
        public static float clipCoords_x_head;
        public static float clipCoords_y_head;
        public static float clipCoords_z_head;
        public static float clipCoords_w_head;
        public static float Rectangle_wight;
        public static float Rectangle_hight;
        public static float NDC_x;
        public static float NDC_y;
        public static float NDC_x_head;
        public static float NDC_y_head;
        public static float[] Matrix = new float[16];
        public static float[] screen_x = new float[20];
        public static float[] screen_y = new float[20];
        public static float[] screen_x_head = new float[20];
        public static float[] screen_y_head = new float[20];
        public static string gamename = "csgo";
        public static int windows_x = 1920;
        public static int windows_y = 1080;
        public static IntPtr hAddress = OpenProcess(0x1F0FFF, false, GetPidByProcessName(gamename));
        public static IntPtr csgo_handle = Process.GetProcessById(GetPidByProcessName(gamename)).MainWindowHandle;
        static void Main(string[] args)
        {
            int Player_Num;
            int dev = 0x0;
            GetWindowsXandY();
            IntPtr MainAddress = (IntPtr)(GetDllAddress("client.dll") + 0x4DFFF14);
            IntPtr MatrixAddress = new IntPtr(GetDllAddress("client.dll") + 0x4DF0D6C - 0x28);

            for (Player_Num = 0; Player_Num <= 20; Player_Num++)
            {
                int tem_Address = 0;
                ReadProcessMemory((int)hAddress, (IntPtr)MainAddress + dev, out tem_Address, 4, 0);
                Player.Address[Player_Num] = tem_Address;
                if (tem_Address == 0)
                {
                    break;
                }
                dev = dev + 0x10;
            }

            ThreadPool.QueueUserWorkItem(item =>
            {
                Pen pen_DR = new Pen(Color.FromArgb(0x66ff0000), 2);
                Pen pen_DY = new Pen(Color.Green, 2);

                Graphics e = Graphics.FromHwnd(csgo_handle);
                e.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                e.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                byte x = 0;
                byte y = 0;
                byte z = 0;
                byte[] x_ = new byte[4];
                byte[] y_ = new byte[4];
                byte[] z_ = new byte[4];
                while (true)
                {
                    for (int i = 0; i <= Player_Num - 1; i++)
                    {
                        for (int a = 0; a < 4; a++)
                        {
                            ReadProcessMemory((int)hAddress, (IntPtr)Player.Address[i] + 0xA0 + a, out x, 4, 0);
                            x_[a] = x;
                            ReadProcessMemory((int)hAddress, (IntPtr)Player.Address[i] + 0xA4 + a, out y, 4, 0);
                            y_[a] = y;
                            ReadProcessMemory((int)hAddress, (IntPtr)Player.Address[i] + 0xA8 + a, out z, 4, 0);
                            z_[a] = z;
                        }
                        ReadProcessMemory((int)hAddress, (IntPtr)Player.Address[i] + 0xF4, out Player.Camp[i], 4, 0);
                        ReadProcessMemory((int)hAddress, (IntPtr)Player.Address[i] + 0x100, out Player.Heal[i], 4, 0);
                        Player.Head[i] = Player.LocatZ[i] + 70f;
                        Player.LocatX[i] = ToFloat(x_);
                        Player.LocatY[i] = ToFloat(y_);
                        Player.LocatZ[i] = ToFloat(z_);

                        Matrix[0] = MatrixToFloat(MatrixAddress);
                        Matrix[1] = MatrixToFloat(MatrixAddress + 0x4);
                        Matrix[2] = MatrixToFloat(MatrixAddress + 0x8);
                        Matrix[3] = MatrixToFloat(MatrixAddress + 0x8 + 0x4);
                        Matrix[4] = MatrixToFloat(MatrixAddress + 0x10);
                        Matrix[5] = MatrixToFloat(MatrixAddress + 0x14);
                        Matrix[6] = MatrixToFloat(MatrixAddress + 0x18);
                        Matrix[7] = MatrixToFloat(MatrixAddress + 0x18 + 0x4);
                        Matrix[8] = MatrixToFloat(MatrixAddress + 0x20);
                        Matrix[9] = MatrixToFloat(MatrixAddress + 0x24);
                        Matrix[10] = MatrixToFloat(MatrixAddress + 0x28);
                        Matrix[11] = MatrixToFloat(MatrixAddress + 0x28 + 0x4);
                        Matrix[12] = MatrixToFloat(MatrixAddress + 0x30);
                        Matrix[13] = MatrixToFloat(MatrixAddress + 0x34);
                        Matrix[14] = MatrixToFloat(MatrixAddress + 0x38);
                        Matrix[15] = MatrixToFloat(MatrixAddress + 0x38 + 0x4);
                        GetclipCoord(Player.LocatX[i], Player.LocatY[i], Player.LocatZ[i], Player.Head[i], i);

                        Rectangle_hight = (screen_y_head[i] - screen_y[i]) * -1;
                        Rectangle_wight = 0.5f * Rectangle_hight;

                        if (Player.Address[i] != Player.Address[0] && Player.Heal[i] != 0)
                        {
                            if (Player.Camp[i] != Player.Camp[0])
                            {
                                e.DrawLine(pen_DR, windows_x / 2, windows_y, screen_x[i], screen_y[i]);
                                e.DrawRectangle(pen_DR, screen_x[i] - Rectangle_wight / 2, screen_y[i] - Rectangle_hight, Rectangle_wight, Rectangle_hight);
                            }
                            else
                            {
                                e.DrawLine(pen_DY, windows_x / 2, windows_y, screen_x[i], screen_y[i]);
                                e.DrawRectangle(pen_DY, screen_x[i] - Rectangle_wight / 2, screen_y[i] - Rectangle_hight, Rectangle_wight, Rectangle_hight);
                            }
                        }


                    }
                }
            });

            //Console.WriteLine(Player_Num);
            Console.ReadKey();
        }
        public static float MatrixToFloat(IntPtr MatrixAddress)
        {
            byte data;
            byte[] C = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                ReadProcessMemory((int)hAddress, MatrixAddress + i, out data, 4, 0);
                C[i] = data;
            }
            return ToFloat(C);
        }
        public static bool GetclipCoord(float pos_x, float pos_y, float pos_z, float head, int i)
        {
            clipCoords_x = pos_x * Matrix[0] + pos_y * Matrix[1] + pos_z * Matrix[2] + Matrix[3];
            clipCoords_y = pos_x * Matrix[4] + pos_y * Matrix[5] + pos_z * Matrix[6] + Matrix[7];
            clipCoords_z = pos_x * Matrix[8] + pos_y * Matrix[9] + pos_z * Matrix[10] + Matrix[11];
            clipCoords_w = pos_x * Matrix[12] + pos_y * Matrix[13] + pos_z * Matrix[14] + Matrix[15];

            clipCoords_x_head = pos_x * Matrix[0] + pos_y * Matrix[1] + head * Matrix[2] + Matrix[3];
            clipCoords_y_head = pos_x * Matrix[4] + pos_y * Matrix[5] + head * Matrix[6] + Matrix[7];
            clipCoords_z_head = pos_x * Matrix[8] + pos_y * Matrix[9] + head * Matrix[10] + Matrix[11];
            clipCoords_w_head = pos_x * Matrix[12] + pos_y * Matrix[13] + head * Matrix[14] + Matrix[15];
            if (clipCoords_w < 0.1f || clipCoords_w_head < 0.1f)
            {
                return false;
            }
            NDC_x = clipCoords_x / clipCoords_w;
            NDC_y = clipCoords_y / clipCoords_w;

            NDC_x_head = clipCoords_x_head / clipCoords_w_head;
            NDC_y_head = clipCoords_y_head / clipCoords_w_head;

            screen_x[i] = (windows_x / 2 * NDC_x) + (NDC_x + windows_x / 2);
            screen_y[i] = -(windows_y / 2 * NDC_y) + (NDC_y + windows_y / 2);
            screen_x_head[i] = (windows_x / 2 * NDC_x_head) + (NDC_x_head + windows_x / 2);
            screen_y_head[i] = -(windows_y / 2 * NDC_y_head) + (NDC_y_head + windows_y / 2);
            return true;
        }
        public static float ToFloat(byte[] data)
        {
            float a = 0;
            byte i;
            byte[] x = data;
            unsafe
            {
                void* pf;
                fixed (byte* px = x)
                {
                    pf = &a;
                    for (i = 0; i < data.Length; i++)
                    {
                        *((byte*)pf + i) = *(px + i);
                    }
                }
            }
            return a;
        }
        public static int GetDllAddress(string DllName)
        {
            int address = 0;
            foreach (Process p in Process.GetProcessesByName(gamename))
            {
                foreach (ProcessModule m in p.Modules)
                {
                    if (m.ModuleName == DllName)
                    {
                        address = m.BaseAddress.ToInt32();
                    }
                }
            }
            return address;
        }
        public static int GetPidByProcessName(string processName)
        {
            Process[] arrayProcess = Process.GetProcessesByName(processName);

            foreach (Process p in arrayProcess)
            {
                return p.Id;
            }
            return 0;
        }

        [DllImportAttribute("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        static extern bool ReadProcessMemory(int hProcess, IntPtr lpBaseAddress, out int lpBuffer, int nSize, int lpNumberOfBytesRead);
        [DllImportAttribute("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        static extern bool ReadProcessMemory(int hProcess, IntPtr lpBaseAddress, out byte lpBuffer, int nSize, int lpNumberOfBytesRead);
        [DllImportAttribute("kernel32.dll", EntryPoint = "OpenProcess")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;//最左坐标
            public int Top;//最上坐标
            public int Right;//最右坐标
            public int Bottom;//最下坐标
        }
        public static void GetWindowsXandY()
        {
            RECT rect = new RECT();
            GetWindowRect(csgo_handle, ref rect);
            windows_x = rect.Right - rect.Left;
            windows_y = rect.Bottom - rect.Top;
        }
    }
}
