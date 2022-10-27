define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'kindeditor.zh-CN'],
    function (router, ko, $, kv, utils, loginInfo, dialog, jqueryui, uploadify, kcn) {
        var vmObj;
        var vmUploads = function (id) {
            var self = this;
            self.id = id;
            self.remark = ko.observable('').extend({
                maxLength: { params: 100, message: "最大长度为100个字符！" },
                required: {
                    params: true,
                    message: "上传说明不能为空！"
                }
            });
            self.selRemark = ko.observable(false);
            self.files = ko.observableArray([]);
            self.video = ko.observableArray([]);
            self.filePath = ko.observable('');
            self.videoPath = ko.observable('');
            self.ImgInfo = ko.observable('');
            self.selImgInfo = ko.observable(false);
            self.loadingImage = {
                fileUrl: '../../images/load.gif', //ko.observable(result[1])
                fileUrls: '../../images/load.gif'
            };
            self.loadingvideo = {
                videoUrl: '../../images/load.gif', //ko.observable(result[1])
                videoUrls: '../../images/load.gif'
            };
            self.init = function () {
                var sendData = {
                    id: self.id
                }
                $.ajax({
                    type: "POST",
                    url: "/Uploads/GetModel",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        self.remark(jsonResult.ObjModel.Remark);
                        self.files(jsonResult.ObjModel.imgs);
                        self.video(jsonResult.ObjModel.videos);
                        for (var i = 0; i < jsonResult.ObjModel.imgs.length; i++) {
                            self.filePath(self.filePath() + jsonResult.ObjModel.imgs[i].fileUrl + "&" + jsonResult.ObjModel.imgs[i].fileUrls + "<br>");
                        }
                        for (var i = 0; i < jsonResult.ObjModel.videos.length; i++) {
                            self.videoPath(self.videoPath() + jsonResult.ObjModel.videos[i].videoUrl + "&" + jsonResult.ObjModel.videos[i].videoUrls + "<br>");
                        }
                    }
                });
            };
            self.Save = function (data, event) {
                var currentObj = $(event.target);
                currentObj.blur();
                self.errors = ko.validation.group(self);
                if (self.errors().length <= 0 && !(self.files() == null || self.files() == "" || self.files() == "undefined")) {
                    var sendData = {
                        id: self.id,
                        remark: self.remark(),
                        //                            materialMemo: editorMaterial.html(),
                        imgInfo: JSON.stringify(self.files()),
                        video: JSON.stringify(self.video())
                    }
                    $.ajax({
                        type: "POST",
                        url: "/Uploads/Edit",
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
                        },
                        error: function (Error) {
                            //alert(1);
                        }
                    })
                } else {
                    if (self.files() == "") {
                        self.selImgInfo(true);
                    }
                    else {
                        self.selImgInfo(false);
                    }
                    self.errors.showAllMessages();
                }
            };
        };

        vmUploads.prototype.binding = function () {
            //    editorMaterial = KindEditor.create("#txtInfos", {
            //        cssPath: '/lib/kindeditor/plugins/code/prettify.css',
            //        uploadJson: '/lib/kindeditor/upload_json.ashx',
            //        fileManagerJson: '/lib/kindeditor/file_manager_json.ashx',
            //        allowFileManager: true,
            //        afterCreate: function () { },
            //        afterBlur: function () { this.sync(); }
            //    });
            //    editorMaterial.html(vmObj.materialMemo());
            $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

            $("#image_upload").uploadify({
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
                    vmObj.files.push(vmObj.loadingImage);
                    //alert(2);
                    //preview(file);
                },
                //返回一个错误，选择文件的时候触发
                'onSelectError': function (file, errorCode, errorMsg) {
                    switch (errorCode) {
                        case -100:
                            alert("上传的文件数量已经超出系统限制的" + $('#image_upload').uploadify('settings', 'queueSizeLimit') + "个文件！");
                            break;
                        case -110:
                            alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#image_upload').uploadify('settings', 'fileSizeLimit') + "大小！");
                            break;
                        case -120:
                            alert("文件 [" + file.name + "] 大小异常！");
                            break;
                        case -130:
                            alert("文件 [" + file.name + "] 类型不正确！");
                            break;
                    }
                },
                'onUploadError': function (file, errorCode, errorMsg, errorString) {
                    //alert(2);
                },
                //检测FLASH失败调用
                'onFallback': function () {
                    alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
                },
                //上传到服务器，服务器返回相应信息到data里
                'onUploadSuccess': function (file, data, response) {
                    vmObj.files.splice($.inArray(vmObj.loadingImage, vmObj.files), 1)
                    //alert(2);
                    var dataObj = JSON.parse(data);
                    if (dataObj.code == 0) {
                        var single = {
                            fileUrl: dataObj.Msg, //ko.observable(result[1])
                            fileUrls: dataObj.sMsg
                        }
                        vmObj.filePath(vmObj.filePath() + single.fileUrl + "&" + single.fileUrls + "<br>");
                        vmObj.files.push(single);
                        vmObj.selImgInfo(false);
                    }

                }
            });
            /*************************************/
            $("#video_upload").uploadify({
                'debug': false, //开启调试
                'auto': true, //是否自动上传
                'buttonText': '',
                'buttonImage': '',
                'buttonClass': '',
                'swf': '/lib/jquery.uploadify-v3.2/swf/uploadify.swf', //flash
                'queueID': 'uploadfileQueue', //文件选择后的容器ID
                'uploader': '/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx',
                'width': '79',
                'height': '79',
                'multi': false,
                'queueSizeLimit': 1,
                'uploadLimit': 0,
                'fileTypeDesc': '支持的格式：',
                'fileTypeExts': '*.avi;*.mp4',
                'fileSizeLimit': '200MB',
                'removeTimeout': 0,
                'overrideEvents': ['onSelectError', 'onDialogOpen', 'onDialogClose', 'onFallback', 'onUploadError'],
                'onSelect': function (file) {
                    vmObj.video.push(vmObj.loadingvideo);
                    //preview(file);
                },
                //返回一个错误，选择文件的时候触发
                'onSelectError': function (file, errorCode, errorMsg) {
                    switch (errorCode) {
                        case -100:
                            alert("正在上传文件，请稍后再试！");
                            break;
                        case -110:
                            alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#video_upload').uploadify('settings', 'fileSizeLimit') + "大小！");
                            break;
                        case -120:
                            alert("文件 [" + file.name + "] 大小异常！");
                            break;
                        case -130:
                            alert("文件 [" + file.name + "] 类型不正确！");
                            break;
                    }
                },
                'onUploadError': function (file, errorCode, errorMsg, errorString) {

                },
                //检测FLASH失败调用
                'onFallback': function () {
                    alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
                },
                //上传到服务器，服务器返回相应信息到data里
                'onUploadSuccess': function (file, data, response) {
                    vmObj.video.splice($.inArray(vmObj.loadingvideo, vmObj.video), 1)
                    var dataObj = JSON.parse(data);
                    if (dataObj.code == 0) {
                        //alert(dataObj.sMsg)
                        var single = {
                            videoUrl: dataObj.Msg,
                            videoUrls: dataObj.sMsg
                        }
                        //vmObj.video(new Array());
                        vmObj.videoPath(vmObj.videoPath() + single.videoUrl + "&" + single.videoUrls + "<br>");
                        vmObj.video.push(single);
                    }
                }
            });
            /*************************************/
        }
        vmUploads.prototype.delImage = function (data, event) {
            var index = vmObj.files.indexOf(data);
            vmObj.files.splice(index, 1);
            var temp = vmObj.filePath();
            temp = temp.replace(data.fileUrls + "<br>", '');
            vmObj.filePath(temp);
            if (vmObj.files().length == 0) {
                vmObj.selImgInfo(true);
            }
        }
        vmUploads.prototype.delVideo = function (data, event) {
            var index = vmObj.video.indexOf(data);
            var temp = vmObj.videoPath();
            temp = temp.replace(data.videoUrl + "<br>", '');
            vmObj.videoPath(temp);
            vmObj.video.splice(index, 1);
        }
        vmUploads.prototype.AddProperty = function (data, event) {
            if (!vmObj.propertyName()) {
                vmObj.selPrototype(true);
                return;
            }
            if (!vmObj.propertyValue()) {
                vmObj.selPrototype(true);
                return;
            }
            vmObj.selPrototype(false);
            var all = vmObj.propertyName() + "：" + vmObj.propertyValue();
            var single = {
                pName: vmObj.propertyName(),
                pValue: vmObj.propertyValue(),
                allprototype: all
            }
            vmObj.property.push(single);
            vmObj.propertyName('');
            vmObj.propertyValue('');

        }
        vmUploads.prototype.delProperty = function (data, event) {
            var index = vmObj.property.indexOf(data);
            vmObj.property.splice(index, 1);
        }
        vmUploads.prototype.close = function () {
            $("#image_upload").uploadify('destroy');
            dialog.close(this);
        }
        vmUploads.prototype.closeOK = function (id) {
            dialog.close(this, id);
        }
        vmUploads.show = function (id) {
            vmObj = new vmUploads(id);
            vmObj.init();
            return dialog.show(vmObj);
        };
        return vmUploads;
    });