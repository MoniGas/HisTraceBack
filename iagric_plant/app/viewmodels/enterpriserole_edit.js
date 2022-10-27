define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'jquery', 'utils', 'logininfo'],
function (dialog, ko, jqueryui, km, $, utils, loginInfo) {
    var array = new Array();
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

    //获取权限字符串,例如：1,2,3
    var getPermissionList = function () {
        var arr = new Array();
        $("#table1").find("input[name='cbx']:checkbox").each(function () {
            if (this.checked == true) {
                var parentItemID = $(this).attr("eleflag1");
                if ($.inArray(parentItemID, arr) == -1) {
                    arr.push(parentItemID);
                }
                arr.push($(this).val());
            }
        });

        $("#table1").find("input[name='cbx2']:checkbox").each(function () {
            if (this.checked == true) {
                var parentItemID = $(this).val();
                if ($.inArray(parentItemID, arr) == -1) {
                    arr.push(parentItemID);
                }
            }
        });

        return arr.join(",");
    }

    var getRoleListKO = function () {
        var list = km.fromJS(getData());
        return list;
    }

    var getData = function () {
        var sendData = {
    }
    var data;
    $.ajax({
        type: "POST",
        url: "/Admin_Enterprise_Role/GetModelList",
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
var vm = function (id) {
    var self = this;
    self.name = ko.observable('').extend({
        maxLength: { params: 50, message: "名称最大长度为50个字符" },
        required: {
            params: true,
            message: "请输入角色名称!"
        }
    });
    self.selTitle = ko.observable(false);

    self.isChecked = function (rId) {
        var rId = rId.toString();
        if ($.inArray(rId, array) == -1) {
            return false;
        } else {
            return true;
        }
    }
    self.init = function () {
        var sendData = {
            id: id
        }
        $.ajax({
            type: "POST",
            url: "/Admin_Enterprise_Role/GetModel",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code == 1) {
                    self.name(jsonResult.ObjModel.RoleName);
                    array = jsonResult.ObjModel.Modual_ID_Array.split(',');
                }
            }
        })
    }

    self.save = function (data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        self.errors = ko.validation.group(self);
        if (self.errors().length <= 0) {
            var sendData = {
                rId: id,
                roleName: self.name(),
                //获取选中权限1,2,3
                modelIdArray: getPermissionList()
            }
            $.ajax({
                type: "POST",
                url: "/Admin_Enterprise_Role/Update",
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
        } else {
            self.errors.showAllMessages();
        }
    }
}
vm.prototype.binding = function () {
    $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
}

vm.prototype.initMenus2 = function (levelId, parentid) {
    vm.prototype.HasTd = false;
    return ko.utils.arrayFilter(dataList, function (item) {
        return item.Modual_Level == levelId && item.Parent_ID == parentid;
    });
}
vm.prototype.initMenus1 = function (levelId, platModual) {
    vm.prototype.HasTd = true;
    var data = ko.utils.arrayFilter(dataList, function (item) {
        return item.Modual_Level == levelId && item.PlatModual == platModual;
    });
    vm.prototype.RowCount = data.length;
    return data;
}
vm.prototype.HasTd = true;
vm.prototype.RowCount = 0;
vm.prototype.close = function () {
    dialog.close(this);
}
vm.show = function (id) {
    var vmObj = new vm(id);
    vmObj.init();
    return dialog.show(vmObj);
};

return vm;
});