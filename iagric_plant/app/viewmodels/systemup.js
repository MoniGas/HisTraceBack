define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo'],
function (router, ko, km, $, jq, loginInfo) {
    var moduleInfo = {
        moduleID: '27000',
        parentModuleID: '10002'
    }
    var list = new Array();
    var Getlist1 = function () {
        var data;
        $.ajax({
            type: "POST",
            url: "/Public/GetSysUp",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            async: false,
            success: function (jsonResult) {
                data = jsonResult;
            },
            Error: function (Error) {
                alert(Error);
            }
        });
        return data;
    }
    var vmsystemup = function () {
        var self = this;
        self.modual = ko.observable();
        self.value = ko.observable();
        self.date = ko.observable();
        self.Getlist = function () {
            var data;
            $.ajax({
                type: "POST",
                url: "/Public/GetSysUp",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                async: false,
                success: function (jsonResult) {
                    data = jsonResult;
                },
                Error: function (Error) {
                    alert(Error);
                }
            });
            return data;
        }
        self.init = function () {
            //            list = Getlist();
            //            alert(list[0].value);
            //            self.modual(JSON.stringify(jsoncontent.sysItem[0].modual));
            //            self.value(JSON.stringify(jsoncontent.sysItem[1].value));
            //            self.date(JSON.stringify(jsoncontent.sysItem[2].date));
        }
    }
    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            vm.vmsystemup = new vmsystemup();
            vm.vmsystemup.init();
        },
        goBack: function () {
            router.navigateBack();
        },
        vmsystemup: null,
        list: list
    }
    return vm;
});