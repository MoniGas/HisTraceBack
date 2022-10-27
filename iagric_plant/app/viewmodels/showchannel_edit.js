define(['plugins/router', 'knockout','jquery', 'knockout.validation', 'utils','logininfo', 'plugins/dialog', 'jquery-ui'],
function (router, ko,$, kv, utils,loginInfo, dialog, jqueryui) {

    var vm = function (id) {

        var self = this;

        self.id = id;

        self.channelName = ko.observable('').extend({
            maxLength: { params: 4, message: "栏目名称最大长度为4个字符" },
            required: {
                params: true,
                message: "请输入栏目名称!"
            }
        });
        self.selChannelName = ko.observable(false);

        self.init = function () {
            var sendData = {
                id: self.id
            }
            $.ajax({
                type: "POST",
                url: "/Admin_ShowChannel/Info",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    self.channelName(jsonResult.ObjModel.ChannelName);
                }
            });
        };

        self.SaveChannelName = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0) {
                var sendData = {
                    id: self.id,
                    channelName: self.channelName()
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_ShowChannel/Edit",
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
                });
            } else {
                self.errors.showAllMessages();
            }
        };
    }

    vm.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vm.prototype.close = function () {
        dialog.close(this);
    }
    vm.show = function (id) {
        var vmObj = new vm(id);
        vmObj.init();
        return dialog.show(vmObj);
    };

    return vm;

});