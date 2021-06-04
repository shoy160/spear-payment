import Vue from 'vue'
import Router from 'vue-router'
import Index from '@/views/Index'
import Scan from '@/views/Scan'

Vue.use(Router)

export default new Router({
  routes: [{
    path: '/:id',
    name: 'Index',
    component: Index
  }, {
    path: '/scan/:id',
    name: 'Scan',
    component: Scan
  }]
})
