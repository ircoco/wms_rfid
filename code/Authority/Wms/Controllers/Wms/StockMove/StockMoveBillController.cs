﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.WebUtil;

namespace Authority.Controllers.Wms.StockMove
{
    public class StockMoveBillController : Controller
    {
        [Dependency]
        public IMoveBillMasterService MoveBillMasterService { get; set; }
        [Dependency]
        public IMoveBillDetailService MoveBillDetailService { get; set; }
        //
        // GET: /MovePositionBill/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasAudit = true;
            ViewBag.hasAntiTrial = true;
            ViewBag.hasSettle = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        //
        // GET: /MoveBillMaster/Details/

        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string BillNo = collection["BillNo"] ?? "";
            string WareHouseCode = collection["WareHouseCode"] ?? "";
            string beginDate = collection["beginDate"] ?? "";
            string endDate = collection["endDate"] ?? "";
            string OperatePersonCode = collection["OperatePersonCode"] ?? "";
            string CheckPersonCode = collection["CheckPersonCode"] ?? string.Empty;
            string Status = collection["Status"] ?? "";
            string IsActive = collection["IsActive"] ?? "";
            var moveBillMaster = MoveBillMasterService.GetDetails(page, rows, BillNo, WareHouseCode, beginDate, endDate, OperatePersonCode, CheckPersonCode, Status, IsActive);
            return Json(moveBillMaster, "text", JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /MoveBillDetail/MoveBillDetails/

        public ActionResult MoveBillDetails(int page, int rows, string BillNo)
        {
            var moveBillDetail = MoveBillDetailService.GetDetails(page, rows, BillNo);
            return Json(moveBillDetail, "text", JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /MoveBillMaster/GenMoveBillNo/

        public ActionResult GenMoveBillNo()
        {
            var moveBillNo = MoveBillMasterService.GenMoveBillNo(this.User.Identity.Name.ToString());
            return Json(moveBillNo, "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /MoveBillMaster/Create/

        [HttpPost]
        public ActionResult Create(MoveBillMaster moveBillMaster)
        {
            string strResult = string.Empty;
            bool bResult = MoveBillMasterService.Add(moveBillMaster, this.User.Identity.Name.ToString(), out strResult);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /MoveBillMaster/Edit/

        [HttpPost]
        public ActionResult Edit(MoveBillMaster moveBillMaster)
        {
            string strResult = string.Empty;
            bool bResult = MoveBillMasterService.Save(moveBillMaster, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /MoveBillMaster/Delete/
        public ActionResult Delete(string BillNo)
        {
            string strResult = string.Empty;
            bool bResult = MoveBillMasterService.Delete(BillNo, out strResult);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /MoveBillMaster/Audit/
        public ActionResult Audit(string BillNo)
        {
            string strResult = string.Empty;
            bool bResult = MoveBillMasterService.Audit(BillNo, this.User.Identity.Name.ToString(), out strResult);
            string msg = bResult ? "审核成功" : "审核失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /MoveBillMaster/AntiTrial/
        public ActionResult AntiTrial(string BillNo)
        {
            string strResult = string.Empty;
            bool bResult = MoveBillMasterService.AntiTrial(BillNo, out strResult);
            string msg = bResult ? "反审成功" : "反审失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /MoveBillDetail/MoveBillDetailDelete/

        public ActionResult MoveBillDetailDelete(string ID)
        {
            string strResult = string.Empty;
            bool bResult = MoveBillDetailService.Delete(ID, out strResult);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /MoveBillDetail/MoveBillDetailCreate/

        [HttpPost]
        public ActionResult MoveBillDetailCreate(MoveBillDetail moveBillDetail)
        {
            string strResult = string.Empty;
            bool bResult = MoveBillDetailService.Add(moveBillDetail, out strResult);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /MoveBillDetail/MoveBillDetailEdit/

        [HttpPost]
        public ActionResult MoveBillDetailEdit(MoveBillDetail moveBillDetail)
        {
            string strResult = string.Empty;
            bool bResult = MoveBillDetailService.Save(moveBillDetail, out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GeneratePalletTag(string billNo)
        {
            string strResult = string.Empty;
            bool bResult = MoveBillMasterService.GeneratePalletTag(billNo, ref strResult);
            string msg = bResult ? "BC类烟自动组盘成功" : "BC类烟自动组盘失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /MoveBillMaster/moveBillMasterSettle/

        public ActionResult MoveBillMasterSettle(string billNo)
        {
            string strResult = string.Empty;
            bool bResult = MoveBillMasterService.Settle(billNo, out strResult);
            string msg = bResult ? "结单成功" : "结单失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        #region /MoveBillMaster/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string billNo = Request.QueryString["billNo"];
            System.Data.DataTable dt = MoveBillDetailService.GetMoveBillDetail(page, rows, billNo);
            string strHeaderText = "移库单明细";
            string exportDate = "导出时间：" + System.DateTime.Now.ToString("yyyy-MM-dd");
            string filename = strHeaderText + DateTime.Now.ToString("yyMMdd-HHmm-ss");
            Response.Clear();
            Response.BufferOutput = false;
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Uri.EscapeDataString(filename) + ".xls");
            Response.ContentType = "application/ms-excel";
            string[] str = {
                               "20",        //[0]大标题字体大小
                               "700",       //[1]大标题字体粗宽
                               "10",        //[2]列标题字体大小
                               "700",       //[3]列标题字体粗宽
                               "300",       //[4]excel中有数据表格的大小
                               "微软雅黑",  //[5]大标题字体
                               "Arial",     //[6]小标题字体
                           };
            System.IO.MemoryStream ms = THOK.Common.ExportExcel.ExportDT(dt, strHeaderText, str, exportDate);
            return new FileStreamResult(ms, "application/ms-excel");
        } 
        #endregion
    }
}
