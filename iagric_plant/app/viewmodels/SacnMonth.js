define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'bootstrap-datepicker', 'jqPaginator', 'logininfo', 'plugins/dialog', 'utils'], function (router, ko, km, $, bdp, jq, loginInfo, dialog, utils) {
    var moduleInfo = {
        moduleID: '111000',
        parentModuleID: '110000'
    }
    var addDate = ko.observable(utils.dateFormat(new Date(), 'yyyy'));
    //ajax获取数据
    var getData = function (years) {
        var sendData = {
            year: years
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/DataSum/DataSumList",
            contentType: "application/json;charset=utf-8", //必须有
            dataType: "json", //表示返回值类型，不必须
            data: JSON.stringify(sendData),
            async: false,
            success: function (jsonResult) {
                if (jsonResult.code === "1") {
                    $("#timeInfo").val(jsonResult.timeInfo);
                    $("#countInfo").val(jsonResult.countInfo);
                    $("#areaInfo").val(jsonResult.pieArea);
                    $("#areacount").val(jsonResult.piecount);
                }
                data = jsonResult;
            }
        });
        return data;
    }
    //搜索时更新数据源
    var updateData = function (list) {
        if (list == undefined) {
            getData();
        }
        getChartData();
        getChartDataPie();
    }

    var getDataKO = function (originName) {
        var da = getData(originName);
        var list = da; //km.fromJS(da);
        return list;
    }

    var searchYear = function () {
        var list = getDataKO(addDate());
        updateData(list);
    }

  
    /*****************报表*************************/
    function getChartData() {
        //var container = 'chartDIV';
        //var title = '消费者拍码情况';
        //        var units = '拍码次数';
        //        var lineName = '月拍码情况';
        var time = $("#timeInfo").val().split(",");
        var data1 = [];
        $.each($("#countInfo").val().split(","), function (i, v) {
            data1.push(Number(v));
        });

        var chart = {
            type: 'column'
        };
        var title = {
            text: '月拍码情况'
        };
        var subtitle = {
            text: ''
        };
        var xAxis = {
            categories: time,
            crosshair: true
        };
        var yAxis = {
            min: 0,
            title: {
                text: '拍码情况 (次数)'
            }
        };
        var tooltip = {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
            '<td style="padding:0"><b>{point.y:.0f} 次</b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        };
        var plotOptions = {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
        };
        var credits = {
            enabled: false
        };

        var series = [{
            name: '拍码情况',
            data: data1
        }];

        var json = {};
        json.chart = chart;
        json.title = title;
        json.subtitle = subtitle;
        json.tooltip = tooltip;
        json.xAxis = xAxis;
        json.yAxis = yAxis;
        json.series = series;
        json.plotOptions = plotOptions;
        json.credits = credits;
        $('#chartDIV').highcharts(json);
    }

    function getChartDataPie() {

        var chart = {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false
        };
        var title = {
            text: '消费者拍码区域分析'
        };
        var tooltip = {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        };
        var plotOptions = {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    }
                }
            }
        };
        var series = [{
            type: 'pie',
            name: 'Browser share',
            data: [
                ['Firefox', 45.0],
                ['IE', 26.8],
                {
                    name: 'Chrome',
                    y: 12.8,
                    sliced: true,
                    selected: true
                },
                ['Safari', 8.5],
                ['Opera', 6.2],
                ['Others', 0.7]
            ]
        }];
        var categories = $("#areaInfo").val().split(",");
        var piedata = [];
        $.each($("#areacount").val().split(","), function (i, v) {
            var tmp = [];
            tmp.push(categories[i], Number(v));
            piedata.push(tmp);
        });
        var seriesPie = [{
            type: 'pie',
            name: '',
            data: piedata
        }];

        var container = 'chartDIVPie';
        var colors = Highcharts.getOptions().colors;

        var data = [];
        $.each($("#areacount").val().split(","), function (i, v) {
            data.push(Number(v));
        });

        var json = {};
        json.chart = chart;
        json.title = title;
        json.tooltip = tooltip;
        json.series = seriesPie;
        json.plotOptions = plotOptions;
        $('#chartDIVPie').highcharts(json);
    }

    var vm = {
        router: loginInfo.router,
        binding: function () {
//            //初始化导航状态
//            loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            $('#AddDate').datepicker({
                startView: 2,
                maxViewMode: 2,
                minViewMode: 2,
                autoclose: true,
                endDate: new Date(),
                language: 'cn'
            });
            updateData();
        },
        goBack: function () {
            router.navigateBack();
        },
        AddDate: addDate,
        searchYear: searchYear
    }
    return vm;
});
