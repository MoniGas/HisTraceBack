define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'utils', 'logininfo'],
function (router, ko, km, $, jq, utils, loginInfo) {
    var searchTitle = ko.observable();
    var moduleInfo = {
        moduleID: '10000',
        parentModuleID: '0'
    }

    var navigateTo = function (routeName, data, event) {
        switch (routeName) {
            case 'guide1':
                router.navigate('#' + routeName + '?pc=10000');
                break;
            default:
                router.navigate('#' + routeName);
        }
    }

    var vm = {
        binding: function () {
            //初初化导航状态
            loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
        },
        navigateTo: navigateTo,
        loginInfo: loginInfo
    }
    return vm;
});