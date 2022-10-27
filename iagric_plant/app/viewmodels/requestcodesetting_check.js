define(['plugins/router', 'knockout', 'jquery', 'kindeditor.zh-CN', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'jquery.querystring', 'bootbox', 'webuploader'],
function (router, ko, $, kcn, kv, utils, loginInfo, dialog, jqueryui, uploadify, qs, bootbox, webuploader) {
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
    var vmCheckAdd = function () {
        var self = this;
        self.subId = ko.observable(0);
        self.checkId = ko.observable(0);
        //操作人
        self.CheckUser = ko.observable().extend({
            required: {
                params: true,
                message: "请输入操作人员！"
            }
        });
        //操作时间
        self.CheckTime = ko.observable(complementHistoryDate()).extend({
            required: {
                params: true,
                message: "请输入操作时间！"
            }
        });
        //巡检内容
        self.CheckContent = ko.observable().extend({
            required: {
                params: true,
                message: "请输入巡检描述！"
            }
        });
        //巡检图片数组
        self.CheckImgs = ko.observableArray();
        //巡检视频数组
        self.CheckVideos = ko.observableArray();
        self.loadingImage = {
            fileUrl: '../../images/load.gif',
            fileUrls: '../../images/load.gif'
        };
        self.init = function (subId, checkId, rowData) {
            self.subId(subId);
            self.checkId(checkId);
            if (rowData != null) {
                self.CheckUser(rowData.UserName);
                self.CheckTime(rowData.StrAddDate.replace('年', '-').replace('月', '-').replace('日', ''));
                self.CheckContent(rowData.Content);
                self.CheckImgs(rowData.imgs);
                self.CheckVideos(rowData.videos);
            }
        }
        //添加巡检
        self.AddCheck = function () {
            var currentObj = $(event.target);
            currentObj.blur();
            try {
                if ($.inArray(vmObj.loadingImage, vmObj.CheckImgs()) >= 0) {
                    dialog.showMessage('请等待图片上传完成！', '系统提示', [{ title: '确定', callback: function () {
                        return;
                    }
                    }]);
                }
                else if ($.inArray(vmObj.loadingImage, vmObj.CheckVideos()) >= 0) {
                    dialog.showMessage('请等待视频上传完成！', '系统提示', [{ title: '确定', callback: function () {
                        return;
                    }
                    }]);
                }
                else {
                    var errors = ko.validation.group(this);
                    if (errors().length <= 0) {
                        var sendData;
                        var Action = "AddCheck";
                        if (self.checkId() == 0) {
                            var sendData = {
                                settingId: self.subId(),
                                userName: self.CheckUser(),
                                addDate: self.CheckTime(),
                                content: self.CheckContent(),
                                files: JSON.stringify(self.CheckImgs()),
                                video: JSON.stringify(self.CheckVideos())
                            }
                        }
                        else {
                            var sendData = {
                                id: self.checkId(),
                                userName: self.CheckUser(),
                                addDate: self.CheckTime(),
                                content: self.CheckContent(),
                                files: JSON.stringify(self.CheckImgs()).replace('[[', '[').replace(']]', ']'),
                                video: JSON.stringify(self.CheckVideos()).replace('[[', '[').replace(']]', ']')
                            }
                            Action = "EditCheck";
                        }
                        $.ajax({
                            type: "POST",
                            url: "/Admin_Batch_XunJian/" + Action,
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
                        errors.showAllMessages();
                    }
                }
            }
            catch (e) { }
        }
        self.cancle = function () {
            self.close();
        }
    }
    vmCheckAdd.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

        var A2_uploader = webuploader.create({

            // swf文件路径
            swf: '/lib/webuploader/Uploader.swf',
            duplicate: true,
            // 文件接收服务端。
            server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

            // 选择文件的按钮。可选。
            // 内部根据当前运行是创建，可能是input元素，也可能是flash.
            pick: { id: '#A2', multiple: false },
            auto: true,
            //            formData: { guid: guid },
            // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
            resize: false,
            //切片
            chunked: true,
            //每片的大小，C#接收文件也有默认大小，也可以自己在C#中修改
            chunkSize: 2 * 1024 * 1024,
            fileSingleSizeLimit: 5 * 1024 * 1024, // 限制在5M
            threads: 1,
            accept: {
                title: 'Images',
                extensions: 'gif,jpg,jpeg,bmp,png',
                mimeTypes: 'image/*'
            }
        });
        A2_uploader.on('uploadSuccess', function (file, data, response) {
            vmObj.CheckImgs.splice($.inArray(vmObj.loadingImage, vmObj.CheckImgs), 1);
            var dataObj = data; //JSON.parse(data);
            if (dataObj.code == 0) {
                var single = {
                    fileUrl: dataObj.Msg,
                    fileUrls: dataObj.sMsg
                }
                vmObj.CheckImgs.push(single);
            }
        });
        A2_uploader.on('uploadStart', function (file, data, response) {
            vmObj.CheckImgs.push(vmObj.loadingImage);
        });
        //        try {
        //            $("#A2").uploadify('destroy');
        //        }
        //        catch (Error) { }
        //        $("#A2").uploadify({
        //            'debug': false, //开启调试
        //            'auto': true, //是否自动上传
        //            'buttonText': '',
        //            'buttonImage': '',
        //            'buttonClass': '',
        //            'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
        //            'queueID': 'uploadfileQueue', //文件选择后的容器ID
        //            'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
        //            'width': '74',
        //            'height': '74',
        //            'multi': false,
        //            'queueSizeLimit': 1,
        //            'uploadLimit': 0,
        //            'fileTypeDesc': '支持的格式：',
        //            'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
        //            'fileSizeLimit': '5MB',
        //            'removeTimeout': 0,
        //            'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
        //            'onSelect': function (file) {
        //                vmObj.CheckImgs.push(vmObj.loadingImage);
        //                //$("#aa").mask({ spinner: { lines: 10, length: 5, width: 1, radius: 10} });
        //            },
        //            //返回一个错误，选择文件的时候触发
        //            'onSelectError': function (file, errorCode, errorMsg) {
        //                switch (errorCode) {
        //                    case -100:
        //                        alert("上传的文件数量已经超出系统限制的" + vmObj.CheckImgs().length + "个文件！");
        //                        break;
        //                    case -110:
        //                        alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#A2').uploadify('settings', 'fileSizeLimit') + "大小！");
        //                        break;
        //                    case -120:
        //                        alert("文件 [" + file.name + "] 大小异常！");
        //                        break;
        //                    case -130:
        //                        alert("文件 [" + file.name + "] 类型不正确！");
        //                        break;
        //                }
        //            },
        //            //检测FLASH失败调用
        //            'onFallback': function () {
        //                alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
        //            },
        //            //上传到服务器，服务器返回相应信息到data里
        //            'onUploadSuccess': function (file, data, response) {
        //                vmObj.CheckImgs.splice($.inArray(vmObj.loadingImage, vmObj.CheckImgs), 1);
        //                var dataObj = JSON.parse(data);
        //                if (dataObj.code == 0) {
        //                    var single = {
        //                        fileUrl: dataObj.Msg,
        //                        fileUrls: dataObj.sMsg
        //                    }
        //                    vmObj.CheckImgs.push(single);
        //                }
        //            }
        //        });
        var video_uploader = webuploader.create({

            // swf文件路径
            swf: '/lib/webuploader/Uploader.swf',

            // 文件接收服务端。
            server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

            // 选择文件的按钮。可选。
            // 内部根据当前运行是创建，可能是input元素，也可能是flash.
            pick: { id: '#A3', multiple: false },
            auto: true,
            //            formData: { guid: guid },
            // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
            resize: false,
            //切片
            chunked: true,
            //每片的大小，C#接收文件也有默认大小，也可以自己在C#中修改
            chunkSize: 2 * 1024 * 1024,
            fileSingleSizeLimit: 200 * 1024 * 1024, // 限制在5M
            threads: 1,
            accept: {
                title: 'Videos',
                extensions: 'mp4',
                mimeTypes: 'video/*'
            }
        });
        video_uploader.on('uploadSuccess', function (file, data, response) {
            vmObj.CheckVideos.splice($.inArray(vmObj.loadingImage, vmObj.CheckVideos), 1)
            var dataObj = data; //JSON.parse(data);
            if (dataObj.code == 0) {
                var single = {
                    videoUrl: dataObj.Msg,
                    videoUrls: dataObj.sMsg
                }
                setTimeout(function () {
                    vmObj.CheckVideos.push(single);
                }, 100);
            }

        });
        video_uploader.on('uploadStart', function (file, data, response) {
            vmObj.CheckVideos.push(vmObj.loadingImage);
            $("#Videos").attr("src", "/images/load.gif");
        });



        //        try {
        //            $("#A3").uploadify('destroy');
        //        }
        //        catch (Error) { }
        //        $("#A3").uploadify({
        //            'debug': false, //开启调试
        //            'auto': true, //是否自动上传
        //            'buttonText': '',
        //            'buttonImage': '',
        //            'buttonClass': '',
        //            'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
        //            'queueID': 'uploadfileQueue', //文件选择后的容器ID
        //            'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
        //            'width': '79',
        //            'height': '79',
        //            'multi': false,
        //            'queueSizeLimit': 1,
        //            'uploadLimit': 0,
        //            'fileTypeDesc': '支持的格式：',
        //            'fileTypeExts': '*.mp4',
        //            'fileSizeLimit': '200MB',
        //            'removeTimeout': 0,
        //            'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
        //            'onSelect': function (file) {
        //                vmObj.CheckVideos.push(vmObj.loadingImage);
        //                //preview(file);
        //            },
        //            //返回一个错误，选择文件的时候触发
        //            'onSelectError': function (file, errorCode, errorMsg) {
        //                switch (errorCode) {
        //                    case -100:
        //                        alert("上传的文件数量已经超出系统限制的" + vmObj.CheckVideos().length + "个文件！");
        //                        break;
        //                    case -110:
        //                        alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#A3').uploadify('settings', 'fileSizeLimit') + "大小！");
        //                        break;
        //                    case -120:
        //                        alert("文件 [" + file.name + "] 大小异常！");
        //                        break;
        //                    case -130:
        //                        alert("文件 [" + file.name + "] 类型不正确！");
        //                        break;
        //                }
        //            },
        //            'onUploadError': function (file, errorCode, errorMsg, errorString) {

        //            },
        //            //检测FLASH失败调用
        //            'onFallback': function () {
        //                alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
        //            },
        //            //上传到服务器，服务器返回相应信息到data里
        //            'onUploadSuccess': function (file, data, response) {
        //                vmObj.CheckVideos.splice($.inArray(vmObj.loadingImage, vmObj.CheckVideos), 1)
        //                var dataObj = JSON.parse(data);
        //                if (dataObj.code == 0) {
        //                    var single = {
        //                        videoUrl: dataObj.Msg,
        //                        videoUrls: dataObj.sMsg
        //                    }
        //                    setTimeout(function () {
        //                        vmObj.CheckVideos.push(single);
        //                    }, 100);
        //                }

        //            }
        //        });
        $('#txtCheckTime').datepicker({
            autoclose: true,
            todayHighlight: true,
            language: 'cn'
        });
    }
    vmCheckAdd.prototype.delCheckImage = function (data, event) {
        var index = vmObj.CheckImgs.indexOf(data);
        vmObj.CheckImgs.splice(index, 1);
    }
    vmCheckAdd.prototype.delCheckVideo = function (data, event) {
        var index = vmObj.CheckVideos.indexOf(data);
        vmObj.CheckVideos.splice(index, 1);
    }
    vmCheckAdd.prototype.close = function () {
        dialog.close(this);
    }
    vmCheckAdd.show = function (subId, checkId, rowdata) {
        vmObj = new vmCheckAdd();
        vmObj.init(subId, checkId, rowdata);
        return dialog.show(vmObj);
    };
    return vmCheckAdd;
});