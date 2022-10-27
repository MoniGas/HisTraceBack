define(['plugins/dialog', 'plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', "./EwmSpecificationManageAdd", "./EwmSpecificationManageEdit", 'utils', 'logininfo'],
function (dialog, router, ko, km, jq, EwmSpecificationManageAdd, EwmSpecificationManageEdit, utils, loginInfo) {
    var ModuleInfo = {
        ModuleId: '10080',
        ParentModuleId: '10001'
    }
    //添加
    var AddSpecification = function (data, event) {
        var CurrentObj = $(event.target);
        CurrentObj.blur();
        var JsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(JsonResult.code, JsonResult.Msg, 1)) {
            return;
        };
        EwmSpecificationManageAdd.show().then(function () {
            Value('');
            var List = GetDataKo(1);
            UpDateData(List);
        });
    }

    //编辑
    var EditSpecification = function (Id, data, event) {
        var CurrentObj = $(event.target);
        CurrentObj.blur();
        var JsonResult = loginInfo.isLoginTimeoutForServer();
        if (loginInfo.isLoginTimeout(JsonResult.code, JsonResult.Msg, 1)) {
            return;
        };
        var Id = ko.utils.unwrapObservable(Id);
        EwmSpecificationManageEdit.show(Id).then(function (Value) {
            if (Value != undefined) {
                data.Value(Value);
            }
            var List = GetDataKo(1);
            UpDateData(List);
        });
    }

    var Value = ko.observable();
    //ajax获取数据
    var GetData = function (PageIndex) {
        var sendData = {
            pageIndex: PageIndex,
            Value: Value()
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_Specification/Index",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (JsonResult) {
                if (loginInfo.isLoginTimeout(JsonResult.code, JsonResult.Msg)) {
                    return;
                };
                data = JsonResult;
            }
        })
        return data;
    }

    //把获取的ajax数据转化为ko
    var GetDataKo = function (PageIndex) {
        var List = km.fromJS(GetData(PageIndex));
        return List;
    }

    //搜索
    var SearchSpecification = function (data, event) {
        var List = GetDataKo(1);
        UpDateData(List);
    };

    //分页、搜索时更新数据源
    var UpDateData = function (List) {
        ViewModel.ViewModelSpecification.ObjList(List.ObjList());
        ViewModel.ViewModelSpecification.pageSize(List.pageSize());
        ViewModel.ViewModelSpecification.totalCounts(List.totalCounts());
        ViewModel.ViewModelSpecification.pageIndex(List.pageIndex());
    }

    //删除
    var DelSpecification = function (Id, data, event) {

        var CurrentObj = $(event.target);
        CurrentObj.blur();
        var Id = ko.utils.unwrapObservable(Id);
        dialog.showMessage("确定删除选择的数据吗？", '系统提示', [
        {
            title: '确定',
            callback: function () {
                var sendData = {
                    Id: Id
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_Specification/Delete",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (JsonResult) {
                        if (loginInfo.isLoginTimeout(JsonResult.code, JsonResult.Msg, 1)) {
                            return;
                        }
                        else {
                            dialog.showMessage(JsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                                if (JsonResult.code != 0) {
                                    var currentPageRow = ViewModel.ViewModelSpecification.ObjList().length;
                                    var PageIndex = ViewModel.ViewModelSpecification.pageIndex();
                                    if (currentPageRow - 1 == 0 && PageIndex > 1) {
                                        PageIndex = ViewModel.ViewModelSpecification.pageIndex() - 1;
                                    }
                                    var List = GetDataKo(PageIndex);
                                    UpDateData(List);

                                    this.PageSize = parseInt(ViewModel.ViewModelSpecification.pageSize());
                                    this.TotalCounts = parseInt(ViewModel.ViewModelSpecification.totalCounts());
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
    ko.bindingHandlers.SpecificationBH = {
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
                        var List = GetDataKo(num);
                        UpDateData(List);
                        this.pageSize = parseInt(ViewModel.ViewModelSpecification.pageSize());
                        this.totalCounts = parseInt(ViewModel.ViewModelSpecification.totalCounts());
                    }
                }
            });
        }
    };

    var MouseoverFun = function (data, event) {
        // 删除
        var BtnDel = $("#BtnDel" + data.ID());
        // 修改
        var BtnEdit = $("#BtnEdit" + data.ID());

        BtnDel.css({ "display": "" });
        BtnEdit.css({ "display": "" });

    }

    var MouseoutFun = function (data, event) {
        // 删除
        var BtnDel = $("#BtnDel" + data.ID());
        // 修改
        var BtnEdit = $("#BtnEdit" + data.ID());

        BtnDel.css({ "display": "none" });
        BtnEdit.css({ "display": "none" });


    }

    var ViewModel = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            //初始化brand列表数据
            ViewModel.ViewModelSpecification = GetDataKo(1);
        },
        goBack: function () {
            router.navigateBack();
        },
        Value: Value,
        ViewModelSpecification: null,
        AddSpecification: AddSpecification,
        SearchSpecification: SearchSpecification,
        DelSpecification: DelSpecification,
        EditSpecification: EditSpecification,
        MouseoverFun: MouseoverFun,
        MouseoutFun: MouseoutFun

    }
    return ViewModel;

});