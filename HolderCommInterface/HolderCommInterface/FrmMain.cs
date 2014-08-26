using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Net;
using Common;

namespace HolderCommInterface
{
    public partial class frmMain : Form
    {

        public bool btnstatu = true;  //开始停止服务按钮状态
        public Thread myThread;       //声明一个线程实例
        public Socket newsock;        //声明一个Socket实例
        public Socket server1;
        public Socket Client;
        public IPEndPoint localEP;
        public int localPort;
        public EndPoint remote;
        public Hashtable _sessionTable;
        public bool m_Listening;
        //用来设置服务端监听的端口号
        public PlcSvr plc;
        public bool bConnFlag;

        public string errMsg;

        public delegate void MyInvoke(string str);

        public frmMain()
        {
            InitializeComponent();

            InitialObj();
        }

        public void InitialObj()
        {
            plc = new PlcSvr();

            bConnFlag = false;

            btnReadItem.Enabled = false;
            btnWriteItem.Enabled = false;

            Start_Service();
        }

        public int setPort
        {
            get { return localPort; }
            set { localPort = value; }
        }


        //用来往richtextbox框中显示消息
        public void showClientMsg(string msg)
        {
            //在线程里以安全方式调用控件
            if (showinfo.InvokeRequired)
            {
                MyInvoke _myinvoke = new MyInvoke(showClientMsg);
                showinfo.Invoke(_myinvoke, new object[] { msg });
            }
            else
            {
                showinfo.AppendText(msg);
            }
        }

        public void userListOperate(string msg)
        {
            //在线程里以安全方式调用控件
            if (userList.InvokeRequired)
            {
                MyInvoke _myinvoke = new MyInvoke(userListOperate);
                userList.Invoke(_myinvoke, new object[] { msg });
            }
            else
            {
                userList.Items.Add(msg);
            }
        }

        public void userListOperateR(string msg)
        {
            //在线程里以安全方式调用控件
            if (userList.InvokeRequired)
            {
                MyInvoke _myinvoke = new MyInvoke(userListOperateR);
                userList.Invoke(_myinvoke, new object[] { msg });
            }
            else
            {
                userList.Items.Remove(msg);
            }
        }       

        //监听函数
        public void Listen()
        {   //设置端口
            setPort  = int.Parse(txtSktPort.Text.Trim());
            //初始化SOCKET实例
            newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //允许SOCKET被绑定在已使用的地址上。
            newsock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //初始化终结点实例
            localEP = new IPEndPoint(IPAddress.Any, setPort);
            try
            {
                _sessionTable = new Hashtable(53);
                //绑定
                newsock.Bind(localEP);
                //监听
                newsock.Listen(10);
                //开始接受连接，异步。
                newsock.BeginAccept(new AsyncCallback(OnConnectRequest), newsock);
            }
            catch (Exception ex)
            {
               showClientMsg(ex.Message);
            }
        }

        //当有客户端连接时的处理
        public void OnConnectRequest(IAsyncResult ar)
        {
            //初始化一个SOCKET，用于其它客户端的连接
            server1 = (Socket)ar.AsyncState;
            Client = server1.EndAccept(ar);
            //将要发送给连接上来的客户端的提示字符串
            DateTimeOffset now = DateTimeOffset.Now;
      //      string strDateLine = "Welcome";

     //       Byte[] byteDateLine = System.Text.Encoding.UTF8.GetBytes(strDateLine);

            //将提示信息发送给客户端,并在服务端显示连接信息。
            remote = Client.RemoteEndPoint;

            showClientMsg(now.ToString("G") + Client.RemoteEndPoint.ToString() + "  Connected " + "\r\n");
     //       Client.Send(byteDateLine, byteDateLine.Length, 0);
            userListOperate(Client.RemoteEndPoint.ToString());

            //把连接成功的客户端的SOCKET实例放入哈希表
            _sessionTable.Add(Client.RemoteEndPoint, null);

            //等待新的客户端连接
            server1.BeginAccept(new AsyncCallback(OnConnectRequest), server1);

            byte[] RevBuffer = new byte[1024];

            while (true)
            {
                int recv = Client.Receive(RevBuffer);
                string stringdata = Encoding.UTF8.GetString(RevBuffer, 0, recv);
                string ip = Client.RemoteEndPoint.ToString();
                //获取客户端的IP和端口
                if (stringdata == "STOP")
                {
                    //当客户端终止连接时
                    showClientMsg(now.ToString("G") + "-->" + ip + "  Disconnected " + "\r\n");
                    _sessionTable.Remove(Client.RemoteEndPoint);
                    break;
                }

                //显示客户端发送过来的信息
                showClientMsg(now.ToString("G") + "--> " + ip + " " + stringdata + "\r\n");

                string SeqNo = stringdata.Substring(0, 6);
                string BcrNo = stringdata.Substring(6, 1);
                string Flag = stringdata.Substring(7, 1);

                bool Result = HandleDatafromErp(SeqNo, BcrNo, Flag);
         
                string Msg = string.Format("{0}{1}", SeqNo, Result ? "1" : "0");

                Byte[] sendData = Encoding.UTF8.GetBytes(Msg);

                Client.Send(sendData, sendData.Length, 0);
            }
        }

