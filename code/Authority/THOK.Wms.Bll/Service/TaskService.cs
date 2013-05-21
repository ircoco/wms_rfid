﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.Bll.Models;
using THOK.Authority.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class TaskService : ServiceBase<Task>, ITaskService
    {
        [Dependency]
        public ITaskRepository TaskRepository { get; set; }

        [Dependency]
        public ICellPositionRepository CellPositionRepository { get; set; }

        [Dependency]
        public IPositionRepository PositionRepository { get; set; }

        [Dependency]
        public IPathRepository PathRepository { get; set; }

        [Dependency]
        public IInBillAllotRepository InBillAllotRepository { get; set; }
        
        [Dependency]
        public IOutBillAllotRepository OutBillAllotRepository { get; set; }

        [Dependency]
        public IMoveBillDetailRepository MoveBillDetailRepository { get; set; }

        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        /// <summary>
        /// 入库单据作业
        /// </summary>
        /// <param name="billNo">单据号</param>
        /// <param name="errInfo">错误消息</param>
        /// <returns></returns>
        public bool InBillTask(string billNo, out string errInfo)
        {
            bool result = true;
            errInfo = string.Empty;
            try
            {
                var originPositionSystem = SystemParameterRepository.GetQueryable().FirstOrDefault(s => s.ParameterName == "IsDefaultProduct");//入库查询其实位置ID
                var allotQuery = InBillAllotRepository.GetQueryable().Where(i => i.BillNo == billNo);
                int param = Convert.ToInt32(originPositionSystem.Id);
                if (allotQuery.Any())
                {
                    foreach (var inItem in allotQuery.ToArray())
                    {
                        //根据入库货位去找货位位置信息
                        var targetCellPosition = CellPositionRepository.GetQueryable().FirstOrDefault(c => c.CellCode == inItem.CellCode);
                        if (targetCellPosition != null)
                        {
                            //根据入库位置ID去找目标区域ID信息
                            var targetPosition = PositionRepository.GetQueryable().FirstOrDefault(p => p.ID == targetCellPosition.StockInPositionID);
                            //根据起始位置ID去找起始区域ID信息
                            var originPosition = PositionRepository.GetQueryable().FirstOrDefault(p => p.ID == param);
                            if (targetPosition != null && originPosition != null)
                            {
                                //根据入库的目标区域和起始位置区域去找路径信息
                                var path = PathRepository.GetQueryable().FirstOrDefault(p => p.OriginRegionID == originPosition.RegionID && p.TargetRegionID == targetPosition.RegionID);
                                if (path != null)
                                {
                                    var inTask = new Task();
                                    inTask.TaskType = "01";
                                    inTask.TaskLevel = 0;
                                    inTask.PathID = path.ID;
                                    inTask.ProductCode = inItem.Product.ProductCode;
                                    inTask.ProductName = inItem.Product.ProductName;
                                    inTask.OriginStorageCode = inItem.CellCode;
                                    inTask.TargetStorageCode = inItem.CellCode;
                                    inTask.OriginPositionID = Convert.ToInt32(originPositionSystem.Id);
                                    inTask.TargetPositionID = targetPosition.ID;
                                    inTask.CurrentPositionID = Convert.ToInt32(originPositionSystem.Id);
                                    inTask.CurrentPositionState = "01";
                                    inTask.State = "01";
                                    inTask.TagState = "01";
                                    inTask.Quantity = Convert.ToInt32(inItem.RealQuantity);
                                    inTask.TaskQuantity = Convert.ToInt32(inItem.RealQuantity);
                                    inTask.OperateQuantity = Convert.ToInt32(inItem.AllotQuantity);
                                    inTask.OrderID = inItem.BillNo;
                                    inTask.OrderType = "01";
                                    inTask.AllotID = inItem.ID;
                                    inTask.DownloadState = "0";
                                    TaskRepository.Add(inTask);
                                }
                                else
                                {
                                    errInfo = "未找到路径信息！";
                                    result = false;
                                }
                            }
                            else
                            {
                                errInfo = "未找到入库位置或位置信息！";
                                result = false;
                            }
                            TaskRepository.SaveChanges();
                        }
                        else
                        {
                            errInfo = "未找到货位位置信息！";
                            result = false;
                        }
                    }
                }
                else
                    errInfo = "当前选择订单没有分配数据，请重新选择！";
            }
            catch (Exception e)
            {
                result = false;
                errInfo = e.Message;
            }
            return result;
        }

        /// <summary>
        /// 出库单据作业
        /// </summary>
        /// <param name="billNo">单据号</param>
        /// <param name="errInfo">错误消息</param>
        /// <returns></returns>
        public bool OutBillTask(string billNo, out string errInfo) 
        {
            bool result = true;
            errInfo = string.Empty;
            try
            {
                var originPositionSystem = SystemParameterRepository.GetQueryable().FirstOrDefault(s => s.ParameterName == "IsDefaultProduct");//出库查询起始位置ID
                var allotQuery = OutBillAllotRepository.GetQueryable().Where(i => i.BillNo == billNo);
                int param = Convert.ToInt32(originPositionSystem.Id);
                if (allotQuery.Any())
                {
                    foreach (var outItem in allotQuery.ToArray())
                    {
                        //根据 出库货位 去找 货位位置信息
                        var targetCellPosition = CellPositionRepository.GetQueryable().FirstOrDefault(c => c.CellCode == outItem.CellCode);
                        if (targetCellPosition != null)
                        {
                            //根据 出库位置ID 去找 目标区域ID 信息
                            var targetPosition = PositionRepository.GetQueryable().FirstOrDefault(p => p.ID == targetCellPosition.StockInPositionID);
                            //根据 起始位置ID 去找 起始区域ID 信息
                            var originPosition = PositionRepository.GetQueryable().FirstOrDefault(p => p.ID == param);
                            if (targetPosition != null && originPosition != null)
                            {
                                //根据 出库的目标区域 和 起始位置区域 去找 路径信息
                                var path = PathRepository.GetQueryable().FirstOrDefault(p => p.OriginRegionID == originPosition.RegionID && p.TargetRegionID == targetPosition.RegionID);
                                if (path != null)
                                {
                                    var outTask = new Task();
                                    outTask.TaskType = "01";
                                    outTask.TaskLevel = 0;
                                    outTask.PathID = path.ID;
                                    outTask.ProductCode = outItem.Product.ProductCode;
                                    outTask.ProductName = outItem.Product.ProductName;
                                    outTask.OriginStorageCode = outItem.CellCode;
                                    outTask.TargetStorageCode = outItem.CellCode;
                                    outTask.OriginPositionID = Convert.ToInt32(originPositionSystem.Id);
                                    outTask.TargetPositionID = targetPosition.ID;
                                    outTask.CurrentPositionID = Convert.ToInt32(originPositionSystem.Id);
                                    outTask.CurrentPositionState = "01";
                                    outTask.State = "01";
                                    outTask.TagState = "01";
                                    outTask.Quantity = Convert.ToInt32(outItem.RealQuantity);
                                    outTask.TaskQuantity = Convert.ToInt32(outItem.RealQuantity);
                                    outTask.OperateQuantity = Convert.ToInt32(outItem.AllotQuantity);
                                    outTask.OrderID = outItem.BillNo;
                                    outTask.OrderType = "01";
                                    outTask.AllotID = outItem.ID;
                                    outTask.DownloadState = "0";
                                    TaskRepository.Add(outTask);
                                }
                                else
                                {
                                    errInfo = "未找到路径信息！";
                                    result = false;
                                }
                            }
                            else
                            {
                                errInfo = "未找到出库位置或位置信息！";
                                result = false;
                            }
                            TaskRepository.SaveChanges();
                        }
                        else
                        {
                            errInfo = "未找到货位位置信息！";
                            result = false;
                        }
                    }
                }
                else
                    errInfo = "当前选择订单没有分配数据，请重新选择！";
            }
            catch (Exception e)
            {
                result = false;
                errInfo = e.Message;
            }
            return result;
        }

        /// <summary>
        /// 移库单据作业
        /// </summary>
        /// <param name="billNo">单据号</param>
        /// <param name="errInfo">错误消息</param>
        /// <returns></returns>
        public bool MoveBillTask(string billNo, out string errInfo)
        {
            bool result = true;
            errInfo = string.Empty;
            var moveQuery = MoveBillDetailRepository.GetQueryable().Where(i => i.BillNo == billNo);
            try
            {
                if (moveQuery.Any())
                {
                    foreach (var moveItem in moveQuery.ToArray())
                    {
                        //根据移出货位查找起始位置信息
                        var originCellPosition = CellPositionRepository.GetQueryable().FirstOrDefault(c => c.CellCode == moveItem.OutCellCode);
                        //根据移入货位查找目标位置信息
                        var targetCellPosition = CellPositionRepository.GetQueryable().FirstOrDefault(c => c.CellCode == moveItem.InCellCode);
                        //根据移出位置ID去找起始区域ID信息
                        var originPosition = PositionRepository.GetQueryable().FirstOrDefault(p => p.ID == originCellPosition.StockOutPositionID);
                        //根据移入位置ID去找目标区域ID信息
                        var targetPosition = PositionRepository.GetQueryable().FirstOrDefault(p => p.ID == targetCellPosition.StockInPositionID);
                        //根据入库的目标区域和起始位置区域去找路径信息
                        var path = PathRepository.GetQueryable().FirstOrDefault(p => p.OriginRegionID == originPosition.RegionID && p.TargetRegionID == targetPosition.RegionID);
                        var moveTask = new Task();
                        moveTask.TaskType = "01";
                        moveTask.TaskLevel = 0;
                        moveTask.PathID = path.ID;
                        moveTask.ProductCode = moveItem.Product.ProductCode;
                        moveTask.ProductName = moveItem.Product.ProductName;
                        moveTask.OriginStorageCode = moveItem.OutCellCode;
                        moveTask.TargetStorageCode = moveItem.InCellCode;
                        moveTask.OriginPositionID = originPosition.ID;
                        moveTask.TargetPositionID = targetPosition.ID;
                        moveTask.CurrentPositionID = originPosition.ID;
                        moveTask.CurrentPositionState = "01";
                        moveTask.State = "01";
                        moveTask.TagState = "01";
                        moveTask.Quantity = Convert.ToInt32(moveItem.RealQuantity);
                        moveTask.TaskQuantity = Convert.ToInt32(moveItem.RealQuantity);
                        moveTask.OperateQuantity = Convert.ToInt32(moveItem.RealQuantity);
                        moveTask.OrderID = moveItem.BillNo;
                        moveTask.OrderType = "02";
                        moveTask.AllotID = moveItem.ID;
                        TaskRepository.Add(moveTask);
                    }
                    TaskRepository.SaveChanges();
                }
                else
                    errInfo = "当前选择订单没有移库细表数据，请重新选择！";
            }
            catch (Exception e)
            {
                result = false;
                errInfo = e.Message;
            }
            return result;
        }
    }
}
