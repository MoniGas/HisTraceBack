define(['plugins/router', 'knockout', 'jquery', 'bootstrap-datepicker', 'logininfo', 'knockout.validation', 'jquery.querystring', 'utils', 'plugins/dialog'],
function (router, ko, $, bdp, loginInfo, kv, qs, utils, dialog) {
    var vmOrder = function (id) {
        var self = this;
        self.id = ko.observable(id);
        self.kdcomp = ko.observable('').extend({
            maxLength: { params: 20, message: "快递公司最大长度为20个字符" },
            required: {
                params: true,
                message: "请输入快递公司!"
            }
        });
        self.selkdcomp = ko.observable(false);

        self.kdnum = ko.observable('').extend({
            maxLength: { params: 50, message: "快递单号最大长度为50个字符" },
            required: {
                params: true,
                message: "请输入快递单号!"
            }
        });
        self.selkdnum = ko.observable(false);

        self.SaveOrder = function () {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0) {

                var sendData = {
                    id: self.id(),
                    ydComp: self.kdcomp(),
                    ydNum: self.kdnum()
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_Material_OnlineOrder/Edit",
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
                        }
                     ]);
                    }
                })

            }
            else {
                self.errors.showAllMessages();
            }
        }
        self.init = function (id) {
            var sendData = {
                id: id
            };
            $.ajax({
                type: "POST",
                url: "/Admin_Material_OnlineOrder/Info",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    self.kdcomp(jsonResult.ObjModel.ExpressComp);
                    self.kdnum(jsonResult.ObjModel.ExpressNum);
                }
            })
        }
    };
    vmOrder.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vmOrder.prototype.close = function () {
        dialog.close(this);
    }
    vmOrder.show = function (id) {
        var vmObj = new vmOrder(id);
        vmObj.init(id);
        return dialog.show(vmObj);
    };
    return vmOrder;
});