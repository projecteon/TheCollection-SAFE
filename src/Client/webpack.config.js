var path = require("path");
var webpack = require("webpack");
var MinifyPlugin = require("terser-webpack-plugin");
var HtmlWebpackPlugin = require('html-webpack-plugin');
var MiniCssExtractPlugin = require('mini-css-extract-plugin');
var CopyWebpackPlugin = require('copy-webpack-plugin');
const webpackModuleCommon = require('./webpack.config.common.module');

function resolve(filePath) {
    return path.join(__dirname, filePath)
}

var CONFIG = {
    fsharpEntry: {
        "app": [
            "whatwg-fetch",
            "@babel/polyfill",
            resolve("./Client.fsproj")
        ]
    },
    devServerProxy: {
        '/api/*': {
            target: 'http://localhost:' + (process.env.SUAVE_FABLE_PORT || "8085"),
            changeOrigin: true
        }
    },
    historyApiFallback: {
        index: resolve("./index.html")
    },
    outputDir: './deploy',
    contentBase: resolve("./public"),
    indexHtmlTemplate: './index.html',
    assetsDir: './public',
    devServerPort: 8080,
}

var isProduction = process.argv.indexOf("-p") >= 0;
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

var commonPlugins = [
  new HtmlWebpackPlugin({
      filename: 'index.html',
      template: resolve(CONFIG.indexHtmlTemplate)
  })
];

module.exports = {
    entry : CONFIG.fsharpEntry,
    output: {
      path: resolve(CONFIG.outputDir),
      filename: isProduction ? '[name].[hash].js' : '[name].js'
    },
    mode: isProduction ? "production" : "development",
    devtool: isProduction ? 'source-map' : 'eval-source-map',
    resolve: {
        symlinks: false
    },
    optimization: {
        // Split the code coming from npm packages into a different file.
        // 3rd party dependencies change less often, let the browser cache them.
        splitChunks: {
            cacheGroups: {
                commons: {
                    test: /node_modules/,
                    name: "vendors",
                    chunks: "all"
                }
            }
        },
        minimizer: isProduction ? [new MinifyPlugin()] : []
    },
    // DEVELOPMENT
    //      - HotModuleReplacementPlugin: Enables hot reloading when code changes without refreshing
    plugins: isProduction ?
    commonPlugins.concat([
        new MiniCssExtractPlugin({ filename: 'style.[hash].css' }),
        new CopyWebpackPlugin([{ from: resolve(CONFIG.assetsDir) }]),
    ])
    : commonPlugins.concat([
        new webpack.HotModuleReplacementPlugin(),
    ]),
    // Configuration for webpack-dev-server
    devServer: {
      publicPath: '/',
      hot: true,
      inline: true,
      port: CONFIG.devServerPort,
      proxy: CONFIG.devServerProxy,
      // historyApiFallback: CONFIG.historyApiFallback, // https://stackoverflow.com/questions/37271062/historyapifallback-doesnt-work-in-webpack-dev-server/38207496#38207496
      historyApiFallback: true,
      contentBase: CONFIG.contentBase
    },
    // - fable-loader: transforms F# into JS
    // - babel-loader: transforms JS to old syntax (compatible with old browsers)
    module: {
        rules: [
            webpackModuleCommon.fableLoaderRule(),
            webpackModuleCommon.bableLoaderRule(),
            webpackModuleCommon.sassLoaderRuledev(),
            webpackModuleCommon.fileLoaderRule(),
        ]
    }
};
