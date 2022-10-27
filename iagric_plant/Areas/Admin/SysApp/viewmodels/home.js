define(['knockout'], function (ko) {
    var vm = {
        activate: function () {
            //router.navigate('#guide1');
        },
        binding: function () {

            var sendData = {
        }
//        $.ajax({
//            type: "POST",
//            url: "/Logined/Guide",
//            contentType: "application/json;charset=utf-8", //必须有
//            dataType: "json", //表示返回值类型，不必须
//            data: JSON.stringify(sendData),
//            async: false,
//            success: function (jsonResult) {
//                //                if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
////                    return;
////                };
//                if (jsonResult.code == "True") {
//                    vm.homePage = "viewmodels/guide1";
//                }
//                else {
//                    vm.homePage = "viewmodels/batch";
//                }
//            }
//        });
    },
    homePage: null
}
return vm;
});