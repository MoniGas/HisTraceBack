define(['plugins/router', 'knockout','jquery', 'knockout.validation', 'utils','logininfo', 'plugins/dialog', 'jquery-ui'],
function (router, ko,$, kv, utils,loginInfo, dialog, jqueryui) {
    var vm = function () {
        var self = this;

        self.channelName = ko.observable('').extend({
            maxLength: { params: 4, message: "栏目名称最大长度为4个字符" },
            required: {
                params: true,
                message: "请输入栏目名称!"
            }
        });
        self.selChannelName = ko.observable(false);


        self.AddChannel = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0) {
                var sendData = {
                    channelName: self.channelName()
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_ShowChannel/Add",
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
        }
    }

    vm.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vm.prototype.close = function () {
        dialog.close(this);
    }
    vm.show = function () {
        return dialog.show(new vm());
    };

    //    var vm = {
    //        activate: function () {
    //        },
    //        binding: function () {
    //            vm.vmShowChannel = new vmShowChannel();
    //        },
    //        saveData: function () {
    //            if (vm.vmShowChannel.errors().length > 0) {
    //                vm.vmShowChannel.errors.showAllMessages();
    //                return;
    //            }
    //        },
    //        goBack: function () {
    //            router.navigateBack();
    //        },
    //        vmShowChannel: null
    //    }
    return vm;
});