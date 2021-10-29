<template>
  <el-main class="app-container">
    <div class="app-container">
      <el-input
        v-model="listQuery.tradeNo"
        placeholder="交易号/订单号"
        style="width: 300px"
      />
      <el-select v-model="listQuery.mode" clearable placeholder="支付方式">
        <el-option
          v-for="item in modes"
          :key="item.value"
          :label="item.text"
          :value="item.value"
        />
      </el-select>
      <el-select v-model="listQuery.type" clearable placeholder="支付类型">
        <el-option
          v-for="item in types"
          :key="item.value"
          :label="item.text"
          :value="item.value"
        />
      </el-select>
      <el-select v-model="listQuery.status" clearable placeholder="支付状态">
        <el-option
          v-for="item in statusList"
          :key="item.key"
          :value="item.key"
          :label="item.text"
        />
      </el-select>
      <el-button
        class="filter-item"
        type="primary"
        icon="el-icon-search"
        @click="handleSearch"
      >
        {{ $t("table.search") }}
      </el-button>
    </div>
    <el-table v-loading="listLoading" :data="list">
      <el-table-column type="expand">
        <template slot-scope="props">
          <el-form inline class="table-expand">
            <el-form-item label="商户订单号">
              <span>{{ props.row.orderNo }}</span>
            </el-form-item>
            <el-form-item label="第三方交易号">
              <span>{{ props.row.outTradeNo }}</span>
            </el-form-item>
            <el-form-item label="跳转地址">
              <span>{{ props.row.redirectUrl }}</span>
            </el-form-item>
            <el-form-item label="支付描述">
              <span>{{ props.row.body }}</span>
            </el-form-item>
            <el-form-item label="扩展信息">
              <span>{{ props.row.extend }}</span>
            </el-form-item>
          </el-form>
        </template>
      </el-table-column>
      <el-table-column type="index" />
      <el-table-column prop="tradeNo" label="交易号" width="180px" />
      <el-table-column prop="title" label="标题" min-width="100px" />
      <el-table-column prop="projectName" label="项目" />
      <el-table-column label="金额">
        <template slot-scope="scope">
          <b
            style="color: #f56c6c"
          >￥ {{ scope.row.amount | amountFormatter() }}</b
          >
        </template>
      </el-table-column>
      <el-table-column
        :label="$t('table.paymode')"
        :filters="modes"
        :filter-method="handleFilter"
        prop="mode"
      >
        <template slot-scope="scope">
          <el-tag :type="['', 'info'][scope.row.mode]">{{
            scope.row.modeCn
          }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.paytype')" prop="type">
        <template slot-scope="scope">
          <el-tag type="warning">{{ scope.row.type }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.status')">
        <template slot-scope="scope">
          <el-tag :type="['danger', 'success'][scope.row.status]">{{
            scope.row.statusCn
          }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.date')" prop="createTime" sortable>
        <template slot-scope="scope">
          {{ scope.row.createTime | parseTime("{y}-{m}-{d} {h}:{i}:{s}") }}
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.paidTime')" prop="paidTime" sortable>
        <template slot-scope="scope">
          {{ scope.row.paidTime | parseTime("{y}-{m}-{d} {h}:{i}:{s}") }}
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.actions')">
        <template slot-scope="scope">
          <el-button
            v-if="scope.row.status === 1 || scope.row.status === 3"
            :disabled="
              !scope.row.notify ||
                scope.row.notifying ||
                scope.row.notifyTime > 0
            "
            :title="
              scope.row.notifyTime > 0
                ? `${scope.row.notifyTime}秒`
                : '异步通知'
            "
            size="mini"
            icon="el-icon-bell"
            circle
            @click="handleNotify(scope.$index, scope.row)"
          />
          <el-button
            v-if="scope.row.status === 1"
            type="danger"
            title="退款"
            size="mini"
            icon="el-icon-remove"
            circle
            @click="handleRefund(scope.row.tradeNo)"
          />
          <a
            v-if="scope.row.status === 0"
            :href="`/#/scan/${scope.row.id}`"
            class="el-button el-button--primary el-button--mini is-circle"
            target="_blank"
            title="收银台"
          >
            <i class="fa fa-money" />
          </a>
          <el-button
            v-if="scope.row.status === 0"
            title="支付校验"
            size="mini"
            icon="el-icon-refresh"
            circle
            @click="handleVerifyTrade(scope.row.tradeNo)"
          />
        </template>
      </el-table-column>
    </el-table>
    <div class="pagination-container">
      <el-pagination
        :total="listTotal"
        :page-sizes="[10, 20, 30, 50]"
        background
        layout="total, sizes, prev, pager, next"
        @current-change="handleCurrentChange"
        @size-change="handleSizeChange"
      />
    </div>
  </el-main>
</template>
<script>
import { tradeList, tradeNotify, verifyTrade, refund } from '@/api/trade'
export default {
  name: 'Trade',
  data() {
    return {
      modes: [{ text: '支付宝', value: 0 }, { text: '微信', value: 1 }],
      types: [
        { value: 0, text: 'Web' },
        { value: 1, text: 'H5' },
        { value: 2, text: 'App' },
        { value: 3, text: 'Public' },
        { value: 4, text: 'Scan' },
        { value: 5, text: 'Barcode' },
        { value: 6, text: 'Applet' }
      ],
      statusList: [
        { key: 0, text: '待支付' },
        { key: 1, text: '已支付' },
        { key: 2, text: '已关闭' },
        { key: 3, text: '已退款' },
        { key: 4, text: '退款中' }
      ],
      list: [],
      listLoading: true,
      listTotal: 0,
      notifyInterval: 60,
      listQuery: {
        page: 1,
        size: 10,
        mode: null,
        type: undefined,
        tradeNo: ''
      },
      timer: null
    }
  },
  mounted() {
    this.getList()
  },
  methods: {
    startTimer() {
      if (this.timer) return
      // 启动定时器
      this.timer = setInterval(() => {
        var continueTimer = false
        this.list.forEach((item, index) => {
          if (!item.notifyTime || item.notifyTime <= 0) {
            return
          }
          item.notifyTime--
          this.$set(this.list, index, item)
          if (!continueTimer && item.notifyTime > 0) {
            continueTimer = true
          }
        })
        if (!continueTimer) {
          clearInterval(this.timer)
          this.timer = null
        }
      }, 1000)
    },
    getList() {
      this.listLoading = true
      tradeList(this.listQuery).then(json => {
        this.list = json.data
        this.listTotal = json.total
        this.listLoading = false
      })
    },
    handleSearch() {
      this.listQuery.page = 1
      this.getList()
    },
    handleNotify(index, row) {
      // 通知
      row.notifying = true
      this.$set(this.list, index, row)
      tradeNotify(row.id)
        .then(() => {
          this.$message.success('通知成功')
          row.notifying = false
          row.notifyTime = this.notifyInterval
          this.startTimer()
        })
        .catch(() => {
          row.notifying = false
          this.$set(this.list, index, row)
        })
    },
    handleFilter(value, row, column) {
      // 列表过滤
      const property = column['property']
      return row[property] === value
    },
    handleSizeChange(val) {
      this.listQuery.size = val
      this.listQuery.page = 1
      this.getList()
    },
    handleCurrentChange(val) {
      this.listQuery.page = val
      this.getList()
    },
    handleVerifyTrade(tradeNo) {
      verifyTrade(tradeNo).then(() => {
        this.$message.success('校验成功')
        this.getList()
      })
    },
    handleRefund(tradeNo) {
      this.$prompt('请输入退款原因', '交易退款', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        inputPattern: /^.+$/,
        inputErrorMessage: '请输入退款原因'
      }).then(({ value }) => {
        refund(tradeNo, -1, value || '').then(() => {
          this.$message.success('退款成功')
          this.getList()
        })
      })
    }
  },
  distroyed() {
    this.timer && clearInterval(this.timer)
  }
}
</script>
<style lang="scss">
.table-expand {
  font-size: 0;
  label {
    width: 100px;
    color: #99a9bf;
  }
  .el-form-item {
    margin-right: 0;
    margin-bottom: 0;
    width: 50%;
  }
}
</style>
