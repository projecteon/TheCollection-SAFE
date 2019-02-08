const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const path = require('path');
const fs = require('fs');

// Make sure any symlinks in the project folder are resolved:
// https://github.com/facebook/create-react-app/issues/637
const appDirectory = fs.realpathSync(process.cwd());
const resolveApp = relativePath => path.resolve(appDirectory, relativePath);

exports.imageLoaderRule = function (assetsPath) {
  return {
    test: [/\.bmp$/, /\.gif$/, /\.jpe?g$/, /\.png$/],
    loader: require.resolve('url-loader'),
    options: {
      limit: 10000,
      name: assetsPath + 'media/[name].[hash:8].[ext]',
    },
  }
};

cssLoaderRule = function (options) {
  return  {
    test: /\.css$/,
    use: [
      {
        loader: require.resolve('css-loader'),
        options: {...options, ...{
            importLoaders: 2,
          }
        },
      },
      {
        // Options for PostCSS as we reference these options twice
        // Adds vendor prefixing based on your specified browser support in
        // package.json
        loader: require.resolve('postcss-loader'),
        options: {...options, ...{
            // Necessary for external CSS imports to work
            // https://github.com/facebook/create-react-app/issues/2677
            ident: 'postcss',
            plugins: () => [
              require('postcss-flexbugs-fixes'),
              require('postcss-preset-env')({
                autoprefixer: {
                  flexbox: 'no-2009',
                },
                stage: 3,
              }),
            ],
          }
        },
      },
    ],
  };
};

exports.cssLoaderRuleDev = function() {
  let baseRule = cssLoaderRule();
  baseRule.use = [require.resolve('style-loader')].concat(baseRule.use);
  return baseRule;
};

exports.cssLoaderRuleProd = function() {
  let baseRule = cssLoaderRule();
  baseRule.use = [MiniCssExtractPlugin.loader].concat(baseRule.use);
  return baseRule;
}

exports.sassLoaderRuledev = function() {
  let baseRule = exports.cssLoaderRuleDev();
  baseRule.test = /\.(scss|sass)$/;
  baseRule.use.push({
    loader: require.resolve('sass-loader'),
  });

  return baseRule;
}

exports.sassLoaderRuleProd = function() {
  let baseRule = exports.cssLoaderRuleProd();
  baseRule.test = /\.(scss|sass)$/;
  baseRule.use.push({
    loader: require.resolve('sass-loader'),
    options: {sourceMap: true},
  });

  return baseRule;
}

exports.fileLoaderRule = function (assetsPath) {
  return {
    loader: require.resolve('file-loader'),
    // Exclude `js` files to keep "css" loader working as it injects
    // it's runtime that would otherwise be processed through "file" loader.
    // Also exclude `html` and `json` extensions so they get processed
    // by webpacks internal loaders.
    exclude: [/\.(js|jsx)$/, /\.html$/, /\.json$/],
    options: {
      name: assetsPath + 'media/[name].[hash:8].[ext]',
    },
  }
};
