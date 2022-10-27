define(['plugins/dialog', 'knockout', 'jquery-ui', 'knockout.mapping', 'utils', 'logininfo', 'jquery.fileDownload'],
    function (dialog, ko, jqueryui, km, utils, loginInfo, jfd) {
        var materialId = ko.observable();
        var vmFormula = function () {
            var self = this;

            self.settingId = ko.observable();
            self.FormulaDetail = ko.observableArray();

            //配方
            self.FormulaList = {
                FormulaArray: ko.observableArray(),
                SelectedId: ko.observable()
            }
            self.FormulaList.SelectedId.subscribe(function () {
                var sendData = {
                    formulaId: self.FormulaList.SelectedId()
                };
                $.ajax({
                    type: "POST",
                    url: "/Admin_Formula/GetSubList",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                            return;
                        }
                        self.FormulaDetail(jsonResult.ObjList);
                    }
                });
            });

            self.init = function (settingId, materialId) {
                self.settingId(settingId);
                var sendData = {
                    materialId: materialId
                };
                $.ajax({
                    type: "POST",
                    url: "/Admin_Formula/GetSelectList",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                            return;
                        }
                        self.FormulaList.FormulaArray(jsonResult.ObjList);
                    }
                });
            }
            self.SelectOrigin = function () {
                var sendData = {
                    settingId: self.settingId(),
                    formulaId: self.FormulaList.SelectedId()
                };
                $.ajax({
                    type: "POST",
                    url: "/Admin_Formula/GetOriginByFormula",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                            if (jsonResult.code == 1) {
                                self.close();
                            }
                        }
                        }]);
                    },
                    error: function (e) {
                    }
                });
            }
        }

        vmFormula.prototype.binding = function () {
            $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
        }
        vmFormula.prototype.close = function () {
            dialog.close(this);
        }
        var vmObj;
        vmFormula.show = function (settingId, materialId) {
            vmObj = new vmFormula();
            vmObj.init(settingId, materialId);
            return dialog.show(vmObj);
        };
        return vmFormula;
    }
);