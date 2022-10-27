define(['plugins/router', 'knockout', 'jquery', 'kindeditor.zh-CN', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.uploadify', 'jquery.querystring', 'bootbox', 'webuploader'],
function (router, ko, $, kcn, kv, utils, loginInfo, dialog, jqueryui, uploadify, qs, bootbox, webuploader) {
    var array = new Array();
    var checkedLength = 0;
    var vmObj;
    var cked = 0;
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
    //获取班组信息
    var GetTeamList = function () {
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_RequestCodeSetting/TeamList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                    return;
                };
                data = jsonResult.ObjList;
            },
            error: function (e) {
                //                alert(JSON.stringify(e));
            }
        });
        return data;
    };
    //获取班组人员信息
    var GetTeamUserList = function (teamID) {
        var sendData = {
            teamid: teamID
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_RequestCodeSetting/GetPersonList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                data = jsonResult.ObjList;
                cked = jsonResult.ObjList.length;
            },
            error: function (e) {
                //                alert(JSON.stringify(e));
            }
        })
        return data;
    }
    //自定义绑定-复选框级联选择
    ko.bindingHandlers.checkBoxCascade = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            $(element).on('click', "input[name='cbx']:checkbox", function (e) {
                var status = true;
                $(element).find("input[name='cbx']:checkbox").each(function () {
                    if (this.checked == false) {
                        status = false;
                        return false;
                    }
                });
                $(element).find("input[eleflag='allSelectBtn']").prop('checked', status);
            });
            $(element).on('click', "input[eleflag='allSelectBtn']", function (e) {
                var obj = $(e.target);
                $(element).find("input[name='cbx']:checkbox").each(function () {
                    $(this).prop('checked', obj.prop("checked"));
                });
            });
        },
        update: function (element, valueAccessor, allBindingsAccessor) {

        }
    };
    //获取员工字符串
    var getUserList = function () {
        var arr = new Array();
        $("#table1").find("input[name='cbx']:checkbox").each(function () {
            if (this.checked == true) {
                var UserID = $(this).val();
                if ($.inArray(UserID, arr) == -1) {
                    arr.push(UserID);
                }
            }
        });
        return arr.join(",");
    }

    var vmWorkAdd = function () {
        var self = this;
        self.subId = ko.observable(0);
        self.workId = ko.observable(0);
        //作业类型
        self.Worktype = ko.observable();
        //作业
        self.WorkList = {
            WorkArray: ko.observableArray(),
            SelectedId: ko.observable()
        }
        self.selWorkList = ko.observable(false);
        self.WorkList.SelectedId.subscribe(function () {
            self.selWorkList(self.WorkList.SelectedId() == undefined)
        });
        self.Worktype.subscribe(function () {
            var defaultItem = { OperationTypeName: '暂无相应生产环节', Batch_ZuoYeType_ID: '-1' };
            if (!self.Worktype()) {
                self.WorkList.WorkArray(defaultItem);
                return;
            }
            var sendData = {
                //selecttype: self.Worktype()
            };
            $.ajax({
                type: "POST",
                url: "/Admin_Batch_ZuoYe/OpTypeListAll",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                async: false,
                data: JSON.stringify(sendData),
                success: function (jsonResult) {
                    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                        return;
                    };
                    self.WorkList.WorkArray(jsonResult.ObjList);
                }
            });
        });
        //操作人
        //        self.workUser = ko.observable().extend({
        //            required: {
        //                params: true,
        //                message: "请输入操作人员！"
        //            }
        //        });
        //操作时间
        self.WorkTime = ko.observable(complementHistoryDate()).extend({
            required: {
                params: true,
                message: "请输入操作时间！"
            }
        });
        //作业内容
        self.WorkContent = ko.observable();
        //作业图片数组
        self.WorkImgs = ko.observableArray();
        //作业视频数组
        self.WorkVideos = ko.observableArray();
        self.loadingImage = {
            fileUrl: '../../images/load.gif',
            fileUrls: '../../images/load.gif'
        };
        /*****************************/
        //班组
        self.TeamArray = {
            TeamArray: ko.observableArray(),
            SelectedId: ko.observable()
        }
        //选择班组提示信息
        self.SelTeam = ko.observable(false);
        //班组更改
        self.TeamArray.SelectedId.subscribe(function () {
            self.TeamUserArray(GetTeamUserList(self.TeamArray.SelectedId()));
        });
        //班组人员
        self.TeamUserArray = ko.observableArray();
        self.SelPerson = ko.observable(false);

        //选中人员
        self.nowProcessID = 0;
        //        self.isPersonChecked = function (userId) {
        //            var userId = userId.toString();
        //            for (var i = 0; i < self.WorkList.WorkArray().length; i++)
        //                if (self.WorkList.WorkArray()[i].ProcessID == self.nowProcessID)
        //                    if (self.WorkList.WorkArray()[i].TeamUsers != null)
        //                        for (var j = 0; j < self.WorkList.WorkArray()[i].TeamUsers.length; j++)
        //                            if (self.WorkList.WorkArray()[i].TeamUsers[j].pValue == userId)
        //                                return true;
        //            return false;
        //        }
        self.flag = ko.observable(false);
        self.isChecked = function (userId) {
            var userId = userId.toString();
            if ($.inArray(userId, array) == -1) {
                return false;
            } else {
                return true;
            }
        }

        /*****************************/
        self.init = function (subId, workId, rowData) {
            self.subId(subId);
            self.workId(workId);
            if (rowData != null) {
                self.Worktype(rowData.type);
                self.WorkList.SelectedId(rowData.zuoye_typeId);
                //                self.workUser(rowData.UserName);
                self.WorkTime(rowData.StrAddDate.replace('年', '-').replace('月', '-').replace('日', ''));
                self.WorkContent(rowData.Content);
                self.TeamArray.SelectedId(rowData.TeamID);
                if (rowData.UsersName != null && rowData.UsersName != "" && rowData.UsersName != undefined) {
                    array = rowData.UsersName.split(',');
                    checkedLength = array.length;
                    self.flag = ko.observable(cked == checkedLength ? true : false);
                }
                else {
                    self.flag = ko.observable(false);
                }
                self.WorkImgs(rowData.imgs);
                self.WorkVideos(rowData.videos);
            }
        }
        //添加作业
        self.AddWork = function () {
            var currentObj = $(event.target);
            currentObj.blur();
            try {
                if ($.inArray(vmObj.loadingImage, vmObj.WorkImgs()) >= 0) {
                    dialog.showMessage('请等待图片上传完成！', '系统提示', [{ title: '确定', callback: function () {
                        return;
                    }
                    }]);
                }
                else if ($.inArray(vmObj.loadingImage, vmObj.WorkVideos()) >= 0) {
                    dialog.showMessage('请等待视频上传完成！', '系统提示', [{ title: '确定', callback: function () {
                        return;
                    }
                    }]);
                }
                else {
                    self.errors = ko.validation.group(self);
                    if (self.errors().length <= 0 && self.WorkList.SelectedId() != undefined && self.TeamArray.SelectedId() != undefined && getUserList().length > 0) {
                        self.selWorkList(false);
                        self.SelTeam(false);
                        self.SelPerson(false);
                        var sendData;
                        var Action = "AddWork";
                        if (self.workId() == 0) {
                            var batch_ZuoYeType = 0;
                            if (self.WorkList.SelectedId()) {
                                batch_ZuoYeType = self.WorkList.SelectedId();
                            }
                            var sendData = {
                                settingid: self.subId(),
                                //type: self.Worktype(),
                                batch_ZuoYeType_ID: batch_ZuoYeType,
                                //                                userName: self.workUser(),
                                addDate: self.WorkTime(),
                                content: self.WorkContent(),
                                teamID: self.TeamArray.SelectedId(),
                                usersArray: getUserList(),
                                files: JSON.stringify(self.WorkImgs()),
                                video: JSON.stringify(self.WorkVideos())
                            }
                        }
                        else {
                            var batch_ZuoYeType = 0;
                            if (self.WorkList.SelectedId()) {
                                batch_ZuoYeType = self.WorkList.SelectedId();
                            }
                            var sendData = {
                                id: self.workId(),
                                //type: self.Worktype(),
                                batch_ZuoYeType_ID: batch_ZuoYeType,
                                //                                userName: self.workUser(),
                                addDate: self.WorkTime(),
                                content: self.WorkContent(),
                                teamID: self.TeamArray.SelectedId(),
                                usersArray: getUserList(),
                                files: JSON.stringify(self.WorkImgs()).replace('[[', '[').replace(']]', ']'),
                                video: JSON.stringify(self.WorkVideos()).replace('[[', '[').replace(']]', ']')
                            }
                            Action = "EditWork";
                        }
                        $.ajax({
                            type: "POST",
                            url: "/Admin_Batch_ZuoYe/" + Action,
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
                                    array = [];
                                }
                            },
                            error: function (e) {
                            }
                        });
                    }
                    else {
                        if (self.WorkList.SelectedId() == undefined) {
                            self.selWorkList(true);
                        }
                        if (!self.TeamArray.SelectedId()) {
                            self.SelTeam(true);
                        }
                        if (getUserList().length <= 0) {
                            self.SelPerson(true);
                        }
                        $(".validationMessage").css("display", "");
                        self.errors.showAllMessages();
                    }
                }
            }
            catch (e) { }
        }
        self.cancle = function () {
            self.close();
            array = [];
        }
    }
    vmWorkAdd.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });

        var uploader = webuploader.create({

            // swf文件路径
            swf: '/lib/webuploader/Uploader.swf',

            // 文件接收服务端。
            server: "/lib/jquery.uploadify-v3.2/handler/UploadFile.ashx",

            // 选择文件的按钮。可选。
            // 内部根据当前运行是创建，可能是input元素，也可能是flash.
            pick: { id: '#A1', multiple: false },
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
        uploader.on('uploadSuccess', function (file, data, response) {
            vmObj.WorkImgs.splice($.inArray(vmObj.loadingImage, vmObj.WorkImgs), 1);
            var dataObj = data; //JSON.parse(data);
            if (dataObj.code == 0) {
                var single = {
                    fileUrl: dataObj.Msg,
                    fileUrls: dataObj.sMsg
                }
                vmObj.WorkImgs.push(single);
            }
        });
        uploader.on('uploadStart', function (file, data, response) {
            vmObj.WorkImgs.push(vmObj.loadingImage);
        });
        //        try {
        //            $("#A1").uploadify('destroy');
        //        }
        //        catch (Error) { }
        //        $("#A1").uploadify({
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
        //                vmObj.WorkImgs.push(vmObj.loadingImage);
        //            },
        //            //返回一个错误，选择文件的时候触发
        //            'onSelectError': function (file, errorCode, errorMsg) {
        //                switch (errorCode) {
        //                    case -100:
        //                        alert("上传的文件数量已经超出系统限制的" + vmObj.WorkImgs().length + "个文件！");
        //                        break;
        //                    case -110:
        //                        alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#A1').uploadify('settings', 'fileSizeLimit') + "大小！");
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
        //                vmObj.WorkImgs.splice($.inArray(vmObj.loadingImage, vmObj.WorkImgs), 1);
        //                var dataObj = JSON.parse(data);
        //                if (dataObj.code == 0) {
        //                    var single = {
        //                        fileUrl: dataObj.Msg,
        //                        fileUrls: dataObj.sMsg
        //                    }
        //                    vmObj.WorkImgs.push(single);
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
            pick: { id: '#video_upload', multiple: false },
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
            vmObj.WorkVideos.splice($.inArray(vmObj.loadingImage, vmObj.WorkVideos), 1)
            var dataObj = data; //JSON.parse(data);
            if (dataObj.code == 0) {
                var single = {
                    videoUrl: dataObj.Msg,
                    videoUrls: dataObj.sMsg
                }
                setTimeout(function () {
                    vmObj.WorkVideos.push(single);
                }, 100);
            }

        });
        video_uploader.on('uploadStart', function (file, data, response) {
            vmObj.WorkVideos.push(vmObj.loadingImage);
            $("#Videos").attr("src", "/images/load.gif");
        });
        //        try {
        //            $("#video_upload").uploadify('destroy');
        //        }
        //        catch (Error) { }
        //        $("#video_upload").uploadify({
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
        //                vmObj.WorkVideos.push(vmObj.loadingImage);
        //            },
        //            //返回一个错误，选择文件的时候触发
        //            'onSelectError': function (file, errorCode, errorMsg) {
        //                switch (errorCode) {
        //                    case -100:
        //                        alert("上传的文件数量已经超出系统限制的" + vmObj.WorkVideos().length + "个文件！");
        //                        break;
        //                    case -110:
        //                        alert("文件 [" + file.name + "] 大小超出系统限制的" + $('#video_upload').uploadify('settings', 'fileSizeLimit') + "大小！");
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
        //                vmObj.WorkVideos.splice($.inArray(vmObj.loadingImage, vmObj.WorkVideos), 1)
        //                var dataObj = JSON.parse(data);
        //                if (dataObj.code == 0) {
        //                    var single = {
        //                        videoUrl: dataObj.Msg,
        //                        videoUrls: dataObj.sMsg
        //                    }
        //                    setTimeout(function () {
        //                        vmObj.WorkVideos.push(single);
        //                    }, 100);
        //                }

        //            }
        //        });
        $('#txtWorkTime').datepicker({
            autoclose: true,
            todayHighlight: true,
            language: 'cn'
        });
    }
    vmWorkAdd.prototype.delWorkImage = function (data, event) {
        var index = vmObj.WorkImgs.indexOf(data);
        vmObj.WorkImgs.splice(index, 1);
    }
    vmWorkAdd.prototype.delWorkVideo = function (data, event) {
        var index = vmObj.WorkVideos.indexOf(data);
        vmObj.WorkVideos.splice(index, 1);
    }
    vmWorkAdd.prototype.close = function () {
        dialog.close(this);
    }
    vmWorkAdd.show = function (subId, workId, rowdata) {
        array = [];
        vmObj = new vmWorkAdd();
        vmObj.init(subId, workId, rowdata);
        vmObj.TeamArray.TeamArray = GetTeamList();
        //        allChecked();
        return dialog.show(vmObj);
    };
    return vmWorkAdd;
});