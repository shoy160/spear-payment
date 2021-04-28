<template>
  <el-main>
    <div v-if="role===64" class="app-container">
      <el-input v-model="query.keyword" placeholder="项目名称/项目编码" style="width: 280px" />
      <el-select v-model="query.status" clearable placeholder="项目状态">
        <el-option v-for="item in statusList" :key="item.value" :label="item.text" :value="item.value" />
      </el-select>
      <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleSearch">{{ $t('table.search') }}</el-button>
    </div>
    <el-row v-if="role ===64" type="flex" justify="end">
      <el-col :span="3">
        <el-button type="primary" icon="el-icon-plus" @click="handleCreate">添加项目</el-button>
      </el-col>
    </el-row>
    <el-table v-loading="listLoading" :data="list">
      <el-table-column type="expand">
        <template slot-scope="props">
          <el-form inline class="table-expand">
            <el-form-item label="项目密钥">
              <span>{{ props.row.secret }}</span>
            </el-form-item>
            <el-form-item label="异步通知">
              <span>{{ props.row.notifyUrl }}</span>
            </el-form-item>
            <el-form-item label="同步跳转">
              <span>{{ props.row.redirectUrl }}</span>
            </el-form-item>
          </el-form>
        </template>
      </el-table-column>
      <el-table-column type="index" />
      <el-table-column :label="$t('table.code')" prop="code" />
      <el-table-column :label="$t('table.name')" prop="name" />
      <el-table-column :label="$t('table.channel')">
        <template slot-scope="scope">
          <el-tag v-for="channel in scope.row.channelModels" :key="channel.id" type="info">{{ channel.name }}{{ channel.types | showTypes() }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.date')" prop="createTime" sortable>
        <template slot-scope="scope">
          {{ scope.row.createTime | parseTime('{y}-{m}-{d} {h}:{i}') }}
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.status')">
        <template slot-scope="scope">
          <el-tag>{{ scope.row.statusCn }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('table.actions')">
        <template slot-scope="scope">
          <el-button
            :title="$t('table.edit')"
            size="mini"
            icon="el-icon-edit"
            circle
            @click="handleEdit(scope.$index, scope.row)"/>
          <el-button
            v-if="role ===64"
            :title="$t('table.delete')"
            size="mini"
            type="danger"
            icon="el-icon-delete"
            circle
            @click="handleDelete(scope.$index, scope.row)"/>
        </template>
      </el-table-column>
    </el-table>
    <div v-if="role ===64" class="pagination-container">
      <el-pagination
        :total="total"
        :page-sizes="[10, 20, 30, 50]"
        background
        layout="total, sizes, prev, pager, next"
        @current-change="handleCurrentChange"
        @size-change="handleSizeChange" />
    </div>
    <!--弹窗-->
    <el-dialog :visible.sync="modalVisible" :title="dialogTitleMap[dialogStatus]">
      <el-form ref="dataForm" :rules="rules" :model="temp" label-width="100px">
        <el-form-item label="项目编码" prop="code">
          <el-col :span="11">
            <el-input v-model="temp.code" :disabled="dialogStatus === 'update'" />
          </el-col>
        </el-form-item>
        <el-form-item label="项目名称" prop="name">
          <el-col :span="11">
            <el-input v-model="temp.name" />
          </el-col>
        </el-form-item>
        <el-form-item v-if="dialogStatus === 'update'" label="项目密钥" prop="secret">
          <el-col :span="11">
            <el-input v-model="temp.secret" disabled>
              <template slot="append">
                <el-button @click="handleChangeSecret">更新</el-button>
              </template>
            </el-input>
          </el-col>
        </el-form-item>
        <el-form-item label="登陆密码" prop="password">
          <el-col :span="11">
            <el-input v-model="temp.password" type="password"/>
          </el-col>
        </el-form-item>
        <el-form-item label="队列名称">
          <el-col :span="11">
            <el-input v-model="temp.queueName" />
          </el-col>
        </el-form-item>
        <el-form-item label="异步通知">
          <el-input v-model="temp.notifyUrl" />
        </el-form-item>
        <el-form-item label="同步回调">
          <el-input v-model="temp.redirectUrl" />
        </el-form-item>
        <el-form-item label="支付渠道">
          <el-checkbox-group v-model="temp.channels">
            <el-checkbox v-for="channel in channels" :key="channel.id" :label="channel.id">{{ channel.name }}{{ channel.types | showTypes() }}</el-checkbox>
          </el-checkbox-group>
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
import * as projectApi from '@/api/project'
import { list as channelList } from '@/api/channel'
import { Message } from 'element-ui'
export default {
  name: 'Project',
  data() {
    return {
      modalVisible: false,
      dialogStatus: 'create',
      role: 0,
      dialogTitleMap: {
        create: '添加支付项目',
        update: '编辑支付项目'
      },
      temp: {},
      rules: {
        code: [{ required: true, message: '请输入项目编码', trigger: 'blur' }],
        name: [{ required: true, message: '请输入项目名称', trigger: 'blur' }]
      },
      channels: [],
      defaultChannels: [],
      total: 0,
      query: {
        keyword: '',
        status: '',
        page: 1,
        size: 10
      },
      listLoading: true,
      statusList: [
        { text: '正常', value: 0 },
        { text: '删除', value: 4 }
      ],
      list: []
    }
  },
  mounted() {
    this.getList()
    this.role = this.$store.getters.role
  },
  methods: {
    getChannels() {
      if (this.channels.length > 0) {
        return
      }
      channelList({
        status: 2,
        page: 1,
        size: 500
      }).then(json => {
        for (var index in json.data) {
          var channel = json.data[index]
          if (channel.isDefault) {
            this.defaultChannels.push(channel.id)
          }
          this.channels.push({
            id: channel.id,
            name: channel.name,
            types: channel.types
          })
        }
      })
    },
    getList() {
      this.listLoading = true
      projectApi.list(this.query).then(json => {
        this.list = json.data
        this.total = json.total
        this.listLoading = false
      })
    },
    handleSearch() {
      this.query.page = 1
      this.getList()
    },
    handleCreate() {
      this.getChannels()
      // 添加
      this.modalVisible = true
      this.dialogStatus = 'create'
      this.temp = {
        code: '',
        name: '',
        notifyUrl: '',
        redirectUrl: '',
        channels: this.defaultChannels,
        channelModels: []
      }
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    handleEdit(index, row) {
      this.getChannels()
      // 编辑
      this.modalVisible = true
      this.dialogStatus = 'update'
      this.temp = Object.assign({}, row)
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    handleDelete(index, row) {
      // 删除
      this.$confirm('确认要删除该项目？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'danger'
      })
        .then(() => {
          projectApi.remove(row.id).then(() => {
            this.getList()
            Message({
              message: '删除成功',
              type: 'success'
            })
          })
        })
        .catch(() => {})
    },
    handleSubmit() {
      this.$refs['dataForm'].validate(valid => {
        if (valid) {
          if (this.dialogStatus === 'update') {
            projectApi.edit(this.temp.id, this.temp).then(() => {
              this.getList()
              Message({
                message: '编辑成功',
                type: 'success'
              })
              this.modalVisible = false
            })
          } else {
            projectApi.add(this.temp).then(result => {
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
    },
    handleSizeChange(val) {
      this.query.size = val
      this.query.page = 1
      this.getList()
    },
    handleCurrentChange(val) {
      this.query.page = val
      this.getList()
    },
    handleChangeSecret() {
      this.$confirm('更新项目密钥将会影响项目支付，确认更新？')
        .then(() => {
          projectApi.updateSecret(this.temp.id)
            .then(() => {
              this.$message.success('更新成功')
              this.getList()
              this.modalVisible = false
            })
        })
    }
  }
}
</script>
<style lang="scss">
.el-select .el-input {
  width: 120px;
}
.input-with-select .el-input-group__prepend {
  background-color: #fff;
}
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

