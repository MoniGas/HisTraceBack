define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'logininfo', 'plugins/dialog', 'jquery-ui', 'jquery.querystring', 'bootbox'],
    function (router, ko, $, kv, utils, loginInfo, dialog, jqueryui, qs, bootbox) {
        var vmObj;
        //获取原料
        var GetOriginList = function () {
            var data;
            $.ajax({
                type: "POST",
                url: "/AdminOrigin/GetOriginList",
                contentType: "application/json;charset=utf-8", //必须有
                dataType: "json", //表示返回值类型，不必须
                async: false,
                success: function (jsonResult) {
                    data = jsonResult.ObjList;
                }
            });
            return data;
        }
        var vmFormulaAdd = function () {
            var self = this;

            self.selOrigin = ko.observable(false);
            //原料列表
            self.OriginList = {
                OriginArray: ko.observableArray(),
                SelectedId: ko.observable()
            }
            self.OriginList.SelectedId.subscribe(function () {
                self.selOrigin(self.OriginList.SelectedId() == undefined);
            });

            self.selSupplier = ko.observable(false);
            self.selSupplierTitle = ko.observable();
            //供货商
            self.Supplier = ko.observable();
            self.Supplier.subscribe(function () {
                self.selSupplier(self.Supplier() == '' || self.Supplier() == undefined);
            });

            //用量
            self.Amount = ko.observable();

            //等级
            self.Level = ko.observable();

            //批次
            self.Batch = ko.observable();

            //厂地
            self.Factory = ko.observable();
            self.init = function (data) {
                if (data != null && data != undefined) {
                    self.OriginList.SelectedId(data.OriginID);
                    self.Supplier(data.Supplier);
                    self.Amount(data.Amount);
                    self.Level(data.Level);
                    self.Batch(data.Batch);
                    self.Factory(data.Factory);
                }
            }
            self.AddOrigin = function () {
                self.selOrigin(self.OriginList.SelectedId() == undefined);
                self.selSupplier(self.Supplier() == '' || self.Supplier() == undefined);
                self.selSupplierTitle('请填写供货商！');
                if (!self.selOrigin() && !self.selSupplier()) {
                    var single = {
                        OriginID: self.OriginList.SelectedId(),
                        OriginName: $("#selectOrigin").find("option:selected").text(),
                        Supplier: self.Supplier(),
                        Amount: self.Amount(),
                        Level: self.Level(),
                        Batch: self.Batch(),
                        Factory: self.Factory()
                    };
                    self.OriginList.SelectedId(undefined);
                    self.Supplier('');
                    self.Amount('');
                    self.Level('');
                    self.Batch('');
                    self.Factory('');
                    self.selOrigin(false);
                    self.selSupplier(false);
                    self.closeOK(single);
                }
            }
            self.cancle = function () {
                self.close();
            }
        }
        vmFormulaAdd.prototype.binding = function () {
            $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
        }
        vmFormulaAdd.prototype.close = function () {
            dialog.close(this);
        }
        vmFormulaAdd.prototype.closeOK = function (single) {
            dialog.close(this, single);
        }
        vmFormulaAdd.show = function (data) {
            vmObj = new vmFormulaAdd();
            vmObj.OriginList.OriginArray(GetOriginList());
            vmObj.init(data);
            return dialog.show(vmObj);
        };
        return vmFormulaAdd;
    }
);