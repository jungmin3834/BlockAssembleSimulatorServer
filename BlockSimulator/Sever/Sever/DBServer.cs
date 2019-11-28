using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Sever
{
    class DBServer
    {
        
        string connstring = @"Server= localhost; database=
                                   minminftp;uid=minminftp;pwd=qudwns12;";


        //string connstring = @"Server=DESKTOP-BLRV99I\SQLEXPRESS;;database=
        //                           OurGame;uid=sjm;pwd=legoamigo;";
        #region 서버접속
        public void Connect(SqlConnection conn)
        {
            try
            {
 
                conn.ConnectionString = connstring;
                conn.Open();
                Console.WriteLine("DB Connect Success");
            }
            catch(Exception ex)
            {
                Console.WriteLine("DB Errors : " + ex.Message);
            }
        }

        public void DisConnect(SqlConnection conn)
        {
            conn.Close();
        }

        #endregion

        public bool Wb_AddBlock(string str)
        {
            SqlConnection conn = new SqlConnection();
            try
            {

                Console.WriteLine(str);
                return true;
                Connect(conn);

                string countstr = "SELECT COUNT(*) FROM BlockTable";
                int Count = -1;

                SqlCommand countcommand = new SqlCommand(countstr, conn);
                SqlDataReader myDataReader;
                myDataReader = countcommand.ExecuteReader();
                while (myDataReader.Read())
                {
                    Count = int.Parse(myDataReader[0].ToString());
                }
                myDataReader.Close();

                string[] stringName = { "#Name#" };
                string[] name = str.Split(stringName, StringSplitOptions.RemoveEmptyEntries);
                string[] stringPosX = { "#PosX#" };
                string[] PosX = name[1].Split(stringPosX, StringSplitOptions.RemoveEmptyEntries);
                string[] stringPosY = { "#PosY#" };
                string[] PosY = PosX[1].Split(stringPosY, StringSplitOptions.RemoveEmptyEntries);
                string[] stringPosZ = { "#PosZ#" };
                string[] PosZ = PosY[1].Split(stringPosZ, StringSplitOptions.RemoveEmptyEntries);
                string[] stringRotY = { "#RotY#" };
                string[] RotY = PosZ[1].Split(stringRotY, StringSplitOptions.RemoveEmptyEntries);

                string comtext = "insert into BlockTable (SortNum,BlockName,PosX,PosY,PosZ,RotY,Material) values (@Num,@Name,@Px,@Py,@Pz,@Ry,@Mat)";
                SqlCommand command = new SqlCommand(comtext, conn);


                SqlParameter param_name = new SqlParameter("@Name", name[0]);
                command.Parameters.Add(param_name);

                SqlParameter param_px = new SqlParameter("@Px", PosX[0]);
                command.Parameters.Add(param_px);

                SqlParameter param_py = new SqlParameter("@Py", PosY[0]);
                command.Parameters.Add(param_py);

                SqlParameter param_pz = new SqlParameter("@Pz", PosZ[0]);
                command.Parameters.Add(param_pz);

                SqlParameter param_ry = new SqlParameter("@Ry", RotY[0]);
                command.Parameters.Add(param_ry);

                SqlParameter param_mat = new SqlParameter("@Mat", RotY[1]);
                command.Parameters.Add(param_mat);

                SqlParameter accnum = new SqlParameter();
                accnum.ParameterName = "@Num";
                accnum.SqlDbType = System.Data.SqlDbType.Int;
                accnum.Value = Count;
                command.Parameters.Add(accnum);

                if (command.ExecuteNonQuery() >= 1)
                {
                    DisConnect(conn);
                    return true;
                }
                else
                {
                    DisConnect(conn);
                    return false;
                }



            }
            catch(Exception ex)
            {
                DisConnect(conn);
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool Wb_DeleteBlock(string str)
        {
            SqlConnection conn = new SqlConnection();
            try
            {
                Connect(conn);

                int id = int.Parse(str);

                string comtext = "Delete from BlockTable where SortNum = @Accid";

                SqlCommand command = new SqlCommand(comtext, conn);
                SqlParameter param_Num = new SqlParameter();
                param_Num.ParameterName = "@Accid";
                param_Num.SqlDbType = System.Data.SqlDbType.Int;
                param_Num.Value = id;
                command.Parameters.Add(param_Num);




                command.ExecuteNonQuery();
                
                    //DisConnect(conn);
                    //return false;
                
               comtext = "update BlockTable  set SortNum = SortNum - 1  where SortNum > @id";
     

                SqlCommand Updatecommand = new SqlCommand(comtext, conn);

                SqlParameter Num = new SqlParameter();
                Num.ParameterName = "@id";
                Num.SqlDbType = System.Data.SqlDbType.Int;
                Num.Value = id;
                Updatecommand.Parameters.Add(Num);

                Updatecommand.ExecuteNonQuery();
             
                DisConnect(conn);
                return true;
                //else
                //{
                //    DisConnect(conn);
                //    return false;
                //}

              
            }
            catch(Exception ex)
            {
                DisConnect(conn);
                Console.WriteLine(ex.Message);
                return true;
            }
        }

        public bool Wb_UpdateMaterial(string str)
        {
            SqlConnection conn = new SqlConnection();
            try
            {
                Connect(conn);

                string[] stringMat = { "#Material#" };
                string[] mat = str.Split(stringMat, StringSplitOptions.RemoveEmptyEntries);

                int id = int.Parse(mat[0]);

                string comtext = "Update BlockTable SET Material = @Mat where SortNum = @Id";

                SqlCommand command = new SqlCommand(comtext, conn);

                SqlParameter param_name = new SqlParameter("@Mat", mat[1]);
                command.Parameters.Add(param_name);

                SqlParameter param_id = new SqlParameter();
                param_id.ParameterName = "@Id";
                param_id.SqlDbType = System.Data.SqlDbType.Int;
                param_id.Value = id;
                command.Parameters.Add(param_id);



                if (command.ExecuteNonQuery() == 1)
                {
                    DisConnect(conn);
                    return true;
                }
                else
                {
                    DisConnect(conn);
                    return false;
                }
            }
            catch(Exception ex)
            {
                DisConnect(conn);
                Console.WriteLine(ex.Message);
                return false;
            }

        }

 
 /*
        #region 잠시

        #region 로그 작성

            public int GetAccId(string name)
        {
            string comtext = "select AccId from Account where name = @Name";

            Dictionary<int, Account> accountlist = new Dictionary<int, Account>();
            SqlCommand command = new SqlCommand(comtext, conn);

            SqlParameter param_time = new SqlParameter("@Name", name);
            command.Parameters.Add(param_time);

            SqlDataReader myDataReader;

            myDataReader = command.ExecuteReader();

            int idx = -1;
            while (myDataReader.Read())
            {
                idx = int.Parse(myDataReader["AccId"].ToString());
            }
            myDataReader.Close();

            return idx;

        }
        public int GetBalance(int id)
        {
            string comtext = "select balance from Account where accid = @Name";

            Dictionary<int, Account> accountlist = new Dictionary<int, Account>();
            SqlCommand command = new SqlCommand(comtext, conn);

            SqlParameter param_time = new SqlParameter("@Name", id);
            command.Parameters.Add(param_time);

            SqlDataReader myDataReader;

            myDataReader = command.ExecuteReader();

            int idx = -1;
            while (myDataReader.Read())
            {
                idx = int.Parse(myDataReader["Balance"].ToString());
            }
            myDataReader.Close();

            return idx;

        }

        public string WbLogPrintAll(int id)
        {
            string comtext = "select * from account a1 ,tradeinfo t1 where a1.accid = t1.accid and t1.accid = @ACC";
            
            Dictionary<int, Account> accountlist = new Dictionary<int, Account>();
            List<Log> log = new List<Log>();
            SqlCommand command = new SqlCommand(comtext, conn);

            SqlParameter accnum = new SqlParameter();
            accnum.ParameterName = "@ACC";
            accnum.SqlDbType = System.Data.SqlDbType.Int;
            accnum.Value = id;
            command.Parameters.Add(accnum);

            SqlDataReader myDataReader;

            myDataReader = command.ExecuteReader();
           
            string str=null;
            List<string> msg = new List<string>();
            msg.Clear();
            while (myDataReader.Read())
            {
               DateTime date = DateTime.Parse(myDataReader["Date"].ToString());
                str = null;
                str += int.Parse(myDataReader["AccId"].ToString()) + "@";
                str += myDataReader["Name"].ToString() + "@";
                str += myDataReader["TYPE"].ToString() + "@";
              //  str += GetBalance(date).ToString() + "@";
                str += myDataReader["Balance"].ToString() + "@";
                str += int.Parse(myDataReader["Money"].ToString()) + "@";
                str += DateTime.Parse(myDataReader["Date"].ToString()) + "#";
                msg.Add(str);

            }
            Console.WriteLine("옴" + id);
            myDataReader.Close();
            string pack = Packet.PrintLogAll(msg);
            return pack;
        }

        #endregion

        #region 기본 기능

        public string WbAddLogDate(int accid,string msg,int balance,int money)
        {
            string comtext = "insert into TradeInfo (accid,type,BALANCE,money,Date) values (@ACC,@TYPE,@Balance,@Money,@date)";
            SqlCommand command = new SqlCommand(comtext, conn);

            SqlParameter accnum = new SqlParameter();
            accnum.ParameterName = "@ACC";
            accnum.SqlDbType = System.Data.SqlDbType.Int;
            accnum.Value = accid;
            command.Parameters.Add(accnum);

            SqlParameter param_time = new SqlParameter("@TYPE", msg);
            command.Parameters.Add(param_time);

            SqlParameter Balan = new SqlParameter();
            Balan.ParameterName = "@Balance";
            Balan.SqlDbType = System.Data.SqlDbType.Int;
            Balan.Value = balance;
            command.Parameters.Add(Balan);

            SqlParameter param_price = new SqlParameter();
            param_price.ParameterName = "@Money";
            param_price.SqlDbType = System.Data.SqlDbType.Int;
            param_price.Value = money;
            command.Parameters.Add(param_price);


            SqlParameter time = new SqlParameter("@date", DateTime.Now);
            command.Parameters.Add(time);

            if (command.ExecuteNonQuery() == 1)
                return "";

            return "";
        }
   
        
    
    

        public string WbSelectAccount(string str)
        {
            //1. 토큰 분리
            int id = int.Parse(str);
            string pack = null;


            string comtext = "select * from Account where @Id = AccId";
            Dictionary<int, Account> accountlist = new Dictionary<int, Account>();
            SqlCommand command = new SqlCommand(comtext, conn);
            SqlDataReader myDataReader;


            SqlParameter param_price = new SqlParameter();
            param_price.ParameterName = "@Id";
            param_price.SqlDbType = System.Data.SqlDbType.Int;
            param_price.Value = id;
            command.Parameters.Add(param_price);
        

            myDataReader = command.ExecuteReader();


            while (myDataReader.Read())
            {
                Account acc = new Account(int.Parse(myDataReader["AccId"].ToString()), myDataReader["Name"].ToString(),
                    int.Parse(myDataReader["Balance"].ToString()), DateTime.Parse(myDataReader["a1.Date"].ToString()));

                accountlist.Add(acc.Id, acc);
            }
            myDataReader.Close();

            pack = Packet.PrintAllAccountPacket(accountlist);
            return pack;
     
        }

   
       public string WbLogDelete(int  id)
        {
 
            string comtext = "Delete from Tradeinfo where AccId = @Accid";
            SqlCommand command = new SqlCommand(comtext, conn);
            SqlParameter param_price = new SqlParameter();
            param_price.ParameterName = "@Accid";
            param_price.SqlDbType = System.Data.SqlDbType.Int;
            param_price.Value = id;
            command.Parameters.Add(param_price);

            bool result;
            if (command.ExecuteNonQuery() == 1)
                result = true;
            else
                result = false;

            string pack = Packet.DeleteAccountPacket(result, id);
            //1. 토큰 분리

            return pack;
        }
       

        #endregion
        #endregion
  * 
  * */
    }
}

  
