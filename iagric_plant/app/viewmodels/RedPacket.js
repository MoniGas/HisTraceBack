define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo', 'plugins/dialog', 'utils', 'jquery.querystring'], function (router, ko, km, $, jq, loginInfo, dialog, utils, qs) {
    var moduleInfo = {
        moduleID: '25400',
        parentModuleID: '25000'
    }
    //ajax获取数据
    var getData = function (enterpriseId, settingId) {
        var sendData = {
            enterpriseId: enterpriseId,
            settingId: settingId
        }
        $.ajax({
            type: "POST",
            url: "/RequestCodeMa/GetRedPacket",
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
    var vm = {
        binding: function () {
            var enterpriseId = qs.querystring("enterpriseId");
            var settingId = qs.querystring("settingId");
            var data = getData(enterpriseId, settingId);
            $("#redPacketFrame").attr("src", data.url);
//            $("#redPacketFrame").height($(window).height() - $("#topheader").height() - 160 - $("#tabMenu").height());
        },
        goBack: function () {
            router.navigateBack();
        }
    }
    return vm;
});