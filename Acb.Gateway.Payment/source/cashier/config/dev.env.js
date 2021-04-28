'use strict'
const merge = require('webpack-merge')
const prodEnv = require('./prod.env')

module.exports = merge(prodEnv, {
  NODE_ENV: '"development"',
  GATEWAY: '"http://localhost:25859"'
  // GATEWAY: '"http://pay.test.i-cbao.com:88"'
})
