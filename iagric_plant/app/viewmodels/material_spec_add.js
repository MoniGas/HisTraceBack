define(['plugins/router', 'knockout', 'jquery', 'kindeditor.zh-CN', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui'],
function (router, ko, $, kcn, kv, utils, loginInfo, dialog, jqueryui) {
    var vmObj;
    var vmMaterialSpecAdd = function () {
        var self = this;

        self.materialProperty = ko.observable('');
        self.materialPropertys = [];
        self.selectedProperty = ko.observable('0');
        self.MaterialSpecification = ko.observable('').extend({
            minLength: { params: 2, message: "规格最小长度为2个字符" },
            maxLength: { params: 50, message: "规格最大长度为50个字符" },
            required: {
                params: true,
                message: "请输入商品规格！"
            }
        });
        self.ExpressPrice = ko.observable('').extend({
            min: {
                params: 0,
                message: "输入价格必须大于0！"
            },
            pattern: {
                params: /^\d+[\.]?\d{0,2}$/g,
                message: "必须是数字，并且最多两位小数！"
            }
        });
        self.Price = ko.observable('').extend({
            min: {
                params: 0.001,
                message: "输入价格必须大于0！"
            },
            max: {
                params: 1000000,
                message: "输入价格必须小于100万！"
            },
            required: {
                params: true,
                message: "请输入价格！"
            },
            pattern: {
                params: /^\d+[\.]?\d{0,2}$/g,
                message: "必须是数字，并且最多两位小数！"
            }
        });
        //self.PropertyCheck = ko.observable(false);
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
        self.selprice = ko.observable(false);
        self.selExpressPrice = ko.observable(false);
        self.cancle = function () {
            self.close();
        }
        self.Register = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0 && self.selectedModel()) {
                var material = 0;
                if (self.selectedModel()) {
                    material = self.selectedModel();
                }
                var Condition = "";
                $("input[name='ChkCondition']:checkbox:checked").each(function () {
                    Condition += $(this).val() + ',';
                });
                var sendData = {
                    materialID: material,
                    materialSpecification: self.MaterialSpecification(),
                    price: self.Price(),
                    Propertys: self.selectedProperty(),
                    Condition: Condition,
                    ExpressPrice: self.ExpressPrice()
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_Material_Spec/Add",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                            return;
                        };
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                            if (jsonResult.code != 0) {
                                self.closeOK(jsonResult.ObjModel);
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
                self.errors.showAllMessages();
            }
        }
        self.selectedProperty.subscribe(function () {
            if (self.selectedProperty() == undefined) {
                $("#ChkCondition").attr("disabled", "disabled");
                $("#ChkCondition").removeAttr("checked");
            }
            else {
                $("#ChkCondition").removeAttr("disabled");
            }
        });
    }
    //获取产品模块
    var getNewsModules = function () {
        var sendData = {};
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Batch/MaterialList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                    return;
                };
                data = jsonResult.ObjList;
            }
        });
        return data;
    }
    vmMaterialSpecAdd.prototype.SearchProperty = function () {
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Material_Spec/GetMaterialPropertyList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                    return;
                };
                data = jsonResult.ObjList;
            },
            error: function (Error) {
                alert(Error);
            }
        });
        return data;
    }
    var SearchProperty = function () {
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Material_Spec/GetMaterialPropertyList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                    return;
                };
                data = jsonResult.ObjList;
            },
            error: function (Error) {
                alert(Error);
            }
        });
        return data;
    }
    vmMaterialSpecAdd.prototype.close = function (a) {
        //alert(this.province().code);
        dialog.close(this, a);
    }
    vmMaterialSpecAdd.prototype.closeOK = function (id) {
        dialog.close(this, id);
    }
    vmMaterialSpecAdd.show = function () {
        vmObj = new vmMaterialSpecAdd();
        vmObj.materialModelsArray = getNewsModules();
        vmObj.materialPropertys = SearchProperty();
        return dialog.show(vmObj);
    };
    return vmMaterialSpecAdd;
});