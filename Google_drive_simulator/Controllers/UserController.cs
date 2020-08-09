using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DS.DAL;
using Google_drive_simulator.Security;
using Microsoft.WindowsAPICodePack.Shell;
using System.Drawing;
using DS.Entities;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Diagnostics;

namespace Google_drive_simulator.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public ActionResult Login2(String login, String password)
        {
            if (Request["btnLogin"] == "Login")
            {
                if (Request["login"] == null || Request["password"] == null)
                {
                    ViewBag.error = "Enter all fields plz";
                    return View();
                }
                var obj = DS.DAL.UserDAO.ValidateUser(login, password);
                if (obj != null)
                {

                    ViewBag.login = obj.Login;
                    SessionManager.User = obj;
                    return RedirectToAction("Home");

                }

                ViewBag.MSG = "Invalid Login/Password";
                ViewBag.Login = login;


            }
            return View();
        }
        [HttpGet]
        public ActionResult Logout()
        {
            SessionManager.ClearSession();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public ActionResult Home()
        {
            if (SessionManager.IsValidUser)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [HttpPost]
        [ActionName("Home")]
        public ActionResult Home2()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Folder(string action)
        {

            return View("Home");
        }

        [HttpPost]
        [ActionName("Folder")]
        public JsonResult Folder2(int id)
        {
            Object data = null;
            try
            {
                List<DS.Entities.FolderDTO> f = DS.DAL.FolderDAO.getFoldersByID(id);
                if (f.Count > 0)
                {
                    data = new
                    {
                        flag = true,
                        Folders = f
                    };
                }
                else
                {
                    data = new
                    {
                        flag = false,
                        Folders = "null"
                    };
                }



            }
            catch (Exception ex)
            {

                data = new
                {
                    exp = ex,
                    flag = false,
                    Folders = "null"
                };
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ActionName("CreateFolder")]
        public JsonResult CreateFolder2(String name, int parent)
        {
            Object data = null;
            try
            {
                var f = DS.DAL.FolderDAO.createFolder(name, parent);
                if (f != 0)
                {
                    data = new
                    {
                        created = true
                    };
                }
                else
                {
                    data = new
                    {
                        created = false
                    };
                }

            }
            catch (Exception ex)
            {

                data = new
                {
                    exp = ex,
                    created = false
                };
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ActionName("DeleteFolder")]
        public JsonResult DeleteFolder2(int id)
        {
            Object data = null;
            try
            {
                var f = DS.DAL.FolderDAO.deleteFolder(id);
                if (f != 0)
                {
                    data = new
                    {
                        deleted = true
                    };
                }
                else
                {
                    data = new
                    {
                        deleted = false
                    };
                }

            }
            catch (Exception ex)
            {

                data = new
                {
                    exp = ex,
                    deleted = false
                };
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFile(int id)
        {
            Object data = null;
            try
            {
                var f = DS.DAL.FileDAO.deleteFile(id);
                if (f != 0)
                {
                    data = new
                    {
                        deleted = true
                    };
                }
                else
                {
                    data = new
                    {
                        deleted = false
                    };
                }

            }
            catch (Exception ex)
            {

                data = new
                {
                    exp = ex,
                    deleted = false
                };
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ActionName("SaveFiles")]
        public JsonResult SaveFiles2(int parent)
        {
            Object data = null;
            if (Request.Files.Count > 0)
            {
                Boolean flag = true;
                List<FileDTO> Files = new List<FileDTO>();
                try
                {
                    foreach (var fileName in Request.Files.AllKeys)
                    {
                        FileDTO dto = new FileDTO();

                        HttpPostedFileBase file = Request.Files[fileName];
                        if (file != null)
                        {
                            //Generate a unique name using Guid
                            var ext = System.IO.Path.GetExtension(file.FileName);
                            var uniqueName = file.FileName;
                            dto.Name = uniqueName;
                            dto.ParentFolderID = parent;
                            dto.FileExt = ext;
                            dto.FileSizeInKb = Math.Round(((decimal)file.ContentLength / (decimal)1024));
                           
                                //Get physical path of our folder where we want to save images
                                var rootPath = Server.MapPath("~/UploadedFiles");

                                var fileSavePath = System.IO.Path.Combine(rootPath, uniqueName + ext);
                                // Save the uploaded file to "UploadedFiles" folder
                                file.SaveAs(fileSavePath);

                                DS.DAL.FileDAO.Save(dto);

                                // thumbnail work
                                var ThumbPath = Server.MapPath("~/Thumbnails");
                                ShellFile shellFile = ShellFile.FromFilePath(fileSavePath);
                                Bitmap shellThumb = shellFile.Thumbnail.ExtraLargeBitmap;
                                var savingpath = Path.Combine(ThumbPath, dto.Name + ".bmp");
                                shellThumb.Save(savingpath);
                            

                        }
                    }
                    if (flag)
                    {
                        data = new
                        {

                            uploaded = true
                        };
                    }
                    else
                    {
                        data = new
                        {
                            alert = "You cant uplaod more than 8Mbs",
                            uploaded = false
                        };
                    }
                }
                catch (Exception ex)
                {

                    data = new
                    {
                        exp = ex,
                        uploaded = false
                    };
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [ActionName("GetFile")]
        public JsonResult GetFiles2(int id)
        {
            Object data = null;
            try
            {
                List<DS.Entities.FileDTO> f = DS.DAL.FileDAO.getFilesByID(id);

                if (f.Count > 0)
                {
                    data = new
                    {
                        flag = true,
                        Files = f
                    };
                }
                else
                {
                    data = new
                    {
                        flag = false,
                        Files = "null",
                        Thumb = "null"
                    };
                }



            }
            catch (Exception ex)
            {

                data = new
                {
                    exp = ex,
                    flag = false,
                    Files = "null",
                    Thumb = "null"
                };
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ActionName("GetFilesByName")]
        public JsonResult GetFilesByName(String text)
        {
            Object data = null;
            try
            {
                List<DS.Entities.FileDTO> f = DS.DAL.FileDAO.getFilesByName(text);

                if (f.Count > 0)
                {
                    data = new
                    {
                        flag = true,
                        Files = f
                    };
                }
                else
                {
                    data = new
                    {
                        flag = false,
                        Files = "null",
                        Thumb = "null"
                    };
                }



            }
            catch (Exception ex)
            {

                data = new
                {
                    exp = ex,
                    flag = false,
                    Files = "null",
                    Thumb = "null"
                };
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private static byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public FileResult DownloadFile(int id)
        {
            FileDTO f = FileDAO.getFileByID(id);
            var path = Server.MapPath("~/UploadedFiles/");
            //It takes path of file
            String fullpath = path + f.Name + f.FileExt;
            return File(fullpath, System.Net.Mime.MediaTypeNames.Application.Octet, f.Name + f.FileExt);
        }


        public static byte[] ConvertToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


      
        public FileResult DownloadMetaData(int id)
        {


            PdfDocument pdf = new PdfDocument();
            pdf.Info.Title = "My First PDF";
            PdfPage pdfPage = pdf.AddPage();
            XGraphics graph = XGraphics.FromPdfPage(pdfPage);
#pragma warning disable CS0618 // Type or member is obsolete
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.WinAnsi, PdfFontEmbedding.Default);
#pragma warning restore CS0618 // Type or member is obsolete
            XFont fontRegular = new XFont("Arial", 10, XFontStyle.Regular, options);

            List<FolderDTO> folderList = new List<FolderDTO>();
            List<FileDTO> filesList = new List<FileDTO>();        
            Stack<FolderDTO> s = new Stack<FolderDTO>();
            if (id > 0) 
                s.Push(FolderDAO.getFolderByID(id));
           else
            {
                folderList = FolderDAO.getFoldersByID(id);
                if (folderList.Count != 0)
                {
                    for (int i = 0; i < folderList.Count; i++)
                    {
                        s.Push(folderList[i]);
                    }
                }
            }
            FolderDTO fol = new FolderDTO();
            FileDTO file = new FileDTO();
            int height = 30;
            while (s.Count != 0)
            {
                fol = s.Pop();
                int curr = fol.FolderID;
                folderList = FolderDAO.getFoldersByID(fol.FolderID);
                if (folderList.Count != 0)
                    for (int i = 0; i < folderList.Count; i++)
                        s.Push(folderList[i]);
                graph.DrawString("Name: "+fol.Name, fontRegular, XBrushes.Black, 10, height);
                height += 10;
                if(height>=pdfPage.Height)
                {
                    height = 30;
                    pdfPage = pdf.AddPage();
                    graph = XGraphics.FromPdfPage(pdfPage);
                }
                graph.DrawString("Type: Folder", fontRegular, XBrushes.Black, 10, height);
                height += 10;
                if (height >= pdfPage.Height)
                {
                    height = 30;
                    pdfPage = pdf.AddPage();
                    graph = XGraphics.FromPdfPage(pdfPage);
                }
                graph.DrawString("Size: None", fontRegular, XBrushes.Black, 10, height);
                height += 10;
                if (height >= pdfPage.Height)
                {
                    height = 30;
                    pdfPage = pdf.AddPage();
                    graph = XGraphics.FromPdfPage(pdfPage);
                }
                if (fol.ParentFolderID > 0)
                {
                   FolderDTO fol2 = FolderDAO.getFolderByID(fol.ParentFolderID);
                    graph.DrawString("Parent: " + fol2.Name, fontRegular, XBrushes.Black, 10, height);
                }
                else
                    graph.DrawString("Parent: ROOT", fontRegular, XBrushes.Black, 10, height);

                height += 20;
                if (height >= pdfPage.Height)
                {
                    height = 30;
                    pdfPage = pdf.AddPage();
                    graph = XGraphics.FromPdfPage(pdfPage);
                }

                filesList = FileDAO.getFilesByID(fol.FolderID);

                for(int i=0;i<filesList.Count;i++)
                {
                    graph.DrawString("Name: " + filesList[i].Name, fontRegular, XBrushes.Black, 10, height);
                    height += 10;
                    if (height >= pdfPage.Height)
                    {
                        height = 30;
                        pdfPage = pdf.AddPage();
                        graph = XGraphics.FromPdfPage(pdfPage);
                    }
                    graph.DrawString("Type: File", fontRegular, XBrushes.Black, 10, height);
                    height += 10;
                    if (height >= pdfPage.Height)
                    {
                        height = 30;
                        pdfPage = pdf.AddPage();
                        graph = XGraphics.FromPdfPage(pdfPage);
                    }
                    graph.DrawString("Size: "+ filesList[i].FileSizeInKb.ToString()+" KB", fontRegular, XBrushes.Black, 10, height);
                    height += 10;
                    if (height >= pdfPage.Height)
                    {
                        height = 30;
                        pdfPage = pdf.AddPage();
                        graph = XGraphics.FromPdfPage(pdfPage);
                    }
                    if (filesList[i].ParentFolderID > 0)
                    {
                        FolderDTO fol3 = FolderDAO.getFolderByID(filesList[i].ParentFolderID);
                        graph.DrawString("Parent: " + fol3.Name, fontRegular, XBrushes.Black, 10, height);
                    }
                    else
                        graph.DrawString("Parent: ROOT", fontRegular, XBrushes.Black, 10, height);

                    height += 20;
                    if (height >= pdfPage.Height)
                    {
                        height = 30;
                        pdfPage = pdf.AddPage();
                        graph = XGraphics.FromPdfPage(pdfPage);
                    }
                }
            }


            var path = Server.MapPath("~/Meta/abc.pdf");
            var fpatah = Server.MapPath("~/Meta/");
            pdf.Save(path);
            //Process.Start(pdfFilename);  
            return File(path, System.Net.Mime.MediaTypeNames.Application.Pdf, id.ToString()+" MetaData.pdf");
        }
    }


}