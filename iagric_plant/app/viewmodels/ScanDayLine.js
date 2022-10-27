define(['plugins/router', 'knockout', 'knockout.mapping', 'jquery', 'bootstrap-datepicker', 'jqPaginator', 'logininfo', 'plugins/dialog', 'utils'], function (router, ko, km, $, bdp, jq, loginInfo, dialog, utils) {
    var moduleInfo = {
        moduleID: '112000',
        parentModuleID: '110000'
    }
    var now = new Date();
    var firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
    var beginTime = ko.observable(utils.dateFormat(firstDay, 'yyyy-MM-dd'));
    //now.setDate(firstDay + 2);
    var endTime = ko.observable(utils.dateFormat(now, 'yyyy-MM-dd'));

    //ajax获取数据
    var getData = function (sdate, edate) {
        var sendData = {
            sdate: sdate,
            edate: edate
        }
        var data;
        $.ajax({
            type: "POST",
            url: "/DataSum/ScanDayLines",
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

    var getDataKO = function (sdate, edate) {
        var da = getData(sdate, edate);
        var list = da; //km.fromJS(da);
        return list;
    }

    var searchTime = function () {
        var list = getDataKO(beginTime(), endTime());
        updateData(list);
    }

    /*****************报表*************************/
    function getChartData() {
        var container = 'chartDIV';
        var title = '消费者拍码情况';
        var units = '拍码次数';
        var lineName = '日拍码情况';
        var time = $("#timeInfo").val().split(",");
        var data = [];
        $.each($("#countInfo").val().split(","), function (i, v) {
            data.push(Number(v));
        });
        var seriesdata = [
                {
                    name: '拍码情况',
                    data: data
                }];
        var chart1 = new Highcharts.Chart({
            chart: {
                renderTo: container,
                width: 503,
                hight: 100,
                type: 'line'
            },
            title: {
                text: title
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: time
            },
            yAxis: {
                title: {
                    text: "扫码次数"
                },
                allowDecimals: false //是否允许刻度有小数
            },
            tooltip: {
                enabled: true,
                formatter: function () {
                    return '<b>' + this.series.name + '</b><br/>' + this.x + ': ' + this.y + '次';
                }
            },
            plotOptions: {
                line: {
                    dataLabels: {
                        enabled: true
                    },
                    enableMouseTracking: true
                }
            },
            series: seriesdata
        });
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
            // loginInfo.setActiveItemToParent(moduleInfo.moduleID, moduleInfo.parentModuleID);
            $('#beginTime').datepicker({
                language: 'cn',
                autoclose: true,
                todayHighlight: true,
                format: 'yyyy-mm-dd'
            }).on('changeDate', function (event) {
                var starttime = $("#beginTime").val();
                $("#endTime").datepicker('setStartDate', starttime);
                $("#beginTime").datepicker('hide');
            });
            $('#endTime').datepicker({
                language: 'cn',
                autoclose: true,
                todayHighlight: true,
                format: 'yyyy-mm-dd'
            }).on('changeDate', function (event) {
                var starttime = $("#beginTime").val();
                var endtime = $("#endTime").val();
                $("#beginTime").datepicker('setEndDate', endtime);
                $("#endTime").datepicker('hide'); 
            });
            getData();
            getChartData();
            getChartDataPie();
        },
        goBack: function () {
            router.navigateBack();
        },
        beginTime: beginTime,
        endTime: endTime,
        searchTime: searchTime
    }
    return vm;
});
