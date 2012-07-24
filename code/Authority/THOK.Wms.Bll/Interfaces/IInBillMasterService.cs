﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IInBillMasterService:IService<InBillMaster>
    {
        object GetDetails(int page, int rows, string BillNo, string BillDate, string OperatePersonCode, string Status, string IsActive);

        bool Add(InBillMaster inBillMaster, string userName);

        bool Delete(string BillNo);

        bool Save(InBillMaster inBillMaster);

        object GenInBillNo();

        bool Audit(string BillNo, string userName);

        bool AntiTrial(string BillNo);

        object GetBillTypeDetail(string BillClass, string IsActive);

        object GetWareHouseDetail(string IsActive);
    }
}
