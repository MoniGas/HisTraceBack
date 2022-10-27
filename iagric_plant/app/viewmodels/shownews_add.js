define(['plugins/router', 'knockout', 'jquery', 'kindeditor.zh-CN', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui'],
function (router, ko, $, kcn, kv, utils, loginInfo, dialog, jqueryui) {

    selNewsContent = ko.observable(false);

    var vm = function () {
        var self = this;

        self.NewsTitle = ko.observable('').extend({
            maxLength: { params: 50, message: "新闻标题最大长度为50个字符!" },
            required: {
                params: true,
                message: "请输入新闻标题"
            }
        });
        self.selNewsTitle = ko.observable(false);


        //        self.channelsArray = {
        //            channels: ko.observableArray(),
        //            selectchannel: ko.observableArray()
        //        }
        //        self.channelsArray.channels(gethannel());


        //        self.selNewsChannel = ko.observable(false);

        //        var i = 0;
        //        self.channelsArray.selectchannel.subscribe(function () {
        //            if (i == 0) { i = i + 1; return; }
        //            if (self.channelsArray.selectchannel()) {
        //                self.selNewsChannel(false);
        //            }
        //            else {
        //                self.selNewsChannel(true);
        //            }
        //        });


        self.AddNews = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0 /*&& self.channelsArray.selectchannel()*/ && editorInfos.text() != "") {
                //                var channel = 0;
                //                if (self.channelsArray.selectchannel()) {
                //                    channel = self.channelsArray.selectchannel().ID
                //                }
                var sendData = {
                    newsTitle: self.NewsTitle(),
                    //channelId: channel,
                    newsContent: editorInfos.html()
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_ShowNews/Add",
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
                //                if (!self.channelsArray.selectchannel()) {
                //                    self.selNewsChannel(true);
                //                }
                if (editorInfos.text() == "") {
                    selNewsContent(true);
                }
                self.errors.showAllMessages();
            }
        }
    }

    var gethannel = function () {
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_ShowChannel/Index",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必
            async: false,
            success: function (jsonResult) {
                data = jsonResult.ObjList;
            },
            error: function (Error) {
                alert(Error);
            }
        })
        return data;
    }

    vm.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

        editorInfos = KindEditor.create("#txtInfos", {
            cssPath: '/lib/kindeditor/plugins/code/prettify.css',
            uploadJson: '/lib/kindeditor/upload_json.ashx',
            fileManagerJson: '/lib/kindeditor/file_manager_json.ashx',
            allowFileManager: true,
            items: [
						'fontname', 'fontsize', '|', 'forecolor', 'bold', 'italic', 'underline',
						'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
						'insertunorderedlist', '|', 'image'],
            afterCreate: function () { },
            afterBlur: function () {
                this.sync();
                if (this.text() == "") {
                    selNewsContent(true);
                }
                else {
                    selNewsContent(false);
                }
            }
        });
    }
    vm.prototype.close = function () {
        dialog.close(this);
    }
    vm.show = function () {
        return dialog.show(new vm());
    };
    return vm;
});