// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
// import './utils/vconsole'
import App from './App'
import router from './router'
import Mint from 'mint-ui'
import './assets/css/base.css'
import 'mint-ui/lib/style.css'
import * as filters from './utils/filters' // global filters

Vue.use(Mint)
Vue.config.productionTip = false

Object.keys(filters).forEach(key => {
  Vue.filter(key, filters[key])
})

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  components: {
    App
  },
  template: '<App/>'
})
