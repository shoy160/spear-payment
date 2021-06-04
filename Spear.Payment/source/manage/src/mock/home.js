var AP = Date.prototype
AP.addDays = AP.addDays || function(days) {
  this.setDate(this.getDate() + days)
  return this
}
AP.format = AP.format || function(strFormat) {
  var o = {
    'M+': this.getMonth() + 1,
    'd+': this.getDate(),
    'h+': this.getHours(),
    'm+': this.getMinutes(),
    's+': this.getSeconds(),
    'q+': Math.floor((this.getMonth() + 3) / 3), // 季度
    'S': this.getMilliseconds() // 毫秒
  }
  if (/(y+)/.test(strFormat)) {
    strFormat = strFormat.replace(RegExp.$1, (this.getFullYear() + '').substr(4 - RegExp.$1.length))
  }
  for (var k in o) {
    if (new RegExp('(' + k + ')').test(strFormat)) {
      strFormat =
        strFormat.replace(RegExp.$1, (RegExp.$1.length === 1)
          ? (o[k])
          : (('00' + o[k]).substr(('' + o[k]).length)))
    }
  }
  return strFormat
}
/**
 * 随机数
 * @param {*} min
 * @param {*} max
 * @param {int} dits
 */
const random = (min, max, dits) => {
  var r = Math.random()
  var number = r * (max - min) + min
  dits = dits > 0 ? Math.pow(10, dits) : 0
  return dits ? Math.floor(number * dits) / dits : Math.floor(number)
}

/**
 * 模拟交易
 * @param {*} days
 */
const mockTrading = (days, max, dits) => {
  var data = {}
  for (var i = days; i > 0; i--) {
    var date = new Date().addDays(-i)
    var key = date.format('yyyy-MM-dd')
    data[key] = random(0, max, dits)
  }
  return data
}

export default {
  homedata: () => {
    return {
      statistic: {
        todayCount: random(0, 200),
        todayAmount: random(0, 5000, 2),
        count: random(200, 2000),
        amount: random(1000, 100000, 2)
      },
      platforms: [{
        name: '支付宝交易数',
        data: mockTrading(10, 500)
      }, {
        name: '支付宝交易金额(元)',
        data: mockTrading(10, 50000, 2)
      }, {
        name: '微信交易数',
        data: mockTrading(10, 500)
      }, {
        name: '微信交易金额(元)',
        data: mockTrading(10, 50000, 2)
      }]
    }
  }
}
