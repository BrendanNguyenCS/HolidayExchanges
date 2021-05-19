"use strict";

var fs = require('fs');
var path = require('path');
var merge = require('merge-stream');
var named = require('vinyl-named');
var webpack = require('webpack');
var webpackStream = require('webpack-stream');
var gulp = require('gulp');
var sass = require('gulp-sass');
var postcss = require("gulp-postcss");
var cssnano = require("cssnano");
var glob = require('glob');
var autoprefixer = require('autoprefixer');
var sourcemaps = require('gulp-sourcemaps');
var uglify = require('gulp-uglify-es').default;
var concat = require('gulp-concat');
var runSequence = require('run-sequence');
var nunjucks = require('gulp-nunjucks-render');
var data = require('gulp-data');
var del = require('rimraf');
var streamqueue = require('streamqueue');
var debug = require('gulp-debug');
var rename = require('gulp-rename');
var browserSync = require('browser-sync').create();
var stripComments = require('gulp-strip-comments');
var exec = require('child_process').exec;
var spawn = require('child_process').spawn;

var paths = {
    src: 'Resources',
    dist: 'dist',
    sassIncludePaths: ['./node_modules'],
    binaries: 'bin'
};

var fonts = {
    fontAwesome: 'font-awesome'
}

var autoprefixerConfig = {
    browsers: '> 5%, last 2 Chrome versions, last 2 Firefox versions, IE >= 9, last 2 iOS versions'
};

function processJsNames(named) {
    var fileName = "main.min";

    if (named && named.sourceMap && named.sourceMap.file) {
        fileName = named.sourceMap.file.replace('.js', '.min');
    }

    return fileName;
}

function processCssNames(path) {
    if (path.basename === 'base') {
        path.basename = 'style';
    }
    path.extname = ".min.css";
}

function processHtmlNames(path) {
    if (path.dirname.indexOf('pages/') > -1) {
        path.dirname = path.dirname.replace('pages/', '');
    }
}

function formatListingName(name) {
    var results = name.replace('.nj', '').replace(/\-/g, ' ');
    var updateName = (origString, replaceChar, index) => {
        let firstPart = origString.substr(0, index);
        let lastPart = origString.substr(index + 1);

        let newString = firstPart + replaceChar + lastPart;
        return newString;
    }

    for (var i = 0; i < results.length; i++) {
        if (i > 0 && i + 1 < results.length) {
            if (results.charAt(i) === " ")
                results = updateName(results, results.charAt(i + 1).toUpperCase(), i + 1);
        } else if (i == 0) {
            results = updateName(results, results.charAt(i).toUpperCase(), i);
        }
    }

    return results;
}

function getHtmlDirectoryListing(basePath) {
    var results = [];
    var traverse = (id, name, cwd, baseUrl) => {
        let listing = {
            'id': id,
            'title': formatListingName(name)
        };
        if (fs.statSync(path.join(cwd, name)).isDirectory()) {
            listing['items'] = fs.readdirSync(path.join(cwd, name)).map((child, i) => {
                return traverse(`${id}-${i + 1}`, child, path.join(cwd, name), `${baseUrl}/${name}`);
            });
        } else {
            listing['url'] = `${baseUrl}/${name.replace('.nj', '')}.html`;
        }
        return listing;
    };
    fs.readdirSync(basePath).map((name, i) => {
        results.push(traverse(`${i + 1}`, name, basePath, ''));
    });
    return results;
}

function getFolders(dir) {
    if (!dir)
        dir = paths.src;

    return fs.readdirSync(dir)
        .filter(function (file) {
            return fs.statSync(path.join(dir, file)).isDirectory();
        });
}

const handleParallelTasks = (processName, tasks, done) => {
    var callback = (cb) => {
        cb();
        done();
    };

    callback.displayName = processName;

    return gulp.parallel(tasks, callback)();
}

const handleSequentialTasks = (processName, tasks, done) => {
    var callback = (cb) => {
        cb();
        done();
    };

    callback.displayName = processName;

    return gulp.series(tasks, callback)();
}

const getImageTask = (folder, destination) => {
    var processor = () => {
        return gulp.src(path.join(paths.src, folder, '/images/**/*'),
            { base: path.join(paths.src, folder, 'images') })
            .pipe(gulp.dest(destination));
    };
    processor.displayName = `Updating ${folder} images.`;

    return processor;
}

const getFontTask = (folder, destination) => {
    var processor = () => {
        return gulp.src(path.join(paths.src, folder, '/fonts/**/*'),
            { base: path.join(paths.src, folder, 'fonts') })
            .pipe(gulp.dest(destination));
    };
    processor.displayName = `Updating ${folder} fonts.`;

    return processor;
}

