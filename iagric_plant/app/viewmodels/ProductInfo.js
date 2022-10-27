define(['plugins/dialog', 'plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', "./ProductInfoForEnterpriseAdd", "./ProductInfoForEnterpriseEdit", 'utils', 'logininfo'],
function (dialog, router, ko, km, jq, ProductInfoForEnterpriseAdd, ProductInfoForEnterpriseEdit, utils, loginInfo) {

    var ModuleInfo = {
        moduleID: '21000',
        parentModuleID: '10001'
    }

    var VmMaterial = {
        MaterialArray: ko.observableArray(),
        SelectedId: ko.observable()
    }

    //获取活动动态模块
    var GetMaterialModules = function () {

        var sendData = {
    }
    var data;
    $.ajax({
        type: "POST",
        url: "/Admin_Request/SearchNameList",
        contentType: "application/json;charset=utf-8", //必须有
        dataType: "json", //表示返回值类型，不必须
        data: JSON.stringify(sendData),
        async: false,
        success: function (jsonResult) {
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                return;
            };
            data = jsonResult.ObjList;
        }
    });
    return data;
}

//关联信息模块
var AddInfoModel = function (data, event) {
    var currentObj = $(event.target);
    currentObj.blur();
    var jsonResult = loginInfo.isLoginTimeoutForServer();
    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
        return;
    };

    ProductInfoForEnterpriseAdd.show().then(function () {
        var list = GetDataKO(1);
        UpdateData(list);
    });
}

//关联信息模块
var EditInfoModel = function (Id, data, event) {
    var currentObj = $(event.target);
    currentObj.blur();

    Id = ko.utils.unwrapObservable(Id);

    var jsonResult = loginInfo.isLoginTimeoutForServer();
    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
        return;
    };
    ProductInfoForEnterpriseEdit.show(Id).then(function () {
        var list = GetDataKO(1);
        UpdateData(list);
    });
}

var DelInfoModel = function (Id, data, event) {
    Id = ko.utils.unwrapObservable(Id);

    var currentObj = $(event.target);
    currentObj.blur();
    dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    Id: Id
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_ProductInfo/DelProductInfoForEnterprise",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                            return;
                        }
                        else {
                            dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                if (jsonResult.code != 0) {
                                    var currentPageRow = vm.ViewModel.ObjList().length;
                                    var pageIndex = vm.ViewModel.pageIndex();
                                    if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                        pageIndex = vm.ViewModel.pageIndex() - 1;
                                    }
                                    var list = GetDataKO(1);
                                    UpdateData(list);

                                    this.pageSize = parseInt(vm.ViewModel.pageSize());
                                    this.totalCounts = parseInt(vm.ViewModel.totalCounts());
                                }
                            }
                            }]);
                        }
                    }
                });
            }
        },
            {
                title: '取消',
                callback: function () {
                }
            }
            ]);
}

//ajax获取数据
var GetData = function (pageIndex) {
    var sendData = {
        pageIndex: pageIndex,
        MaterialId: VmMaterial.SelectedId()
    }
    var data;
    $.ajax({
        type: "POST",
        url: "/Admin_ProductInfo/Index",
        contentType: "application/json;charset=utf-8", //必须有
        dataType: "json", //表示返回值类型，不必须
        data: JSON.stringify(sendData),
        async: false,
        success: function (jsonResult) {
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                return;
            };
            data = jsonResult;
        }
    })
    return data;
}

//把获取的ajax数据转化为ko
var GetDataKO = function (pageIndex) {
    var list = km.fromJS(GetData(pageIndex));
    return list;
}

//搜索生产基地
var SearchProductInfo = function (data, event) {
    var list = GetDataKO(1);
    UpdateData(list);
};

//分页、搜索时更新数据源
var UpdateData = function (list) {
    vm.ViewModel.ObjList(list.ObjList());
    vm.ViewModel.pageSize(list.pageSize());
    vm.ViewModel.totalCounts(list.totalCounts());
    vm.ViewModel.pageIndex(list.pageIndex());
}

//自定义绑定-分页控件
ko.bindingHandlers.ProductInfoPagerBH = {
    init: function (element, valueAccessor, allBindingsAccessor) {
    },
    update: function (element, valueAccessor, allBindingsAccessor) {

        var value = valueAccessor();
        var allBindings = allBindingsAccessor();
        var pageSize = parseInt(ko.utils.unwrapObservable(value));
        var totalCounts = parseInt(ko.utils.unwrapObservable(allBindings.totalCounts));
        var pageIndex = parseInt(ko.utils.unwrapObservable(allBindings.pageIndex));

        $(element).jqPaginator({
            totalCounts: totalCounts,
            pageSize: pageSize,
            visiblePages: 10,
            currentPage: pageIndex,
            first: '<li class="first"><a href="javascript:;">首页</a></li>',
            last: '<li class="last"><a href="javascript:;">尾页</a></li>',
            prev: '<li class="prev"><a href="javascript:;">上一页</a></li>',
            next: '<li class="next"><a href="javascript:;">下一页</a></li>',
            page: '<li class="page"><a href="javascript:;">{{page}}</a></li>',
            onPageChange: function (num, type) {
                if (type == 'change') {
                    var list = GetDataKO(num);
                    UpdateData(list);
                    this.pageSize = parseInt(vm.ViewModel.pageSize());
                    this.totalCounts = parseInt(vm.ViewModel.totalCounts());
                }
            }
        });
    }
};

var MouseoverFun = function (data, event) {
    // 删除
    var BtnDel = $("#BtnDel_" + data.Id());
    // 修改
    var BtnEdit = $("#BtnEdit_" + data.Id());

    BtnDel.css({ "display": "" });
    BtnEdit.css({ "display": "" });
}

var MouseoutFun = function (data, event) {
    // 删除
    var BtnDel = $("#BtnDel_" + data.Id());
    // 修改
    var BtnEdit = $("#BtnEdit_" + data.Id());

    BtnDel.css({ "display": "none" });
    BtnEdit.css({ "display": "none" });
}

var ViewPropertyIdArray = function (PropertyIdArray) {
    PropertyIdArray = ko.utils.unwrapObservable(PropertyIdArray);

    if (PropertyIdArray.length == 0) {
        return "否";
    } else {
        return "是";
    }
}

var ViewStatus = function (Item) {
    Item = ko.utils.unwrapObservable(Item);
    if (Item == 0) {
        return "否";
    } else {
        return "是";
    }
}

var vm = {
    binding: function () {
        //初初化导航状态
        //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
        //初始化brand列表数据
        vm.ViewModel = GetDataKO(1);

        VmMaterial.MaterialArray(GetMaterialModules());
    },
    goBack: function () {
        router.navigateBack();
    },
    ViewModel: null,
    AddInfoModel: AddInfoModel,
    SearchProductInfo: SearchProductInfo,
    MouseoverFun: MouseoverFun,
    MouseoutFun: MouseoutFun,
    VmMaterial: VmMaterial,
    EditInfoModel: EditInfoModel,
    DelInfoModel: DelInfoModel,
    ViewStatus: ViewStatus,
    ViewPropertyIdArray: ViewPropertyIdArray


}
return vm;

});