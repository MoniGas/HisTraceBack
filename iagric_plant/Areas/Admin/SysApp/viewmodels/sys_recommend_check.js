define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo'], function (dialog, ko, jqueryui, km, utils, loginInfo) {
    var moduleInfo = {
        moduleID: '11151',
        parentModuleID: '11150'
    }

    //获取权限字符串,例如：1,2,3
    var getTrueList = function () {
        var arr = new Array();
        $("#table1").find("input[name='cbx']:checkbox").each(function () {
            if (this.checked == true) {
                var parentItemID = $(this).val();
                if ($.inArray(parentItemID, arr) == -1) {
                    arr.push(parentItemID);
                }
            }
        });
        return arr.join(",");
    }
    var getFalseList = function () {
        var arr = new Array();
        $("#table1").find("input[name='cbx']:checkbox").each(function () {
            if (this.checked == false) {
                var parentItemID = $(this).val();
                if ($.inArray(parentItemID, arr) == -1) {
                    arr.push(parentItemID);
                }
            }
        });
        return arr.join(",");
    }
    var getData = function () {
        var data;
        $.ajax({
            type: "POST",
            url: "/SysRecommend/GetEnterpriseList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            async: false,
            success: function (jsonResult) {
                data = jsonResult.ObjList;
            }
        })
        return data;
    };

    var vm = function () {
        var self = this;
        self.dataList = ko.observableArray();
        self.save = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var sendData = {
                arrayId: getTrueList()
            }
            $.ajax({
                type: "POST",
                url: "/SysRecommend/Add",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                        return;
                    };
                    dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                        if (jsonResult.code == 1) {
                            self.close();
                        }
                    }
                    }]);
                }
            })
        }
        self.init = function () {
            self.dataList(getData());
        }
    }

    vm.prototype.binding = function () {
        $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
    }
    vm.prototype.close = function () {
        dialog.close(this);
    }
    vm.show = function () {
        var vmObj = new vm();
        vmObj.init();
        return dialog.show(vmObj);
    };

    return vm;
})