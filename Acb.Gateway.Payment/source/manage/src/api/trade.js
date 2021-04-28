import request from '@/utils/request'

/**
 * 交易列表
 * @param {*} mode 筛选模型
 */
export const tradeList = mode => {
  return request.post('/manage/trade/list', mode)
}

/**
 * 交易异步通知
 * @param {*} id 交易ID
 */
export const tradeNotify = id => {
  return request.put(`/manage/trade/notify/${id}`)
}

/**
 * 检验支付
 * @param {string} tradeNo
 */
export const verifyTrade = tradeNo => {
  return request.get(`/manage/trade/verify/${tradeNo}`)
}

export const refund = (tradeNo, amount = -1, reason = '') => {
  return request.put(`/manage/trade/refund/${tradeNo}`, {
    params: {
      amount: amount,
      reason: reason
    }
  })
}

/**
 * 通知列表
 * @param {*} mode 筛选模型
 */
export const notifyList = node => {
  return request.get('/manage/trade/notifys', {
    params: node
  })
}
