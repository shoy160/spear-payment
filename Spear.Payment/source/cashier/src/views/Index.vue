<template>
  <div>
    <mt-header title="收银台"></mt-header>
    <div class="p-amount">
      <h3>{{trade.title}}</h3>
      <div>
        <small>￥</small>
        <em>{{trade.amount | formatMoney()}}</em>
      </div>
      <div class="p-body" v-if="trade.body">
        {{trade.body}}
      </div>
    </div>
    <div class="mint-radiolist">
      <label class="mint-radiolist-title">选择支付方式</label>
      <a class="mint-cell" v-for="item in modes" :key="item.value">
        <div class="mint-cell-left"></div>
        <div class="mint-cell-wrapper">
          <div class="mint-cell-title">
            <label class="mint-radiolist-label">
              <span class="mint-radio is-right">
                <input type="radio" name="mode" class="mint-radio-input" v-model="mode" :value="item.value">
                <span class="mint-radio-core"></span>
              </span>
              <span class="mint-radio-label">
                <img :src="item.icon" :alt="item.label"> {{item.label}}
              </span>
            </label>
          </div>
          <div class="mint-cell-value">
            <span></span>
          </div>
        </div>
        <div class="mint-cell-right"></div>
      </a>
    </div>
    <div class="p-btn">
      <mt-button type="primary" size="large" @click.native="paymentHandler" :disabled="btnDisabled">确认支付</mt-button>
    </div>
    <mt-popup v-model="paying" :closeOnClickModal="false" popup-transition="popup-fade">
      <div class="paying-dialog">
        <h5>请确认支付是否完成</h5>
        <mt-button type="primary" size="large" plain @click.native="handleComplete">已完成支付</mt-button>
        <mt-button type="default" size="large" plain @click.native="handleRepay">支付遇到问题，重新支付</mt-button>
      </div>
    </mt-popup>
  </div>
</template>
<script>
import iconMicropay from '@/assets/img/pay_weixin.png'
import iconAlipay from '@/assets/img/pay_alipay.png'
import { UA } from 'singerjs'
import { wechatSdk, alipaySdk } from '@/utils'
import { detail, payment, platformLogin } from '@/services'
import { Indicator, Toast } from 'mint-ui'
export default {
  name: 'index',
  data () {
    return {
      mode: 1,
      type: 1,
      alipay: false,
      wechat: false,
      detailId: '',
      trade: {
        amount: 0
      },
      paying: false,
      btnDisabled: true,
      modes: []
    }
  },
  created () {
    this.detailId = this.$route.params.id
    var ua = UA()
    if (!ua.mobile) {
      this.$router.replace(`/scan/${this.detailId}`)
    }
    this.alipay = ua.alipay || false
    this.wechat = ua.wechat || false

    if (this.alipay) {
      // APP支付
      this.type = 2
    }
    if (this.wechat) {
      // 公众号支付
      this.type = 3
    }
  },
  mounted () {
    if (!this.wechat) {
      this.modes.push({
        label: '支付宝',
        value: '0',
        icon: iconAlipay
      })
    }
    if (!this.alipay) {
      this.modes.push({
        label: '微信',
        value: '1',
        icon: iconMicropay
      })
    }
    if (this.modes.length === 0) {
      return
    }
    Indicator.open({
      text: '加载中...',
      spinnerType: 'fading-circle'
    })
    detail(this.detailId)
      .then(json => {
        if (json.status === 1) {
          // 已支付
          location.href = json.redirectUrl
          return
        }
        if (json.status !== 0) {
          Toast('交易状态异常')
          Indicator.close()
          this.btnDisabled = true
          return
        }
        if (this.wechat && !json.platformId) {
          // 授权
          platformLogin(this.detailId, 1)
          return
        }
        // if (this.alipay && !json.platformId) {
        //   // 授权
        //   platformLogin(this.detailId, 0)
        //   return
        // }
        this.btnDisabled = false
        this.trade = json
        if (this.modes.length > 1) {
          this.mode = json.mode
        } else {
          this.mode = this.modes[0].value
        }
        Indicator.close()
      })
      .catch(e => {
        Indicator.close()
      })
  },
  methods: {
    paymentHandler () {
      this.btnDisabled = true
      Indicator.open({
        text: '支付中...',
        spinnerType: 'fading-circle'
      })
      // 支付调用
      payment(this.detailId, this.mode, this.type)
        .then(json => {
          Indicator.close()
          if (this.alipay) {
            // 支付宝内支付
            this.paying = true
            alipaySdk(json).then(() => {
              if (this.trade.redirectUrl) {
                location.href = this.trade.redirectUrl
              } else {
                this.paying = this.btnDisabled = false
              }
            })
          } else if (this.wechat) {
            // 微信内支付
            this.paying = true
            wechatSdk(json).then(() => {
              if (this.trade.redirectUrl) {
                location.href = this.trade.redirectUrl
              } else {
                this.paying = this.btnDisabled = false
              }
            })
          } else {
            location.href = json
          }
        })
        .catch(e => {
          Indicator.close()
          this.paying = this.btnDisabled = false
        })
    },
    handleRepay () {
      location.reload(true)
    },
    handleComplete () {
      if (this.trade.redirectUrl) {
        location.href = this.trade.redirectUrl
      } else {
        this.paying = this.btnDisabled = false
      }
    }
  }
}
</script>
<style scoped>
.p-amount {
  background-color: #fff;
  padding: 1rem 0 2rem;
  text-align: center;
}

.p-amount h3 {
  font-size: 1rem;
  color: #888;
  margin: 0 0 0.3rem;
}

.p-amount small {
  font-size: 1.5rem;
  color: #ff3300;
}

.p-amount em {
  font-size: 2.2rem;
  color: #ff3300;
  font-style: normal;
  /* font-family: Georgia, 'Times New Roman', Times, serif; */
}

.p-amount .p-body {
  color: #aaa;
  padding: 0.7rem 1rem 0 1rem;
}

/* .mint-radiolist-label {
    padding: 0 0.3rem;
  } */
.mint-header {
  background-color: rgba(222, 222, 222, 0.5);
  color: #666;
  font-size: 1rem;
}

.mint-radio-label img {
  width: 1.5rem;
  padding: 0 0.3rem 0 0;
}

.p-btn {
  padding: 1.5rem 1.2rem;
}

.paying-dialog {
  width: 15rem;
}

.paying-dialog h5 {
  margin: 0.5rem 0;
  text-align: center;
  font-size: 1rem;
  color: #666;
}

.paying-dialog .mint-button {
  border-radius: 0;
  border-color: #bbb;
  border-left: none;
  border-right: none;
  border-bottom: none;
  font-size: 1rem;
}
</style>
