using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DS.Entities;

namespace DS.DAL
{
    public class FileDAO
    {

        public static int deleteFile(int id)
        {
            String sqlQuery = String.Format("Delete from files where id={0}", id);
            int a = 0;
            using (DBHelper helper = new DBHelper())
            {
                a = helper.ExecuteQuery(sqlQuery);
            }
            return a;

        }
        public static int Save(FileDTO dto)
        {
           
           
            dto.UploadedOn= DateTime.Now;
            dto.isActive = 1;
           
            String sqlQuery = String.Format("INSERT INTO files(Name, ParentFolderID,FileExt,FileSizeInKB, Uploadedon,isActive,CreatedBy) VALUES('{0}',{1},'{2}',{3},'{4}',{5},{6})",
                dto.Name, dto.ParentFolderID, dto.FileExt, dto.FileSizeInKb,dto.UploadedOn.ToString("yyyy-MM-dd hh:mm:ss"),dto.isActive,dto.CreatedBy);


            using (DBHelper helper = new DBHelper())
            {
                return helper.ExecuteQuery(sqlQuery);
            }

        }
        private static FileDTO FillDTO(MySqlDataReader reader)
        {
            var dto = new FileDTO();
            dto.FileID = reader.GetInt32(0);
            dto.Name = reader.GetString(1);
            dto.ParentFolderID = reader.GetInt32(2);
            dto.FileExt = reader.GetString(3);
            dto.FileSizeInKb = reader.GetInt32(4);
            dto.UploadedOn = reader.GetDateTime(5);
            dto.CreatedBy = reader.GetInt32(7);
            return dto;
        }
        public static List<FileDTO> getFilesByID(int id)
        {
            List<FileDTO> list = new List<FileDTO>();
            String query;
            if (id > 0)
                query = String.Format("Select * from files Where parentFolderID={0}", id);
            else
                query = String.Format("Select * from files Where parentFolderID={0}", -1);

            using (DBHelper helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                FileDTO dto = null;

                while (reader.Read())
                {
                    dto = FillDTO(reader);
                    list.Add(dto);
                }

                return list;
            }
        }


        public static List<FileDTO> getFilesByName(String name)
        {
            List<FileDTO> list = new List<FileDTO>();
            String query;
           
                query = String.Format("Select * from files Where Name like '%{0}%'", name);
           

            using (DBHelper helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                FileDTO dto = null;

                while (reader.Read())
                {
                    dto = FillDTO(reader);
                    list.Add(dto);
                }

                return list;
            }
        }
        public static FileDTO getFileByID(int id)
        {
           
            String query = String.Format("Select * from files Where ID={0}", id);
           
            using (DBHelper helper = new DBHelper())
            {
                var reader = helper.ExecuteReader(query);

                FileDTO dto = null;

                while (reader.Read())
                {
                    dto = FillDTO(reader);
                   
                }

                return dto;
            }
        }
    }
}
