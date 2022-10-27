//引入gulp  
var gulp = require('gulp');


//引入组件  
var concat = require('gulp-concat');           //合并  
var jshint = require('gulp-jshint');           //js规范验证  
var uglify = require('gulp-uglify');           //压缩  
var rename = require('gulp-rename');          //文件名命名  
var rjs = require('gulp-requirejs');         //require优化  
var watch = require('gulp-watch');
var del = require('del');
var runSequence = require('run-sequence');
var replace = require('gulp-replace');

//脚本检查  
gulp.task('lint', function () {
    gulp.src('app/*.js')
        .pipe(jshint())
        .pipe(jshint.reporter('default'));
});

gulp.task('build:clean', function (cb) {
    del(['app/main-built.js', 'app/main-built.min.js'], cb);
    //del(['app/main-built.min.js'], cb);
});

gulp.task('build:js:replace', function (cb) {
    gulp.src('app/main-built.js')
    .pipe(replace('define(\'main\',', 'define(\'main-built\','))
    .pipe(gulp.dest('app'));

    //    gulp.src('app/main-built.min.js')
    //    .pipe(replace('define("main",', 'define("main1",'))
    //    .pipe(gulp.dest('app'));
});

//require合并
gulp.task('build:js', function () {
    rjs({
        name: 'main',
        baseUrl: 'app',
        out: 'main-built.js',
        //dir:'app',
        mainConfigFile: 'app/main.js',
        shim: {},
        //include: ['plugins/dialog', 'plugins/router', 'plugins/widget', 'text', 'transitions/entrance', 'viewmodels/shell']
        include: ['plugins/dialog', 'plugins/router', 'plugins/widget', 'text', 'transitions/entrance', 'jquery-ui', 'utils', 'logininfo', 'jquery.querystring', 'knockout.mapping', 'knockout.validation', 'spin', 'jquery.loadmask.spin', 'jquery.poshytip',
'bootstrap-datepicker',
        'jquery.uploadify',
        'jqPaginator']
    })
     .pipe(gulp.dest("app"))
    .pipe(replace('define(\'main\',', 'define(\'main-built\','))
     .pipe(gulp.dest("app"))
    .pipe(rename("main-built.min.js"))
     .pipe(replace('define(\'main-built\',', 'define(\'main-built.min\',')) //替换入口函数名
    .pipe(uglify())
    .pipe(gulp.dest("app"));
});
gulp.task('build', function (cb) {
    runSequence(
    ['build:clean', 'build:js'], cb);
});
gulp.task('default1', function () {
    //监听js变化  
    gulp.watch('app/*.js', function () {       //当js文件变化后，自动检验 压缩  
        //gulp.run('lint', 'scripts');  
        gulp.run('lint', 'rjs');
    });
});
gulp.task('default', ['build']);