using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace Hi_Fresh
{
    // 이미지 정보 형식 셋팅
    class statusList
    {
        public Bitmap imgName;
        public int returnCount;
        public string Des;
    }

    class Library
    {
        // 이미지 비교
        public static int imgComp(string a, statusList b)
        {
            using (Bitmap A = new Bitmap(a))
            {
                using (Bitmap B = new Bitmap(b.imgName))
                {
                    for (int i = 0; i < A.Width; i++)
                    {
                        for (int j = 0; j < B.Height; j++)
                        {
                            if (A.GetPixel(i, j).ToString() != B.GetPixel(i, j).ToString())
                            {
                                return 0;
                            }
                        }
                    }
                }
            }
            return 1;
        }

        // 이미지 리스트
        public static statusList[] imgList = new statusList[]
        {
            new statusList { imgName = Resource1.nc_image1, returnCount = 1, Des = "시작" },
        };
    }

    public partial class Form1 : Form
    {
        // 핸들을 잡기위한 Dll Import
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);
        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);
        [DllImport("user32")]
        public static extern int SetWindowPos(int hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        // PostMessage 를 위한 Message Value
        public enum WMessages : int
        {
            WM_LBUTTONDOWN = 0x201, //Left mousebutton down
            WM_LBUTTONUP = 0x202,  //Left mousebutton up
            WM_LBUTTONDBLCLK = 0x203, //Left mousebutton doubleclick
            WM_RBUTTONDOWN = 0x204, //Right mousebutton down
            WM_RBUTTONUP = 0x205,   //Right mousebutton up
            WM_RBUTTONDBLCLK = 0x206, //Right mousebutton doubleclick
            WM_KEYDOWN = 0x100,  //Key down
            WM_KEYUP = 0x101,   //Key up
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105,
            WM_CHAR = 0x102,
            WM_COMMAND = 0x111
        }

        // 핸들 잡기 함수
        public static IntPtr NoxFind(string window)
        {
            IntPtr hw1 = FindWindow("Qt5QWindowIcon", window);
            IntPtr hw2 = FindWindowEx(hw1, IntPtr.Zero, null, "ScreenBoardClassWindow");
            IntPtr hw3 = FindWindowEx(hw2, IntPtr.Zero, null, "sub");
            return hw3;
        }

        public static string Image_file_tmp = @"C:\Capt_tmp.bmp";
        public static string Image_file = @"C:\Capt.bmp";

        // 캡쳐 함수
        public void Capt(int x, int y, string name1)
        {
            try
            {
                Clipboard.Clear();
                var hwW = FindWindow(null, name1);                
                SetWindowPos((int)hwW, 0, 0, 0, 720, 490, 0x10);                

                Rectangle rect = Screen.PrimaryScreen.Bounds;

                Bitmap bmp = new Bitmap(30, 30, PixelFormat.Format32bppArgb);
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.CopyFromScreen(x, y, 0, 0, rect.Size);
                }
                bmp.Save(Image_file_tmp);
                bmp.Dispose();
                bmp = null;

                Bitmap bmp4 = new Bitmap(Image_file_tmp);
                var bitmap4 = bmp4.Clone(new Rectangle(0, 0, bmp4.Width, bmp4.Height), PixelFormat.Format4bppIndexed);
                bitmap4.Save(Image_file);
                bitmap4.Dispose();
                bitmap4 = null;
                bmp4.Dispose();
                bmp4 = null;

                textBox1.Text += "캡쳐 완료. \r\n";
            }
            catch (Exception)
            {
                textBox1.Text += "[Error]캡쳐 \r\n";
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        // 핸들 잡기
        private void button1_Click(object sender, EventArgs e)
        {   
            if( (IntPtr)NoxFind("녹스 플레이어") == (IntPtr)0)
            {
                textBox1.Text += "핸들을 찾을 수 없습니다. \r\n";
            }
            else
            {
                textBox1.Text += NoxFind("녹스 플레이어") + "\r\n";
            }
            
        }

        // 캡쳐 하기
        private void button2_Click(object sender, EventArgs e)
        {
            Capt(460, 170, "녹스 플레이어");

            foreach(var item in Library.imgList)
            {
                if(Library.imgComp(Image_file, item) == 1)
                {
                    textBox1.Text += "이미지가 일치합니다 클릭 GO GO!!\r\n";
                }
                else
                {
                    textBox1.Text += "이미지 불일치\r\n";
                }
            }
        }

        // 이미지 삭제
        private void button3_Click(object sender, EventArgs e)
        {
            System.IO.File.Delete(Image_file);
            System.IO.File.Delete(Image_file_tmp);
            textBox1.Text += "캡쳐한 이미지가 삭제 되었습니다\r\n";
        }
    }
}