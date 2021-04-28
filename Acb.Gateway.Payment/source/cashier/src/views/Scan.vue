<template>
  <div class="scan" :class="mode >=0 ?`mode-${mode}`:''">
    <div class="scan-container" v-if="mode === -1">
      <h3>扫码付款</h3>
      <div class="qr">
        <vue-qr :text="code" :logoSrc="logo" :size="size" :margin="margin" />
      </div>
      <div class="logo">
        <img :src="payLogo" alt="">
      </div>
    </div>
    <div v-else-if="mode === 1" class="wechat">
      <h3>微信支付</h3>
      <div class="qr-box">
        <vue-qr class="qr" :text="code" :logoSrc="logo" :size="size" :margin="margin" />
        <img :src="timg" class="timg" alt="">
      </div>
      <h5>{{ mobile ? '长按' : '扫描' }}二维码识别</h5>
    </div>
    <div v-else-if="mode === 0" class="alipay">
      <template v-if="alipay">
        <h4>即将跳转到支付页面({{time}}s)</h4>
        <!-- <div class="redpack">
          <img :src="redpack" alt="">
          <mt-button type="primary" size="small" @click.native="handleRedpack">立即领取</mt-button>
        </div> -->
      </template>
      <template v-else>
        <h3>支付宝支付</h3>
        <div class="qr-box">
          <vue-qr class="qr" :text="code" :logoSrc="logo" :size="size" :margin="margin" />
          <img :src="timg" class="timg" alt="">
        </div>
        <h5>{{ mobile ? '长按' : '扫描' }}二维码识别</h5>
      </template>
    </div>
  </div>
</template>
<script>
import { modes, scan } from '@/services'
import { Indicator } from 'mint-ui'
import { UA } from 'singerjs'
import VueQr from 'vue-qr'

export default {
  name: 'Scan',
  data () {
    return {
      mode: -1,
      mobile: false,
      alipay: false,
      wechat: false,
      detailId: '',
      code: '',
      logo: '/static/icb.png',
      payLogo: '/static/pay_logo.png',
      timg: '/static/timg.gif',
      redpack: '/static/alipay_redpack.png',
      redpackUrl: 'hEYzc1x02556gmntkj9jkovdf382wD', // 'https://qr.alipay.com/c1x02556gmntkj9jkovdf38',
      size: 300,
      margin: 0,
      timer: null,
      time: 8
    }
  },
  components: {
    VueQr
  },
  created () {
    this.detailId = this.$route.params.id
    if (this.$route.query.mode !== undefined) {
      this.mode = ~~this.$route.query.mode
    }
    var ua = UA()
    this.alipay = ua.alipay
    this.wechat = ua.wechat
    this.mobile = ua.mobile
    if (this.wechat) {
      this.mode = 1
    } else if (this.alipay) {
      this.mode = 0
    } else if (this.mobile) {
      // 手机浏览器 -> H5收银台
      this.$router.replace(`/${this.detailId}`)
    }
  },
  mounted () {
    if (this.mode === 0 || this.mode === 1) {
      this.loadScan()
      return
    }
    this.loadModes()
  },
  methods: {
    /**
     * 加载支付方式
     */
    loadModes () {
      Indicator.open({
        text: '加载中...',
        spinnerType: 'fading-circle'
      })
      modes(this.detailId)
        .then(json => {
          if (json.length === 1) {
            var item = json[0]
            this.mode = item.mode
            if (item.url) {
              this.code = item.url
            } else {
              this.loadScan()
            }
          } else {
            this.code = location.href
          }
          Indicator.close()
        })
        .catch(e => {
          Indicator.close()
        })
    },
    /**
     * 加载支付码
     */
    loadScan () {
      Indicator.open({
        text: '加载中...',
        spinnerType: 'fading-circle'
      })
      scan(this.detailId, this.mode)
        .then(json => {
          Indicator.close()
          if (this.alipay) {
            location.href = json
            // // 支付宝扫码直接跳转
            // this.timer = later(
            //   () => {
            //     this.time--
            //     if (this.time <= 0) {
            //       this.timer.cancel()
            //       location.href = json
            //     }
            //   },
            //   1000,
            //   true
            // )
            return
          }
          this.code = json
        })
        .catch(e => {
          Indicator.close()
        })
    },
    handleRedpack () {
      this.timer && this.timer.cancel()
      location.href = this.redpackUrl
    }
  },
  destroyed () {
    this.timer && this.timer.cancel()
  }
}
</script>
<style>
#app {
  height: 100%;
}
</style>

<style scoped>
.scan {
  background-color: #5c92ef;
  width: 100%;
  min-height: 100%;
}
.scan.mode-1 {
  background-color: #54bc6e;
}
.scan.mode-0 {
  background-color: #c01920;
}
.scan-container {
  max-width: 768px;
  margin: 0 auto;
  text-align: center;
  border: 1px solid transparent;
}
.scan-container h3 {
  color: #fff;
  font-size: 1.5rem;
  margin: 2rem 0;
}
.scan-container .qr {
  background-color: #fff;
  padding: 10px;
  margin: 0 3rem;
  border-radius: 5px;
  display: inline-block;
}
.scan-container .logo {
  margin: 1rem 0;
}
.wechat {
  max-width: 768px;
  margin: 0 auto;
  border: 1px solid transparent;
}
.wechat h3,
.alipay h3 {
  color: #fff;
  font-size: 1.5rem;
  margin: 1.5rem 0;
  text-align: center;
}
.wechat h5,
.alipay h5 {
  color: #fff;
  font-size: 1.2rem;
  margin: 3rem 0;
  text-align: center;
}
.wechat .qr-box,
.alipay .qr-box {
  font-size: 0;
  background-color: #fff;
}
.wechat .qr,
.alipay .qr {
  width: 65%;
  display: inline-block;
  vertical-align: middle;
  padding: 1.5rem;
}
.wechat .timg,
.alipay .timg {
  width: 35%;
  display: inline-block;
  vertical-align: middle;
}
.wechat .logo {
  text-align: center;
}
.alipay {
  max-width: 768px;
  margin: 0 auto;
  position: relative;
  border: 1px solid transparent;
}
.alipay h4 {
  color: #fff;
  font-size: 0.8rem;
  text-align: center;
  position: absolute;
  top: 2px;
  width: 100%;
}
.alipay .redpack {
  text-align: center;
  margin: 2rem 1rem 0;
}
.alipay .redpack img {
  display: block;
  margin: 0 auto 1rem;
}
.alipay button {
  background-color: #f0ad4e;
  padding: 0 1.5rem;
}
</style>
