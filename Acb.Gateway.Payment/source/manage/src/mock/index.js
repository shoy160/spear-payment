import Mock from 'mockjs'
import loginAPI from './auth'
import homeApi from './home'

// Mock.setup({
//   timeout: '350-600'
// })

// 登录相关
// Mock.mock(/\/manage\/account\/login/, 'post', loginAPI.loginByUsername)
Mock.mock(/\/login\/logout/, 'post', loginAPI.logout)
Mock.mock(/\/user\/info\.*/, 'get', loginAPI.getUserInfo)

Mock.mock(/\/api\/home/, 'get', homeApi.homedata)

export default Mock
