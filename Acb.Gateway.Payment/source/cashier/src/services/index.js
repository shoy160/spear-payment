import fetch from './fetch'

/**
 * 交易详情
 * @param {string} id 交易ID
 */
export const detail = id => {
  return fetch.get(`trade/${id}`)
}

/**
 * 支付
 * @param {*} id 交易ID
 * @param {*} mode 支付方式
 * @param {*} type 支付类型
 */
export const payment = (id, mode, type) => {
  return fetch.post(`trade/${id}`, {
    mode,
    type
  })
}

/**
 * 获取支持的支付方式
 * @param {*} id 交易ID
 */
export const modes = id => {
  return fetch.get(`trade/modes/${id}`)
}

/**
 * 扫描支付
 * @param {*} id
 * @param {*} mode
 */
export const scan = (id, mode) => {
  return fetch.get(`trade/scan/${id}`, {
    params: {
      mode: mode
    }
  })
}

/**
 * 平台授权
 * @param {*} id
 * @param {*} mode
 */
export const platformLogin = (id, mode) => {
  location.href = `${process.env.GATEWAY}/trade/platform/login?id=${id}&mode=${mode}&redirect=${encodeURIComponent(location.href)}`
}
