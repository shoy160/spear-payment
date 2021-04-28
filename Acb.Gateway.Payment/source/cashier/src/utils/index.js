// /**
//  * 是否是微信内核
//  */
// export function isWechat () {
//   // return true
//   var ua = navigator.userAgent.toLowerCase()
//   return /micromessenger/i.test(ua)
// }

// /**
//  * 是否是支付宝内核
//  */
// export function isAlipay () {
//   // return true
//   var ua = navigator.userAgent.toLowerCase()
//   return /alipay/i.test(ua)
// }

/**
 * 微信支付Sdk调用
 * @param {*} paymentParams
 */
export function wechatSdk (paymentParams) {
  return new Promise((resolve, reject) => {
    function onBridgeReady () {
      window.WeixinJSBridge.invoke('getBrandWCPayRequest', paymentParams, function (res) {
        console.log(paymentParams)
        console.log(res)
        if (res.err_msg === 'get_brand_wcpay_request:ok') {
          resolve()
          // 使用以上方式判断前端返回,微信团队郑重提示：res.err_msg将在用户支付成功后返回    ok，但并不保证它绝对可靠。
        } else {
          var error = {
            message: ''
          }
          if (res.err_msg === 'get_brand_wcpay_request:cancel') {
            error.message = '支付取消'
          } else {
            // 支付失败
            error.message = res.err_desc + res.err_code
          }
          reject(error)
        }
      })
    }
    if (typeof WeixinJSBridge === 'undefined') {
      if (document.addEventListener) {
        document.addEventListener('WeixinJSBridgeReady', onBridgeReady, false)
      } else if (document.attachEvent) {
        document.attachEvent('WeixinJSBridgeReady', onBridgeReady)
        document.attachEvent('onWeixinJSBridgeReady', onBridgeReady)
      }
    } else {
      onBridgeReady()
    }
  })
}

/**
 * 支付宝支付sdk调用
 * @param {*} paymentParams 参数
 */
export function alipaySdk (paymentParams) {
  return new Promise((resolve, reject) => {
    function ready (callback) {
      // 如果jsbridge已经注入则直接调用
      if (window.AlipayJSBridge) {
        callback && callback()
      } else {
        // 如果没有注入则监听注入的事件
        document.addEventListener('AlipayJSBridgeReady', callback, false)
      }
    }
    ready(function () {
      console.log('start alipay')
      window.AlipayJSBridge.call('tradePay', paymentParams, function (result) {
        console.log(paymentParams)
        console.log(result)
        if (result.resultCode === '9000') {
          resolve()
        } else {
          var data = {
            message: result.resultCode === '8000' || result.resultCode === '6004' ? '支付异常' : '支付取消'
          }
          reject(data)
        }
      })
    })
  })
}
