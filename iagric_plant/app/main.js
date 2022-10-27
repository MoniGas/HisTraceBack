requirejs.config({
    //urlArgs: "v=" + (new Date()).getTime(),
    //    map: {
    //        '*': {
    //            'css': '../lib/require/css'
    //        }
    //    },
    paths: {
        'text': '../lib/require/text',
        //        'domReady': '../lib/require/domReady',
        'durandal': '../lib/durandal/js',
        'plugins': '../lib/durandal/js/plugins',
        'transitions': '../lib/durandal/js/transitions',
        'knockout': '../lib/knockout/knockout-2.3.0',
        'knockout.mapping': '../lib/knockout/knockout.mapping.min',
        'knockout.validation': '../lib/Knockout-Validation-master/dist/knockout.validation.min',
        'bootstrap': '../lib/bootstrap/js/bootstrap',
        'jquery': '../lib/jquery/jquery-1.9.1',
        'jqPaginator': '../lib/jqPaginator/js/jqPaginator',
        //'chosen.jquery': '../lib/bootstrap-chosen/chosen.jquery',
        'kindeditor': '../lib/kindeditor/kindeditor-all-min',
        'kindeditor.zh-CN': '../lib/kindeditor/lang/zh-CN',
        'bootstrap-datepicker': '../lib/date-time/js/bootstrap-datepicker',
        'daterangepicker': '../lib/date-time/js/daterangepicker',
        'jquery.uploadify': '../lib/jquery.uploadify-v3.2/js/jquery.uploadify',
        'webuploader': '../lib/webuploader/js/webuploader',
        'bootbox': '../lib/bootbox/bootbox',
        'jquery.querystring': '../lib/jquery.querystring/jquery.querystring',
        'utils': '../js/utils',
        'logininfo': '../js/logininfo',
        'spin': '../lib/jquery.loadmask.spin/js/spin',
        'jquery.loadmask.spin': '../lib/jquery.loadmask.spin/js/jquery.loadmask.spin',
        'jquery-ui': '../lib/jquery-ui-1.10.4/js/jquery-ui-1.10.4.custom.min',
        'jquery.fileDownload': '../lib/jquery.fileDownload/js/jquery.fileDownload',
        'jquery.poshytip': '../lib/poshytip-1.2/js/jquery.poshytip.min',
        'baidumap': '../lib/baidumap/api',
        'charts': '../lib/highcharts/highcharts'
    },
    shim: {
        'bootstrap': {
            deps: ['jquery'],
            exports: '$.fn.collapse'
        },
        'jqPaginator': {
            deps: ['jquery']
        },
        //        'chosen.jquery': {
        //            deps: ['jquery'],
        //            exports: '$.fn.chosen'
        //        },
        'kindeditor.zh-CN': {
            deps: ['kindeditor']
        },
        'kindeditor': {
            deps: ['../lib/kindeditor/plugins/code/prettify.js']
        },
        //        'kindeditor': {
        //            deps: ['../lib/kindeditor/plugins/code/prettify.js',
        //                                    'css!../lib/kindeditor/themes/default/default.css', 'css!../lib/kindeditor/plugins/code/prettify.css'
        //                                    ]
        //        },
        'bootstrap-datepicker': {
            deps: ['bootstrap']
        },
        'jquery.uploadify': {
            deps: ['jquery', '../lib/jquery.uploadify-v3.2/js/swfobject.js'], //, 
            exports: '$.fn.uploadify'
        },
        'webuploader': {
            deps: ['jquery', '../lib/webuploader/js/webuploader.js'] //, 
        },
        'bootbox': {
            deps: ['bootstrap']
        },
        'spin': {
            deps: ['jquery']
        },
        'jquery.loadmask.spin': {
            deps: ['jquery', 'spin'],
            //exports: '$.fn.mask'
            init: function () {
                return {
                    mask: $.fn.mask,
                    unmask: $.fn.unmask
                }
            }
        },
        'blockUI': {
            deps: ['jquery']
        },
        'jquery-ui': {
            deps: ['jquery']
        },
        'jquery.fileDownload': {
            deps: ['jquery']
        },
        'baidumap': {
            deps: ['../lib/baidumap/getscript']
        },
        'daterangepicker': {
            deps: ['jquery', 'bootstrap', '../lib/date-time/js/moment']
        },
        'jquery.poshytip': {
            deps: ['jquery']
        }
    },
    deps: ['bootstrap']
});

define(['durandal/system', 'durandal/app', 'durandal/viewLocator'], function (system, app, viewLocator) {
    //>>excludeStart("build", true);
    system.debug(true);
    //>>excludeEnd("build");

    app.title = '医疗器械（UDI）服务云平台';

    //specify which plugins to install and their configuration
    app.configurePlugins({
        router: true,
        dialog: true,
        widget: {
            kinds: ['expander']
        }
    });

    app.start().then(function () {
        //Replace 'viewmodels' in the moduleId with 'views' to locate the view.
        //Look for partial views in a 'views' folder in the root.
        viewLocator.useConvention();

        //Show the app by setting the root view model for our application.
        app.setRoot('viewmodels/shell');
    });
});