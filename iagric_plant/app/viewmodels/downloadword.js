define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'logininfo'],
function (router, ko, km, $, jq, loginInfo) {
    var moduleInfo = {
        moduleID: '25000',
        parentModuleID: '10002'
    }
    var vm = {
        binding: function () {
            //初初化导航状态
            //loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
        },
        goBack: function () {
            router.navigateBack();
        }
    }
    return vm;
});