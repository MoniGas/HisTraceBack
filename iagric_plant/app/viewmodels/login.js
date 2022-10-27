define(['plugins/router', 'knockout', 'knockout.mapping', 'bootbox'],
function (router, ko, km, bootbox) {
    //****************************************************************************************************//
    var vmNewsModels = {
        newsModelsArray: ko.observableArray(),
        selectedOption: ko.observable()
    }
    //****************************************************************************************************//
    var View_EnterpriseInfoUser = function () {
        var self = this;
        //登录名
        self.LoginName = ko.observable('');
        //密码
        self.LoginPassWord = ko.observable('');
        //登录类型
        self.LoginType = ko.observable('');
        self.reset = function () {
            self.LoginName('');
            self.LoginPassWord('');
            self.LoginType('');
        }
        //***************************************登录**********************************************//
        self.Login = function () {
            //按钮变为正在提交字样
            //$("#saveData").addClass("disabled").html("正在提交，请稍候");
            var sendData = {
                uName: self.LoginName(),
                uPwd: self.LoginPassWord(),
                loginType: self.LoginType
            }
            $.ajax({
                type: "POST",
                url: "/Interface/Index",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                data: JSON.stringify(sendData),
                async: false,
                success: function (jsonResult) {
                    //var obj = JSON.stringify(jsonResult);
                    if (jsonResult.code == -1) {
                        bootbox.alert({
                            title: "提示",
                            message: jsonResult.Msg,
                            callback: function () {
                                window.location.href = "/";
                            }
                        });
                        return;
                    }
                    else {
                        alert(jsonResult.Msg);
                    }
                }
            })
        };
    }

    //****************************************************************************************************//

    //****************************************************************************************************//
    var vm = {
        activate: function () {
        },
        binding: function () {
            vm.View_EnterpriseInfoUser = new View_EnterpriseInfoUser();
        },
        bindingComplete: function () {
        },
        attached: function (view, parent) {
        },
        compositionComplete: function (view) {
        },
        detached: function (view) {

        },
        saveData: function () {
            if (vm.View_EnterpriseInfoUser.errors().length > 0) {
                vm.View_EnterpriseInfoUser.errors.showAllMessages();
                return;
            }
        },
        goBack: function () {
            router.navigateBack();
        },
        View_EnterpriseInfoUser: null,
        View_EnterpriseInfoUser: View_EnterpriseInfoUser
    }
    return vm;
    //****************************************************************************************************//
})