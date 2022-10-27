define(['plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', 'logininfo', 'plugins/dialog', 'utils'],
function (router, ko, km, jq, loginInfo, dialog, utils) {
    var array1 = new Array();
    var array2 = new Array();
    var array3 = new Array();
    var companyName = ko.observable('');
    var FileUrls = ko.observable('');
    var moduleInfo = {
        moduleID: '24001',
        parentModuleID: '10001'
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

    //获取品牌字符串,例如：1,2,3
    var getBrandList = function () {
        var arr = new Array();
        $("#table1").find("input[name='cbx']:checkbox").each(function () {
            if (this.checked == true) {
                var BrandID = $(this).val();
                if ($.inArray(BrandID, arr) == -1) {
                    arr.push(BrandID);
                }
            }
        });
        return arr.join(",");
    }
    var getBrandDataKO = function () {
        var list = km.fromJS(getBrandData());
        return list;
    }
    var getBrandData = function () {
        var sendData = {
    }
    var data;
    $.ajax({
        type: "POST",
        url: "/Admin_Configure/GetBrandList",
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
//检查复选框是否勾选
var isBrandChecked = function (bId) {
    var bId = bId.toString();
    if ($.inArray(bId, array2) == -1) {
        return false;
    } else {
        return true;
    }
}

var isAllChecked1 = function () {
    for (var i = 0; i < vm.dataBrandList.length; i++) {
        if ($.inArray(vm.dataBrandList[i].Brand_ID.toString(),array2) == -1) {
            return false;
        }
    }
    return true;
}

//获取员工字符串
var getUserList = function () {
    var arr = new Array();
    $("#table2").find("input[name='cbx']:checkbox").each(function () {
        if (this.checked == true) {
            var UserID = $(this).val();
            if ($.inArray(UserID, arr) == -1) {
                arr.push(UserID);
            }
        }
    });
    return arr.join(",");
}
var getUserDataKO = function () {
    var list = km.fromJS(getUserData());
    return list;
}
var getUserData = function () {
    var sendData = {
}
var data;
$.ajax({
    type: "POST",
    url: "/Admin_Configure/GetUserList",
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
var isUserChecked = function (uId) {
    var uId = uId.toString();
    if ($.inArray(uId, array1) == -1) {
        return false;
    } else {
        return true;
    }
}
//加载时判断“全选”按钮是否该勾上
var isAllChecked2 = function () {
    for (var i = 0; i < vm.dataUserList.length; i++) {
        if ($.inArray(vm.dataUserList[i].UserID.toString(), array1) == -1) {
            return false;
        }
    }
    return true;
}

//获取新闻字符串
var getNewsList = function () {
    var arr = new Array();
    $("#table3").find("input[name='cbx']:checkbox").each(function () {
        if (this.checked == true) {
            var NewsID = $(this).val();
            if ($.inArray(NewsID, arr) == -1) {
                arr.push(NewsID);
            }
        }
    });
    return arr.join(",");
}
var getNewsDataKO = function () {
    var list = km.fromJS(getNewsData());
    return list;
}
var getNewsData = function () {
    var sendData = {
}
var data;
$.ajax({
    type: "POST",
    url: "/Admin_Configure/GetNewsList",
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
var isNewsChecked = function (nId) {
    var nId = nId.toString();
    if ($.inArray(nId, array3) == -1) {
        return false;
    } else {
        return true;
    }
}

var isAllChecked3 = function () {
    var flag = true;
    for (var i = 0; i < vm.dataNewsList.length; i++) {
        if ($.inArray(vm.dataNewsList[i].ID.toString(), array3) == -1) {
            return false;
        }
    }
    return flag;
}

var init = function () {
    var sendData = {
}
$.ajax({
    type: "POST",
    url: "/Admin_Configure/GetModel",
    contentType: "application/json;charset=utf-8", //必须有
    dataType: "json", //表示返回值类型，不必须
    async: false,
    success: function (jsonResult) {
        if (jsonResult.code == 1) {
            array1 = jsonResult.ObjModel.User_ID_Array.split(',');
            array2 = jsonResult.ObjModel.Brand_ID_Array.split(',');
            array3 = jsonResult.ObjModel.News_ID_Array.split(',');
        }
    }
});
}

//获取企业信息，企业名称和Logo
var getCompanyData = function () {
    $.ajax({
        type: "POST",
        url: "/Admin_Enterprise_Show/Index",
        contentType: "application/json;charset=utf-8", //必须有
        dataType: "json", //表示返回值类型，不必须
        async: false,
        success: function (jsonResult) {
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                return;
            };
            companyName(jsonResult.ObjModel.CompanyName);
            if (jsonResult.ObjModel.imgs.length != 0) {
                FileUrls(jsonResult.ObjModel.imgs[0].fileUrls);
            }
        }
    })
}

var save = function (data, event) {
    var currentObj = $(event.target);
    currentObj.blur();
    var self = this;
    var sendData = {
        UserIdArray: getUserList(),
        BrandIdArray: getBrandList(),
        NewsIdArray: getNewsList()
    }
    $.ajax({
        type: "POST",
        url: "/Admin_Configure/Update",
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
                    self.close;
                }
            }
            }]);
        }
    })
}
var vm = {
    binding: function () {
        //初初化导航状态
        //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
        //初始化brand列表数据
        vm.dataBrandList = getBrandData();
        vm.dataUserList = getUserData();
        vm.dataNewsList = getNewsData();
        getCompanyData();
        init();
    },
    goBack: function () {
        router.navigateBack();
    },

    dataBrandList: null,
    dataUserList: null,
    dataNewsList: null,
    save: save,
    companyName: companyName,
    isNewsChecked: isNewsChecked,
    isUserChecked: isUserChecked,
    isBrandChecked: isBrandChecked,
    FileUrls: FileUrls,
    isAllChecked1: isAllChecked1,
    isAllChecked2: isAllChecked2,
    isAllChecked3: isAllChecked3
}
return vm;
});