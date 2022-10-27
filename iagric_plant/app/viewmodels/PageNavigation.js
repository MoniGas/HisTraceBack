define(['plugins/dialog', 'plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', "./PageNavigationAdd", 'utils', 'logininfo', './PageNavigationSorting'],
function (dialog, router, ko, km, jq, PageNavigationAdd, utils, loginInfo, PageNavigationSorting) {
    var moduleInfo = {
        moduleID: '22000',
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

var ViewSorting = function (MaterialId, data, event) {
    var currentObj = $(event.target);
    currentObj.blur();
    var jsonResult = loginInfo.isLoginTimeoutForServer();
    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
        return;
    };
    PageNavigationSorting.show(ko.utils.unwrapObservable(MaterialId)).then(function () {
        var list = GetDataKO(1);
        UpdateData(list);
    });
}

//添加生产基地
var AddNavigation = function (data, event) {
    var currentObj = $(event.target);
    currentObj.blur();
    var jsonResult = loginInfo.isLoginTimeoutForServer();
    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
        return;
    };
    PageNavigationAdd.show().then(function () {
        var list = GetDataKO(1);
        UpdateData(list);
    });
}

//上移
var MoveChange = function (Id, MaterialId, Type, data, event) {
    var sendData = {
        Id: ko.utils.unwrapObservable(Id),
        MaterialId: ko.utils.unwrapObservable(MaterialId),
        Type: ko.utils.unwrapObservable(Type)
    }
    var data;
    $.ajax({
        type: "POST",
        url: "/Admin_PageNavigation/UpdateViewNum",
        contentType: "application/json;charset=utf-8", //必须有
        dataType: "json", //表示返回值类型，不必须
        data: JSON.stringify(sendData),
        async: false,
        success: function (jsonResult) {
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                return;
            };
            if (jsonResult.code == 1) {
                var currentPageRow = ViewModel.VmNavigation.ObjList().length;
                var pageIndex = ViewModel.VmNavigation.pageIndex();
                if (currentPageRow - 1 == 0 && pageIndex > 1) {
                    pageIndex = ViewModel.VmNavigation.pageIndex() - 1;
                }
                var list = GetDataKO(pageIndex);
                UpdateData(list);

                this.pageSize = parseInt(ViewModel.VmNavigation.pageSize());
                this.totalCounts = parseInt(ViewModel.VmNavigation.totalCounts());
            }
        }
    });
    return data;
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
        url: "/Admin_PageNavigation/Index",
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
var SearchNavigation = function (data, event) {
    var list = GetDataKO(1);
    UpdateData(list);
};

//分页、搜索时更新数据源
var UpdateData = function (list) {
    ViewModel.VmNavigation.ObjList(list.ObjList());
    ViewModel.VmNavigation.pageSize(list.pageSize());
    ViewModel.VmNavigation.totalCounts(list.totalCounts());
    ViewModel.VmNavigation.pageIndex(list.pageIndex());
}

//删除生产基地
var DelNavigation = function (MaterialId, data, event) {
    var currentObj = $(event.target);
    currentObj.blur();
    dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    MaterialId: ko.utils.unwrapObservable(MaterialId)
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_PageNavigation/DelMaterialList",
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
                                    var currentPageRow = ViewModel.VmNavigation.ObjList().length;
                                    var pageIndex = ViewModel.VmNavigation.pageIndex();
                                    if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                        pageIndex = ViewModel.VmNavigation.pageIndex() - 1;
                                    }
                                    var list = GetDataKO(pageIndex);
                                    UpdateData(list);

                                    this.pageSize = parseInt(ViewModel.VmNavigation.pageSize());
                                    this.totalCounts = parseInt(ViewModel.VmNavigation.totalCounts());
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

//自定义绑定-分页控件
ko.bindingHandlers.greenhousePagerBH = {
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
                    this.pageSize = parseInt(ViewModel.VmNavigation.pageSize());
                    this.totalCounts = parseInt(ViewModel.VmNavigation.totalCounts());
                }
            }
        });
    }
};

var MouseoverFun = function (data, event) {
    // 删除
    var BtnDel = $("#BtnDel_" + data.MaterialId());
    // 排序
    var BtnSort = $("#BtnSort_" + data.MaterialId());

    BtnDel.css({ "display": "" });
    BtnSort.css({ "display": "" });

}

var MouseoutFun = function (data, event) {
    // 删除
    var BtnDel = $("#BtnDel_" + data.MaterialId());
    // 排序
    var BtnSort = $("#BtnSort_" + data.MaterialId());

    BtnDel.css({ "display": "none" });
    BtnSort.css({ "display": "none" });
}

var ViewModel = {
    binding: function () {
        //初初化导航状态
        //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
        //初始化brand列表数据
        ViewModel.VmNavigation = GetDataKO(1);

        VmMaterial.MaterialArray(GetMaterialModules());
    },
    goBack: function () {
        router.navigateBack();
    },
    VmNavigation: null,
    AddNavigation: AddNavigation,
    SearchNavigation: SearchNavigation,
    DelNavigation: DelNavigation,
    MouseoverFun: MouseoverFun,
    MouseoutFun: MouseoutFun,
    VmMaterial: VmMaterial,
    MoveChange: MoveChange,
    ViewSorting: ViewSorting

}
return ViewModel;

});