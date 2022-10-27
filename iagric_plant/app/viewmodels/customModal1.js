define(['plugins/dialog', 'knockout', 'jquery-ui'], function (dialog, ko, jqueryui) {
    var vm = {
        binding: function () {
            //alert($("#divMessageBox").attr("id"));
            $("#divMessageBox").draggable({ opacity: 0.75, handle: "#divMessageBoxHeader" });
            //alert($('#select1').attr("id"));

        },
        bindingComplete: function () {
           
        },
        attached: function (view, parent) {
           
        },
        compositionComplete: function (view) {
            //$('#select1').chosen();
        },
        close: function () {
            dialog.close(this);
        }
    };
    return vm;
});