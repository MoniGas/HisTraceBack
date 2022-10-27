define(['plugins/router', 'knockout', 'jquery', 'knockout.validation', 'utils', 'plugins/dialog', 'jquery-ui', 'baidumap'],
    function (router, ko, $, kv, utils, dialog, jqueryui, baidumap) {

        var vmDealer = function (latget, lngget) {
            var self = this;
            self.lat = latget;
            self.lng = lngget;
            self.map = null;
            self.initMap = function () {
                //alert('dfdf');
                self.map = new BMap.Map("map-canvas");
                if (self.lat == null && self.lng == null) {
                    self.lat = "116.404";
                    self.lng = "39.915";
                }
                var pt = new BMap.Point(self.lat, self.lng);
                self.map.centerAndZoom(pt, 15);
                self.map.addControl(new BMap.NavigationControl());  //添加默认缩放平移控件
                self.map.enableScrollWheelZoom();    //启用滚轮放大缩小，默认禁用
                self.map.enableContinuousZoom();    //启用地图惯性拖拽，默认禁用
                var marker = new BMap.Marker(pt);
                self.map.addOverlay(marker);
            }
        }
        vmDealer.prototype.binding = function () {
            var self = this;
            $("#divMessageBox").draggable({ opacity: 0.35, handle: "#divMessageBoxHeader" });
            setTimeout(function () {
                self.initMap();
            }, 300);
        }

        vmDealer.prototype.close = function () {
            dialog.close(this);
        }
        vmDealer.show = function (location) {
            if (location == null) {
                location = "116.404, 39.915";
            }
            var l = location.split(",");
            var vmObj = new vmDealer(l[0], l[1]);
            return dialog.show(vmObj);
        };
        return vmDealer;
    });