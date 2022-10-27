define(['plugins/router', 'jquery', 'knockout.validation', 'utils', 'plugins/dialog', 'knockout', 'jquery-ui','logininfo'],
function (router,$, kv, utils, dialog, ko, jqueryui,loginInfo) {
    var vmBatch_Add = function () {
        var self = this;
        self.BatchName = ko.observable('').extend({
            minLength: { params: 2, message: "名称最小长度为2个字符" },
            maxLength: { params: 50, message: "名称最大长度为50个字符" },
            required: {
                params: true,
                message: "请输入批次号！"
            }
        });
        self.selTitle = ko.observable(false);
        self.materialModelsArray = [];
        self.selectedModel = ko.observable();
        self.selmaterialmodel = ko.observable(false);
        self.selectedModel.subscribe(function () {
            if (self.selectedModel()) {
                self.selmaterialmodel(false);
            }
            else {
                self.selmaterialmodel(true);
            }
        });
        self.greenHouseModelsArray = [];
        self.selectedGreenhouse = ko.observable();
        self.selgreenhousemodel = ko.observable(false);
        self.selectedGreenhouse.subscribe(function () {
            if (self.selectedGreenhouse()) {
                self.selgreenhousemodel(false);
            }
            else {
                self.selgreenhousemodel(true);
            }
        });
        self.calcel = function () {
            self.close(this);
        }
        self.Register = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0 && self.selectedModel() && self.selectedGreenhouse()) {
                var material = 0;
                if (self.selectedModel()) {
                    material = self.selectedModel();
                }
                var greenhouses = 0;
                if (self.selectedGreenhouse()) {
                    greenhouses = self.selectedGreenhouse();
                }
                var sendData = {
                    materialId: material,
                    batchName: self.BatchName(),
                    greenhousesId: greenhouses
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_Batch/Add",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg,1)) {
                            return;
                        };
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                            if (jsonResult.code != 0) {
                                self.close();
                            }
                        }
                        }]);
                    }
                })
            }
            else {
                if (!self.selectedModel()) {
                    self.selmaterialmodel(true);
                }
                if (!self.selectedGreenhouse()) {
                    self.selgreenhousemodel(true);
                }
                self.errors.showAllMessages();
            }
        }
    }
    //获取产品模块
    var getNewsModules = function () {
        var sendData = {
        };
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Batch/MaterialList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg,1)) {
                    return;
                };
                data = jsonResult.ObjList;
            }
        });
        return data;
    }
    //获取生产基地模块
    var getGreenHouseModules = function () {
        var sendData = {
            greenName: "",
            greenewm: ""
        };
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Batch/GreenHouseList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg,1)) {
                    return;
                };
                data = jsonResult.ObjList;
            }
        });
        return data;
    }
    vmBatch_Add.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vmBatch_Add.prototype.close = function () {
        //alert(this.province().code);
        dialog.close(this);
    }
    vmBatch_Add.show = function () {
        var vmObj = new vmBatch_Add();
        vmObj.materialModelsArray = getNewsModules();
        vmObj.greenHouseModelsArray = getGreenHouseModules();
        return dialog.show(vmObj);
    };
    return vmBatch_Add;

});
