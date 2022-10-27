define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'bootstrap-datepicker', './comment_edit', 'plugins/dialog', 'utils', 'jquery.fileDownload', 'logininfo'],
    function (router, ko, km, $, jq, bdp, comment_edit, dialog, utils, jfd, loginInfo) {
        var moduleInfo = {
            moduleID: '31000',
            parentModuleID: '10001'
        }

        var name = ko.observable();
        var bDate = ko.observable();
        var eDate = ko.observable();
        //搜索
        var searchComment = function (data, event) {
            var list = getDataKO(name(), bDate(), eDate(), vm.vmComment.pageIndex());
            updateData(list);
        };
        //把获取的ajax数据转化为ko
        var getDataKO = function (name, bDate, eDate, pageIndex) {
            var list = km.fromJS(getData(name, bDate, eDate, pageIndex));
            return list;
        }
        //ajax获取数据
        var getData = function (name, bDate, eDate, pageIndex) {
            var sendData = {
                name: name,
                bDate: bDate,
                eDate: eDate,
                pageIndex: pageIndex
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/Admin_Comment/Index",
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
        //分页、搜索时更新数据源
        var updateData = function (list) {
            //alert(list.pageIndex());
            vm.vmComment.ObjList(list.ObjList());
            vm.vmComment.pageSize(list.pageSize());
            vm.vmComment.totalCounts(list.totalCounts());
            vm.vmComment.pageIndex(list.pageIndex());
        }
        //自定义绑定-分页控件
        ko.bindingHandlers.commentPager = {
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
                            vm.vmComment.pageIndex(num);
                            var list = getDataKO(name(), bDate(), eDate(), vm.vmComment.pageIndex());
                            updateData(list);
                            this.pageSize = parseInt(vm.vmComment.pageSize());
                            this.totalCounts = parseInt(vm.vmComment.totalCounts());
                        }
                    }
                });
            }
        };
        var mouseoverFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowInfo = self.find("div[eleflag='ShowComment']");
            if (data.ReContent() == '暂无回复')
                ShowInfo.css({ "display": "" });
        }
        var mouseoutFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowInfo = self.find("div[eleflag='ShowComment']");
            ShowInfo.css({ "display": "none" });
        }
        var commentOrder = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var jsonResult = loginInfo.isLoginTimeoutForServer();
            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                return;
            };
            var id = ko.utils.unwrapObservable(id);
            comment_edit.show(id).then(function () {
                searchComment();
            });
        }
        var vm = {
            binding: function () {
                //初初化导航状态
                //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);

                //初始化dealer列表数据
                vm.vmComment = getDataKO("", "", "", 1);
                updateData(vm.vmComment);

                $('#bDate').datepicker({
                    language: 'cn',
                    autoclose: true,
                    todayHighlight: true
                });
                $('#eDate').datepicker({
                    language: 'cn',
                    autoclose: true,
                    todayHighlight: true
                });
            },
            goBack: function () {
                router.navigateBack();
            },
            vmComment: null,
            name: name,
            bDate: bDate,
            eDate: eDate,
            searchComment: searchComment,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun,
            commentOrder: commentOrder
        }
        return vm;
    });