const getNugetFontTask = (folder, destination) => {
    var processor = () => {
        return gulp.src(path.join('node_modules/', folder, '/fonts/**/*'),
            { base: path.join('node_modules/', folder, 'fonts') })
            .pipe(gulp.dest(destination));
    };
    processor.displayName = `Updating ${folder} fonts.`;
    console.log(path.join('node_modules/', folder, '/fonts/**/*'));
    console.log(destination);

    return processor;
}

const getHtmlTask = (folder) => {
    var processor = () => {
        var dataFolder = path.join(paths.src, folder, '/templates/data/');
        var views = path.join(paths.src, folder, '/templates/pages/');
        return gulp.src([path.join(paths.src, folder, '/templates/*.+(html|nj)'),
        path.join(paths.src, folder, '/templates/pages/**/*.+(html|nj)')],
            { base: path.join(paths.src, folder, 'templates') })
            .pipe(debug())
            .pipe(data(function (file) {
                var d = {};
                fs.readdirSync(dataFolder).map((name) => {
                    var dataFile = path.join(paths.src, folder, '/templates/data/', name);
                    var tempData = JSON.parse(fs.readFileSync(dataFile));
                    d = Object.assign(d, tempData);
                });
                if (fs.existsSync(views)) {
                    d.index = getHtmlDirectoryListing(views);
                }
                return d;
            }))
            .pipe(nunjucks({
                path: [path.join(paths.src, folder, '/templates')]
            }))
            .pipe(rename(processHtmlNames))
            .pipe(gulp.dest(path.join(paths.dist, folder, '/html')))
            .pipe(browserSync.stream());
    };
    processor.displayName = `Building ${folder} pages.`;

    return processor;
}

const getCssTask = (folder, destination, mode) => {
    let processor;
    mode = mode || 'debug';

    if (mode == 'release') {
        processor = () => {
            return gulp.src([
                path.join(paths.src, folder, '/css/**/*.css'),
                path.join(paths.src, folder, '/sass/**/*.scss')
            ],
                { base: path.join(paths.src, folder, 'sass') })
                .pipe(sass({ includePaths: paths.sassIncludePaths }))
                .pipe(rename(processCssNames))
                .pipe(postcss([autoprefixer(), cssnano()]))
                .pipe(gulp.dest(destination));
        };
    } else {
        processor = () => {
            return gulp.src([
                path.join(paths.src, folder, '/css/**/*.css'),
                path.join(paths.src, folder, '/sass/**/*.scss')
            ],
                { base: path.join(paths.src, folder, 'sass') })
                .pipe(sourcemaps.init())
                .pipe(sass({ includePaths: paths.sassIncludePaths }))
                .pipe(rename(processCssNames))
                .pipe(postcss([autoprefixer()]))
                .pipe(sourcemaps.write())
                .pipe(gulp.dest(destination))
                .pipe(browserSync.stream());
        };
    }

    processor.displayName = `Building ${folder} sass in ${mode} mode.`;

    return processor;
}

const getJsTask = (folder, destination, mode) => {
    let processor;
    mode = mode || 'debug';

    if (mode == 'release') {
        processor = () => {
            return gulp.src(path.join(paths.src, folder, '/js/*.js'),
                { base: path.join(paths.src, folder, 'js') })
                .pipe(sourcemaps.init())
                .pipe(sourcemaps.write())
                .pipe(named(processJsNames))
                .pipe(webpackStream({ mode: 'production', stats: { warnings: false } }))
                .pipe(uglify())
                .pipe(stripComments())
                .pipe(gulp.dest(destination));
        };
    } else {
        processor = () => {
            return gulp.src(path.join(paths.src, folder, '/js/*.js'),
                { base: path.join(paths.src, folder, 'js') })
                .pipe(sourcemaps.init())
                .pipe(sourcemaps.write())
                .pipe(named(processJsNames))
                .pipe(webpackStream({ mode: 'development' }))
                .pipe(gulp.dest(destination))
                .pipe(browserSync.stream());
        };
    }

    processor.displayName = `Building ${folder} js in ${mode} mode.`;

    return processor;
}

const createWatch = (project, patterns, folder, message, callback) => {
    if (fs.existsSync(path.join('.', paths.src, project, folder))) {
        let src = [];

        for (var i = 0; i < patterns.length; i++) {
            src.push('./' + paths.src + '/' + project + patterns[i]);
        }

        let process = (done) => {
            return callback(done);
        };

        process.displayName = message;
        return gulp.watch(src, process);
    }
}

gulp.task('build:css', function (done) {
    return handleParallelTasks("finalizing css", getFolders().map((folder) => {
        return getCssTask(folder, path.join(paths.dist, folder, '/css'), 'release');
    }), done);
});

gulp.task('build:js', function (done) {
    return handleSequentialTasks("finalizing js", getFolders().map((folder) => {
        return getJsTask(folder, path.join(paths.dist, folder, '/js'), 'release');
    }), done);
});

gulp.task('build:html', function (done) {
    return handleParallelTasks("finalizing html", getFolders().map(getHtmlTask), done);
});

