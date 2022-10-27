define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo', 'plugins/dialog', 'utils'],
function (router, ko, km, $, jq, loginInfo, dialog, utils) {
    //    var moduleInfo = {
    //        moduleID: '24300',
    //        parentModuleID: '20000'
    //    }
    //    //var searchTitle = ko.observable();


//    var viewModel = function () {
//        var self = this;
//        self.list = ko.observableArray([]);
//        getData();

//        //ajax获取数据
//        function getData() {
//            var data;
//            $.ajax({
//                type: "POST",
//                url: "/Admin_EnterpriseInfo/MbIndex",
//                contentType: "application/json;charset=utf-8", //必须有
//                dataType: "json", //表示返回值类型，不必须
//                //data: JSON.stringify(sendData),
//                async: false,
//                success: function (jsonResult) {
//                    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
//                        return;
//                    };
//                    self.list(jsonResult.ObjList);
//                    //data = jsonResult;
//                }
//            });
//            //return data;
//        }
//    }
    //    return viewModel;
    //ajax获取数据
    var getData = function () {
        var data;
        $.ajax({
            type: "POST",
            url: "/Admin_EnterpriseInfo/MbIndex",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            //data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                    return;
                };
                data = jsonResult;
            }
        });
        return data;
    }
    //把获取的ajax数据转化为ko
    var getDataKO = function () {
        var list = km.fromJS(getData());
        return list;
    }
    var lists = function () {
        var self = this;
        self.VMMB = getDataKO;
    };
    ko.applyBindings(new lists());
});