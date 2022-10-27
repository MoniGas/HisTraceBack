define(['plugins/dialog', 'plugins/router', 'knockout', 'knockout.mapping', 'jqPaginator', 'utils', 'logininfo', 'jquery.querystring', 'plugins/dialog'],
function (dialog, router, ko, km, jq, utils, loginInfo, qs, dialog) {
    //自定义绑定-复选框级联选择
    ko.bindingHandlers.checkBoxCascade = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            $(element).on('click', "input[name='cbx2']:checkbox", function (e) {
                if (this.checked == true) {
                    $("input[type='checkbox']").prop("checked", true);
                }
                else {
                    $("input[type='checkbox']").prop("checked", false);
                }
            });
        },
        update: function (element, valueAccessor, allBindingsAccessor) {
        }
    };
    var moduleInfo = {
        moduleID: '11032',
        parentModuleID: '11030'
    }
    var id = ko.observable();
    var name = ko.observable();
    var enterpriseName = ko.observable('');
    var SelType = ko.observable(false);
    //获取解析产品类型列表
//    var GetAnalysisTypeList = function () {
//        var data = [{ "selcode": "0", "selname": "已停止解析的产品" }, { "selcode": "1", "selname": "未停止解析的产品" }];
//        return data;
//    }

//    var vmAnalysisType = {
//        AnalysisTypeArray: ko.observableArray(GetAnalysisTypeList()),
//        selectedOption: ko.observable()
//    }
    var SelTypeModel = {
        SelTypeArray: ko.observable(),
        SelectedId: ko.observable()
    }
//    var onchangeType = function () {
//        var list = getDataKO(1,"");
//        vmAnalysis.ObjList(list.ObjList());
//    }

//    var stop = getData;
    //ajax获取数据
    var getData = function (pageIndex, name) {
        var sendData = {
            pageIndex: pageIndex,
            name: name,
            //            IsAnalyse: vmAnalysisType.selectedOption(),
            IsAnalyse: ko.utils.unwrapObservable(SelTypeModel.SelectedId),
            id: id
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/SysAnalysis/Index",
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

    var setAnalysis = function (pageIndex, name) {
        var sendData = {
            eId: id,
            materialIdArray: getList(),
            IsAnalyse: ko.utils.unwrapObservable(SelTypeModel.SelectedId)
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/SysAnalysis/SetAnalysis",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                    return;
                };
                data = jsonResult;
                var list = getDataKO(1, "");
                updateData(list);
                dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                    if (jsonResult.code != 0) {
//                        self.close();
                    }
                }
                }]);
            }
        })
        return data;
    }
    //获取复选框id字符串,例如：1,2,3
    var getList = function () {
        var arr = new Array();
        $("#simple-table").find("input[name='cbx']:checkbox").each(function () {
            if (this.checked == true) {
                var parentItemID = $(this).val();
                if ($.inArray(parentItemID, arr) == -1) {
                    arr.push(parentItemID);
                }
            }
        });
        return arr.join(",");
    }
//    var getChecked = function () {
//        var arr = new Array();
//        $(".col-md-12").find("input[name='Analysis']:checkbox").each(function () {
//            if (this.checked == true) {
//                var parentItemID = $(this).val();
//                if ($.inArray(parentItemID, arr) == -1) {
//                    arr.push(parentItemID);
//                }
//            }
//        });
//        return arr.join(",");
//    }
    //ajax获取数据
    var InitData = function () {
        var sendData = {
            id: id
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/SysAnalysis/GetModel",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                enterpriseName(jsonResult.ObjModel.EnterpriseName);
            }
        })
    }
    //分页、搜索时更新数据源
    var updateData = function (list) {
        vm.vmAnalysis.ObjList(list.ObjList());
        vm.vmAnalysis.pageSize(list.pageSize());
        vm.vmAnalysis.totalCounts(list.totalCounts());
        vm.vmAnalysis.pageIndex(list.pageIndex());
    }

    //自定义绑定-分页控件
    ko.bindingHandlers.analysisPagerBH = {
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
                        this.pageSize = parseInt(vm.vmAnalysis.pageSize());
                        this.totalCounts = parseInt(vm.vmAnalysis.totalCounts());
                    }
                }
            });
        }
    };
    var vm = {
        binding: function () {
            //初初化导航状态
            loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            id = qs.querystring("Enterprise_Info_ID");
            InitData();
            $("#btn1").css({ "display": "" });
            $("#btn2").css({ "display": "none" });
            SelTypeModel.SelTypeArray([{ "Text": "未停止解析的产品", "Value": 1 }, { "Text": "已停止解析的产品", "Value": 2}]);
            SelTypeModel.SelectedId.subscribe(function () {
                var TypeIndex = ko.utils.unwrapObservable(SelTypeModel.SelectedId);
                if (TypeIndex == 1) {
                    SelType(false);
                    $("#btn1").css({ "display": "" });
                    $("#btn2").css({ "display": "none" });
                    var list = getDataKO(1, "");
                    updateData(list);
                } else if (TypeIndex == 2) {
                    SelType(false);
                    $("#btn1").css({ "display": "none" });
                    $("#btn2").css({ "display": "" });       
                    var list = getDataKO(1, "");
                    updateData(list);
                } else {
                    SelType(true);
                }
            });
            //初始化brand列表数据
            vm.vmAnalysis = getDataKO(1, "");
        },
        goBack: function () {
            router.navigateBack();
        },
        vmAnalysis: null,
        setAnalysis: setAnalysis,
        getList: getList,
        name: name,
        enterpriseName: enterpriseName,
//        vmAnalysisType: vmAnalysisType,
        SelTypeModel: SelTypeModel,
        SelType: SelType,
//        onchangeType: onchangeType,
//        GetAnalysisTypeList: GetAnalysisTypeList,
        //        mouseoverFun: mouseoverFun,
        //        mouseoutFun: mouseoutFun,
        InitData: InitData,
        loginInfo: loginInfo

    }
    return vm;

});