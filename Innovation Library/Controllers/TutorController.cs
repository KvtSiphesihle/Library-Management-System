using Innovation_Library.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.UI.WebControls;

namespace Innovation_Library.Controllers
{
    public class TutorController : Controller
    {
        // GET: Tutor
        ApplicationDbContext _db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View(_db.Tutors.ToList());
        }

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public TutorController()
        {

        }

        public TutorController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult RegisterTutor()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> RegisterTutor(Tutor tutor)
        {
            if (ModelState.IsValid)
            {
                var Password = "Tutor@inno11";
                var user = new ApplicationUser { UserName = tutor.EmailAddress, Email = tutor.EmailAddress, ContactNo = tutor.PhoneNumber };
                var result = await UserManager.CreateAsync(user, Password);

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_db));

                if (result.Succeeded)
                {
                    Tutor _tutor = new Tutor
                    {
                        EmailAddress = user.Email,
                        UserId = user.Id,
                        Name = tutor.Name,
                        PhoneNumber = tutor.PhoneNumber
                    };

                    _db.Tutors.Add(_tutor);
                    _db.SaveChanges();

                    if (!roleManager.RoleExists("Tutor"))
                    {
                        var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                        {
                            Name = "Tutor"
                        };
                        roleManager.Create(role);
                        UserManager.AddToRole(user.Id, "Tutor");
                    }
                    else
                    {
                        UserManager.AddToRole(user.Id, "Tutor");
                    }
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "User Already Exist.");
            }
            return View();
        }

        //[Authorize(Roles = "Tutor")]
        public ActionResult AddAnnouncement()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddAnnouncement(Announcement announcement)
        {
            var UserID = User.Identity.GetUserId();
            announcement.Id = UserID;
            _db.Announcements.Add(announcement);
            _db.SaveChanges();
            return RedirectToAction("AnnouncementIndex");
        }
        public ActionResult DeleteAnnouncement(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            LearningContent content = _db.LearningContents.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            return View(content);
        }
        [HttpPost]
        public ActionResult DeleteAnnouncement(int id)
        {
            Announcement announcement = _db.Announcements.Find(id);
            _db.Announcements.Remove(announcement);
            _db.SaveChanges();
            return RedirectToAction("AnnouncementIndex");
        }
        public ActionResult AnnouncementIndex()
        {
            return View(_db.Announcements.ToList());
        }
        public ActionResult NewSession()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewSession(LearningContent content)
        {
            var UserID = User.Identity.GetUserId();
            content.Id = UserID;
            content.UserId = UserID;
            string fileName = Path.GetFileNameWithoutExtension(content.ImageFile.FileName);
            string extension = Path.GetExtension(content.ImageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            content.SessionCover = "../Content/" + fileName;
            fileName = Path.Combine(Server.MapPath("../Content/"), fileName);
            content.ImageFile.SaveAs(fileName);
            _db.LearningContents.Add(content);
            _db.SaveChanges();
            return RedirectToAction("SessionIndex");
        }
        public ActionResult DeleteSession()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DeleteSession(int? id)
        {
            LearningContent learningContent = _db.LearningContents.Find(id);
            _db.LearningContents.Remove(learningContent);
            _db.SaveChanges();
            return RedirectToAction("SessionIndex");
        }
        public ActionResult SessionIndex()
        {
            return View(_db.LearningContents.ToList());
        }
        public ActionResult Announcements()
        {
            return View(_db.Announcements.ToList());
        }
        public ActionResult InnoLearn()
        {
            return View(_db.LearningContents.ToList());
        }
        public ActionResult AnnouncementDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Announcement announcement = _db.Announcements.Find(id);
            if (announcement == null)
            {
                return HttpNotFound();
            }
            return View(announcement);
        }
        public ActionResult SessionDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            LearningContent content = _db.LearningContents.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            return View(content);
        }
        [HttpPost]
        public ActionResult CommentSession(int SessionID, ContentComment comment)
        {
            LearningContent learningContent = _db.LearningContents.Find(SessionID);
            comment.CommentedAt = DateTime.Now;
            var UserID = User.Identity.GetUserId();
            comment.Id = UserID;
            comment.LearningContentId = SessionID;
            learningContent.ContentComments.Add(comment);
            _db.SaveChanges();
            return View("SessionDetails", learningContent);
        }

        [HttpPost]
        public ActionResult CommentAnnouncement(int AnnouncementID, AnnouncementComment comment)
        {
            Announcement announcement = _db.Announcements.Find(AnnouncementID);
            comment.CommentedAt = DateTime.Now;
            var UserID = User.Identity.GetUserId();
            comment.Id = UserID;
            comment.AnnouncementId = AnnouncementID;
            announcement.AnnouncementComments.Add(comment);
            _db.SaveChanges();
            return View("AnnouncementDetails", announcement);
        }
        public ActionResult DeleteCommentSession(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ContentComment content = _db.ContentComments.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            var SessionID = content.LearningContentId;
            LearningContent learningContent = _db.LearningContents.Find(SessionID);
            _db.ContentComments.Remove(content);
            _db.SaveChanges();
            return View("SessionDetails", learningContent);
        }
        public ActionResult DeleteCommentAnnouncement(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            AnnouncementComment content = _db.AnnouncementComments.Find(id);
            if (content == null)
            {
                return HttpNotFound();
            }
            var SessionID = content.AnnouncementId;
            LearningContent learningContent = _db.LearningContents.Find(SessionID);
            _db.AnnouncementComments.Remove(content);
            _db.SaveChanges();
            return View("AnnouncementDetails", learningContent);
        }
        // Slides
        public ActionResult SlidesIndex()
        {
            return View(_db.Slidess.ToList());
        }
        [Authorize(Roles ="Tutor")]
        public ActionResult NewSlide()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewSlide(Slides slides)
        {
            string UserID = User.Identity.GetUserId();
            slides.UserID = UserID;
            string fileName = Path.GetFileNameWithoutExtension(slides.SlideFile.FileName);
            string extension = Path.GetExtension(slides.SlideFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            slides.SlideUrl = "../Content/" + fileName;
            fileName = Path.Combine(Server.MapPath("../Content/"), fileName);
            slides.SlideFile.SaveAs(fileName);
            _db.Slidess.Add(slides);
            _db.SaveChanges();
            return RedirectToAction("Slides");
        }
        //[Authorize(Roles = "Student")]
        public ActionResult Slides()
        {
            return View(_db.Slidess.OrderByDescending(t => t.UserID).ToList());
        }
        //public ActionResult ViewSlide(string SlideUrl)
        //{
        //    byte[] FileBytes = System.IO.File.ReadAllBytes(SlideUrl);
        //    return File(FileBytes, "application/pdf");
        //}
     
    }
}