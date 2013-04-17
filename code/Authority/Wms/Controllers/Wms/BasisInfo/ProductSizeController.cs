﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.WebUtil;
using THOK.Security;

namespace Wms.Controllers.Wms.BasisInfo
{
    [TokenAclAuthorize]
    public class ProductSizeController : Controller
    {
        [Dependency]
        public IProductSizeService ProductSizeService { get; set; }
        //
        // GET: /ProductSize/

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

        //
        // GET: /ProductSize/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string ProductCode = collection["ProductCode"] ?? "";
            string SizeNo = collection["SizeNo"] ?? "";
            string AreaNo = collection["AreaNo"] ?? "";
            var productSize = ProductSizeService.GetDetails(page, rows,ProductCode, SizeNo, AreaNo);
            return Json(productSize, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchPage()
        {
            return View();
        }

        public ActionResult AddPage()
        {
            return View();
        }

        //
        // POST: /ProductSize/Create/
        [HttpPost]
        public ActionResult Create(ProductSize productSize)
        {
            string strResult = string.Empty;
            bool bResult = ProductSizeService.Add(productSize, out strResult);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /ProductSize/Edit/5
        public ActionResult Edit(ProductSize productSize)
        {
            string strResult = string.Empty;
            bool bResult = ProductSizeService.Save(productSize, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }


        //
        // POST: /ProductSize/Delete/
        [HttpPost]
        public ActionResult Delete(int productSizeId)
        {
            string strResult = string.Empty;
            bool bResult = false;
            bResult = ProductSizeService.Delete(productSizeId, out strResult);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        // POST: /ProductSize/GetProductSize/
        public ActionResult GetProductSize(int page, int rows, string queryString, string value)
        {
            if (queryString == null)
            {
                queryString = "ID";
            }
            if (value == null)
            {
                value = "";
            }
            var productSize = ProductSizeService.GetProductSize(page, rows, queryString, value);
            return Json(productSize, "text", JsonRequestBehavior.AllowGet);
        }

        //  /ProductSize/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string productCode = Request.QueryString["productCode"];
            //  string state = Request.QueryString["state"];

            THOK.NPOI.Models.ExportParam ep = new THOK.NPOI.Models.ExportParam();
            ep.DT1 = ProductSizeService.GetProductSize(page, rows, productCode);
            ep.HeadTitle1 = "卷烟件烟尺寸信息";
            System.IO.MemoryStream ms = THOK.NPOI.Service.ExportExcel.ExportDT(ep);
            return new FileStreamResult(ms, "application/ms-excel");
        }  
    }
}
