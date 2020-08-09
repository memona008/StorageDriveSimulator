using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.Entities
{
    public class FolderDTO
    {
        public int FolderID { get; set; }
        public String Name { get; set; }
        public int ParentFolderID { get; set; }
        public int isActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}
