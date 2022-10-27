define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.querystring', 'bootbox', './materialaddsimple', './formula_add'],
    function (router, ko, $, kv, utils, loginInfo, dialog, jqueryui, qs, bootbox, materialaddsimple, formula_add) {
        //获取当前时间
        var GetNowDate = function () {
            var date = new Date();
            var seperator1 = "-";
            var strYear = date.getFullYear();
            var strMonth = (date.getMonth() + 1).toString();
            var strDate = date.getDate();
            if (strMonth >= 1 && strMonth <= 9) {
                strMonth = "0" + strMonth;
            }
            if (strDate >= 0 && strDate <= 9) {
                strDate = "0" + strDate;
            }
            var currentdate = strYear + seperator1 + strMonth + seperator1 + strDate;
            return currentdate;
        }
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
        var selFormulaName = ko.observable(false);
        var FormulaName = ko.observable('');
        FormulaName.subscribe(function () {
            selFormulaName(FormulaName() == "" || FormulaName() == undefined);
        });

        var MaterialName = ko.observable('');
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
        var FormulaSpec = ko.observable('');

        var selAddDate = ko.observable(false);
        var AddDate = ko.observable(GetNowDate());
        AddDate.subscribe(function () {
            selAddDate(AddDate() == "" || AddDate() == undefined);
        });
        var NextButton = function () {
            selFormulaName(FormulaName() == "" || FormulaName() == undefined);
            selMaterial(MaterialList.SelectedId() == undefined);
            selAddDate(AddDate() == "" || AddDate() == undefined);
            if (!selFormulaName() && !selMaterial() && !selAddDate()) {
                $("#First").hide();
                $("#Second").show();
                $("#li1").removeClass('active');
                $("#li1").addClass('visited');
                $("#li2").addClass('active');
                MaterialName($("#selectMaterial").find("option:selected").text());
            }
        }

        var AddOrigin = function () {
            formula_add.show().then(function (single) {
                if (single != undefined) {
                    FormulaDetail.push(single);
                    selHasOrigin(false);
                }
            });
        }
        var EditOrigin = function (data, event) {
            formula_add.show(data).then(function (single) {
                if (single != undefined) {
                    var index = FormulaDetail.indexOf(data);
                    FormulaDetail.splice(index, 1);
                    FormulaDetail.splice(index, 0, single);
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
        var TopSubPageClick = function () {
            $("#First").show();
            $("#Second").hide();
            $("#li1").removeClass('visited');
            $("#li1").addClass('active');
            $("#li2").removeClass('active');
        }
        var AddFormula = function () {
            var currentObj = $(event.target);
            currentObj.blur();
            selMaterial(MaterialList.SelectedId() == undefined);
            selFormulaName(FormulaName() == '' || FormulaName() == undefined);
            selHasOrigin(FormulaDetail().length <= 0);
            if (!selFormulaName() && !selMaterial() && !selHasOrigin()) {
                sendData = {
                    formulaName: FormulaName(),
                    materialId: MaterialList.SelectedId(),
                    spec: FormulaSpec(),
                    strSub: JSON.stringify(FormulaDetail()).replace('[[', '[').replace(']]', ']')
                };
                $.ajax({
                    type: "POST",
                    url: "/Admin_Formula/Add",
                    contentType: "application/json;charset=utf-8", //必须有
                    dataType: "json", //表示返回值类型，不必须
                    data: JSON.stringify(sendData),
                    async: false,
                    success: function (jsonResult) {
                        dialog.showMessage(jsonResult.Msg, '系统提示', [{ title: '确定', callback: function () {
                            if (jsonResult.code == 1) {
                                FormulaName('');
                                FormulaSpec('');
                                MaterialList.SelectedId(undefined);
                                FormulaDetail(new Array());
                                selMaterial(false);
                                selAddDate(false);
                                selHasOrigin(false);
                                selFormulaName(false);
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
        var vm = {
            binding: function () {
                MaterialList.MaterialArray(GetMaterialList());

                FormulaName('');
                FormulaSpec('');
                MaterialList.SelectedId(undefined);
                FormulaDetail(new Array());
                selMaterial(false);
                selAddDate(false);
                selHasOrigin(false);
                selFormulaName(false);
            },
            goBack: function () {
                router.navigateBack();
            },
            selFormulaName: selFormulaName,
            FormulaName: FormulaName,
            selMaterial: selMaterial,
            MaterialList: MaterialList,
            AddMaterial: AddMaterial,
            FormulaSpec: FormulaSpec,
            selAddDate: selAddDate,
            AddDate: AddDate,
            MaterialName: MaterialName,
            NextButton: NextButton,
            AddOrigin: AddOrigin,
            EditOrigin: EditOrigin,
            DelOrigin: DelOrigin,
            FormulaDetail: FormulaDetail,
            selHasOrigin: selHasOrigin,
            mouseoverFun: mouseoverFun,
            mouseoutFun: mouseoutFun,
            TopSubPageClick: TopSubPageClick,
            AddFormula: AddFormula
        }
        return vm;
    }
);