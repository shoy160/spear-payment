import {
  param2Obj
} from '@/utils'

const userMap = {
  admin: {
    roles: ['admin'],
    token: 'admin',
    introduction: '我是超级管理员',
    avatar: 'https://oss.i-cbao.com/icb.png',
    name: 'Super Admin'
  },
  editor: {
    roles: ['editor'],
    token: 'editor',
    introduction: '我是编辑',
    avatar: 'https://oss.i-cbao.com/icb.png',
    name: 'Normal Editor'
  }
}

export default {
  loginByUsername: config => {
    const {
      account
    } = JSON.parse(config.body)
    return userMap[account]
  },
  getUserInfo: config => {
    const {
      token
    } = param2Obj(config.url)
    if (userMap[token]) {
      return userMap[token]
    } else {
      return false
    }
  },
  logout: () => 'success'
}
