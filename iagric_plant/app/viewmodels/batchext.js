define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator','utils', 'logininfo'], function (router, ko, km, $, jq,utils, loginInfo) {
    //var searchTitle = ko.observable();
    var batchExtName = ko.observable();
    //ajax获取数据
    var getData = function (pageIndex, batchExtName) {
        var sendData = {
            pageIndex: pageIndex,
            batchExtName: batchExtName
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_BatchExt/Index",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                //                var obj = JSON.parse(jsonResult)
                //                data = obj;
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                    return;
                };
                data = jsonResult;
            }
        })
        return data;
    }
    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.vmBatchExt.ObjList(list.ObjList());
        vm.vmBatchExt.pageSize(list.pageSize());
        vm.vmBatchExt.totalCounts(list.totalCounts());
        vm.vmBatchExt.pageIndex(list.pageIndex());
    }
    //搜索子批次
    var searchBatchExt = function (data, event) {
        var list = getDataKO(1, batchExtName());
        updateData(list);
    };
    //添加批次信息
    var addBatchExt = function (data, event) {
        router.navigate('#batchExt_Add');
    };
    //编辑子批次信息
    var editBatchExt = function (id, data, event) {
        var id = ko.utils.unwrapObservable(id);
        router.navigate('#batchExt_Edit?id=' + id);
    }
    //删除子批次
    var delBatchExt = function (id, data, event) {
        var currentObj = $(event.target);
        currentObj.blur();
        var id = ko.utils.unwrapObservable(id);

        var confirm = bootbox.confirm("确定删除选择的数据吗?", function (result) {
            if (result) {
                var sendData = {
                    batchextId: id
                }
                $.ajax({
                    type: "POST",
                    url: "/Admin_BatchExt/Delete",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        var list = getDataKO(vm.vmBatchExt.pageIndex());
                        updateData(list);
                        this.pageSize = parseInt(vm.vmBatchExt.pageSize());
                        this.totalCounts = parseInt(vm.vmBatchExt.totalCounts());
                    },
                    error: function (e) {
                        alert(e);
                    }
                });
            }
        });
    };
    //把获取的ajax数据转化为ko
    var getDataKO = function (pageIndex, batchExtName) {
        var list = km.fromJS(getData(pageIndex, batchExtName));
        return list;
    }

    ko.bindingHandlers.batchExtPager = {
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
                currentPage: 1,
                first: '<li class="first"><a href="javascript:;">首页</a></li>',
                last: '<li class="last"><a href="javascript:;">尾页</a></li>',
                prev: '<li class="prev"><a href="javascript:;">上一页</a></li>',
                next: '<li class="next"><a href="javascript:;">下一页</a></li>',
                page: '<li class="page"><a href="javascript:;">{{page}}</a></li>',
                onPageChange: function (num, type) {
                    //alert(type + '：' + num);
                    if (type == 'change') {
                        var list = getDataKO(num, batchExtName());
                        updateData(list);
                        this.pageSize = parseInt(vm.vmBatchExt.pageSize());
                        this.totalCounts = parseInt(vm.vmBatchExt.totalCounts());
                    }
                }
            });
        }
    };
    var vm = {
        binding: function () {
            //初始化batchExt列表数据
            vm.vmBatchExt = getDataKO(1, "");
        },
        vmBatchExt: null,
        searchBatchExt: searchBatchExt,
        batchExtName: batchExtName,
        addBatchExt: addBatchExt,
        editBatchExt: editBatchExt,
        delBatchExt: delBatchExt
    }
    return vm;
});