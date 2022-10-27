define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui'],
    function (router, ko, $, kv, utils, loginInfo, dialog, jqueryui) {

        var vmStore = function () {
            var self = this;

            self.storeName = ko.observable('').extend({
                maxLength: { params: 50, message: "仓库名称最大长度为50个字符" },
                required: { params: true, message: "请输入仓库名称!" }
            });
            self.selStoreName = ko.observable(true);

            self.storeCode = ko.observable('').extend({
                maxLength: { params: 50, message: "仓库编码最大长度为50个字符" },
                pattern: { params: /^[0-9a-zA-Z]*$/g, message: "仓库编码格式不正确!" }
            });
            self.selStoreCode = ko.observable(true);

            self.AddStore = function (data, event) {
                var currentObj = $(event.target);
                currentObj.blur();
                self.errors = ko.validation.group(self);
                if (self.errors().length <= 0) {
                    var sendData = {
                        storeName: self.storeName(),
                        storeCode: self.storeCode(),
                        id: 0,
                        type: 1
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Store/Add",
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
                                    self.close();
                                }
                            }
                            }]);
                        }
                    })
                } else {
                    self.errors.showAllMessages();
                }
            };
        }

        vmStore.prototype.binding = function () {
            $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
        }

        vmStore.prototype.close = function () {
            dialog.close(this);
        }
        vmStore.show = function () {
            return dialog.show(new vmStore());
        };

        return vmStore;
    });