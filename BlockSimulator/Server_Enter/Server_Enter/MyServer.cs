using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server_Enter
{
    class MyServer
    {
        #region 변수

        private DBServer myDB = new DBServer();

        public string ipAdress = "172.16.14.49";
        public int port = 8000;
        private byte[] m_buffer = new byte[65535];

        private int m_bytesReceived = 0;
        private string m_receivedMessage = "";


        private TcpListener m_server = null;
        public List<TcpClient> m_client = new List<TcpClient>();

        private NetworkStream m_netStream = null;

        private bool CloseServer = false;

        #endregion
  
        #region 서버 스타트 & 커넥션
        public void ServerStart()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("========= 서버 서비스 시작 ==========");
                CloseServer = false;
                IPAddress ip = IPAddress.Parse(ipAdress);

                m_server = new TcpListener(ip, port);
                m_server.Start();

                m_server.BeginAcceptTcpClient(ClientConnected, null);

                Thread serverListener  = new Thread(ClientCommunication);
                serverListener.Start();
                Console.WriteLine("성공");
                // myDB.Connect();

            }
            catch
            {
                Console.WriteLine("========= 에러작 ==========");
                //Debug.Log("에러");
            }
        }

        private void ClientConnected(IAsyncResult res)
        {
         
            if (CloseServer == true)
                return;
            int num = -1;
            try
            {
                TcpClient tcpcli = m_server.EndAcceptTcpClient(res);
                Console.WriteLine("무언가 입장");
                m_client.Add(tcpcli);
                //InitMakeManager(tcpcli);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[서버 에러 <커넥트>] : " + ex.Message);
                if (num != 1)
                {
                  //  CloseConnection(num);
                }
            }

            m_server.BeginAcceptTcpClient(ClientConnected, null);
        }
        #endregion
        


        #endregion

        #region Recive 컨트롤
        bool MessageControll()
        {
            Console.WriteLine("왔어");
            string[] stringSeparators = { "#Commend#" };
            string[] str = m_receivedMessage.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

            switch (str[0])
            {
                case "Chat": return ChatManager(m_receivedMessage); 
                case "InitMake": return InitMakeManager(m_client[m_client.Count - 1]);
                case "Enter": return EnterManager();
                    //===========없어질것
                case "Build": return BuildManager(m_receivedMessage);
                    
            }
            return false;
        }

        private void MessageReceived(IAsyncResult result)
        {
            if (result.IsCompleted)// && m_client[i].Connected)
            {
                try
                {
                    int Count = m_client.Count;
                    for (int j = 0; j < Count; j++)
                    {
                        try
                        {
                            m_netStream = m_client[j].GetStream();
                            m_bytesReceived = m_netStream.EndRead(result);
                            m_receivedMessage = Encoding.UTF8.GetString(m_buffer, 0, m_bytesReceived);
                            break;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    if (MessageControll() == false)
                        return;
                    if (m_bytesReceived > 0)
                    {
                        m_bytesReceived = 0;
                        m_receivedMessage = null;
                        GC.Collect();
                    }
                }
                catch
                {

                }
            }
        }

        private void ClientCommunication()
        {
            m_bytesReceived = 0;
            m_buffer = new byte[1000];
            int num = -1;
            while (true)
            {
                if (CloseServer == true)
                    return;
             
                    int Count = m_client.Count;
                    for (int i = Count - 1; i > -1; i--)
                    {
                        try
                        {
                            num = i;
                            m_netStream = m_client[i].GetStream();
                            if (m_netStream.DataAvailable)
                                m_netStream.BeginRead(m_buffer, 0, m_buffer.Length, MessageReceived, m_netStream);
                            if (m_bytesReceived > 0)
                            {
                                if (m_receivedMessage == "Close")
                                {
                                    byte[] msgOut = Encoding.UTF8.GetBytes("Close");
                                    m_netStream.Write(msgOut, 0, msgOut.Length);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (num != -1)
                            {
                                Console.WriteLine("[서버 에러 <리스너>] : " + ex.Message);
                               // CloseConnection(num);
                            }
                            continue;
                        }
                    }
                    num = -1;
           
            }
        }
        #endregion


    }
}