        /// <summary>
        /// 处理上位系统发来的信息，含是否生成条码数据成功，此过程负责处理把标志写给下位机
        /// </summary>
        /// <param name="Data"></param>
        public bool HandleDatafromErp(string psSeqNo,string psBcrNo,string psFlag)
        {
            bool Result = false;

            switch (psBcrNo)
            {
                case "1":
                    Result = SendData2Plc(psSeqNo, 0, Convert.ToInt16(psFlag));
                    break;
                case "2":
                    Result = SendData2Plc(psSeqNo, 1, Convert.ToInt16(psFlag));
                    break;
                default:
                    break;
            }

            return Result;
        }

        private string[] Addr = {"D0100","D0101"};

        public bool SendData2Plc(string psSeqNo, int piIdx, int value)
        {
            object[] data = { Addr[piIdx], value };
            if (plc.Write(data))
            {
                showClientMsg("Write 2 Plc Sucessed ");

                return true;
            }
            else
            {
                showClientMsg("Write 2 Plc Failed ");

                return false;
            }               
        }

        /// <summary>
        /// start & stop TcpServer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnListen_Click(object sender, EventArgs e)
        {
            //新建一个委托线程
            ThreadStart myThreadDelegate = new ThreadStart(Listen);
            //实例化新线程
            myThread = new Thread(myThreadDelegate);

            if (btnstatu)
            {
                myThread.Start();
                showClientMsg( "Barcode Service Started，waiting for Client...... "+"\r\n");
                btnstatu = false;
                btnListen.Text = "Stop Listen";
            }
            else
            {
                //停止服务（绑定的套接字没有关闭,因此客户端还是可以连接上来）
                myThread.Interrupt();
                myThread.Abort();

                showClientMsg("Barcode Service Stopped......"+"\r\n");
                btnstatu = true;
                btnListen.Text = "Start listen";        
            }  
        }

        public void Start_Service()
        {

            if (bConnFlag) return;


            if (string.IsNullOrEmpty(txtPlcIP.Text.Trim())||string.IsNullOrEmpty(txtPlcPort.Text.Trim()))
            {
                MessageBox.Show("Please Input correct Ip Address or Port");

                return;
            }

            if (plc.creatConn(txtPlcIP.Text, txtPlcPort.Text, ref errMsg))
            {
                showClientMsg("Connect 2 Plc Sucessed.");
                bConnFlag = true;
            }
            else
            {
                showClientMsg("Connect 2 Plc Failed!" );
                bConnFlag = false;

                LogHelper.WriteLog(string.Format("Error :{0}-->{1}", DateTime.Now.ToString(), "Connect 2 Plc Failed" + errMsg));
            }

            btnReadItem.Enabled = bConnFlag;
            btnWriteItem.Enabled = bConnFlag;
        }

        private void btnStartService_Click(object sender, EventArgs e)
        {
            Start_Service();
        }

        private void btnReadItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDataAddr.Text.Trim()))
            {
                MessageBox.Show("Please Input correct Dm Address!", "Message");
                return;                
            }
            
            int[] data = { Convert.ToInt16(txtDataAddr.Text),1};

            if (plc.Read(ref data))
            {
                showClientMsg("Read from  Plc Sucessed");
            }
            else
                showClientMsg("Read from Plc Failed ");
        }

        private void btnWriteItem_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtDataAddr.Text.Trim())||string.IsNullOrEmpty(txtValue.Text.Trim()))
            {
                MessageBox.Show("Please Input correct Dm Address or Value ","Message");

                return;
            }

            object[] data = { txtDataAddr.Text, txtValue.Text };
            if (plc.Write(data))
            {
                showClientMsg("Write 2 Plc Sucessed ");
            }
            else
                showClientMsg("Write 2 Plc Failed ");
        }

        //以下实现消息广播
        public void SendBroadMsg(string msg)
        {
            string strDataLine = msg;
            Byte[] sendData = Encoding.UTF8.GetBytes(strDataLine);

            foreach (DictionaryEntry de in _sessionTable)
            {
                EndPoint temp = (EndPoint)de.Key;

                Client.SendTo(sendData, temp);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (myThread != null)
            {
                myThread.Abort();
            }

            if (plc != null)
            {
                plc.close();
            }
        }
    }
}
