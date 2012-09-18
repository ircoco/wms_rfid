﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.WebUtil;
using THOK.WMS.DownloadWms.Bll;
using THOK.Wms.DownloadWms.Bll;

namespace Authority.Controllers.Wms.SortingInfo
{
    public class SortingOrderController : Controller
    {
        [Dependency]
        public ISortOrderService SortOrderService { get; set; }
        [Dependency]
        public ISortOrderDetailService SortOrderDetailService { get; set; }
        [Dependency]
        public IDeliverLineService DeliverLineService { get; set; }
        [Dependency]
        public ICustomerService CustomerService { get; set; }
        //
        // GET: /SortingOrder/
        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasDownload = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        //查询主单
        // GET: /SortingOrder/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string OrderID = collection["OrderID"] ?? "";
            string orderDate = collection["orderDate"] ?? "";
            var sortOrder = SortOrderService.GetDetails(page, rows, OrderID, orderDate);
            return Json(sortOrder, "text", JsonRequestBehavior.AllowGet);
        }

        //查询细单
        // GET: /SortingOrder/sortOrderDetails/
        public ActionResult sortOrderDetails(int page, int rows, string OrderID)
        {
            var SortOrderDetail = SortOrderDetailService.GetDetails(page, rows, OrderID);
            return Json(SortOrderDetail, "text", JsonRequestBehavior.AllowGet);
        }

        //根据时间分组查询主单
        // GET: /SortingOrder/GetOrderMaster/
        public ActionResult GetOrderMaster(string orderDate)
        {
            var sortOrder = SortOrderService.GetDetails(orderDate);
            return Json(sortOrder, "text", JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /SortingOrder/DownSortOrder/
        public ActionResult DownSortOrder(string beginDate, string endDate, string sortLineCode, bool isSortDown,string batch)
        {
            string errorInfo = string.Empty;
            string lineErrorInfo = string.Empty;
            string custErrorInfo = string.Empty;
            bool bResult = false;
            bool lineResult = false;
            if (beginDate == string.Empty || endDate == string.Empty)
            {
                beginDate = DateTime.Now.ToString("yyyyMMdd");
                endDate = DateTime.Now.ToString("yyyyMMdd");
            }
            else
            {
                beginDate = Convert.ToDateTime(beginDate).ToString("yyyyMMdd");
                endDate = Convert.ToDateTime(endDate).ToString("yyyyMMdd");
            }
            DownSortingInfoBll sortBll = new DownSortingInfoBll();
            DownRouteBll routeBll = new DownRouteBll();
            DownSortingOrderBll orderBll = new DownSortingOrderBll();
            DownCustomerBll custBll = new DownCustomerBll();
            routeBll.DeleteTable();
            bool custResult = custBll.DownCustomerInfo();
            if (isSortDown)
            {
                //从分拣下载分拣数据
                lineResult = routeBll.DownSortRouteInfo();
                bResult = sortBll.GetSortingOrderDate(beginDate, endDate, sortLineCode, batch, out errorInfo);
            }
            else
            {
                //从营销下载分拣数据
                lineResult = routeBll.DownRouteInfo();                
                bResult = orderBll.GetSortingOrderDate(beginDate, endDate, out errorInfo);
            }

            string info = "线路：" + lineErrorInfo + "。客户：" + custErrorInfo + "。分拣" + errorInfo;
            string msg = bResult ? "下载成功" : "下载失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, errorInfo), "text", JsonRequestBehavior.AllowGet);
        }

        #region /SortingOrder/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string orderId = Request.QueryString["orderId"];

            System.Data.DataTable dt = SortOrderDetailService.GetSortOrderDetail(page, rows, orderId);
            string headText = "分拣订单管理";
            string headFontName = "微软雅黑"; Int16 headFontSize = 20;
            string colHeadFontName = "Arial"; Int16 colHeadFontSize = 10; Int16 colHeadWidth = 300;
            string exportDate = "导出时间：" + System.DateTime.Now.ToString("yyyy-MM-dd");
            string filename = headText + DateTime.Now.ToString("yyMMdd-HHmm-ss");

            Response.Clear();
            Response.BufferOutput = false;
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Uri.EscapeDataString(filename) + ".xls");
            Response.ContentType = "application/ms-excel";

            System.IO.MemoryStream ms = THOK.Common.ExportExcel.ExportDT(dt, null, headText, null, headFontName, headFontSize, colHeadFontName, colHeadFontSize, colHeadWidth, exportDate);
            return new FileStreamResult(ms, "application/ms-excel");
        }
        #endregion
    }
}
