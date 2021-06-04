<template>
  <el-main class="app-container">
    <div class="app-container">
      <el-input v-model="listQuery.tradeNo" placeholder="交易号/订单号" style="width: 300px" />
      <el-select v-model="listQuery.type" clearable placeholder="通知类型">
        <el-option v-for="item in types" :key="item.value" :label="item.text" :value="item.value" />
      </el-select>
      <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleSearch">{{ $t('table.search') }}</el-button>
    </div>
    <el-table v-loading="listLoading" :data="list">
      <el-table-column type="expand">
        <template slot-scope="props">
          <code-viewer v-model="props.row.content" />
        </template>
      </el-table-column>
      <el-table-column type="index" />
      <el-table-column :filters="types" :filter-method="handleFilter" prop="type" label="通知类型">
        <template slot-scope="scope">
          <el-tag :type="['', 'warning'][scope.row.type]">{{ scope.row.typeCn }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="tradeNo" label="交易号" />
      <el-table-column prop="url" label="地址" />
      <el-table-column prop="result" label="结果" />
      <el-table-column :label="$t('table.date')" prop="createTime" sortable>
        <template slot-scope="scope">
          {{ scope.row.createTime | parseTime('{y}-{m}-{d} {h}:{i}:{s}') }}
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.status')">
        <template slot-scope="scope">
          <el-tag :type="['danger','success'][scope.row.status]">{{ scope.row.statusCn }}</el-tag>
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
        @size-change="handleSizeChange" />
    </div>
  </el-main>
</template>
<script>
import { notifyList } from '@/api/trade'
import CodeViewer from '@/components/CodeViewer'
export default {
  name: 'TradeNotify',
  components: {
    CodeViewer
  },
  data() {
    return {
      types: [{ text: '接收', value: 0 }, { text: '发送', value: 1 }],
      list: [],
      listLoading: true,
      listTotal: 0,
      listQuery: {
        page: 1,
        size: 10,
        type: null,
        tradeNo: ''
      }
    }
  },
  mounted() {
    this.getList()
  },
  methods: {
    handleFilter(value, row, column) {
      // 列表过滤
      const property = column['property']
      return row[property] === value
    },
    getList() {
      this.listLoading = true
      notifyList(this.listQuery).then(json => {
        this.list = json.data
        this.listTotal = json.total
        this.listLoading = false
      })
    },
    handleSearch() {
      this.listQuery.page = 1
      this.getList()
    },
    handleSizeChange(val) {
      this.listQuery.size = val
      this.handleSearch()
    },
    handleCurrentChange(val) {
      this.listQuery.page = val
      this.getList()
    }
  }
}
</script>
