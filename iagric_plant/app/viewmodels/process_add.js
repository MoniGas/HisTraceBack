define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', './operationsShow'],
    function (router, ko, $, kv, utils, loginInfo, dialog, jqueryui, operationsShow) {
        var vmObj;
        var vm = function (id) {
            var self = this;
            self.processName = ko.observable('').extend({
                maxLength: { params: 50, message: "名称最大长度为50个字符" },
                required: {
                    params: true,
                    message: "请输入生产流程名称!"
                }
            });
            self.selProcessName = ko.observable(false);
            self.opName = ko.observable(); ;
            self.processOps = [];
            self.selectprocessOps = ko.observableArray();
            self.selectedOp = ko.observable(id);
            self.processOp = ko.observable('');
            self.alertprocessOp = ko.observable('请选择生产环节！');
            self.selprocessOp = ko.observable(false);
            self.memo = ko.observable('').extend({
                maxLength: { params: 2000, message: "描述最大长度为2000个字符" }
            });
            self.AddProcess = function (data, event) {
                var currentObj = $(event.target);
                currentObj.blur();
                self.errors = ko.validation.group(self);
                if (self.errors().length <= 0) {
                    //                    var operation = self.selectedOp();
                    //                    if (!self.selectedOp()) {
                    //                        operation = 0;
                    //                    }
                    var sendData = {
                        processName: self.processName(),
                        operationList: JSON.stringify(self.selectprocessOps()),
                        memo: self.memo()
                    }
                    $.ajax({
                        type: "POST",
                        url: "/AdminProcess/Add",
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
                                    self.closeOK(jsonResult);
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

        var getOpModules = function () {
            var data;
            var sendData = {
        }
        $.ajax({
            type: "POST",
            url: "/AdminProcess/SelectOp",
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
    vm.prototype.AddProperty = function (data, event) {
        if (vmObj.selectedOp() == undefined) {
            vmObj.alertprocessOp('请选择生产环节！');
            vmObj.selprocessOp(true);
            return;
        }
        vmObj.selprocessOp(false);
        var aa = vmObj.processOps;
        var name = "";
        for (var i = 0; aa.length; i++) {
            if (vmObj.selectedOp() == aa[i].Batch_ZuoYeType_ID) {
                name = aa[i].OperationTypeName;
                //$("option[value='" + aa[i].Batch_ZuoYeType_ID + "']").remove();
                break;
            }
        }
        var bb = vmObj.selectprocessOps();
        var id = vmObj.selectedOp();

        for (var i = 0, l = bb.length; i < l; i++) {
            if (bb[i]["opID"] == id) {
                vmObj.alertprocessOp('已存在该生产环节！');
                vmObj.selprocessOp(true);
                return;
            }
        }
        var single = {
            opName: name,
            opID: id
        }
        vmObj.selectprocessOps.push(single);
    }
    vm.prototype.delProperty = function (data, event) {
        var index = vmObj.selectprocessOps.indexOf(data);
        vmObj.selectprocessOps.splice(index, 1);
    }
    vm.prototype.SortProperty = function () {
        operationsShow.show(vmObj.selectprocessOps()).then(function (OutProperty) {
            vmObj.selectprocessOps(OutProperty);
        });
    }
    vm.prototype.close = function () {
        dialog.close(this);
    }
    vm.prototype.closeOK = function (id) {
        dialog.close(this, id);
    }
    vm.show = function (id) {
        vmObj = new vm(id);
        vmObj.processOps = getOpModules();
        return dialog.show(vmObj);
    };
    return vm;
});