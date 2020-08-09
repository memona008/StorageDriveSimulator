using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DS.Entities;

namespace DS.DAL
{
    public class FolderDAO
    {
        public static List<FolderDTO> getFoldersByID(int id)
        {
            List<FolderDTO> list=new List<FolderDTO>();
            String  query;
            if (id>0)
           query = String.Format("Select * from folders Where parentFolderID={0}",id);
            else
             query = String.Format("Select * from folders Where parentFolderID={0}", -1);

            using (DBHelper helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                FolderDTO dto = null;

                while(reader.Read())
                {
                    dto = FillDTO(reader);
                    list.Add(dto);
                }

                return list;
            }
        }
        public static FolderDTO getFolderByID(int id)
        {
            List<FolderDTO> list = new List<FolderDTO>();
            String query;
           
                query = String.Format("Select * from folders Where ID={0}", id);

            using (DBHelper helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                FolderDTO dto = null;

                while (reader.Read())
                {
                    dto = FillDTO(reader);

                }

                return dto;
            }
        }

        private static FolderDTO FillDTO(MySqlDataReader reader)
        {
            var dto = new FolderDTO();
            dto.FolderID = reader.GetInt32(0);
            dto.Name = reader.GetString(1);
            dto.ParentFolderID = reader.GetInt32(2);
            dto.isActive = reader.GetInt32(3);
            dto.CreatedOn = reader.GetDateTime(4);
            dto.CreatedBy = reader.GetInt32(5);
            return dto;
        }

        public static int createFolder(String name , int parent)
        {
            FolderDTO dto = new FolderDTO();
            dto.Name = name;
            dto.isActive = 1;
            dto.ParentFolderID = parent;
            dto.CreatedOn = DateTime.Now;
            dto.CreatedBy = 1;
            String sqlQuery = String.Format("INSERT INTO folders(Name, ParentFolderID,isActive, Createdon,CreatedBy) VALUES('{0}',{1},{2},'{3}',{4})",
                dto.Name, dto.ParentFolderID, dto.isActive, dto.CreatedOn.ToString("yyyy-MM-dd hh:mm:ss"),dto.CreatedBy);
        

            using (DBHelper helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }

        }

        public static int deleteFolder(int id)
        {
            int a = 0;
            String sqlQuery;
            Stack<int> s = new Stack<int>();
            s.Push(id);
            do
            {
                var curr = s.Pop();
                sqlQuery = String.Format("select * from folders where ParentFolderid={0}", curr);
               
                using (DBHelper helper = new DBHelper())
                {
                    var reader = helper.ExecuteReader(sqlQuery);
                    while (reader.Read())
                    {
                        s.Push(reader.GetInt32(0));
                    }
                }
                sqlQuery = String.Format("Delete from folders where id={0}", curr);
                using (DBHelper helper = new DBHelper())
                {
                    a += helper.ExecuteQuery(sqlQuery);               
                }


            } while (s.Count != 0);
           
            return a;         

        }
    }
}
