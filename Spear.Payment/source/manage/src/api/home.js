import request from '@/utils/request'

export const homedata = () => {
  return request.get('/manage/home/statistic')
}
