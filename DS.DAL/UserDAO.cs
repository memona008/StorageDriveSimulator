using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DS.Entities;

namespace DS.DAL
{
   public class UserDAO
    {
        public static int Save(UserDTO dto)
        {
            String sqlQuery = "";
          
                
                sqlQuery = String.Format("INSERT INTO Users(Name, Login,Password, Email) VALUES('{0}','{1}','{2}','{3}')",
                    dto.Name, dto.Login, dto.Password, dto.Email);
            

            using (DBHelper helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }
        }

        public static int UpdatePassword(UserDTO dto)
        {
            String sqlQuery = "";
            sqlQuery = String.Format("Update Users Set Password='{0}' Where ID={1}", dto.Password, dto.UserID);


            using (DBHelper helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }
        }

        public static UserDTO ValidateUser(String pLogin, String pPassword)
        {
            var query = String.Format("Select * from Users Where Login='{0}' and Password='{1}'", pLogin, pPassword);

            using (DBHelper helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                UserDTO dto = null;

                if (reader.Read())
                {
                    dto = FillDTO(reader);
                }

                return dto;
            }
        }

        public static UserDTO GetUserById(int pid)
        {

            var query = String.Format("Select * from Users Where Id={0}", pid);

            using (DBHelper helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                UserDTO dto = null;

                if (reader.Read())
                {
                    dto = FillDTO(reader);
                }

                return dto;
            }
        }

        private static UserDTO FillDTO(MySqlDataReader reader)
        {
            var dto = new UserDTO();
            dto.UserID = reader.GetInt32(0);
            dto.Name = reader.GetString(1);
            dto.Login = reader.GetString(2);
            dto.Password = reader.GetString(3);
            dto.Email= reader.GetString(4);
            

            return dto;
        }

    }
}
