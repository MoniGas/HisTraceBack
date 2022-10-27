define(['plugins/router', 'durandal/system', 'knockout', 'knockout.mapping', 'jquery', 'jqPaginator', 'utils', 'bootstrap-datepicker', 'jquery.poshytip', 'logininfo', 'plugins/dialog', 'jquery.querystring'],
function (router, system, ko, km, $, jq, utils, bdp, poshytip, loginInfo, dialog, qs) {
    var subid = "";
    var navigateTo = function () {
        router.navigate('requestsettingmanage');
    }
    var navigateToSetting = function () {
        router.navigate('requestcodesetting?subid=' + subid + '&type=1');
    }
    var ViewModel = {
        binding: function () {
            subid = qs.querystring("subid");
        },
        navigateTo: navigateTo,
        navigateToSetting: navigateToSetting
    }
    return ViewModel;
});