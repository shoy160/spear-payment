<template>
  <el-main>
    <div class="app-container">
      <el-input v-model="query.keyword" placeholder="商户ID/渠道名称" style="width: 280px" />
      <el-select v-model="query.mode" clearable placeholder="支付类型">
        <el-option v-for="item in modes" :key="item.value" :label="item.text" :value="item.value" />
      </el-select>
      <el-select v-model="query.status" clearable placeholder="渠道状态">
        <el-option v-for="item in statusList" :key="item.value" :label="item.text" :value="item.value" />
      </el-select>
      <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleSearch">{{ $t('table.search') }}</el-button>
    </div>
    <el-row type="flex" justify="end">
      <el-col :span="3">
        <el-button type="primary" icon="el-icon-plus" @click="handleCreate">添加支付渠道</el-button>
      </el-col>
    </el-row>
    <el-table :data="list">
      <el-table-column type="expand">
        <template slot-scope="props">
          <code-viewer v-model="props.row.config" />
          <el-form inline class="table-expand">
            <el-form-item label="渠道备注">
              <span>{{ props.row.remark }}</span>
            </el-form-item>
          </el-form>
        </template>
      </el-table-column>
      <el-table-column type="index" />
      <el-table-column :label="$t('table.paymode')" :filters="modes" :filter-method="handleFilter" prop="mode">
        <template slot-scope="scope">
          <el-tag :type="['primary', 'success'][scope.row.mode]">{{ scope.row.modeCn }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.types')" prop="types">
        <template slot-scope="scope">
          <template v-if="scope.row.types.length>0">
            <el-tag v-for="type in scope.row.types" :key="type">{{ type | paymentType() }}</el-tag>
          </template>
          <template v-else>
            <el-tag type="success">全部类型</el-tag>
          </template>
        </template>

      </el-table-column>
      <el-table-column :label="$t('table.name')" prop="name" />
      <!-- <el-table-column :label="$t('table.remark')" prop="remark" min-width="100px" /> -->
      <el-table-column :label="$t('table.date')" prop="createTime" sortable>
        <template slot-scope="scope">
          {{ scope.row.createTime | parseTime('{y}-{m}-{d} {h}:{i}') }}
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.status')" prop="status" sortable>
        <template slot-scope="scope">
          <el-tag :type="['info', '', 'success', '', 'danger'][scope.row.status]">{{ scope.row.statusCn }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.isEnabled')">
        <template slot-scope="scope">
          <el-switch :value="scope.row.status" :disabled="scope.row.isDefault || (scope.row.status !== 1 && scope.row.status !== 2)" :active-value="2" :inactive-value="1" active-color="#409EFF" inactive-color="#999" @change="handleEnabled(scope.$index, scope.row)" />
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.isDefault')">
        <template slot-scope="scope">
          <el-switch :value="scope.row.isDefault" :disabled="scope.row.status !== 2" active-color="#13ce66" inactive-color="#999" @change="handleDefault(scope.$index, scope.row)" />
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.actions')">
        <template slot-scope="scope">
          <el-button :title="$t('table.edit')" size="mini" icon="el-icon-edit" circle @click="handleEdit(scope.$index, scope.row)" />
          <el-button :disabled="scope.row.status !== 0 && scope.row.status !== 1" :title="$t('table.delete')" size="mini" type="danger" icon="el-icon-delete" circle @click="handleDelete(scope.$index, scope.row)" />
        </template>
      </el-table-column>
    </el-table>
    <div class="pagination-container">
      <el-pagination :total="total" :page-sizes="[10, 20, 30, 50]" v-model="query.page" background layout="total, sizes, prev, pager, next" @current-change="handlePageChange" @size-change="handleSizeChange" />
    </div>

    <!--弹窗-->
    <el-dialog :visible.sync="modalVisible" :title="dialogTitleMap[dialogStatus]">
      <el-form ref="dataForm" :rules="rules" :model="temp" label-width="100px">
        <el-form-item label="支付方式" prop="mode">
          <el-select v-model="temp.mode" :disabled="dialogStatus === 'update'" @change="handleModeChange">
            <el-option v-for="item in modes" :key="item.value" :label="item.text" :value="item.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="支付类型" prop="mode">
          <el-checkbox :indeterminate="isIndeterminate" v-model="checkAll" @change="handleCheckAllChange">全选</el-checkbox>
          <div style="margin: 15px 0;"/>
          <el-checkbox-group v-model="temp.types" @change="handleTypesChange">
            <el-checkbox v-for="item in types" :label="item.value" :key="item.value">{{ item.text }}</el-checkbox>
          </el-checkbox-group>
        </el-form-item>
        <el-form-item label="应用ID" prop="appId">
          <el-col :span="11">
            <el-input v-model="temp.appId" />
          </el-col>
        </el-form-item>
        <el-form-item label="渠道名称" prop="name">
          <el-col :span="11">
            <el-input v-model="temp.name" />
          </el-col>
        </el-form-item>
        <el-form-item label="渠道备注" prop="remark">
          <el-input v-model="temp.remark" :rows="2" type="textarea" />
        </el-form-item>
        <el-form-item label="渠道配置" prop="config">
          <json-editor ref="jsonEditor" v-model="temp.config" />
          <!-- <el-input :rows="2" v-model="temp.config" type="textarea" /> -->
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="modalVisible = false">{{ $t('table.cancel') }}</el-button>
        <el-button type="primary" @click="handleSubmit">{{ $t('table.confirm') }}</el-button>
      </div>
    </el-dialog>
  </el-main>
</template>
<script>
import * as channelApi from '@/api/channel'
import { Message } from 'element-ui'
import JsonEditor from '@/components/JsonEditor'
import CodeViewer from '@/components/CodeViewer'
import { paymentSettings, paymentModes, paymentTypes } from '@/utils'
export default {
  name: 'Channel',
  components: {
    JsonEditor,
    CodeViewer
  },
  data() {
    return {
      modalVisible: false,
      dialogStatus: 'create',
      dialogTitleMap: {
        create: '添加支付渠道',
        update: '编辑支付渠道'
      },
      temp: {},
      rules: {
        mode: [
          { required: true, message: '请选择支付方式', trigger: 'change' }
        ],
        name: [{ required: true, message: '请输入渠道名称', trigger: 'blur' }],
        appId: [
          { required: true, message: '请输入渠道商户ID', trigger: 'blur' }
        ],
        config: [{ required: true, message: '请输入渠道配置', trigger: 'blur' }]
      },
      statusList: [
        { text: '已创建', value: 0 },
        { text: '已验证', value: 1 },
        { text: '已启用', value: 2 },
        { text: '已删除', value: 4 }
      ],
      modes: paymentModes,
      types: [],
      checkAll: false,
      isIndeterminate: false,
      query: {
        keyword: '',
        mode: '',
        page: 1,
        size: 10
      },
      total: 0,
      list: []
    }
  },
  mounted() {
    this.getList()
    for (var type in paymentTypes) {
      if (!paymentTypes.hasOwnProperty(type)) { continue }
      this.types.push({
        text: paymentTypes[type],
        value: ~~type
      })
    }
  },
  methods: {
    getList() {
      channelApi.list(this.query).then(json => {
        this.total = json.total
        this.list = json.data
      })
    },
    handleSearch() {
      this.query.page = 1
      this.getList()
    },
    handlePageChange(page) {
      this.query.page = page
      this.getList()
    },
    handleSizeChange(size) {
      this.query.size = size
      this.query.page = 1
      this.getList()
    },
    handleCreate() {
      // 添加
      this.modalVisible = true
      this.dialogStatus = 'create'
      this.temp = {
        name: '',
        remark: '',
        mode: 0,
        types: [],
        appId: '',
        config: Object.assign({}, paymentSettings[0])
      }
      this.checkAll = false
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    handleModeChange() {
      if (!this.modalVisible || this.dialogStatus !== 'create') {
        return
      }
      this.temp.config = Object.assign({}, paymentSettings[this.temp.mode])
    },
    handleCheckAllChange(val) {
      this.temp.types = val ? Object.keys(paymentTypes) : []
      this.isIndeterminate = false
    },
    handleTypesChange(val) {
      this.checkAll = val.length === this.types.length
      this.isIndeterminate = val.length > 0 && !this.checkAll
    },
    handleEdit(index, row) {
      // 编辑
      this.modalVisible = true
      this.dialogStatus = 'update'
      this.temp = Object.assign({}, row)
      this.checkAll = this.temp.types.length === this.types.length
      this.isIndeterminate = this.temp.types.length > 0 && !this.checkAll
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    handleDelete(index, row) {
      // 删除
      this.$confirm('确认要删除该支付渠道吗？').then(() => {
        channelApi.setStatus(row.id, 4).then(() => {
          this.$message.success('删除成功')
          this.getList()
        })
      })
      console.log(row)
    },
    handleDefault(index, row) {
      console.log(row)
      // 设置默认支付渠道
      channelApi.setDefault(row.id, !row.isDefault).then(() => {
        // row.isDefault = !row.isDefault
        // if (row.isDefault) {
        //   for (var index in this.list) {
        //     var item = this.list[index]
        //     if (item.mode === row.mode) {
        //       item.isDefault = item.id === row.id
        //     }
        //   }
        // }
        this.getList()
        Message({
          message: '设置成功',
          type: 'success'
        })
      })
    },
    handleEnabled(index, row) {
      console.log(row)
      var status = 0
      if (row.status === 1) {
        status = 2
      } else if (row.status === 2) {
        status = 1
      }
      if (status === 0) return
      channelApi.setStatus(row.id, status).then(() => {
        this.getList()
        Message({
          message: '设置成功',
          type: 'success'
        })
      })
    },
    handleFilter(value, row, column) {
      // 列表过滤
      const property = column['property']
      return row[property] === value
    },
    handleSubmit() {
      this.$refs['dataForm'].validate(valid => {
        if (valid) {
          if (this.dialogStatus === 'update') {
            channelApi.edit(this.temp.id, this.temp).then(() => {
              this.getList()
              Message({
                message: '编辑成功',
                type: 'success'
              })
              this.modalVisible = false
            })
          } else {
            channelApi.add(this.temp).then(result => {
              this.getList()
              Message({
                message: '添加成功',
                type: 'success'
              })
              this.modalVisible = false
            })
          }
        }
      })
    }
  }
}
</script>
