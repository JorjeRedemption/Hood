/// <binding BeforeBuild='views' Clean='views' />
// Useful gulp functions for the development of HoodCMS.
// Note this is a demo project and should not be used for production HoodCMS projects.
// In production, you should install the nuget and bower packages to your HoodCMS project.

var gulp = require('gulp'),
    babel = require('gulp-babel'),
    sass = require('gulp-sass'),
    less = require('gulp-less'),
    rimraf = require('gulp-rimraf'),
    concat = require('gulp-concat'),
    uglify = require('gulp-uglify'),
    cssnano = require('gulp-cssnano'),
    rename = require('gulp-rename'),
    imagemin = require('gulp-imagemin'),
    path = require('path'),
    sourcemaps = require('gulp-sourcemaps'),
    lib = './wwwroot/lib/',
    hood = {
        js: './wwwroot/hood/js/',
        css: './wwwroot/hood/css/',
        scss: './wwwroot/hood/scss/',
        less: './wwwroot/hood/less/',
        images: './wwwroot/hood/images/'
    },
    output = {
        js: './../../js/',
        css: './../../css/',
        scss: './../../scss/',
        less: './../../less/',
        images: './../../images/',
        sql: './../../sql/'
    };

// Cleans all dist/src/images output folders, as well as the hood folders.
gulp.task('clean', function (cb) {
    return gulp.src([
        output.css,
        output.scss,
        output.js,
        output.images,
        hood.css
    ], { read: false, allowEmpty: true })
        .pipe(rimraf({ force: true }));
});

// Compiles, compresses and copies all scss files to the output directories.
gulp.task('scss', function () {
    return gulp.src(hood.scss + '*.scss')
        .pipe(sourcemaps.init())
        .pipe(sass({ outputStyle: 'expanded', indentType: 'tab', indentWidth: 1 }).on('error', sass.logError))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(hood.css));
});

gulp.task('scss:copy', function () {
    return gulp.src(hood.scss + "**/*.scss")
        .pipe(gulp.dest(output.scss));
});
// Compiles, compresses and copies all scss files to the output directories.
gulp.task('theme:bootstrap4', function () {
    return gulp.src('./wwwroot/themes/bootstrap4/scss/styles.scss')
        .pipe(sourcemaps.init())
        .pipe(sass({ outputStyle: 'expanded', indentType: 'tab', indentWidth: 1 }).on('error', sass.logError))
        .pipe(sourcemaps.write())
        .pipe(rename('bootstrap4.css'))
        .pipe(gulp.dest('./wwwroot/hood/css/'));
});

// create less for working with bootstrap3 styles.
gulp.task('theme:bootstrap3', function () {
    lss = less({ relativeUrls: true });
    lss.on('error', function (e) {
        console.log(e);
        lss.end();
    });

    return gulp.src([
        './wwwroot/themes/bootstrap3/less/styles.less'
    ])
        .pipe(sourcemaps.init())
        .pipe(lss)
        .pipe(sourcemaps.write())
        .pipe(rename('bootstrap3.css'))
        .pipe(gulp.dest('./wwwroot/hood/css/'));
});

gulp.task('less:copy', function () {
    return gulp.src(hood.less + "**/*.less")
        .pipe(gulp.dest(output.less));
});

gulp.task('cssnano', function () {
    return gulp.src([hood.css + '**/*.css', '!' + hood.css + '**/*.min.css'])
        .pipe(gulp.dest(output.css))
        .pipe(cssnano({
            discardComments: {
                removeAll: true
            }
        }))
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(hood.css))
        .pipe(gulp.dest(output.css));
});

// Copies working views from default theme to be embedded in main assembly.
gulp.task('views:clean', function (cb) {
    return gulp.src(['./../Hood.UI.Bootstrap3/UI/**/*.*', './../Hood.UI.Bootstrap4/UI/**/*.*'], { read: false, allowEmpty: true })
        .pipe(rimraf({ force: true }));
});
gulp.task('views:copy', function () {
    return gulp.src('./Themes/bootstrap3/Views/**/*.*')
        .pipe(gulp.dest('./../Hood.UI.Bootstrap3/UI/'));
});
gulp.task('views:copy:new', function () {
    return gulp.src('./Themes/bootstrap4/Views/**/*.*')
        .pipe(gulp.dest('./../Hood.UI.Bootstrap4/UI/'));
});
gulp.task('views', gulp.series('views:clean', 'views:copy', 'views:copy:new'));

// Copies any image files from the images directories to the distribution images directory.
gulp.task('images', function () {
    return gulp.src(hood.images + '**/*.+(png|jpg|gif|svg)')
        .pipe(imagemin())
        .pipe(gulp.dest(output.images));
});

// Minifies javascript and copies the output to the output directories.
gulp.task('js', function () {
    l = uglify({});
    l.on('error', function (e) {
        console.log(e);
        l.end();
    });
    return gulp.src([hood.js + '**/*.js', '!' + hood.js + '**/*.min.js', '!' + hood.js + '**/*.packaged.js'], { base: hood.js })
        .pipe(babel({
            presets: ['@babel/env']
        }))
        .pipe(gulp.dest(output.js))
        .pipe(l)
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(hood.js))
        .pipe(gulp.dest(output.js));
});

