using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient; 

namespace Server_Enter
{
    class DBServer
    {
        
        string connstring = @"Server=DESKTOP-BLRV99I\SQLEXPRESS;;database=
                                   OurGame;uid=sjm;pwd=legoamigo;";
        #region 서버접속
        public void Connect(SqlConnection conn)
        {
            try
            {
                conn.ConnectionString = connstring;
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Errors : " + ex.Message);
            }
        }

        public void DisConnect(SqlConnection conn)
        {
            conn.Close();
        }

        #endregion

        public string Wb_InfoBlockList()
        {
            SqlConnection conn = new SqlConnection();
            Connect(conn);

            string comtext = "select * from BlockTable";

            SqlCommand command = new SqlCommand(comtext, conn);
            SqlDataReader myDataReader;
            myDataReader = command.ExecuteReader();

            StringBuilder temp = new StringBuilder("");
            int Count = 0;
            while (myDataReader.Read())
            {

                string blockName = myDataReader["BlockName"].ToString();
                string PosX = myDataReader["PosX"].ToString();
                string PosY = myDataReader["PosY"].ToString();
                string PosZ = myDataReader["PosZ"].ToString();
                string RotY = myDataReader["RotY"].ToString();
                string Material = myDataReader["Material"].ToString();
                temp.Append(blockName);
                temp.Append("#Name#");
                temp.Append(PosX);
                temp.Append("#PosX#");
                temp.Append(PosY);
                temp.Append("#PosY#");
                temp.Append(PosZ);
                temp.Append("#PosZ#");
                temp.Append(RotY);
                temp.Append("#RotY#");
                temp.Append(Material);
                temp.Append("#DIV#");
                Count++;
            }
            myDataReader.Close();
            StringBuilder str = new StringBuilder("InitMake#Commend#");
            str.Append(Count.ToString());
            str.Append("#Count#");
            str.Append(temp.ToString());
            DisConnect(conn);
            return str.ToString();
        }

    }
}
