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
    var vmOriginAdd = function () {
        var self = this;
        self.subId = ko.observable(0);
        self.originId = ko.observable(0);
        //原料图片数组
        self.files = ko.observableArray();
        self.hasfiles = ko.observable(false);
        self.loadingImage = {
            fileUrl: '../../images/load.gif',
            fileUrls: '../../images/load.gif'
        };
        //原料检测报告图片数组
        self.jcfiles = ko.observableArray();
        self.jchasfiles = ko.observable(false);
        self.jcloadingImage = {
            jcfileUrl: '../../images/load.gif',
            jcfileUrls: '../../images/load.gif'
        };
        //原料列表
        self.OriginList = {
            OriginArray: ko.observableArray(),
            SelectedId: ko.observable()
        }
        self.selOriginList = ko.observable(false);
        self.OriginList.SelectedId.subscribe(function () {
            self.selOriginList(self.OriginList.SelectedId() == undefined)
        });
        //获取原料
        self.GetOriginList = function () {
            var data;
            $.ajax({
                type: "POST",
                url: "/AdminOrigin/GetOriginList",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                async: false,
                success: function (jsonResult) {
                    data = jsonResult.ObjList;
                }
            });
            return data;
        }
        //来源
        self.Supplier = ko.observable().extend({
            required: {
                params: true,
                message: "请输入供货商！"
            }
        });
        //入库时间
        self.InDate = ko.observable(complementHistoryDate()).extend({
            required: {
                params: true,
                message: "请选择入库日期！"
            }
        });
        //运输车辆
        self.CarNum = ko.observable().extend({
            pattern: {
                params: /^[\u4e00-\u9fa5]{1}[A-Z]{1}[A-Z_0-9]{5}$/,
                message: "请输入正确的车牌号！"
            }
        });
        self.Driver = ko.observable('').extend({
            maxLength: { params: 25, message: "最多可输入25个字符！" }
        });
        self.Level = ko.observable();
        self.BatchNum = ko.observable();
        self.Factory = ko.observable();
        self.EarTag = ko.observable();

        //检验员
        self.checkUser = ko.observable().extend({
            required: {
                params: true,
                message: "请输入检验员！"
            }
        });
        self.init = function (subId, originId, rowData) {
            self.subId(subId);
            self.originId(originId);
            self.OriginList.OriginArray(self.GetOriginList())
            if (rowData != null) {
                self.OriginList.SelectedId(rowData.OriginID);
                self.Driver(rowData.Driver);
                self.CarNum(rowData.CarNum);
                self.Driver(rowData.Driver);
                self.checkUser(rowData.CheckUser);
                self.InDate(rowData.StrInDate.replace('年', '-').replace('月', '-').replace('日', ''));
                self.Supplier(rowData.Supplier);
                self.files.splice(0, self.files().length);
                self.files(rowData.imgs);
                self.jcfiles.splice(0, self.jcfiles().length);
                self.jcfiles(rowData.jcimgs);
                self.Level(rowData.Level);
                self.BatchNum(rowData.BatchNum);
                self.Factory(rowData.Factory);
                self.EarTag(rowData.EarTag);
            }
        }
        //添加原料
        self.AddOrigin = function () {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0 && self.OriginList.SelectedId() != undefined && self.files().length > 0 && self.jcfiles().length > 0) {
                self.selOriginList(false);
                self.hasfiles(false);
                self.jchasfiles(false);
                var sendData;
                var Action = "AddOrigin";
                if (self.originId() == 0) {
                    sendData = {
                        subId: self.subId(),
                        originId: self.OriginList.SelectedId(),
                        driver: $("#sj").val(),
                        carNum: $("#car").val(),
                        checkUser: self.checkUser(),
                        inDate: self.InDate(),
                        supplie: self.Supplier(),
                        img: JSON.stringify(self.files()),
                        jcimg: JSON.stringify(self.jcfiles()),
                        level: self.Level(),
                        batchNum: self.BatchNum(),
                        factory: self.Factory(),
                        earTag: self.EarTag()
                    };
                }
                else {
                    sendData = {
                        id: self.originId(),
                        subId: self.subId(),
                        originId: self.OriginList.SelectedId(),
                        driver: $("#sj").val(),
                        carNum: $("#car").val(),
                        checkUser: self.checkUser(),
                        inDate: self.InDate(),
                        supplie: self.Supplier(),
                        img: JSON.stringify(self.files()).replace('[[', '[').replace(']]', ']'),
                        jcimg: JSON.stringify(self.jcfiles()).replace('[[', '[').replace(']]', ']'),
                        level: self.Level(),
                        batchNum: self.BatchNum(),
                        factory: self.Factory(),
                        earTag: self.EarTag()
                    };
                    Action = "EditOrigin";
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_RequestCodeSetting/" + Action,
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
                if (self.OriginList.SelectedId() == undefined) {
                    self.selOriginList(true);
                }
                if (self.files().length <= 0) {
                    self.hasfiles(true);
                }
                if (self.jcfiles().length <= 0) {
                    self.jchasfiles(true);
                }
                $(".validationMessage").css("display", "");
                self.errors.showAllMessages();
            }
        }
        self.cancle = function () {
            self.close();
        }
    }
    vmOriginAdd.prototype.binding = function () {
        $("head").append("<script src='../../js/SearchInput.js'></script>");
        searchData();
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
        $('#txtInDate').datepicker({
            autoclose: true,
            todayHighlight: true,
            language: 'cn'
        });

        var A0_uploader = webuploader.create({

            // swf文件路径
            swf: '/lib/webuploader/Uploader.swf',
            duplicate: true,
            // 文件接收服务端。
            server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

            // 选择文件的按钮。可选。
            // 内部根据当前运行是创建，可能是input元素，也可能是flash.
            pick: { id: '#A0', multiple: false },
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
        A0_uploader.on('uploadSuccess', function (file, data, response) {
            vmObj.files.splice($.inArray(vmObj.loadingImage, vmObj.files), 1)
            var dataObj = data; //JSON.parse(data);
            if (dataObj.code == 0) {
                var single = {
                    fileUrl: dataObj.Msg,
                    fileUrls: dataObj.sMsg
                }
                vmObj.files.push(single);
                //                    vmObj.originImgInfo(false);
            }
        });
        A0_uploader.on('uploadStart', function (file, data, response) {
            vmObj.files.push(vmObj.loadingImage);
        });
        //        try {
        //            $('#A0').uploadify('destroy');
        //        } catch (Error) {
        //        }
        //        $("#A0").uploadify({
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
        //            //             'uploadLimit': 1,
        //            'fileTypeDesc': '支持的格式：',
        //            'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
        //            'fileSizeLimit': '5MB',
        //            'removeTimeout': 0,
        //            'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
        //            'onSelect': function (file) {
        //                vmObj.files.push(vmObj.loadingImage);
        //                //preview(file);
        //            },
        //            //返回一个错误，选择文件的时候触发
        //            'onSelectError': function (file, errorCode, errorMsg) {
        //                switch (errorCode) {
        //                    case -100:
        //                        alert("上传的文件数量已经超出系统限制的" + vmObj.files().length + "个文件！");
        //                        break;
        //                    case -110:
        //                        alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#A0').uploadify('settings', 'fileSizeLimit') + "大小！");
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
        //                vmObj.files.splice($.inArray(vmObj.loadingImage, vmObj.files), 1)
        //                var dataObj = JSON.parse(data);
        //                if (dataObj.code == 0) {
        //                    var single = {
        //                        fileUrl: dataObj.Msg,
        //                        fileUrls: dataObj.sMsg
        //                    }
        //                    vmObj.files.push(single);
        //                    //                    vmObj.originImgInfo(false);
        //                }
        //            }
        //        });


        var JCA0_uploader = webuploader.create({

            // swf文件路径
            swf: '/lib/webuploader/Uploader.swf',
            duplicate: true,
            // 文件接收服务端。
            server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

            // 选择文件的按钮。可选。
            // 内部根据当前运行是创建，可能是input元素，也可能是flash.
            pick: { id: '#JCA0', multiple: false },
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
        JCA0_uploader.on('uploadSuccess', function (file, data, response) {
            vmObj.jcfiles.splice($.inArray(vmObj.jcloadingImage, vmObj.jcfiles), 1)
            var dataObj = data; //JSON.parse(data);
            if (dataObj.code == 0) {
                var single = {
                    jcfileUrl: dataObj.Msg,
                    jcfileUrls: dataObj.sMsg
                }
                vmObj.jcfiles.push(single);
            }
        });
        JCA0_uploader.on('uploadStart', function (file, data, response) {
            vmObj.jcfiles.push(vmObj.jcloadingImage);
        });
        //        try {
        //            $('#JCA0').uploadify('destroy');
        //        } catch (Error) {
        //        }
        //        $("#JCA0").uploadify({
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
        //            //             'uploadLimit': 1,
        //            'fileTypeDesc': '支持的格式：',
        //            'fileTypeExts': '*.bmp;*.jpg;*.jpeg;*.png;*.gif',
        //            'fileSizeLimit': '5MB',
        //            'removeTimeout': 0,
        //            'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
        //            'onSelect': function (file) {
        //                vmObj.jcfiles.push(vmObj.jcloadingImage);
        //                //preview(file);
        //            },
        //            //返回一个错误，选择文件的时候触发
        //            'onSelectError': function (file, errorCode, errorMsg) {
        //                switch (errorCode) {
        //                    case -100:
        //                        alert("上传的文件数量已经超出系统限制的" + vmObj.files().length + "个文件！");
        //                        break;
        //                    case -110:
        //                        alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#A0').uploadify('settings', 'fileSizeLimit') + "大小！");
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
        //                vmObj.jcfiles.splice($.inArray(vmObj.jcloadingImage, vmObj.jcfiles), 1)
        //                var dataObj = JSON.parse(data);
        //                if (dataObj.code == 0) {
        //                    var single = {
        //                        jcfileUrl: dataObj.Msg,
        //                        jcfileUrls: dataObj.sMsg
        //                    }
        //                    vmObj.jcfiles.push(single);
        //                }
        //            }
        //        });
    }
    vmOriginAdd.prototype.delImage = function (data, event) {
        var index = vmObj.files.indexOf(data);
        vmObj.files.splice(index, 1);
        if (vmObj.files().length == 0) {
            vmObj.hasfiles(true);
        }
    }
    vmOriginAdd.prototype.deljcImage = function (data, event) {
        var jcindex = vmObj.jcfiles.indexOf(data);
        vmObj.jcfiles.splice(jcindex, 1);
        if (vmObj.jcfiles().length == 0) {
            vmObj.jchasfiles(true);
        }
    }
    vmOriginAdd.prototype.close = function () {
        dialog.close(this);
    }
    vmOriginAdd.show = function (subId, originId, rowdata) {
        vmObj = new vmOriginAdd();
        vmObj.init(subId, originId, rowdata);
        return dialog.show(vmObj);
    };
    return vmOriginAdd;
});