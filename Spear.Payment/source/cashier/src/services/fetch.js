import axios from 'axios'
import {
  Toast
} from 'mint-ui'

// 创建axios实例
const $axios = axios.create({
  baseURL: process.env.GATEWAY, // api的base_url
  timeout: 20 * 1000 // 请求超时时间
})

// request拦截器
$axios.interceptors.request.use(
  config => {
    return config
  },
  error => {
    var data = {
      status: -1,
      message: error.message
    }
    return Promise.reject(data)
  }
)

// response拦截器
$axios.interceptors.response.use(
  response => {
    var res = response.data
    if (res.hasOwnProperty('status') && !res.status) {
      Toast(res.message)
      return Promise.reject(res)
    }
    if (res.hasOwnProperty('totalCount') || !res.hasOwnProperty('data')) {
      return res
    }
    return res.data
  },
  error => {
    Toast(error.message)
    var data = {
      status: -1,
      message: error.message
    }
    return Promise.reject(data)
  }
)

export default $axios
