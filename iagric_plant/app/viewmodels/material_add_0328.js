define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'kindeditor.zh-CN'],
    function (router, ko, $, kv, utils, loginInfo, dialog, jqueryui, uploadify, kcn) {
        var vmObj;
        var vm = function (id) {
            var self = this;

            self.materialPrice = ko.observable('').extend({
                min: {
                    params: 0.001,
                    message: "输入价格必须大于0！"
                },
                max: {
                    params: 1000000,
                    message: "输入价格必须小于100万！"
                },
                pattern: {
                    params: /^\d+[\.]?\d{0,2}$/g,
                    message: "必须是数字，并且最多两位小数！"
                }
            });

            self.selMaterialPrice = ko.observable(false);

            self.materialName = ko.observable('').extend({
                maxLength: { params: 50, message: "名称最大长度为50个字符" },
                required: {
                    params: true,
                    message: "请输入产品名称!"
                }
            });
            self.selMaterialName = ko.observable(false);

            self.materialBrands = [];
            self.selectedBrand = ko.observable(id);
            self.processArray = [];
            self.selectedprocess = ko.observable();
            self.files = ko.observableArray();
            self.property = ko.observableArray();
            self.propertyName = ko.observable();
            self.propertyValue = ko.observable();
            self.selPrototype = ko.observable(false);

            self.materialBrand = ko.observable('');
            //self.materialType = ko.observable('');

            self.materialSpec = ko.observable('').extend({
                maxLength: { params: 25, message: "产品规格最大长度为25个字符" }
            });
            self.selMaterialSpec = ko.observable(false);

            self.selectedShelfs = [
                { "shefsid": 0, "shefsname": "长期" },
                { "shefsid": -1, "shefsname": "视存储环境" },
                { "shefsid": 1, "shefsname": "天" },
                { "shefsid": 2, "shefsname": "月" },
                { "shefsid": 3, "shefsname": "年" }
            ];
            self.selectedShelf = ko.observable();
            self.materialShelfLife = ko.observable('');
            self.selMaterialShelfLife = ko.observable(false);
            self.materialMemo = ko.observable('');
            //            self.materialMemo = ko.observable('').extend({
            //                maxLength: { params: 2000, message: "产品描述最大长度为2000个字符" },
            //                required: {
            //                    params: true,
            //                    message: "请输入产品描述!"
            //                }
            //            });
            self.selMaterialMemo = ko.observable(false);

            self.materialMaterialImgInfo = ko.observable('');
            self.selMaterialImgInfo = ko.observable(false);


            self.materialPropertyInfo = ko.observable('');

            self.materialShelfLife.subscribe(function () {
                if (self.materialShelfLife() == "") {
                    self.selMaterialShelfLife(true);
                }
                else {
                    self.selMaterialShelfLife(false);
                }

                var reg = new RegExp("^[0-9]*$");
                if (!reg.test(self.materialShelfLife())) {
                    self.selMaterialShelfLife(true);
                }
                else {
                    self.selMaterialShelfLife(false);
                }
            });
            self.loadingImage = {
                fileUrl: '../../images/load.gif', //ko.observable(result[1])
                fileUrls: '../../images/load.gif'
            };
            self.AddMaterial = function (data, event) {
                var currentObj = $(event.target);
                currentObj.blur();
                self.errors = ko.validation.group(self);
                var a = self.materialShelfLife()
                var b = self.selectedShelf();
                if (self.errors().length <= 0) {
                    if (!(b == "长期" || b == "视存储环境") && a == "") {
                        self.selMaterialShelfLife(true);
                    }
                    else {
                        self.selMaterialShelfLife(false);
                    }
                    if (self.files() == "") {
                        dialog.showMessage('至少上传一张图片', '系统提示', [{ title: '确定', callback: function () { } }]);
                        self.selMaterialImgInfo(true);
                    }
                    else {
                        self.selMaterialImgInfo(false);
                    }
                    if (!((!(b == "长期" || b == "视存储环境") && a == "") || (self.files() == ""))) {
                        var brand = self.selectedBrand();
                        if (!self.selectedBrand()) {
                            brand = 0;
                        }
                        var process = 0;
                        if (self.selectedprocess()) {
                            process = self.selectedprocess();
                        }
                        var sendData = {
                            materialName: self.materialName(),
                            materialBrand: brand,
                            processId: process,
                            materialSpec: self.materialSpec(),
                            materialShelfLife: (a + b),
                            materialPropertyInfo: JSON.stringify(self.property()),
                            materialMemo: editorMaterial.html(),
                            materialMaterialImgInfo: JSON.stringify(self.files()),
                            materialPrice: self.materialPrice()
                        }
                        $.ajax({
                            type: "POST",
                            url: "/Admin_Material/Add",
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
                                        self.closeOK(jsonResult.ObjModel, brand);
                                    }
                                }
                                }]);
                            }
                        })
                    }
                } else {
                    if (!(b == "长期" || b == "视存储环境") && a == "") {
                        self.selMaterialShelfLife(true)
                    } else {
                        self.selMaterialShelfLife(false);
                    }
                    if (self.files() == "") {
                        self.selMaterialImgInfo(true);
                        dialog.showMessage('至少上传一张图片', '系统提示', [{ title: '确定', callback: function () { } }]);
                    }
                    else {
                        self.selMaterialImgInfo(false);
                    }
                    self.errors.showAllMessages();
                }
            };
        };
        //获取品牌
        var getBrandModules = function () {
            var data;
            var sendData = {
                pageIndex: 1
            }
            $.ajax({
                type: "POST",
                url: "/Admin_Brand/SelectBrand",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    data = jsonResult.ObjList;
                }
            });
            return data;
        }
        //获取生产流程
        var getProcessModules = function () {
            var sendData = {
            };
            var data;
            $.ajax({
                type: "POST",
                url: "/Admin_Material/ProcessList",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                        return;
                    };
                    data = jsonResult.ObjList;
                }
            });
            return data;
        }

        vm.prototype.binding = function () {
            editorMaterial = KindEditor.create("#txtInfos", {
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
            editorMaterial.html(vmObj.materialMemo());
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
                    //if (!$("#aa").isMasked()) {
                    //alert('dfdf');
                    //alert($("#aa").attr("id"));
                    $("#aa").mask({ spinner: { lines: 10, length: 5, width: 1, radius: 10} });
                    //}
                },
                //返回一个错误，选择文件的时候触发
                'onSelectError': function (file, errorCode, errorMsg) {
                    switch (errorCode) {
                        case -100:-
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
                },
                'onUploadError': function (file, errorCode, errorMsg, errorString) {
                    //vmObj.upcss = ko.observable('images/load.gif');
                },
                //检测FLASH失败调用
                'onFallback': function () {
                    alert("您未安装FLASH控件，无法上传图片！请安装FLASH控件后再试。");
                },
                //上传到服务器，服务器返回相应信息到data里
                'onUploadSuccess': function (file, data, response) {
                    vmObj.files.splice($.inArray(vmObj.loadingImage, vmObj.files), 1)
                    //vmObj.upcss = ko.observable('images/load.gif');
                    var dataObj = JSON.parse(data);
                    if (dataObj.code == 0) {
                        var single = {
                            fileUrl: dataObj.Msg, //ko.observable(result[1])
                            fileUrls: dataObj.sMsg
                        }
                        vmObj.files.push(single);
                        vmObj.selMaterialImgInfo(false);
                    }
                }
            });

        }
        vm.prototype.delImage = function (data, event) {
            var index = vmObj.files.indexOf(data);
            vmObj.files.splice(index, 1);
            if (vmObj.files().length == 0) {
                vmObj.selMaterialImgInfo(true);
            }
        }
        vm.prototype.AddProperty = function (data, event) {
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
        vm.prototype.delProperty = function (data, event) {
            var index = vmObj.property.indexOf(data);
            vmObj.property.splice(index, 1);
        }
        vm.prototype.close = function () {
            $("#image_upload").uploadify('destroy');
            dialog.close(this);
        }
        vm.prototype.closeOK = function (id, brandId) {
            dialog.close(this, id, brandId);
        }
        vm.show = function (id) {
            vmObj = new vm(id);
            vmObj.materialBrands = getBrandModules();
            vmObj.processArray = getProcessModules();
            //$("#slBrandList").chosen({ allow_single_deselect: true });
            return dialog.show(vmObj);
        };
        return vm;
    });