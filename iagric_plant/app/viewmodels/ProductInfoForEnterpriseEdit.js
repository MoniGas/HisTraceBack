define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo', 'knockout.validation'], function (dialog, ko, jqueryui, km, utils, loginInfo, kv) {

    //获取权限字符串,例如：1,2,3
    var GetPermissionList = function () {
        var arr = new Array();

        $("#table1").find("input[name='cbx2']:checkbox").each(function () {
            if (this.checked == true) {
                var parentItemID = $(this).val();
                if ($.inArray(parentItemID, arr) == -1) {
                    arr.push(parentItemID);
                }
            }
        });
        return arr.join(",");
    }
    var array = new Array();
    var vm = function (Id) {
        var self = this;

        self.MaterialModels = {
            MaterialModelArray: ko.observableArray(),
            selectedAddOption: ko.observable()
        }
        self.MaterialName = ko.observable('');
        self.SelAddProduct = ko.observable(false);
        self.MaterialSpecList = ko.observableArray();
        self.OrderHotline = ko.observable(false);
        self.MaterialPrice = ko.observable(false);
        self.ProductionTime = ko.observable(false);
        self.Factory = ko.observable(false);
        self.ComplaintPhone = ko.observable(false);

        var i = 0;
        self.MaterialModels.selectedAddOption.subscribe(function () {

            self.MaterialSpecList(new Array());
            self.MaterialSpecList(GetMaterialSpecList(self.MaterialModels.selectedAddOption()));

            if (i == 0) { i = i + 1; return; }
            if (self.MaterialModels.selectedAddOption()) {
                self.SelAddProduct(false);
            }
            else {
                self.SelAddProduct(true);
            }
        });

        self.save = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0 && self.MaterialModels.selectedAddOption()) {
                var sendData = {
                    Id: Id,
                    MaterialId: self.MaterialModels.selectedAddOption(),
                    ViewPropertyIdArray: GetPermissionList(),
                    ViewOrderHotline: document.getElementById("CbxOrderHotline").checked,
                    ViewMaterialPrice: document.getElementById("CbxMaterialPrice").checked,
                    ViewProductionTime: document.getElementById("CbxProductionTime").checked,
                    ViewFactory: document.getElementById("CbxFactory").checked,
                    ViewComplaintPhone: document.getElementById("CbxComplaintPhone").checked
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_ProductInfo/EditProductInfoForEnterprise",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                            return;
                        };
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                            if (jsonResult.code == 1) {
                                self.close();
                            }
                        }
                        }]);
                    }
                })
            }
            else {
                if (!self.MaterialModels.selectedAddOption()) {
                    self.SelAddProduct(true);
                }

                self.errors.showAllMessages();
            }
        }

        self.isChecked = function (rId) {
            var rId = rId.toString();
            if ($.inArray(rId, array) == -1) {
                return false;
            } else {
                return true;
            }
        }

        self.init = function () {
            var sendData = {
                Id: Id
            }
            $.ajax({
                type: "POST",
                url: "/Admin_ProductInfo/GetProductInfoForEnterpriseModel",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    if (jsonResult.code == 1) {
                        self.MaterialName(jsonResult.ObjModel.MaterialFullName);
                        self.MaterialModels.selectedAddOption(jsonResult.ObjModel.MaterialId);
                        array = jsonResult.ObjModel.ViewPropertyIdArray.split(',');
                        self.OrderHotline(jsonResult.ObjModel.ViewOrderHotline);
                        self.MaterialPrice(jsonResult.ObjModel.ViewMaterialPrice);
                        self.ProductionTime(jsonResult.ObjModel.ViewProductionTime);
                        self.Factory(jsonResult.ObjModel.ViewFactory);
                        self.ComplaintPhone(jsonResult.ObjModel.ViewComplaintPhone);
                    }
                }
            })
        }
    }

    //获取产品列表
    var GetNewsModules = function () {

        var sendData = {
    }
    var data;
    $.ajax({
        type: "POST",
        url: "/Admin_Request/SearchNameList",
        contentType: "application/json;charset=utf-8", //必须有
        dataType: "json", //表示返回值类型，不必须
        data: JSON.stringify(sendData),
        async: false,
        success: function (jsonResult) {
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                return;
            };
            data = jsonResult.ObjList;
        }
    });
    return data;
}

//获取特殊活动列表
var GetMaterialSpecList = function (MaterialId) {
    var sendData = {
        MaterialId: MaterialId
    }
    var data;
    $.ajax({
        type: "POST",
        url: "/Admin_ProductInfo/GetMaterialSpecList",
        contentType: "application/json;charset=utf-8", //必须有
        dataType: "json", //表示返回值类型，不必须
        data: JSON.stringify(sendData),
        async: false,
        success: function (jsonResult) {
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                return;
            };
            data = jsonResult.ObjList;
        }
    });
    return data;
}

vm.prototype.binding = function () {
    $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

}
vm.prototype.close = function () {
    dialog.close(this);
}
vm.show = function (Id) {
    var vmObj = new vm(Id);
    // 初始化产品列表
    vmObj.MaterialModels.MaterialModelArray(GetNewsModules());
    vmObj.init();
    return dialog.show(vmObj);
};

return vm;
});