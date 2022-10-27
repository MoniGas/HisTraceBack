define(['plugins/router', 'knockout', 'jquery', 'kindeditor.zh-CN', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'jquery.querystring', 'bootbox'],
function (router, ko, $, kcn, kv, utils, loginInfo, dialog, jqueryui, uploadify, qs, bootbox) {
    var vmObj;
    function complementHistoryDate() {
        var date = new Date();
        var seperator1 = "-";
        var strYear = date.getFullYear();
        var strMonth = (date.getMonth() + 1).toString();
        var strDate = date.getDate();
        if (strMonth >= 1 && strMonth <= 9) {
            strMonth = "0" + strMonth;
        }
        if (strDate >= 0 && strDate <= 9) {
            strDate = "0" + strDate;
        }
        var currentdate = strYear + seperator1 + strMonth + seperator1 + strDate;
        return currentdate;
    }
    var vmReportAdd = function () {
        var self = this;
        self.subId = ko.observable(0);
        self.reportId = ko.observable(0);
        //质检标题
        self.ReportTitle = ko.observable().extend({
            required: {
                params: true,
                message: "请输入检测标题！"
            }
        });
        //检测日期
        self.ReportTime = ko.observable(complementHistoryDate()).extend({
            required: {
                params: true,
                message: "请输入检测日期！"
            }
        });
        //检测图片数组
        self.ReportImgs = ko.observableArray();
        self.hasReportImgs = ko.observable(false);
        self.loadingImage = {
            fileUrl: '../../images/load.gif',
            fileUrls: '../../images/load.gif'
        };
        self.init = function (subId, reportId, rowData) {
            self.subId(subId);
            self.reportId(reportId);
            if (rowData != null) {
                self.ReportTitle(rowData.Content);
                self.ReportTime(rowData.StrAddDate.replace('年', '-').replace('月', '-').replace('日', ''));
                self.ReportImgs(rowData.imgs);
            }
        }
        //添加巡检
        self.AddReport = function () {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(this);
            if (self.errors().length <= 0 && self.ReportImgs().length > 0) {
                self.hasReportImgs(false);
                var sendData;
                var Action = "AddReport";
                if (self.reportId() == 0) {
                    var sendData = {
                        settingId: self.subId(),
                        content: self.ReportTitle(),
                        addDate: self.ReportTime(),
                        files: JSON.stringify(self.ReportImgs())
                    }
                }
                else {
                    var sendData = {
                        id: self.reportId(),
                        content: self.ReportTitle(),
                        addDate: self.ReportTime(),
                        files: JSON.stringify(self.ReportImgs()).replace('[[', '[').replace(']]', ']')
                    }
                    Action = "EditReport";
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_Batch_JianYanJianYi/" + Action,
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (jsonResult.code != 1) {
                            dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () { } }]);
                        }
                        else {
                            dialog.showMessage('保存成功！', '系统提示', [{ title: '确定', callback: function () { self.close(); } }]);
                        }
                    },
                    error: function (e) {
                    }
                });
            }
            else {
                $(".validationMessage").css("display", "");
                if (self.ReportImgs().length <= 0) {
                    self.hasReportImgs(true);
                }
                self.errors.showAllMessages();
            }
        }
        self.cancle = function () {
            self.close();
        }
    }
    vmReportAdd.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
        try {
            $("#A4").uploadify('destroy');
        }
        catch (Error) { }
        $("#A4").uploadify({
            'debug': false, //开启调试
            'auto': true, //是否自动上传
            'buttonText': '',
            'buttonImage': '',
            'buttonClass': '',
            'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
            'queueID': 'uploadfileQueue', //文件选择后的容器ID
            'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
            'width': '74',
            'height': '74',
            'multi': false,
            'queueSizeLimit': 1,
            'uploadLimit': 0,
            'fileTypeDesc': '支持的格式：',
            'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
            'fileSizeLimit': '5MB',
            'removeTimeout': 0,
            'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
            'onSelect': function (file) {
                vmObj.ReportImgs.push(vmObj.loadingImage);
                //$("#aa").mask({ spinner: { lines: 10, length: 5, width: 1, radius: 10} });
            },
            //返回一个错误，选择文件的时候触发
            'onSelectError': function (file, errorCode, errorMsg) {
                switch (errorCode) {
                    case -100:
                        alert("上传的文件数量已经超出系统限制的" + vmObj.ReportImgs().length + "个文件！");
                        break;
                    case -110:
                        alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#A4').uploadify('settings', 'fileSizeLimit') + "大小！");
                        break;
                    case -120:
                        alert("文件 [" + file.name + "] 大小异常！");
                        break;
                    case -130:
                        alert("文件 [" + file.name + "] 类型不正确！");
                        break;
                }
            },
            //检测FLASH失败调用
            'onFallback': function () {
                alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
            },
            //上传到服务器，服务器返回相应信息到data里
            'onUploadSuccess': function (file, data, response) {
                vmObj.ReportImgs.splice($.inArray(vmObj.loadingImage, vmObj.ReportImgs), 1);
                var dataObj = JSON.parse(data);
                if (dataObj.code == 0) {
                    var single = {
                        fileUrl: dataObj.Msg,
                        fileUrls: dataObj.sMsg
                    }
                    vmObj.ReportImgs.push(single);
                }
            }
        });
        $('#txtReportTime').datepicker({
            autoclose: true,
            todayHighlight: true,
            language: 'cn'
        });
    }
    vmReportAdd.prototype.delReportImage = function (data, event) {
        var index = vmObj.ReportImgs.indexOf(data);
        vmObj.ReportImgs.splice(index, 1);
        if (vmObj.ReportImgs().length == 0) {
            vmObj.hasReportImgs(true);
        }
    }
    vmReportAdd.prototype.close = function () {
        dialog.close(this);
    }
    vmReportAdd.show = function (subId, reportId, rowdata) {
        vmObj = new vmReportAdd();
        vmObj.init(subId, reportId, rowdata);
        return dialog.show(vmObj);
    };
    return vmReportAdd;
});