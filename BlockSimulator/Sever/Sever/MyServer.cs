using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;

namespace Sever
{
    class MyServer
    {
        #region 변수

        private DBServer myDB = new DBServer();

        public string ipAdress = "172.16.14.49";
        public int port = 8000;
        private byte[] m_buffer = new byte[1000];
        private int m_bytesReceived = 0;
        private string m_receivedMessage = "";

        private TcpListener m_server = null;

        public List<TcpClient> m_client = new List<TcpClient>();
        TcpClient n_Stream = null;

        private NetworkStream m_netStream = null;
        Thread serverListener;

        private bool CloseServer = false;

        #endregion
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
               // ServerLog("Server Started", Color.green);
                m_server.BeginAcceptTcpClient(ClientConnected, null);

                serverListener = new Thread(ClientCommunication);
         
                serverListener.Start();
              //  SqlConnection conn = new SqlConnection();
               // myDB.Connect(conn);
               // myDB.DisConnect(conn);
                Console.WriteLine("성공");
              //  m_ClientComCoroutine = ClientCommunication();
              //  StartCoroutine(m_ClientComCoroutine);
         //       timer.Elapsed += timer_Elapsed;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("========= 에러작 ==========");
              //  Debug.Log("에러");
            }
        }

        public void ServerClose()
        {
            CloseServer = true;
            m_server.Server.Close();
            serverListener.Abort();
            for (int i = m_client.Count-1; i >= 0; i--)
            {
                m_client[i].Close(); 
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
                try
                {
                    int Count = m_client.Count;

                    for (int i= Count - 1; i > -1; i--)
                    {
                        try
                        {
                            num = i;
                            m_netStream = m_client[i].GetStream();
                            
                            if (m_netStream.DataAvailable)
                                m_netStream.BeginRead(m_buffer, 0, m_buffer.Length, MessageReceived, m_netStream);
                        }
                        catch (Exception ex)
                        {
                            if (num != -1)
                            {
                                Console.WriteLine("클라이언트 커뮤니티 연결 에러 <중복 삭제 && 없는 스트림 삭제> " + ex.Message);
                              
                            }
                            continue;
                        }
                    }
                    num = -1;
                }
                catch (Exception ex)
                {
                 
                    Console.WriteLine("[서버 에러 <리스너>] : "  +  ex.Message);
                    continue;
                }

            }
        }

        private void CloseConnection(int mcla)
        {
            m_client[mcla].Close();
            m_client.RemoveAt(mcla);  
        }


        #region Message Controll
        private bool BuildManager(string msg)
        {
            return myDB.Wb_AddBlock(msg);
        
        }
        private bool DestroyManager(string msg)
        {
            return myDB.Wb_DeleteBlock(msg);
          
        }
        private bool ColorChangeManager(string msg)
        {
            return myDB.Wb_UpdateMaterial(msg);      
        }

        private bool CloseClient(string msg)
        {

                string ipstr = ((IPEndPoint)n_Stream.Client.RemoteEndPoint).Address.ToString();
                byte[] bytemsg = Encoding.UTF8.GetBytes("Disconnect : " + ipstr);
                int Count = m_client.Count;
                for (int j = 0; j < Count; j++)
                {
                    try
                    {
                        m_client[j].GetStream().Write(bytemsg, 0, bytemsg.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[서버 에러  <메세지>] : " + ex.Message);
                        continue;
                    }
                }
               // CloseConnection(num);
                return true;
            
        }
        #endregion

        #region Recive 컨트롤
        bool MessageControll()
        {
            string[] stringSeparators = { "#Commend#" };
            string[] str = m_receivedMessage.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

            if (str.Length <= 1)
            {
                m_bytesReceived = 0;
                m_receivedMessage = string.Empty;
                return false;
            }

            switch (str[0])
            {
                case "Build": return BuildManager(str[1]);  //빌딩
                case "Destroy": return DestroyManager(str[1]);   //부시기   
                case "ColorChange": return ColorChangeManager(str[1]);   //컬러 첸이지
            }

            return true;
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
                            n_Stream = m_client[j];
                            m_bytesReceived = m_netStream.EndRead(result);
                            m_receivedMessage = Encoding.UTF8.GetString(m_buffer, 0, m_bytesReceived);       
                            break;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    //if(MessageControll() == false)
                    //    return;

                    #region 일반 메시지 처리
                    int num = -1;
                    for (int j = Count - 1; j > -1; j--)
                    {
                        try
                        {
                            num = j;
                            m_client[j].GetStream().Write(m_buffer, 0, m_bytesReceived);
                        } 
                        catch (Exception ex)
                        {
                            if (num != -1)
                            {
                                Console.WriteLine("[서버 에러  <메세지>] < 중복 처리 ID 삭제 && 없는 소켓 삭제 > : " + ex.Message);
                                CloseConnection(num);
                                continue;
                            }
                        }
                    }

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
                    #endregion
            
        }
        #endregion

        private void ClientConnected(IAsyncResult res)
        {
            if (CloseServer == true)
                return;
            try
            {
                m_client.Add(m_server.EndAcceptTcpClient(res));
            }
            catch (Exception ex)
            {
                Console.WriteLine("[서버 에러 <커넥트>] : " + ex.Message);
            }

            m_server.BeginAcceptTcpClient(ClientConnected, null);
        }
    }
}
