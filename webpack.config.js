// Template for webpack.config.js in Fable projects
// Find latest version in https://github.com/fable-compiler/webpack-config-template

// In most cases, you'll only need to edit the CONFIG object (after dependencies)
// See below if you need better fine-tuning of Webpack options

var path = require('path');
var webpack = require('webpack');
var HtmlWebpackPlugin = require('html-webpack-plugin');
var CopyWebpackPlugin = require('copy-webpack-plugin');
var MiniCssExtractPlugin = require('mini-css-extract-plugin')

var CONFIG = {
    // The tags to include the generated JS and CSS will be automatically injected in the HTML template
    // See https://github.com/jantimon/html-webpack-plugin
    indexHtmlTemplate: './src/Client/index.html',
    fsharpEntry: './src/Client/output/App.js',
    outputDir: './deploy/public',
    assetsDir: './src/Client/public',
    devServerPort: 8080,
    // When using webpack-dev-server, you may need to redirect some calls
    // to a external API server. See https://webpack.js.org/configuration/dev-server/#devserver-proxy
    devServerProxy: [
        {
            // redirect requests that start with /api/ or /resource/ to the server on port 8085
            context: ['/api', '/resource'],
            target: 'http://localhost:' + (process.env.SERVER_PROXY_PORT || "8085"),
            changeOrigin: true
        },
        {
            // redirect websocket requests that start with /socket/ to the server on the port 8085
            context: ['/socket'],
            target: 'http://localhost:' + (process.env.SERVER_PROXY_PORT || "8085"),
            ws: true
        }
    ]
}

// If we're running webpack serve, assume we're in development mode
var isProduction = !process.argv.find(v => v.indexOf('serve') !== -1);
var environment = isProduction ? 'production' : 'development';
process.env.NODE_ENV = environment;
console.log('Bundling for ' + environment + '...');

// The HtmlWebpackPlugin allows us to use a template for the index.html page
// and automatically injects <script> or <link> tags for generated bundles.
var commonPlugins = [
    new HtmlWebpackPlugin({
        filename: 'index.html',
        template: resolve(CONFIG.indexHtmlTemplate)
    })
];

module.exports = {
    // In development, split the JavaScript and CSS files in order to
    // have a faster HMR support. In production bundle styles together
    // with the code because the MiniCssExtractPlugin will extract the
    // CSS in a separate files.
    entry: {
        app: resolve(CONFIG.fsharpEntry)
    },
    // Add a hash to the output file name in production
    // to prevent browser caching if code changes
    output: {
        path: resolve(CONFIG.outputDir),
        filename: isProduction ? '[name].[contenthash].js' : '[name].js'
    },
    mode: isProduction ? 'production' : 'development',
    devtool: isProduction ? 'source-map' : 'eval-source-map',
    // Bundle-size hints are just noise for this SPA; keep the build output clean.
    performance: { hints: false },
    optimization: {
        splitChunks: {
            chunks: 'all'
        },
    },
    // Besides the HtmlPlugin, we use the following plugins:
    // PRODUCTION
    //      - MiniCssExtractPlugin: Extracts CSS from bundle to a different file
    //      - CopyWebpackPlugin: Copies static assets to output directory
    // DEVELOPMENT
    //      - HMR is enabled automatically by webpack-dev-server (hot: true)
    plugins: isProduction ?
        commonPlugins.concat([
            new MiniCssExtractPlugin({ filename: 'style.[name].[contenthash].css' }),
            new CopyWebpackPlugin({ patterns: [{ from: resolve(CONFIG.assetsDir) }]}),
        ])
        : commonPlugins,
    resolve: {
        // See https://github.com/fable-compiler/Fable/issues/1490
        symlinks: false
    },
    // Configuration for webpack-dev-server
    devServer: {
        static: {
            directory: resolve(CONFIG.assetsDir),
        },
        host: '0.0.0.0',
        port: CONFIG.devServerPort,
        proxy: CONFIG.devServerProxy,
        hot: true,
        historyApiFallback: true,
        allowedHosts: 'all',
        // Don't let build warnings (e.g. SCSS deprecations) throw a full-screen
        // overlay over the app in debug mode; still surface real errors.
        client: {
            overlay: { errors: true, warnings: false }
        }
    },
    // - sass-loaders: transforms SASS/SCSS into JS
    // - asset/resource: Moves files referenced in the code (fonts, images) into output folder
    module: {
        rules: [
            {
                test: /\.(sass|scss|css)$/,
                use: [
                    isProduction
                        ? MiniCssExtractPlugin.loader
                        : 'style-loader',
                    'css-loader',
                    {
                        loader: 'sass-loader',
                        options: {
                            implementation: require('sass'),
                            // Bulma/FontAwesome + our index.scss use legacy Sass
                            // features (@import, if(), global color fns, /-division).
                            // Silence the legacy JS-API notice and route all other Sass
                            // deprecation warnings through a no-op logger so they don't
                            // flood the build or trigger the dev-server overlay.
                            sassOptions: {
                                quietDeps: true,
                                silenceDeprecations: ['legacy-js-api'],
                                logger: { warn() {}, debug() {} }
                            }
                        }
                    }
                ],
            },
            {
                test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*)?$/,
                type: 'asset/resource'
            },
            {
                test: /\.js$/,
                enforce: "pre",
                use: ['source-map-loader'],
            }
        ]
    }
};

function resolve(filePath) {
    return path.isAbsolute(filePath) ? filePath : path.join(__dirname, filePath);
}
