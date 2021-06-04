<template>
  <div class="app-container">
    <panel-group :panel-data="statistics" />
    <el-row style="background:#fff;padding:16px 16px 0;margin-bottom:32px;">
      <line-chart :chart-data="countChartData" />
    </el-row>
    <el-row style="background:#fff;padding:16px 16px 0;margin-bottom:32px;">
      <line-chart :chart-data="amountChartData" />
    </el-row>
  </div>
</template>
<script>
import PanelGroup from './dashboard/components/PanelGroup'
import LineChart from './dashboard/components/LineChart'
import { homedata } from '@/api/home'
export default {
  name: 'Home',
  components: {
    PanelGroup,
    LineChart
  },
  data() {
    return {
      countChartData: {
        xAxis: [],
        yAxis: []
      },
      amountChartData: {
        xAxis: [],
        yAxis: []
      },
      statistics: {}
    }
  },
  mounted() {
    homedata().then(json => {
      console.log(json)
      this.statistics = json.statistic
      // this.countChartData.xAxis = this.amountChartData.xAxis = Object.keys(json.platforms[0].data)
      var options = [
        {
          color: '#FF005A',
          easing: 'cubicInOut',
          duration: 2800
        },
        {
          color: '#3888fa',
          easing: 'quadraticOut',
          duration: 2800
        }
      ]
      var dates = []
      var alipayCounts = []
      var wechatCounts = []
      var alipayAmounts = []
      var wechatAmounts = []
      json.platforms.forEach((element, index) => {
        dates.push(element.date)
        alipayCounts.push(element.alipayCount)
        alipayAmounts.push(element.alipayAmount / 100.0)
        wechatCounts.push(element.wechatCount)
        wechatAmounts.push(element.wechatAmount / 100.0)
      })
      this.countChartData.xAxis = this.amountChartData.xAxis = dates
      this.countChartData.yAxis.push({
        title: '支付宝交易数',
        option: options[0],
        data: alipayCounts
      })
      this.countChartData.yAxis.push({
        title: '微信交易数',
        option: options[1],
        data: wechatCounts
      })
      this.amountChartData.yAxis.push({
        title: '支付宝交易额(元)',
        option: options[0],
        data: alipayAmounts
      })
      this.amountChartData.yAxis.push({
        title: '微信交易额(元)',
        option: options[1],
        data: wechatAmounts
      })
    })
  }
}
// 文档地址 http://element-cn.eleme.io/#/zh-CN/component/installation
</script>
