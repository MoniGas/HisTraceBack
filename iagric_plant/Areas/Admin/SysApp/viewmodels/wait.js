define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo'],
function (dialog, ko, jqueryui, km, utils, loginInfo) {

    var vmCode = function () {

    }
    vmCode.close = function () {
        dialog.close(this);
    };
    vmCode.show = function () {
        var vmObj = new vmCode();
        return dialog.show(vmObj);
    };
    return vmCode;
})