// Bundle the app into the packaged form and copies the output to the output directories.
gulp.task('js:core', function () {
    l = uglify({});
    l.on('error', function (e) {
        console.log(e);
        l.end();
    });
    return gulp.src([

        hood.js + 'core/production.js',
        hood.js + 'core/globals.js',

        hood.js + 'core/helpers.js',
        hood.js + 'core/handlers.js',
        hood.js + 'core/stringhelpers.js',
        hood.js + 'core/validator.js',

        hood.js + 'core/addresses.js',
        hood.js + 'core/alerts.js',
        hood.js + 'core/forms.js',
        hood.js + 'core/inline.js',
        hood.js + 'core/media.js'

    ], { base: '.' })
        .pipe(concat('core.js'))
        .pipe(babel({
            presets: ['@babel/env']
        }))
        .pipe(gulp.dest(hood.js))
        .pipe(gulp.dest(output.js))
        .pipe(l)
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(hood.js))
        .pipe(gulp.dest(output.js));
});

gulp.task('js:package:app', function () {
    l = uglify({});
    l.on('error', function (e) {
        console.log(e);
        l.end();
    });
    return gulp.src([

        hood.js + 'core/production.js',
        hood.js + 'core/globals.js',

        hood.js + 'core/helpers.js',
        hood.js + 'core/handlers.js',
        hood.js + 'core/stringhelpers.js',
        hood.js + 'core/validator.js',

        hood.js + 'core/addresses.js',
        hood.js + 'core/alerts.js',
        hood.js + 'core/forms.js',
        hood.js + 'core/inline.js',
        hood.js + 'core/media.js',

        hood.js + 'app/cart.js',
        hood.js + 'app/stripe.js',
        hood.js + 'app.js'

    ], { base: '.' })
        .pipe(babel({
            presets: ['@babel/env']
        }))
        .pipe(concat('app.packaged.js'))
        .pipe(l)
        .pipe(gulp.dest(hood.js))
        .pipe(gulp.dest(output.js));
});

// Package the login javascript and copy the output to the output directories.
gulp.task('js:package:login', function () {
    l = uglify({});
    l.on('error', function (e) {
        console.log(e);
        l.end();
    });
    return gulp.src([

        lib + 'jquery-mask/jquery.mask.js',

        hood.js + 'login.js'

    ], { base: '.' })
        .pipe(babel({
            presets: ['@babel/env']
        }))
        .pipe(concat('login.packaged.js'))
        .pipe(l)
        .pipe(gulp.dest(hood.js))
        .pipe(gulp.dest(output.js));
});

// Package the admin Javascript and copy the output to the output directories.
gulp.task('js:package:admin', function () {
    l = uglify({});
    l.on('error', function (e) {
        console.log(e);
        l.end();
    });
    return gulp.src([

        hood.js + 'core.js',

        hood.js + 'admin/content.js',
        hood.js + 'admin/forums.js',
        hood.js + 'admin/logs.js',
        hood.js + 'admin/property.js',
        hood.js + 'admin/subscriptions.js',
        hood.js + 'admin/themes.js',
        hood.js + 'admin/users.js',

        hood.js + 'app/google.js',

        hood.js + 'admin.js'

    ], { base: '.' })
        .pipe(babel({
            presets: ['@babel/env']
        }))
        .pipe(concat('admin.packaged.js'))
        .pipe(l)
        .pipe(gulp.dest(hood.js))
        .pipe(gulp.dest(output.js));

});

gulp.task('package', gulp.series('js', 'js:core', gulp.parallel('js:package:admin', 'js:package:app', 'js:package:login')));
gulp.task('build', gulp.series(gulp.parallel('scss', 'js', 'images'), gulp.parallel('theme:bootstrap3', 'theme:bootstrap4'), gulp.parallel('scss:copy', 'less:copy', 'cssnano', 'package')));

// Site workload, to compile theme less/scss and JS.
gulp.task('themes:scss', function () {
    return gulp.src([
        './wwwroot/themes/**/scss/styles.scss'
    ])
        .pipe(sourcemaps.init())
        .pipe(sass({ outputStyle: 'expanded', indentType: 'tab', indentWidth: 1 }).on('error', sass.logError))
        .pipe(sourcemaps.write())
        .pipe(rename(function (filePath) {
            let parentFolder = path.dirname(filePath.dirname);
            filePath.dirname = path.join(parentFolder, 'css');
        }))
        .pipe(gulp.dest('./wwwroot/themes/'));
});
gulp.task('themes:less', function () {
    lss = less({ relativeUrls: true });
    lss.on('error', function (e) {
        console.log(e);
        lss.end();
    });

    return gulp.src([
        './wwwroot/themes/**/less/styles.less'
    ])
        .pipe(sourcemaps.init())
        .pipe(lss)
        .pipe(sourcemaps.write())
        .pipe(rename(function (filePath) {
            let parentFolder = path.dirname(filePath.dirname);
            filePath.dirname = path.join(parentFolder, 'css');
        }))
        .pipe(gulp.dest('./wwwroot/themes/'));
});
gulp.task('themes:cssnano', function () {
    return gulp.src(['./wwwroot/themes/**/css/styles.css'])
        .pipe(cssnano({
            discardComments: {
                removeAll: true
            }
        }))
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest('./wwwroot/themes/'));
});

gulp.task('themes', gulp.parallel('themes:scss', 'themes:less'));

gulp.task('publish', gulp.series('clean', 'build', 'themes', 'themes:cssnano'));

gulp.task('watch', function () {
    gulp.watch(hood.scss, gulp.series('scss'));
});
