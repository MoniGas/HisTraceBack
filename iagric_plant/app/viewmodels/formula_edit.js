define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.querystring', 'bootbox', './formula_add'],
    function (router, ko, $, kv, utils, loginInfo, dialog, jqueryui, qs, bootbox, formula_add) {
        //获取产品
        var GetMaterialList = function () {
            var data;
            $.ajax({
                type: "POST",
                url: "/Admin_Request/SearchNameList",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                async: false,
                success: function (jsonResult) {
                    if (loginInfo.isLoginTimeout(jsonResult.code, jsonResult.Msg)) {
                        return;
                    };
                    data = jsonResult.ObjList;
                }
            });
            return data;
        }
        var MainId = ko.observable();
        var selFormulaName = ko.observable(false);
        var FormulaName = ko.observable();
        FormulaName.subscribe(function () {
            selFormulaName(FormulaName() == "" || FormulaName() == undefined);
        });

        var MaterialName = ko.observable();
        var selMaterial = ko.observable(false);
        //产品列表
        var MaterialList = {
            MaterialArray: ko.observableArray(),
            SelectedId: ko.observable()
        }
        MaterialList.SelectedId.subscribe(function () {
            selMaterial(MaterialList.SelectedId() == undefined);
            MaterialName($("#selectMaterial").find("option:selected").text());
        });
        var AddMaterial = function () {
            var currentObj = $(event.target);
            currentObj.blur();
            materialaddsimple.show().then(function (id, brandId) {
                MaterialList.MaterialArray(GetMaterialList());
                MaterialList.SelectedId(id);
            });
        }

        var FormulaSpec = ko.observable();

        var AddDate = ko.observable();

        var AddOrigin = function () {
            formula_add.show().then(function (single) {
                if (single != undefined) {
                    FormulaDetail.push(single);
                }
            });
        }
        var EditOrigin = function (data, event) {
            formula_add.show(data).then(function (single) {
                if (single != undefined) {
                    var index = FormulaDetail.indexOf(data);
                    FormulaDetail.splice(index, 1);
                    FormulaDetail.splice(index, 0, single);
                    selHasOrigin(false);
                }
            });
        }
        var DelOrigin = function (data, event) {
            dialog.showMessage('确定要删除该原料吗？', '系统提示', [{ title: '确定', callback: function () {
                var index = FormulaDetail.indexOf(data);
                FormulaDetail.splice(index, 1);
            }
            }, { title: '取消', callback: function () { } }]);
        }
        var FormulaDetail = ko.observableArray();
        var selHasOrigin = ko.observable(false);

        var mouseoverFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowAll = self.find("div[eleflag='ShowAll']");
            ShowAll.css({ "display": "" });
        }
        var mouseoutFun = function (data, event) {
            var self = $(event.target).closest('tr');
            var ShowAll = self.find("div[eleflag='ShowAll']");
            ShowAll.css({ "display": "none" });
        }
        var AddFormula = function () {
            var currentObj = $(event.target);
            currentObj.blur();
            selMaterial(MaterialList.SelectedId() == undefined)
            selFormulaName(FormulaName() == '' || FormulaName() == undefined)
            selHasOrigin(FormulaDetail().length <= 0);
            if (!selFormulaName() && !selMaterial() && !selHasOrigin()) {
                sendData = {
                    mainId: MainId(),
                    formulaName: FormulaName(),
                    materialId: MaterialList.SelectedId(),
                    spec: FormulaSpec(),
                    strSub: JSON.stringify(FormulaDetail()).replace('[[', '[').replace(']]', ']')
                };
                $.ajax({
                    type: "POST",
                    url: "/Admin_Formula/Edit",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                            if (jsonResult.code == 1) {
                                router.navigate('#formula?menu=29000');
                            }
                        }
                        }]);
                    },
                    error: function (e) {
                    }
                });
            }
        }
        var init = function (mainId, materialId, formulaName, spec, addDate) {
            MainId(mainId);
            MaterialList.MaterialArray(GetMaterialList());
            MaterialList.SelectedId(materialId);
            FormulaName(formulaName);
            AddDate(addDate);
            FormulaSpec(spec);
            var sendData = {
                formulaId: MainId()
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
                    };
                    FormulaDetail(jsonResult.ObjList);
                }
            })
        }
        var vm = {
            binding: function () {
                var mainId = qs.querystring("mainId");
                var materialId = qs.querystring("materialId");
                var formulaName = qs.querystring("formulaName");
                var spec = qs.querystring("spec");
                var addDate = qs.querystring("addDate");
                if (spec == "null")
                    spec = '';
                init(mainId, materialId, formulaName, spec, addDate.split(' ')[0]);
            },
            goBack: function () {
                router.navigateBack();
            },
            selFormulaName: selFormulaName,
            FormulaName: FormulaName,
            selMaterial: selMaterial,
            MaterialList: MaterialList,
            AddMaterial: AddMaterial,
            AddDate: AddDate,
            FormulaSpec: FormulaSpec,
            MaterialName: MaterialName,
            AddOrigin: AddOrigin,
            EditOrigin: EditOrigin,
            DelOrigin: DelOrigin,
            FormulaDetail: FormulaDetail,
            selHasOrigin: selHasOrigin,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun,
            AddFormula: AddFormula
        }
        return vm;
    }
);