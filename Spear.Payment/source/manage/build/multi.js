'use strict'
// 多页应用配置
var glob = require('glob')
const path = require('path')
var HtmlWebpackPlugin = require('html-webpack-plugin')
var merge = require('webpack-merge')

const defaultOptions = {
  /**
   * 模块路径
   */
  path: '../src/modules',
  /**
   * 是否保留原有页面
   */
  retain: false
}

const getModuleName = path => {
  var m = path.match(/\/([0-9a-z]+)\/[^\/]+\.((js)|(html))/i)
  return m[1] || (path.substring(path.lastIndexOf('\/') + 1, path.lastIndexOf('.')))
}
const getEntries = path => {
  var entryFiles = glob.sync(path + '/*/*.js')
  var map = {}
  entryFiles.forEach((filePath) => {
    var moduleName = getModuleName(filePath)
    map[moduleName] = filePath
  })
  return map
}

const getPlugins = path => {
  const entryHtml = glob.sync(path + '/*/*.html')
  const arr = []
  entryHtml.forEach((filePath) => {
    const moduleName = getModuleName(filePath)
    let conf = {
      template: filePath,
      filename: (moduleName === 'app' ? 'index' : moduleName) + '.html',
      chunks: ['manifest', 'vendor', moduleName],
      inject: true
    }
    if (process.env.NODE_ENV === 'production') {
      conf = merge(conf, {
        minify: {
          removeComments: true,
          collapseWhitespace: true,
          removeAttributeQuotes: true
        }
        // chunksSortMode: 'dependency'
      })
    }
    arr.push(new HtmlWebpackPlugin(conf))
  })
  return arr
}
/**
 * 多页应用
 * @param {*} options
 */
exports.multiPage = (baseConfig, options) => {
  const opts = merge(defaultOptions, options)
  var rootPath = path.resolve(__dirname, opts.path)
  if (opts.retain) {
    // 保留原有配置
    baseConfig.entry = merge(baseConfig.entry, getEntries(rootPath))
    baseConfig.plugins = baseConfig.plugins.concat(getPlugins(rootPath))
  } else {
    baseConfig.entry = getEntries(rootPath)
    var plugins = getPlugins(rootPath)
    baseConfig.plugins.forEach(plugin => {
      if (plugin instanceof HtmlWebpackPlugin) {
        return
      }
      plugins.push(plugin)
    })
    baseConfig.plugins = plugins
  }
}
