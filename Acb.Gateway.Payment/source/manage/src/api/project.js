import request from '@/utils/request'

/**
 * 添加项目
 * @param {*} model 模型
 */
export const add = model => {
  return request.post('/manage/project', model)
}

/**
 * 编辑项目
 * @param {*} id 项目ID
 * @param {*} model 模型
 */
export const edit = (id, model) => {
  return request.put(`/manage/project/${id}`, model)
}

/**
 * 更新密钥
 * @param {*} id 项目ID
 */
export const updateSecret = id => {
  return request.put(`/manage/project/secret/${id}`)
}

/**
 * 删除项目
 * @param {*} id 项目ID
 */
export const remove = id => {
  return request.delete(`/manage/project/${id}`)
}

/**
 * 项目列表
 * @param {*} page 分页页码
 * @param {*} size 单页数量
 */
export const list = query => {
  return request.get('/manage/project', {
    params: query
  })
}
