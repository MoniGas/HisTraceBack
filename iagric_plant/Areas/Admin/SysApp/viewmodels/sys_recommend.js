define(['plugins/dialog', 'plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', 'utils', 'logininfo'],
    function (dialog, router, ko, km, jq, utils, loginInfo) {
        var name = ko.observable();
        var moduleInfo = {
            moduleID: '11140',
            parentModuleID: '0'
        }
        //ajax获取数据
        var getData = function (pageIndex, name) {
            var sendData = {
                pageIndex: pageIndex,
                name: name,
                type: 1
            }
            var data;
            $.ajax({
                type: "POST",
                url: "/SysRecommend/GetList",
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
        var getDataKO = function (pageIndex, name) {
            var list = km.fromJS(getData(pageIndex, name));
            return list;
        }

        //搜索
        var searchEnterprise = function (data, event) {
            var list = getDataKO(1, name());
            updateData(list);
        };

        //分页、搜索时更新数据源
        var updateData = function (list) {
            vm.vmRecommend.ObjList(list.ObjList());
            vm.vmRecommend.pageSize(list.pageSize());
            vm.vmRecommend.totalCounts(list.totalCounts());
            vm.vmRecommend.pageIndex(list.pageIndex());
        }
        //自定义绑定-分页控件
        ko.bindingHandlers.recommendPager = {
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
                            var list = getDataKO(num, name());
                            updateData(list);
                            this.pageSize = parseInt(vm.vmRecommend.pageSize());
                            this.totalCounts = parseInt(vm.vmRecommend.totalCounts());
                        }
                    }
                });
            }
        };
        var addRecommend = function (data, event) { }
        var Verify = function (id, type, data, event) {
            var currentObj = $(event.target);
            currentObj.blur();
            var sendData = {
                id: id(),
                type: type
            }
            $.ajax({
                type: "POST",
                url: "/SysRecommend/Verify",
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
                            searchEnterprise();
                        }
                    }
                    }]);
                }
            })
        }
        var mouseoverFun = function (data, event) {
            var self = $(event.target).closest('tr');

            var TypeText = self.find("span[eleflag='TypeText']");
            var VerifyText = self.find("span[eleflag='VerifyText']");
            if (TypeText[0].textContent == '企业推荐') {
                if (VerifyText[0].textContent == '审核不通过') {
                    var ShowPass = self.find("button[eleflag='ShowPass']");
                    ShowPass.css({ "display": "" });
                }
                else if (VerifyText[0].textContent == '审核通过') {
                    var ShowNoPass = self.find("button[eleflag='ShowNoPass']");
                    ShowNoPass.css({ "display": "" });
                }
                else {
                    var ShowPass = self.find("button[eleflag='ShowPass']");
                    ShowPass.css({ "display": "" });
                    var ShowNoPass = self.find("button[eleflag='ShowNoPass']");
                    ShowNoPass.css({ "display": "" });
                }
            }
            else {
                var ShowCancel = self.find("button[eleflag='ShowCancel']");
                ShowCancel.css({ "display": "none" });
            }
            var ShowPreview = self.find("button[eleflag='ShowPreview']");
            ShowPreview.css({ "display": "none" });
        }

        var mouseoutFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowPass = self.find("button[eleflag='ShowPass']");
            ShowPass.css({ "display": "none" });
            var ShowNoPass = self.find("button[eleflag='ShowNoPass']");
            ShowNoPass.css({ "display": "none" });
            var ShowCancel = self.find("button[eleflag='ShowCancel']");
            ShowCancel.css({ "display": "none" });
            var ShowPreview = self.find("button[eleflag='ShowPreview']");
            ShowPreview.css({ "display": "none" });
        }
        var vm = {
            binding: function () {
                //初初化导航状态
                loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
                //初始化brand列表数据
                vm.vmRecommend = getDataKO(1, "");
            },
            goBack: function () {
                router.navigateBack();
            },
            vmRecommend: null,
            name: name,
            searchEnterprise: searchEnterprise,
            addRecommend: addRecommend,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun,
            Verify: Verify
        }
        return vm;
    }
);