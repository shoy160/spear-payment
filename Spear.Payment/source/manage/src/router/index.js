import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

/* Layout */
import Layout from '@/views/layout/Layout'

export const constantRouterMap = [{
  path: '/login',
  component: () =>
      import('@/views/login/index'),
  hidden: true
},
{
  path: '/authredirect',
  component: () =>
      import('@/views/login/authredirect'),
  hidden: true
},
{
  path: '/404',
  component: () =>
      import('@/views/errorPage/404'),
  hidden: true
},
{
  path: '/401',
  component: () =>
      import('@/views/errorPage/401'),
  hidden: true
}, {
  path: '/locking',
  name: 'locking',
  component: () =>
      import('@/components/LockScreen/components/LockingPage'),
  hidden: true
},
{
  path: '',
  component: Layout,
  redirect: '/home/index',
  hidden: true
}
]

export default new Router({
  // mode: 'history', // require service support
  scrollBehavior: () => ({
    y: 0
  }),
  routes: constantRouterMap
})

// 配置中心
export const asyncRouterMap = [{
  path: '/home',
  component: Layout,
  redirect: 'noredirect',
  children: [{
    path: 'index',
    component: () =>
        import('@/views/pages/index'),
    name: 'Home',
    meta: {
      title: 'home',
      icon: 'chart'
    }
  }]
}, {
  path: '/channel',
  component: Layout,
  redirect: '/channel/index',
  meta: {
    role: 64
  },
  children: [{
    path: 'index',
    component: () =>
        import('@/views/pages/channel/index'),
    name: 'Channel',
    meta: {
      title: 'channel',
      icon: 'guide',
      role: 64,
      noCache: true
    }
  }]
},
{
  path: '/project',
  component: Layout,
  redirect: '/project/index',
  children: [{
    path: 'index',
    component: () =>
        import('@/views/pages/project/index'),
    name: 'Project',
    meta: {
      title: 'project',
      icon: 'clipboard',
      noCache: true
    }
  }]
}, {
  path: '/trade',
  component: Layout,
  redirect: '/trade/index',
  name: 'Trade',
  meta: {
    title: 'trade',
    icon: 'money',
    noCache: true
  },
  children: [{
    path: 'index',
    component: () =>
        import('@/views/pages/trade/index'),
    name: 'TradeList',
    meta: {
      title: 'tradeList',
      icon: 'list',
      noCache: true
    }
  }, {
    path: 'notify',
    component: () =>
        import('@/views/pages/trade/notify'),
    name: 'TradeNotify',
    meta: {
      title: 'tradeNotify',
      icon: 'link',
      noCache: true
    }
  }]
},
{
  path: '*',
  redirect: '/404',
  hidden: true
}
]
