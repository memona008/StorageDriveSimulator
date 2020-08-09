using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
using System.Drawing;
namespace DS.Entities
{
    public class FileDTO
    {
        public int FileID { get; set; }
        public String Name { get; set; }
        public int ParentFolderID { get; set; }

        public int  isActive { get; set; }

        public decimal FileSizeInKb { get; set; }

        public String FileExt { get; set; }

        public DateTime UploadedOn { get; set; }

       public ShellFile thumb { get; set; }
        public int CreatedBy { get; set; }
    }
}
