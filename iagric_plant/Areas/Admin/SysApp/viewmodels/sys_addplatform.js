define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo'], function (dialog, ko, jqueryui, km, utils, loginInfo) {
    var moduleInfo = {
        moduleID: '11101',
        parentModuleID: '11100'
    }

    var array = new Array();

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
        var sendData = {
    }
    var data;
    $.ajax({
        type: "POST",
        url: "/SysPlatForm/SearchPlatForm",
        contentType: "application/json;charset=utf-8", //必须有
        dataType: "json", //表示返回值类型，不必须
        data: JSON.stringify(sendData),
        async: false,
        success: function (jsonResult) {
            data = jsonResult.ObjList;
        }
    })
    return data;
}

var dataList = getData();

var vm = function () {
    var self = this;
    self.save = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var sendData = {
            //获取选中权限1,2,3
            arrayId: getTrueList(),
            falseId: getFalseList()
        }
        $.ajax({
            type: "POST",
            url: "/SysPlatForm/Save",
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

    self.isChecked = function (rId) {
        if ($.inArray(rId.toString(), array) == -1) {
            return false;
        } else {
            return true;
        }
    }

    self.init = function () {
        var sendData = {
    }
    //url: "/SysPlatForm/GetPlatForm",
    $.ajax({
        type: "POST",
        url: "/SysPlatForm/SearchPlatForm",
        contentType: "application/json;charset=utf-8", //必须有
        dataType: "json", //表示返回值类型，不必须
        data: JSON.stringify(sendData),
        async: false,
        success: function (jsonResult) {
            array = jsonResult.ObjList;
        }
    });
}
}

vm.prototype.binding = function () {
    $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
}
vm.prototype.initMenus = function () {
    return ko.utils.arrayFilter(dataList, function (item) {
        return item.Status == 1;
    });
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