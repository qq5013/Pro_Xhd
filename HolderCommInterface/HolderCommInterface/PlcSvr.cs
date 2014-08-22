using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HolderCommInterface
{
     public class PlcSvr
    {
        private TcpClient plc_Socket = null;
        private NetworkStream myNetworkStream;
        private byte client_node_no;
        private byte server_node_no;

        private int readCount = 1;//read address count from begin address
        private int writeCount = 1;//write address count from begin address


        private byte[] StringToBytes16(string source)
        {
            byte[] destination = new byte[source.Length / 2];
            for (int i = 0, j = 0; i < source.Length && j < source.Length / 2; i += 2, j++)
            {
                string item = source.Substring(i, 2);
                destination[j] = Convert.ToByte(item, 16);
            }
            return destination;
        }

        public bool creatConn(string Plc_IP, string Tcp_Port,ref string errMsg )
        {
            bool execResu = true;
            //创建Socket连接
            string tcp_header = "46494E530000000C000000000000000000000000";
            byte[] fins_tcp_header = StringToBytes16(tcp_header);
            try
            {
                plc_Socket = new TcpClient();
                plc_Socket.Connect(IPAddress.Parse(Plc_IP), int.Parse(Tcp_Port));
                myNetworkStream = plc_Socket.GetStream();
            }
            catch (SocketException ex)
            {
  
                errMsg = ex.ToString();
                execResu = false;
                return execResu;
            }

            WriteData(fins_tcp_header);
            byte[] responseMessage = ReadData();
            //response Packet check
            if (responseMessage.Length == 24)
            {
                if (responseMessage[8] != 0x00 || responseMessage[9] != 0x00 || responseMessage[10] != 0x00 || responseMessage[11] != 0x01
                    || responseMessage[12] != 0x00 || responseMessage[13] != 0x00 || responseMessage[14] != 0x00 || responseMessage[15] != 0x00)
                {
                    return false;
                }
                client_node_no = responseMessage[19];
                server_node_no = responseMessage[23];
            }
            else
            {
                return false;
            }
            return execResu;
        }

        public bool Read(ref int[] data)
        {
            string Ssend_header = "46494E530000001A";
            Ssend_header += "0000000200000000";
            byte[] send_header = StringToBytes16(Ssend_header);
            Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
            int SID = ra.Next(1, 100);//generate random sid in orde to check response packet
            byte[] fins_cmd = new byte[12];
            fins_cmd[0] = 0x80;//ICF
            fins_cmd[1] = 0x00;//RSV
            fins_cmd[2] = 0x02;//GCT
            fins_cmd[3] = 0x00;//DNA          
            fins_cmd[4] = server_node_no;//PLC端节点号
            fins_cmd[5] = 0x00;//DA2
            fins_cmd[6] = 0x00;//SNA
            fins_cmd[7] = client_node_no;//PC端节点号,通过连接程序直接获得的
            fins_cmd[8] = 0x00;//SA2
            fins_cmd[9] = Convert.ToByte(SID.ToString(), 16);//SID
            fins_cmd[10] = 0x01;
            fins_cmd[11] = 0x01;//读命令

            fins_cmd[12] = 0x82; // variable type : Dm

            //fins_header_comm[13] = 0x00; // start addr  D100
            //fins_header_comm[14] = 0x64;
            //fins_header_comm[15] = 0x00; // bit

            //fins_header_comm[16] = 0x00; // word read len 150 
            //fins_header_comm[17] = 0x96;

            string saddr_value = "821C2000"; //D7200
            saddr_value += Convert.ToString(readCount, 16).PadLeft(4, '0');
            byte[] addr_value = StringToBytes16(saddr_value);
            WriteData(send_header);
            WriteData(fins_cmd);
            WriteData(addr_value);
            byte[] Reseponse = ReadData();
            //MessageBox.Show(Reseponse.Length.ToString());
            //check response packet 
            if (Reseponse[8] != 0 || Reseponse[9] != 0 || Reseponse[10] != 0 || Reseponse[11] != 2
                || Reseponse[12] != 0 || Reseponse[13] != 0 || Reseponse[14] != 0 || Reseponse[15] != 0
                || Reseponse[26] != 1 || Reseponse[27] != 1 || Reseponse[28] != 0 || Reseponse[29] != 0
                || Reseponse[25] != Convert.ToByte(SID.ToString(), 16))
            {
                return false;
            }

       //   MessageBox.Show((Convert.ToInt32(Reseponse[Reseponse.Length - 2].ToString("X2") + Reseponse[Reseponse.Length - 1].ToString("X2"), 16)).ToString());

            return true;
        }

        public bool Write(object[] data)
        {
            string Ssend_header = "46494E53000000";
            Ssend_header += Convert.ToString((2 * writeCount + 1 + 25), 16).PadLeft(2, '0');
            Ssend_header += "0000000200000000";
            byte[] send_header = StringToBytes16(Ssend_header);
            Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
            int SID = ra.Next(1, 100);//generate random sid in orde to check response packet
            byte[] fins_header_comm = new byte[12];
            fins_header_comm[0] = 0x80;//ICF4294967295
            fins_header_comm[1] = 0x00;//RSV
            fins_header_comm[2] = 0x02;//GCT
            fins_header_comm[3] = 0x00;  //DNA          
            fins_header_comm[4] = server_node_no;//PLC端节点号
            fins_header_comm[5] = 0x00;//DA2
            fins_header_comm[6] = 0x00;//SNA
            fins_header_comm[7] = client_node_no;//PC端节点号,通过连接程序直接获得的
            fins_header_comm[8] = 0x00;//SA2
            fins_header_comm[9] = Convert.ToByte(SID.ToString(), 16);//SID
            fins_header_comm[10] = 0x01; // MRC
            fins_header_comm[11] = 0x02;//SWC 写命令

                                             // dm address   len    data 
            string saddr_value = "821C2000"; // 82 72 00 00  00 00  00 00 
            saddr_value += Convert.ToString(writeCount, 16).PadLeft(4, '0');
            for (int i = 0; i < data.Length; i++)
            {
                if ("System.Int64" == data[i].GetType().ToString())
                {
                    saddr_value += Convert.ToString((long)data[i], 16).PadLeft(8, '0').Substring(4, 4);
                    saddr_value += Convert.ToString((long)data[i], 16).PadLeft(8, '0').Substring(0, 4);
                }
                else
                {
                    saddr_value += Convert.ToString((int)data[i], 16).PadLeft(4, '0');
                }
            }
            byte[] addr_value = StringToBytes16(saddr_value);
            WriteData(send_header);
            WriteData(fins_header_comm);
            WriteData(addr_value);
            byte[] Reseponse = ReadData();
            //check response packet 
            if (Reseponse[8] != 0 || Reseponse[9] != 0 || Reseponse[10] != 0 || Reseponse[11] != 2
                || Reseponse[12] != 0 || Reseponse[13] != 0 || Reseponse[14] != 0 || Reseponse[15] != 0
                || Reseponse[26] != 1 || Reseponse[27] != 2 || Reseponse[28] != 0 || Reseponse[29] != 0
                || Reseponse[25] != Convert.ToByte(SID.ToString(), 16))
            {
             //   MessageBox.Show(Reseponse[8].ToString() + Reseponse[9].ToString() + Reseponse[10].ToString() + Reseponse[11].ToString());
                return false;
            }

            return true;
        }


        private void WriteData(byte[] myByte)
        {

            byte[] writeBytes = myByte;
            myNetworkStream.Write(writeBytes, 0, writeBytes.Length);
            myNetworkStream.Flush();

        }
        private byte[] ReadData()
        {

            int k = plc_Socket.Available;
            while (k == 0)
            {
                k = plc_Socket.Available;
            }
            byte[] myBufferBytes = new byte[k];
            myNetworkStream.Read(myBufferBytes, 0, k);
            myNetworkStream.Flush();
            return myBufferBytes;
        }

        /// <summary>
        /// （若返回的头指令为3）检查命令头中的错误代码
        /// </summary>
        /// <param name="Code">错误代码</param>
        /// <returns>指示程序是否可以继续进行</returns>
        bool CheckHeadError(byte Code)
        {
            switch (Code)
            {
                case 0x00: return true;
                case 0x01: RaiseException("the head is not 'FINS'"); return false;
                case 0x02: RaiseException("the data length is too long"); return false;
                case 0x03: RaiseException("the command is not supported"); return false;
            }
            //no hit
            RaiseException("unknown exception"); return false;
        }

         /// <summary>
         /// 抛出异常
         /// </summary>
         /// <param name="p"></param>
        private void RaiseException(string p)
        {
           
        }

        /// <summary>
        /// 检查命令帧中的EndCode
        /// </summary>
        /// <param name="Main">主码</param>
        /// <param name="Sub">副码</param>
        /// <returns>指示程序是否可以继续进行</returns>
        bool CheckEndCode(byte Main, byte Sub)
        {
            switch (Main)
            {
                case 0x00:
                    switch (Sub)
                    {
                        case 0x00: return true;//the only situation of success
                        case 0x01: RaiseException("service canceled"); return false;
                    }
                    break;
                case 0x01:
                    switch (Sub)
                    {
                        case 0x01: RaiseException("local node not in network"); return false;
                        case 0x02: RaiseException("token timeout"); return false;
                        case 0x03: RaiseException("retries failed"); return false;
                        case 0x04: RaiseException("too many send frames"); return false;
                        case 0x05: RaiseException("node address range error"); return false;
                        case 0x06: RaiseException("node address duplication"); return false;
                    }
                    break;
                case 0x02:
                    switch (Sub)
                    {
                        case 0x01: RaiseException("destination node not in network"); return false;
                        case 0x02: RaiseException("unit missing"); return false;
                        case 0x03: RaiseException("third node missing"); return false;
                        case 0x04: RaiseException("destination node busy"); return false;
                        case 0x05: RaiseException("response timeout"); return false;
                    }
                    break;
                case 0x03:
                    switch (Sub)
                    {
                        case 0x01: RaiseException("communications controller error"); return false;
                        case 0x02: RaiseException("CPU unit error"); return false;
                        case 0x03: RaiseException("controller error"); return false;
                        case 0x04: RaiseException("unit number error"); return false;
                    }
                    break;
                case 0x04:
                    switch (Sub)
                    {
                        case 0x01: RaiseException("undefined command"); return false;
                        case 0x02: RaiseException("not supported by model/version"); return false;
                    }
                    break;
                case 0x05:
                    switch (Sub)
                    {
                        case 0x01: RaiseException("destination address setting error"); return false;
                        case 0x02: RaiseException("no routing tables"); return false;
                        case 0x03: RaiseException("routing table error"); return false;
                        case 0x04: RaiseException("too many relays"); return false;
                    }
                    break;
                case 0x10:
                    switch (Sub)
                    {
                        case 0x01: RaiseException("command too long"); return false;
                        case 0x02: RaiseException("command too short"); return false;
                        case 0x03: RaiseException("elements/data don't match"); return false;
                        case 0x04: RaiseException("command format error"); return false;
                        case 0x05: RaiseException("header error"); return false;
                    }
                    break;
                case 0x11:
                    switch (Sub)
                    {
                        case 0x01: RaiseException("area classification missing"); return false;
                        case 0x02: RaiseException("access size error"); return false;
                        case 0x03: RaiseException("address range error"); return false;
                        case 0x04: RaiseException("address range exceeded"); return false;
                        case 0x06: RaiseException("program missing"); return false;
                        case 0x09: RaiseException("relational error"); return false;
                        case 0x0a: RaiseException("duplicate data access"); return false;
                        case 0x0b: RaiseException("response too long"); return false;
                        case 0x0c: RaiseException("parameter error"); return false;
                    }
                    break;
                case 0x20:
                    switch (Sub)
                    {
                        case 0x02: RaiseException("protected"); return false;
                        case 0x03: RaiseException("table missing"); return false;
                        case 0x04: RaiseException("data missing"); return false;
                        case 0x05: RaiseException("program missing"); return false;
                        case 0x06: RaiseException("file missing"); return false;
                        case 0x07: RaiseException("data mismatch"); return false;
                    }
                    break;
                case 0x21:
                    switch (Sub)
                    {
                        case 0x01: RaiseException("read-only"); return false;
                        case 0x02: RaiseException("protected , cannot write data link table"); return false;
                        case 0x03: RaiseException("cannot register"); return false;
                        case 0x05: RaiseException("program missing"); return false;
                        case 0x06: RaiseException("file missing"); return false;
                        case 0x07: RaiseException("file name already exists"); return false;
                        case 0x08: RaiseException("cannot change"); return false;
                    }
                    break;
                case 0x22:
                    switch (Sub)
                    {
                        case 0x01: RaiseException("not possible during execution"); return false;
                        case 0x02: RaiseException("not possible while running"); return false;
                        case 0x03: RaiseException("wrong PLC mode"); return false;
                        case 0x04: RaiseException("wrong PLC mode"); return false;
                        case 0x05: RaiseException("wrong PLC mode"); return false;
                        case 0x06: RaiseException("wrong PLC mode"); return false;
                        case 0x07: RaiseException("specified node not polling node"); return false;
                        case 0x08: RaiseException("step cannot be executed"); return false;
                    }
                    break;
                case 0x23:
                    switch (Sub)
                    {
                        case 0x01: RaiseException("file device missing"); return false;
                        case 0x02: RaiseException("memory missing"); return false;
                        case 0x03: RaiseException("clock missing"); return false;
                    }
                    break;
                case 0x24:
                    switch (Sub)
                    { case 0x01:RaiseException("table missing"); return false; }
                    break;
                case 0x25:
                    switch (Sub)
                    {
                        case 0x02: RaiseException("memory error"); return false;
                        case 0x03: RaiseException("I/O setting error"); return false;
                        case 0x04: RaiseException("too many I/O points"); return false;
                        case 0x05: RaiseException("CPU bus error"); return false;
                        case 0x06: RaiseException("I/O duplication"); return false;
                        case 0x07: RaiseException("CPU bus error"); return false;
                        case 0x09: RaiseException("SYSMAC BUS/2 error"); return false;
                        case 0x0a: RaiseException("CPU bus unit error"); return false;
                        case 0x0d: RaiseException("SYSMAC BUS No. duplication"); return false;
                        case 0x0f: RaiseException("memory error"); return false;
                        case 0x10: RaiseException("SYSMAC BUS terminator missing"); return false;
                    }
                    break;
                case 0x26:
                    switch (Sub)
                    {
                        case 0x01: RaiseException("no protection"); return false;
                        case 0x02: RaiseException("incorrect password"); return false;
                        case 0x04: RaiseException("protected"); return false;
                        case 0x05: RaiseException("service already executing"); return false;
                        case 0x06: RaiseException("service stopped"); return false;
                        case 0x07: RaiseException("no execution right"); return false;
                        case 0x08: RaiseException("settings required before execution"); return false;
                        case 0x09: RaiseException("necessary items not set"); return false;
                        case 0x0a: RaiseException("number already defined"); return false;
                        case 0x0b: RaiseException("error will not clear"); return false;
                    }
                    break;
                case 0x30:
                    switch (Sub)
                    { case 0x01:RaiseException("no access right"); return false; }
                    break;
                case 0x40:
                    switch (Sub)
                    { case 0x01:RaiseException("service aborted"); return false; }
                    break;
            }
            //no hit
            RaiseException("unknown exception"); return false;
        }




    }
}
