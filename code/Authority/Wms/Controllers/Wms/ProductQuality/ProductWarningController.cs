﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using System.Web.Routing;
using THOK.WebUtil;

namespace Wms.Controllers.Wms.ProductQuality
{
    public class ProductWarningController : Controller
    {
        //
        // GET: /ProductWarning/
        [Dependency]
        public IProductWarningService ProductWarningServer { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string productCode = collection["ProductCode"];
            decimal minLimited =decimal.Parse(collection["MinLimited"]);
            decimal maxLimited = decimal.Parse(collection["MaxLimited"]);
            decimal assemblyTime = decimal.Parse(collection["AssemblyTime"]);
            var productWarn = ProductWarningServer.GetDetail(page, rows, productCode, minLimited, maxLimited, assemblyTime);
            return Json(productWarn, "text", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create(ProductWarning productWarning)
        {
            bool bResult = ProductWarningServer.Add(productWarning);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(string ProductCode)
        {
            bool bResult = ProductWarningServer.Delete(ProductCode);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(ProductWarning productWarning)
        {
            bool bResult = ProductWarningServer.Save(productWarning);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
    }
}