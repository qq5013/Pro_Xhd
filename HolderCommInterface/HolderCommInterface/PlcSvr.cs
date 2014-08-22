﻿using System;
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
            byte[] fins_header_comm = new byte[12];
            fins_header_comm[0] = 0x80;//ICF
            fins_header_comm[1] = 0x00;//RSV
            fins_header_comm[2] = 0x02;//GCT
            fins_header_comm[3] = 0x00;//DNA          
            fins_header_comm[4] = server_node_no;//PLC端节点号
            fins_header_comm[5] = 0x00;//DA2
            fins_header_comm[6] = 0x00;//SNA
            fins_header_comm[7] = client_node_no;//PC端节点号,通过连接程序直接获得的
            fins_header_comm[8] = 0x00;//SA2
            fins_header_comm[9] = Convert.ToByte(SID.ToString(), 16);//SID
            fins_header_comm[10] = 0x01;
            fins_header_comm[11] = 0x01;//读命令

            string saddr_value = "821C2000"; //D7200
            saddr_value += Convert.ToString(readCount, 16).PadLeft(4, '0');
            byte[] addr_value = StringToBytes16(saddr_value);
            WriteData(send_header);
            WriteData(fins_header_comm);
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
            string saddr_value = "821C2000";
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


    }
}
