define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo', 'knockout.validation'], function (dialog, ko, jqueryui, km, utils, loginInfo,kv) {

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

    var GetData = function (MaterialId) {
        var sendData = {
            MaterialId: MaterialId
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_PageNavigation/GetNavigationList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                data = jsonResult.ObjList;
            }
        })
        return data;
    }
    var NavigationArray = new Array();
    var GetNavigationData = function (MaterialId) {
        var sendData = {
            MaterialId: MaterialId
        }
        $.ajax({
            type: "POST",
            url: "/Admin_PageNavigation/GetNavigationForMaterialList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == "1") {
                    NavigationArray = jsonResult.Msg.split(',');
                }
            }
        })
    }

    //    var dataList = GetData();

    var ViewModel = function () {
        var self = this;
        self.dataList = ko.observableArray();
        self.MaterialModels = {
            ModelsArray: ko.observableArray(),
            SelectedMaterialId: ko.observable()
        }

        self.SelMaterial = ko.observable(false);

        self.MaterialModels.SelectedMaterialId.subscribe(function () {
            if (self.MaterialModels.SelectedMaterialId()) {
                self.SelMaterial(false);

                NavigationArray = new Array();
                GetNavigationData(self.MaterialModels.SelectedMaterialId());

                self.dataList(new Array());
                self.dataList(GetData(self.MaterialModels.SelectedMaterialId()));
            }
            else {
                self.SelMaterial(true);
                self.dataList(GetData());
            }
        });

        self.isChecked = function (rId) {
            var rId = rId.toString();
            if ($.inArray(rId, NavigationArray) == -1) {
                return false;
            } else {
                return true;
            }
        }

        self.Save = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0 && self.MaterialModels.SelectedMaterialId()) {
                var sendData = {
                    MaterialId: self.MaterialModels.SelectedMaterialId(),

                    NavigationIdArray: GetPermissionList()
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_PageNavigation/SaveNavigationForEnterpriseList",
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
                                self.Close();
                            }
                        }
                        }]);
                    }
                })
            } else {
                if (!self.MaterialModels.SelectedMaterialId()) {
                    self.SelMaterial(true);
                }
                self.errors.showAllMessages();
            }
        }
    }

    //获取活动动态模块
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

ViewModel.prototype.binding = function () {
    $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

}
//ViewModel.prototype.initMenus = function () {
//    return ko.utils.arrayFilter(dataList, function (item) {
//        return item;
//    });
//}
ViewModel.prototype.Close = function () {
    dialog.close(this);
}
ViewModel.show = function () {
    NavigationArray = new Array();
    var VmObj = new ViewModel();
    VmObj.dataList(GetData());
    //初始化活动动态模块数据
    VmObj.MaterialModels.ModelsArray(GetNewsModules());
    return dialog.show(VmObj);
};

return ViewModel;
});