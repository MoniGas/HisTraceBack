define(['plugins/router', 'knockout', 'jquery', 'kindeditor.zh-CN', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'jquery.querystring', 'bootbox', 'webuploader'],
function (router, ko, $, kcn, kv, utils, loginInfo, dialog, jqueryui, uploadify, qs, bootbox, webuploader) {
    var vmObj;
    var vmOriginAdd = function () {
        var self = this;
        self.OriginName = ko.observable('').extend({
            maxLength: { params: 50, message: "名称最大长度为50个字符" },
            required: {
                params: true,
                message: "请输入原材料名称！"
            }
        });
        self.selTitle = ko.observable(false);
        //        self.Descriptions = ko.observable('').extend({
        //            maxLength: { params: 1000, message: "描述最大长度为1000个字符" }
        //        });
        self.Descriptions = ko.observable('');
        self.files = ko.observableArray();
        self.originImgInfo = ko.observable(false);
        self.cancle = function () {
            self.close();
        }
        self.loadingImage = {
            fileUrl: '../../images/load.gif', //ko.observable(result[1])
            fileUrls: '../../images/load.gif'
        };
        self.Register = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            self.errors = ko.validation.group(self);
            if (self.errors().length <= 0 && !(self.files() == null || self.files() == "" || self.files() == "undefined")) {
                var sendData = {
                    OriginName: self.OriginName(),
                    Descriptions: editorOrigin.html(),
                    //                    Descriptions: self.Descriptions(),
                    //                    logo: JSON.stringify(self.files())
                    //                    originImgInfo: self.files()[0].fileUrl
                    originImgInfo: JSON.stringify(self.files())
                }
                $.ajax({
                    type: "POST",
                    url: "/AdminOrigin/Add",
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
                                self.closeOK(jsonResult.ObjModel);
                            }
                        }
                        }]);
                    }
                })
            }
            else {
                if (self.files() == null || self.files() == "" || self.files() == "undefined") {
                    self.originImgInfo(true);
                }
                else {
                    self.originImgInfo(false);
                }
                self.errors.showAllMessages();
            }
        }
    }
    vmOriginAdd.prototype.binding = function () {
        editorOrigin = KindEditor.create("#txtInfos", {
            cssPath: '/lib/kindeditor/plugins/code/prettify.css',
            uploadJson: '/lib/kindeditor/upload_json.ashx',
            fileManagerJson: '/lib/kindeditor/file_manager_json.ashx',
            allowFileManager: true,
            items: [
						'fontname', 'fontsize', '|', 'forecolor', 'bold', 'italic', 'underline',
						'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
						'insertunorderedlist', '|', 'image'],
            afterCreate: function () { },
            afterBlur: function () { this.sync(); }
        });
        editorOrigin.html(vmObj.Descriptions());
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
        //        try {
        //            $('#image_upload').uploadify('destroy');
        //        } catch (Error) {
        //        }
        //        $("#image_upload").uploadify({
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
        //                        alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#image_upload').uploadify('settings', 'fileSizeLimit') + "大小！");
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
        //                        fileUrl: dataObj.Msg, //ko.observable(result[1])
        //                        fileUrls: dataObj.sMsg
        //                    }
        //                    //                    vmObj.files(new Array());
        //                    vmObj.files.push(single);
        //                    vmObj.originImgInfo(false);
        //                }

        //            }
        //        });
        //var guid = webuploader.Base.guid(); //当前页面是生成的GUID作为标示
        var uploader = webuploader.create({

            // swf文件路径
            swf: '/lib/webuploader/Uploader.swf',
            //允许重复上传同一张文件
            duplicate: true,
            // 文件接收服务端。
            server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",
            //允许重复上传同一张文件
            duplicate: true,

            // 选择文件的按钮。可选。
            // 内部根据当前运行是创建，可能是input元素，也可能是flash.
            pick: { id: '#image_upload', multiple: true }, // pick: '#image_upload',
            auto: true,
            //            formData: { guid: guid },
            // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
            resize: false,
            //切片
            chunked: true,
            //每片的大小，C#接收文件也有默认大小，也可以自己在C#中修改
            chunkSize: 2 * 1024 * 1024,
            threads: 1,
            accept: {
                title: 'Images',
                extensions: 'gif,jpg,jpeg,bmp,png',
                mimeTypes: 'image/*'
            }
        });
        uploader.on('uploadSuccess', function (file, data, response) {
            // $('#' + file.id).find('p.state').text('已上传');
            vmObj.files.splice($.inArray(vmObj.loadingImage, vmObj.files), 1);
            console.log(data);
            var dataObj = data; //JSON.parse(data);
            if (dataObj.code == 0) {
                var single = {
                    fileUrl: dataObj.Msg, //ko.observable(result[1])
                    fileUrls: dataObj.sMsg
                }
                //                    vmObj.files(new Array());
                vmObj.files.push(single);
                vmObj.originImgInfo(false);
            }
        });

        uploader.on('uploadError', function (file, errorCode, errorMsg) {
            //$('#' + file.id).find('p.state').text('上传出错');
            switch (errorCode) {
                case -100:
                    alert("上传的文件数量已经超出系统限制的" + vmObj.files().length + "个文件！");
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
        });

        //        uploader.on('uploadComplete', function (file) {
        //            $('#' + file.id).find('.progress').fadeOut();
        //        });
    }
    vmOriginAdd.prototype.delImage = function (data, event) {
        //        var index = vmObj.files.indexOf(data);
        //        vmObj.files.splice(index, 1);
        //        vmObj.files(new Array());
        //        vmObj.originImgInfo(true);

        var index = vmObj.files.indexOf(data);
        vmObj.files.splice(index, 1);
        if (vmObj.files().length == 0) {
            vmObj.originImgInfo(true);
        }
    }
    vmOriginAdd.prototype.close = function (a) {
        //alert(this.province().code);
        dialog.close(this, a);
    }
    vmOriginAdd.prototype.closeOK = function (id) {
        dialog.close(this, id);
    }
    vmOriginAdd.show = function () {
        vmObj = new vmOriginAdd();
        return dialog.show(vmObj);
    };
    return vmOriginAdd;

});