gulp.task('build:images', function (done) {
    return handleParallelTasks("finalizing fonts", getFolders().map((folder) => {
        return getImageTask(folder, path.join(paths.dist, folder, 'images'));
    }), done);
});

gulp.task('build:fonts', function (done) {
    return handleParallelTasks("finalizing fonts", getFolders().map((folder) => {
        return gulp.series(getFontTask(folder, path.join(paths.dist, folder, 'fonts')),
            getNugetFontTask(fonts.fontAwesome, path.join(paths.dist, folder, 'fonts'))
        );
    }), done);
});

// Deletes previous files in the dist folder.
gulp.task('clean:dist', function (done) {
    del(path.join(paths.dist, '/*'), done);
});

gulp.task('build', gulp.series('clean:dist', 'build:images', 'build:fonts', 'build:css', 'build:js'));

gulp.task('build:all', gulp.series('clean:dist', 'build:images', 'build:fonts', 'build:css', 'build:js', 'build:html'));

gulp.task('watch', function (done) {
    getFolders().map((folder) => {
        if (folder != 'dist') {
            createWatch(folder, ['/js/**/*.js'], '/js', 'detecting ' + folder + ' javascript changes', function (done) {
                return handleSequentialTasks("deploying js changes...",
                    getJsTask(folder, path.join(paths.dist, folder, '/js'), 'debug'),
                    done);
            });

            createWatch(folder, ['/sass/**/*.scss', '/css/**/*.css'], '/sass', 'detecting ' + folder + ' sass changes', function (done) {
                return handleParallelTasks("deploying sass changes...",
                    getCssTask(folder, path.join(paths.dist, folder, '/css'), 'debug'),
                    done);
            });

            createWatch(folder, ['/fonts/**/*'], '/fonts', 'detecting ' + folder + ' font folder updates', function (done) {
                return handleParallelTasks("deploying fonts changes...",
                    getFontTask(folder, path.join(paths.dist, folder, '/fonts')),
                    done);
            });

            createWatch(folder, ['/images/**/*'], '/images', 'detecting ' + folder + ' image folder updates', function (done) {
                return handleParallelTasks("deploying images changes...",
                    getImageTask(folder, path.join(paths.dist, folder, '/images')),
                    done);
            });
        }
    });

    done();
});

//Map each project html build task
getFolders().map((folder) => {
    if (fs.existsSync(path.join('.', paths.src, folder, 'templates'))) {
        gulp.task('serve:' + folder,
            function (done) {
                browserSync.init({
                    server: {
                        baseDir: [path.join(paths.dist, folder, 'html'), path.join(paths.dist, folder)],
                        index: 'index.html'
                    }
                });

                createWatch(folder, ['/templates/**/*.+(html|nj|json)'], '/templates', 'detecting ' + folder + ' html changes', function (done) {
                    return handleSequentialTasks("updating html changes...", getHtmlTask(folder), done);
                });

                createWatch(folder, ['/js/**/*.js'], '/js', 'detecting ' + folder + ' javascript changes', function (done) {
                    return handleSequentialTasks("updating js changes...", getJsTask(folder, path.join(paths.dist, folder, '/js'), 'debug'), done);
                });

                createWatch(folder, ['/sass/**/*.scss', '/css/**/*.css'], '/sass', 'detecting ' + folder + ' sass changes', function (done) {
                    return handleParallelTasks("updating sass changes...", getCssTask(folder, path.join(paths.dist, folder, '/css'), 'debug'), done);
                });

                createWatch(folder, ['/fonts/**/*'], '/fonts', 'detecting ' + folder + ' font folder updates', function (done) {
                    return handleParallelTasks("deploying fonts changes...", getFontTask(folder, path.join(paths.dist, folder, 'fonts')), done);
                });

                createWatch(folder, ['/images/**/*'], '/images', 'detecting ' + folder + ' image folder updates', function (done) {
                    return handleParallelTasks("deploying images changes...", getImageTask(folder, path.join(paths.dist, folder, 'images')), done);
                });

                gulp.watch(path.join(paths.dist, folder, 'html', '*.+(html|nj)')).on('change', browserSync.reload);
                //gulp.watch(path.join(paths.src, folder, 'templates', '**/*.+(html|nj|json)'), gulp.series('sync-dev-task:' + folder + 'build:html'));
                //gulp.watch(path.join(paths.src, folder, '/sass/**/*.scss'), gulp.series('sync-dev-task:' + folder + ':build:css'));
                //gulp.watch(path.join(paths.dist, folder, 'html', '*.+(html|nj)')).on('change', browserSync.reload);
                //gulp.watch(path.join(paths.src, folder, '/js/**/*.js'), gulp.series('sync-dev-task:' + folder + ':build:js'));
                done();
            });
    }
});

gulp.task('default', gulp.parallel('build'));