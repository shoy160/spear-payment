import request from '@/utils/request'

/**
 * 登陆
 * @param {*} account
 * @param {*} password
 */
export const login = (account, password) => {
  return request.post('/manage/account/login', {
    account,
    password
  })
}

/**
 * 退出登陆
 */
export const logout = () => {
  return Promise.resolve()
}

/**
 * 获取账号信息
 */
export const getAccount = () => {
  return request.get('/manage/account')
}

/**
 * 检查登陆密码
 * @param {*} pwd
 */
export const checkPwd = pwd => {
  return request.put('/manage/account/check', {
    password: pwd
  })
}

/**
 *  修改密码
 * @param {string} oldPwd
 * @param {string} newPwd
 */
export const changePwd = ({ oldPwd, newPwd }) => {
  return request.put('/manage/account/pwd', {
    oldPwd,
    newPwd
  })
}
