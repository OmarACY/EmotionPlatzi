using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EmtionPlatzi.Web.Models;
using EmtionPlatzi.Web.Util;

namespace EmtionPlatzi.Web.Controllers
{
    public class EmoUploaderController : Controller
    {
        private readonly string _serverFolderPath;
        private EmotionHelper _emoHelper;
        EmtionPlatziWebContext db = new EmtionPlatziWebContext();
        

        public EmoUploaderController()
        {
            _serverFolderPath = ConfigurationManager.AppSettings["UPLOAD_DIR"];
            var key = ConfigurationManager.AppSettings["EMOTION_KEY"];
            _emoHelper = new EmotionHelper(key);
        }
        
        // GET: EmoUploader
        public ActionResult Index()
        {
            return View();
        }

        // POST: EmoUploader
        [HttpPost]
        public async Task<ActionResult> Index(HttpPostedFileBase file)
        {
            if (file?.ContentLength > 0)
            {
                // Generate random name
                var pictureName = Guid.NewGuid().ToString();
                // Concatenate file extension
                pictureName += Path.GetExtension(file.FileName);
                // Generate real route
                var route = Server.MapPath(_serverFolderPath);
                route += "/" + pictureName;
                // Save file
                file.SaveAs(route);

                var emoPicture = await _emoHelper.DetectAndExtractFacesAsync(file.InputStream);

                emoPicture.Name = file.FileName;
                emoPicture.Path = $"{_serverFolderPath}/{pictureName}";

                db.EmoPictures.Add(emoPicture);
                await db.SaveChangesAsync();

                return RedirectToAction("Details", "EmoPictures", new {id = emoPicture.Id});
            }

            return View();
        }
    }
}