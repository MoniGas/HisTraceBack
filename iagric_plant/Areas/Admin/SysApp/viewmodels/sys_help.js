define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'plugins/dialog', 'utils', 'logininfo'],
    function (router, ko, km, $, jq, dialog, utils, loginInfo) {
        var moduleInfo = {
            moduleID: '11130',
            parentModuleID: '10000'
        }
        var searchTitle = ko.observable();

        //ajax获取数据
        var getData = function (pageIndex, helpTitle) {
            var sendData = {
                helpTitle: helpTitle,
                pageIndex: pageIndex
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/SysHelp/Index",
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
            vm.vmHelp.ObjList(list.ObjList());
            vm.vmHelp.pageSize(list.pageSize());
            vm.vmHelp.totalCounts(list.totalCounts());
            vm.vmHelp.pageIndex(list.pageIndex());
        }
        //搜索帮助
        var searchHelp = function (data, event) {
            var list = getDataKO(1, searchTitle());
            updateData(list);
        };
        //跳转——添加帮助
        var addHelp = function (data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            router.navigate('#sys_helpadd');
        }
        //        //添加帮助
        //        var addHelp = function (data, event) {
        //            var currentObj = $(event.target);
        //            currentObj.blur();
        //            var jsonResult = loginInfo.isLoginTimeoutForServer();
        //            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
        //                return;
        //            };
        //            sys_helpadd.show().then(function () {
        //                searchTitle('');
        //                var list = getDataKO(1, "");
        //                updateData(list);
        //            });
        //        };
        //置顶
        var topHelp = function (id, sort, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var id = ko.utils.unwrapObservable(id);
            var sort = ko.utils.unwrapObservable(sort);
            var sendData = {
                id: id,
                sort: sort
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/SysHelp/EditSort",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
                        return;
                    };
                    dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                        if (jsonResult.code != 0) {
                            var list = getDataKO(1, searchTitle());
                            updateData(list);
                            closeOK();
                        }
                    }
                    }]);
                }
            })
        }
//        //上移
//        var btnUpEvent = function (data, event) {
//            var index = 0;
//            for (var i = 0; i < vm.vmHelp.ObjList().length; i++) {
//                if (data.HelpTitle == vm.vmHelp.ObjList()[i].HelpTitle) {
//                    index = i;
//                }
//            }
//            if (index != 0) {
//                var up = vm.vmHelp.ObjList()[index - 1];
//                vm.vmHelp.ObjList()[index] = up;
//                var id = vm.vmHelp.ObjList()[index - 1].HelpId();
//                var sort = vm.vmHelp.ObjList()[index - 1].Sort();
//                //                alert(sort); 
//                vm.vmHelp.topHelp(id, sort);
//                vm.vmHelp.ObjList()[index - 1] = data;
//                var id1 = vm.vmHelp.ObjList()[index - 1].HelpId();
//                var sort1 = vm.vmHelp.ObjList()[index - 1].Sort();
//                vm.vmHelp.topHelp(id1, sort1);
//            }
//            vm.vmHelp.ObjList(vm.vmHelp.ObjList());
//        }
//        //下移
//        var btnDownEvent = function (data, event) {
//            var index = 0;
//            for (var i = 0; i < vm.vmHelp.ObjList().length; i++) {
//                if (data.HelpTitle == vm.vmHelp.ObjList()[i].HelpTitle) {
//                    index = i;
//                }
//            }
//            if (index < vm.vmHelp.ObjList().length - 1) {
//                var up = vm.vmHelp.ObjList()[index + 1];
//                vm.vmHelp.ObjList()[index] = up;
//                vm.vmHelp.ObjList()[index + 1] = data;
//            }
//            vm.vmHelp.ObjList(vm.vmHelp.ObjList());
//        }
        //跳转——编辑帮助
        var editHelp = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            id = ko.utils.unwrapObservable(id);
            router.navigate('#sys_helpedit?HelpId=' + id);
        }
        //        //编辑帮助
        //        var editHelp = function (id, data, event) {
        //            var currentObj = $(event.target);
        //            currentObj.blur();
        //            var jsonResult = loginInfo.isLoginTimeoutForServer();
        //            if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg, 1)) {
        //                return;
        //            };
        //            var id = ko.utils.unwrapObservable(id);
        //            sys_helpedit.show(id).then(function () {
        //                var list = getDataKO(vm.vmHelp.pageIndex(), searchTitle());
        //                updateData(list);
        //            });
        //        };

        var delHelp = function (id, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var id = ko.utils.unwrapObservable(id);
            dialog.showMessage("确定删除该帮助吗？", '系统提示', [
            {
                title: '确定',
                callback: function () {
                    var sendData = {
                        id: id
                    }
                    $.ajax({
                        type: "POST",
                        url: "/SysHelp/Delete",
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
                                        var currentPageRow = vm.vmHelp.ObjList().length;
                                        var pageIndex = vm.vmHelp.pageIndex();
                                        if (currentPageRow - 1 == 0 && pageIndex > 1) {
                                            pageIndex = vm.vmHelp.pageIndex() - 1;
                                        }
                                        var list = getDataKO(pageIndex, searchTitle());
                                        updateData(list);
                                        this.pageSize = parseInt(vm.vmHelp.pageSize());
                                        this.totalCounts = parseInt(vm.vmHelp.totalCounts());
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
        };
        //把获取的ajax数据转化为ko
        var getDataKO = function (pageIndex, helpTitle) {
            var list = km.fromJS(getData(pageIndex, helpTitle));
            return list;
        }
        //自定义绑定-分页控件
        ko.bindingHandlers.helpPager = {
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
                            var list = getDataKO(num, searchTitle());
                            updateData(list);
                            this.pageSize = parseInt(vm.vmHelp.pageSize());
                            this.totalCounts = parseInt(vm.vmHelp.totalCounts());
                        }
                    }
                });
            }
        };
        var mouseoverFun = function (data, event) {
            var self = $(event.target).closest('tr');

            var ShowAll = self.find("button[eleflag='ShowAll']");
            ShowAll.css({ "display": "" });
        }
        var mouseoutFun = function (data, event) {
            var self = $(event.target).closest('tr');

            var ShowAll = self.find("button[eleflag='ShowAll']");
            ShowAll.css({ "display": "none" });
        }
        var closeOK = function () {
            dialog.close(this);
        }
        var vm = {
            binding: function () {
                //初初化导航状态
                //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
                //初始化dealer列表数据
                vm.vmHelp = getDataKO(1, "");
                updateData(vm.vmHelp);
            },
            goBack: function () {
                router.navigateBack();
            },
            vmHelp: null,
            searchTitle: searchTitle,
            searchHelp: searchHelp,
            addHelp: addHelp,
            editHelp: editHelp,
            delHelp: delHelp,
//            btnUpEvent: btnUpEvent,
//            btnDownEvent: btnDownEvent,
            topHelp: topHelp,
            closeOK: closeOK,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun
        }
        return vm;
    });