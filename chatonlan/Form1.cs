using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace chatonlan
{
    public partial class Form1 : Form
    {
        Socket socket;
        EndPoint epLocal, epRemote;
        byte[] buffer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            textLIP.Text = getLocalIP();
            textRIP.Text = getLocalIP();
        }


        private string getLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            epLocal = new IPEndPoint(IPAddress.Parse(textLIP.Text), Convert.ToInt32(textLPort.Text));
            //maybe change here
            socket.Bind(epLocal);

            epRemote = new IPEndPoint(IPAddress.Parse(textRIP.Text), Convert.ToInt32(textRPort.Text));
            socket.Connect(epRemote);

            buffer = new byte[1500];
            socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            byte[] sendingMessage = new byte[1500];

            sendingMessage = unicodeEncoding.GetBytes(textMess.Text);

            socket.Send(sendingMessage);
            listMess.Items.Add("Me: " + textMess.Text);
            textMess.Text = "";

        }


        private void MessageCallBack(IAsyncResult asyncResult)
        {
            try
            {
                byte[] receiveData = new byte[1500];
                receiveData = (byte[])asyncResult.AsyncState;

                UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
                string receiveMessage = unicodeEncoding.GetString(receiveData);

                listMess.Items.Add("friend: " + receiveMessage);
                buffer = new byte[1500];
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
