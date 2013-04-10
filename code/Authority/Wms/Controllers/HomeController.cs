﻿using System.Web.Mvc;
using System.Web.Script.Serialization;
using THOK.WebUtil;
using Microsoft.Practices.Unity;
using THOK.Security;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Bll.Models;

namespace Authority.Controllers
{
    public class HomeController : Controller
    {
        [Dependency]
        public IModuleService ModuleService { get; set; }
        [Dependency]
        public ICityService CityService { get; set; }
        [Dependency]
        public IServerService ServerService { get; set; }
        [Dependency]
        public ISystemService SystemService { get; set; }
        [Dependency]
        public IFormsAuthenticationService FormsService { get; set; }
        [Dependency]
        public IUserService UserService { get; set; }
        public ActionResult Index()
        {
            string userName = this.GetCookieValue("username");
            string cityId = this.GetCookieValue("cityid");
            string serverId = this.GetCookieValue("serverid");
            string systemId = this.GetCookieValue("systemid");
            string ipAdress = UserService.GetUserIp(userName);
            string localip = UserService.GetLocalIp(userName);
            if (!cityId.Equals(string.Empty) && !serverId.Equals(string.Empty) && !systemId.Equals(string.Empty))
            {
                ViewBag.CityName = CityService.GetCityByCityID(cityId).ToString();
                ViewBag.ServerName = ServerService.GetServerById(serverId).ToString();
                ViewBag.SystemName = SystemService.GetSystemById(systemId).ToString();
                ViewBag.userName = userName;
                if (!cityId.Equals(string.Empty) && !serverId.Equals(string.Empty) && !systemId.Equals(string.Empty) && !ipAdress.Equals(string.Empty))
                {
                    ViewBag.ipAdress = ipAdress;
                    ViewBag.localip = localip;
                }
                else
                {
                    ViewBag.localip = localip;
                }
            }
            else
            {
                this.RemoveCookie(cityId);
                this.RemoveCookie(serverId);
                this.RemoveCookie(systemId);
                this.RemoveCookie(userName);
                FormsService.SignOut();
            }
            Session["username"] = userName;
            return View();
        }
        public ActionResult GetUser()
        {
            return Json(User,"text", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult GetMenu()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var jmenus = serializer.Deserialize<Menu[]>(JsonHelper.getJsonMenu());

            var menus = ModuleService.GetUserMenus(User.Identity.Name,this.GetCookieValue("cityid"),this.GetCookieValue("systemid"));          
            return Json(menus,"text",JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult GetFun(string moduleId)
        {
            Fun fun = new Fun()
            {
                funs = new Fun[] { 
                new Fun() { funname = "search", iconCls = "icon-search", funid = "EEB02601-9BF6-412F-A63E-92857BF38638", isActive = true },
                new Fun() { funname = "add", iconCls = "icon-search", funid = "EEB02601-9BF6-412F-A63E-92857BF38638" , isActive = true},
                new Fun() { funname = "edit", iconCls = "icon-search", funid = "EEB02601-9BF6-412F-A63E-92857BF38638" , isActive = true},
                new Fun() { funname = "delete", iconCls = "icon-search", funid = "EEB02601-9BF6-412F-A63E-92857BF38638" , isActive = true},
                 new Fun() { funname = "functionadmin", iconCls = "icon-search", funid = "EEB02601-9BF6-412F-A63E-92857BF38638" , isActive = true},
                new Fun() { funname = "permissionadmin", iconCls = "icon-search", funid = "EEB02601-9BF6-412F-A63E-92857BF38638" , isActive = true},
                new Fun() { funname = "useradmin", iconCls = "icon-search", funid = "EEB02601-9BF6-412F-A63E-92857BF38638" , isActive = true},
                new Fun() { funname = "roleadmin", iconCls = "icon-search", funid = "EEB02601-9BF6-412F-A63E-92857BF38638" , isActive = true},
                new Fun() { funname = "authorize", iconCls = "icon-search", funid = "EEB02601-9BF6-412F-A63E-92857BF38638" , isActive = true},                
                new Fun() { funname = "print", iconCls = "icon-search", funid = "EEB02601-9BF6-412F-A63E-92857BF38638" , isActive = true},
                new Fun() { funname = "help", iconCls = "icon-search", funid = "EEB02601-9BF6-412F-A63E-92857BF38638", isActive = true }
                }
            };
            var funs = ModuleService.GetModuleFuns(User.Identity.Name, this.GetCookieValue("cityid"), moduleId);
            return Json(funs,"text",JsonRequestBehavior.AllowGet);
        }

        public ActionResult PageNotFound()
        {
            ViewBag.PageNotFoundLog = Session["PageNotFoundLog"].ToString();
            return View();
        }

        public ActionResult ServerError()
        {
            ViewBag.ServerErrorLog = Session["ServerErrorLog"].ToString();
            return View();
        }

        public ActionResult Error()
        {
            ViewBag.ErrorLog = Session["ErrorLog"].ToString();
            return View();
        }

        public ActionResult Unauthorized()
        {
            ViewBag.UnauthorizedLog = Session["UnauthorizedLog"].ToString();
            return View();
        }

        public ActionResult AjaxPageNotFound()
        {
            string msg = Session["AjaxPageNotFoundLog"].ToString();
            return Json(JsonMessageHelper.getJsonMessage(false,msg), "text", JsonRequestBehavior.AllowGet);
            //, this.HttpContext.Request.RawUrl
        }

        public ActionResult AjaxServerError()
        {
            string msg = Session["AjaxServerErrorLog"].ToString();
            return Json(JsonMessageHelper.getJsonMessage(false, msg), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxError()
        {
            string msg = Session["AjaxErrorLog"].ToString();
            return Json(JsonMessageHelper.getJsonMessage(false, msg), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxUnauthorized()
        {
            string msg = Session["AjaxUnauthorizedLog"].ToString();
            return Json(JsonMessageHelper.getJsonMessage(false, msg), "text", JsonRequestBehavior.AllowGet);
        }
    }
}
