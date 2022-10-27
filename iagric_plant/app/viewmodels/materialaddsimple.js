define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui'],
    function (router, ko, $, kv, utils, loginInfo, dialog, jqueryui) {
        var vmObj;
        var vm = function (id) {
            var self = this;

            self.materialName = ko.observable('').extend({
                maxLength: { params: 50, message: "名称最大长度为50个字符" },
                required: {
                    params: true,
                    message: "请输入产品名称!"
                }
            });
            self.selMaterialName = ko.observable(false);

            self.materialBrands = [];
            self.selectedBrand = ko.observable(id);

            self.materialBrand = ko.observable('');

            self.materialSpec = ko.observable('').extend({
                maxLength: { params: 25, message: "产品规格最大长度为25个字符" }
            });
            self.selMaterialSpec = ko.observable(false);

            self.AddMaterial = function (data, event) {
                var currentObj = $(event.target);
                currentObj.blur();
                self.errors = ko.validation.group(self);
                if (self.errors().length <= 0) {
                    var brand = self.selectedBrand();
                    if (!self.selectedBrand()) {
                        brand = 0;
                    }
                    var sendData = {
                        materialName: self.materialName(),
                        materialBrand: brand,
                        materialSpec: self.materialSpec()
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Admin_Material/AddSimple",
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
                                    self.closeOK(jsonResult.ObjModel, brand);
                                }
                            }
                            }]);
                        }
                    })
                } else {
                    self.errors.showAllMessages();
                }
            };
        };
        //获取品牌
        var getBrandModules = function () {
            var data;
            var sendData = {
                pageIndex: 1
            }
            $.ajax({
                type: "POST",
                url: "/Admin_Brand/SelectBrand",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
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
        vm.prototype.closeOK = function (id, brandId) {
            dialog.close(this, id, brandId);
        }
        vm.show = function (id) {
            vmObj = new vm(id);
            vmObj.materialBrands = getBrandModules();
            return dialog.show(vmObj);
        };
        return vm;
    });