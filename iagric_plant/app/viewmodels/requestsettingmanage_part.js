define(['plugins/router', 'jquery', 'knockout.validation', 'utils', 'plugins/dialog', 'knockout', 'jquery-ui', 'logininfo'],
function (router, $, kv, utils, dialog, ko, jqueryui, loginInfo) {
    var maxCount = 0;
    var type = 0;
    var checkValue = 0;
    function init(subId) {
        var sendData = {
            subId: subId
        };
        $.ajax({
            type: "POST",
            url: "/Admin_RequestCodeSetting/BatchPart",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code != 0) {
                    //dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                }
                else {
                    maxCount = jsonResult.ObjModel.remaining;
                }
            },
            error: function (e) {
            }
        });
    }
    var vmBatchPart = function () {
        var self = this;
        self.SettingId = ko.observable(0);
        self.RequestId = ko.observable(0);
        self.MaterialName = ko.observable('');
        self.BatchName = ko.observable('');
        self.CreateDate = ko.observable('');
        self.CodeCount = ko.observable('');
        self.SunBatchName = ko.observable('').extend({
            required: {
                params: true,
                message: "请输入拆分批次号！"
            }
        });
        self.SubCodeCount = ko.observable('').extend({
            min: {
                params: 1,
                message: "数量最少为1！"
            },
            max: {
                params: maxCount,
                message: "最多输入" + maxCount + "！"
            },
            digit: {
                params: true,
                message: "生成数量为整数！"
            },
            required: {
                params: true,
                message: "请填写配置数量！"
            }
        });
        self.cancel = function () {
            self.close(this);
        }
        self.AddSetting = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if ((checkValue == 1 && self.SubCodeCount() != '') || (checkValue == 2 && self.StartCode() != '' && self.EndCode() != '')) {
                dialog.showMessage('确定要拆分吗？', '系统提示', [{ title: '确定', callback: function () {
                    var sendData = {
                        requestId: self.RequestId(),
                        count: self.SubCodeCount(),
                        batchName: self.SunBatchName(),
                        batchType: checkValue,
                        startCode: self.StartCode(),
                        endCode: self.EndCode()
                    };
                    $.ajax({
                        type: "POST",
                        url: "/Admin_RequestCodeSetting/Add",
                        contentType: "application/json;charset=utf-8", //必须有
                        dataType: "json", //表示返回值类型，不必须
                        data: JSON.stringify(sendData),
                        async: false,
                        success: function (jsonResult) {
                            if (jsonResult.code != 0) {
                                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                            }
                            else {
                                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { self.close(); } }]);
                            }
                        },
                        error: function (e) {
                        }
                    });
                }
                }, { title: '取消', callback: function () { } }]);
            }
            else {
                self.errors.showAllMessages();
            }
        }
        self.init = function (subId) {
            var sendData = {
                subId: subId
            };
            $.ajax({
                type: "POST",
                url: "/Admin_RequestCodeSetting/BatchPart",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    if (jsonResult.code != 0) {
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                    }
                    else {
                        self.MaterialName(jsonResult.ObjModel.materialName);
                        self.BatchName(jsonResult.ObjModel.zbatchName);
                        self.CreateDate(jsonResult.ObjModel.createDate);
                        self.SunBatchName(jsonResult.ObjModel.batchName);
                        self.CodeCount(jsonResult.ObjModel.remaining);
                        self.RequestId(jsonResult.ObjModel.requestId);
                        type = jsonResult.ObjModel.type;
                        checkValue = jsonResult.ObjModel.batchPartType;
                    }
                },
                error: function (e) {
                }
            });
        }
        self.StartCode = ko.observable('').extend({
            required: {
                params: true,
                message: "请填写起始码！"
            }
        });
        self.EndCode = ko.observable('').extend({
            required: {
                params: true,
                message: "请填写结束码！"
            }
        });
    }
    vmBatchPart.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
        if (type == 9) {
            $("#lable1").text("套标数量：");
            $("#lable2").text("使用套标数量：");
        }
        else {
            $("#lable1").text("码数量：");
            $("#lable2").text("使用码数量：");
        }
        $('input:radio[name=CodeTypeCheck]').change(function () {
            if (this.value == '1') {
                checkValue = 1;
                $("#divSplit").show();
                $("#divCustom").hide();
            }
            else {
                checkValue = 2;
                $("#divSplit").hide();
                $("#divCustom").show();
            }
        })
        if (checkValue == 0 || checkValue == '') {
            $("#divSplit").show();
            $("#divCustom").hide();
            checkValue = 1;
            $("input[name='CodeTypeCheck'][value=1]").attr("checked", true);
            $("input[name='CodeTypeCheck'][value=2]").attr("checked", false);
        }
        else if (checkValue == 1) {
            $("#divSplit").show();
            $("#divCustom").hide();
            $("input[name='CodeTypeCheck'][value=1]").attr("checked", true);
            $("input[name='CodeTypeCheck'][value=2]").attr("checked", false);
            $("#divRCustom").hide();
            $("#divRSplit").hide();
        }
        else if (checkValue == 2) {
            $("#divSplit").hide();
            $("#divCustom").show();
            $("input[name='CodeTypeCheck'][value=1]").attr("checked", true);
            $("input[name='CodeTypeCheck'][value=2]").attr("checked", false);
            $("#divRCustom").hide();
            $("#divRSplit").hide();
        }
        else {
            $("#divSplit").show();
            $("#divCustom").hide();
            $("input[name='CodeTypeCheck'][value=1]").attr("checked", false);
            $("input[name='CodeTypeCheck'][value=2]").attr("checked", true);
            $("#divRCustom").hide();
            $("#divRSplit").hide();
        }
    }
    vmBatchPart.prototype.close = function () {
        dialog.close(this);
    }
    vmBatchPart.show = function (subId) {
        init(subId);
        var vmObj = new vmBatchPart();
        vmObj.init(subId);
        vmObj.SettingId(subId);
        return dialog.show(vmObj);
    };
    return vmBatchPart;
});
