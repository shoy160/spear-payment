import request from '@/utils/request'

/**
 * 添加渠道
 * @param {*} model 模型
 */
export const add = model => {
  return request.post('/manage/channel', model)
}

/**
 * 编辑渠道
 * @param {*} id 渠道ID
 * @param {*} model 模型
 */
export const edit = (id, model) => {
  return request.put(`/manage/channel/${id}`, model)
}

/**
 * 更新状态
 * @param {*} id
 * @param {*} status
 */
export const setStatus = (id, status) => {
  return request.put(`/manage/channel/status/${id}?status=${status}`)
}

/**
 * 设置默认支付渠道
 * @param {*} id 渠道ID
 * @param {*} isDefault 是否默认
 */
export const setDefault = (id, isDefault) => {
  return request.put(`/manage/channel/default/${id}?isDefault=${isDefault}`, {})
}

/**
 * 渠道列表
 * @param {*} mode 支付方式
 * @param {*} status 渠道状态
 */
export const list = query => {
  return request.get('/manage/channel', {
    params: query
  })
}
