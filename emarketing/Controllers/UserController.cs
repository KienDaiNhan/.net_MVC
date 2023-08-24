using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using emarketing.Models;
using PagedList;


namespace emarketing.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        dbemarketingEntities db = new dbemarketingEntities();
        public ActionResult Index(int?page)
        {
           
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }

        public ActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SignUp(tbl_user uvm,HttpPostedFileBase imgFile)
        {
            string path = uploadimgfile(imgFile);
            if (path.Equals("-1"))
            {   
                ViewBag.error = "Image could not be uploaded.......";
            }
            else
            {
                tbl_user u = new tbl_user();
                u.u_name = uvm.u_name;
                u.u_password = uvm.u_password;
                u.u_email = uvm.u_email;
                u.u_image = path;
                u.u_contact = uvm.u_contact;
                db.tbl_user.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login");
            
            
            }


                return View();
        }//method--------------------


        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(tbl_user uvm)
        {
            tbl_user ad = db.tbl_user.Where(x => x.u_name == uvm.u_name && x.u_password == uvm.u_password).SingleOrDefault();
            if (ad != null)
            {
                Session["u_id"] = ad.u_id.ToString();
                return RedirectToAction("CreateAd");
            }
            else
            {
                ViewBag.error = "Chưa nhập dữ liệu!";
            }
            return View();
        }



        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }

            return path;
        }

        [HttpGet]
        public ActionResult CreateAd()
        {

            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li,"cat_id", "cat_name");
            return View();
        }

        [HttpPost]
        public ActionResult CreateAd(tbl_product pvm, HttpPostedFileBase  imgfile)
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded.......";
            }
            else
            {
                tbl_product p = new tbl_product();
                p.pro_name = pvm.pro_name;
                p.pro_price = pvm.pro_price;
                p.cat_image = path;
                p.pro_fk_cat = p.pro_fk_cat;
                p.pro_des = pvm.pro_des;
                p.pro_fk_user = 1/*Convert.ToInt32(Session["u_id"].ToString())*/;   //phai dang nhap vao thi no se luu fk_user -- khong dang nhap thi luu tinh~ = 1
                db.tbl_product.Add(p);
                db.SaveChanges();
                Response.Redirect("index");

            }
                return View();
        }

        public ActionResult Ads(int ?id)
        {
            return View();
        }
    }
}