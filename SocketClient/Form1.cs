using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Socket socketSend;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //创建负责通信的socket
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(textBox1.Text);
                IPEndPoint point = new IPEndPoint(ip, int.Parse(textBox2.Text));
                socketSend.Connect(point);
                ShowMsg("连接成功");
                //开启一个线程，不停的接受服务端发来的消息
                Thread th = new Thread(Receive);
                th.IsBackground = true;
                th.Start();
            }
            catch 
            {
                
            }
            
        }
        void ShowMsg(string str) {
            this.textBox3.AppendText(str + "\r\n");
        }
        /// <summary>
        /// 不停的接受服务器发来的消息
        /// </summary>
        void Receive() {
            while (true) {
                try
                {
                    byte[] buffer = new byte[1024 * 1024 * 2];
                    int r = socketSend.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }
                    if (buffer[0] == 0)//文字消息
                    {
                        string str = Encoding.UTF8.GetString(buffer, 1, r - 1);
                        ShowMsg(socketSend.RemoteEndPoint + "：" + str);
                    }
                    else if (buffer[0] == 1) {//表示接受文件
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.InitialDirectory = @"C:\Users\ZP\Desktop";
                        sfd.Title = "请选择要保存的文件";
                        sfd.Filter = "所有文件|*.*";
                        sfd.ShowDialog(this);//加this 确保弹窗弹出
                        string path = sfd.FileName;
                        using (FileStream fsWrite=new FileStream(path,FileMode.OpenOrCreate,FileAccess.Write)) {
                            fsWrite.Write(buffer,1,r-1);
                        }
                        MessageBox.Show("保存成功");
                    }
                    else if (buffer[0] == 2)
                    {
                        ZD();
                    }
                }
                catch 
                {
                    
                }
            }
            
        }
        /// <summary>
        /// 震动
        /// </summary>
        void ZD() {
            for (int i = 0; i < 500; i++)
            {
                this.Location = new Point(200, 200);
                this.Location = new Point(280, 280);
            }
        }
        /// <summary>
        /// 客户端给服务器发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string str = textBox4.Text.Trim();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            socketSend.Send(buffer);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }
    }
}
