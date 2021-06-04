<template>
  <el-menu class="navbar" mode="horizontal">
    <hamburger
      :toggle-click="toggleSideBar"
      :is-active="sidebar.opened"
      class="hamburger-container"
    />

    <breadcrumb class="breadcrumb-container" />

    <div class="right-menu">
      <error-log class="errLog-container right-menu-item" />

      <el-tooltip
        :content="$t('navbar.lockscreen')"
        effect="dark"
        placement="bottom"
      >
        <lock-screen class="lockscreen right-menu-item" />
      </el-tooltip>

      <el-tooltip
        :content="$t('navbar.screenfull')"
        effect="dark"
        placement="bottom"
      >
        <screenfull class="screenfull right-menu-item" />
      </el-tooltip>

      <lang-select class="international right-menu-item" />

      <el-tooltip
        :content="$t('navbar.theme')"
        effect="dark"
        placement="bottom"
      >
        <theme-picker class="theme-switch right-menu-item" />
      </el-tooltip>

      <el-dropdown
        class="avatar-container right-menu-item"
        trigger="click"
        @command="handleCommand"
      >
        <div class="avatar-wrapper">
          <img :src="avatar + '?imageView2/1/w/80/h/80'" class="user-avatar" >
          <i class="el-icon-caret-bottom" />
        </div>
        <el-dropdown-menu slot="dropdown">
          <router-link to="/">
            <el-dropdown-item>
              {{ $t("navbar.dashboard") }}
            </el-dropdown-item>
          </router-link>
          <el-dropdown-item command="changePwd">
            {{ $t("navbar.changePwd") }}
          </el-dropdown-item>
          <el-dropdown-item divided>
            <span style="display: block" @click="logout">
              {{ $t("navbar.logOut") }}
            </span>
          </el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>
    <el-dialog
      :visible.sync="changePwdVisible"
      title="修改密码"
      width="20%"
      center
    >
      <el-form ref="pwdForm" :rules="rules" :model="form" label-width="80px">
        <el-form-item label="原密码" prop="oldPwd">
          <el-input
            v-model="form.oldPwd"
            type="password"
            placeholder="请输入原始密码"
          />
        </el-form-item>
        <el-form-item label="新密码" prop="newPwd">
          <el-input
            v-model="form.newPwd"
            type="password"
            placeholder="请输入新密码"
          />
        </el-form-item>
        <el-form-item label="确认密码" prop="confirmPwd">
          <el-input
            v-model="form.confirmPwd"
            type="password"
            placeholder="请输入新密码"
          />
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="changePwdVisible = false">取 消</el-button>
        <el-button type="primary" @click="confirmChangePwd"> 确 定 </el-button>
      </span>
    </el-dialog>
  </el-menu>
</template>

<script>
import { mapGetters } from 'vuex'
import Breadcrumb from '@/components/Breadcrumb'
import Hamburger from '@/components/Hamburger'
import ErrorLog from '@/components/ErrorLog'
import LockScreen from '@/components/LockScreen'
import Screenfull from '@/components/Screenfull'
import LangSelect from '@/components/LangSelect'
import ThemePicker from '@/components/ThemePicker'
import { Message } from 'element-ui'
import { changePwd } from '@/api/auth'

export default {
  components: {
    Breadcrumb,
    Hamburger,
    ErrorLog,
    LockScreen,
    Screenfull,
    LangSelect,
    ThemePicker
  },
  data() {
    var validateConfirmPwd = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请再次输入密码'))
      } else if (value !== this.form.newPwd) {
        callback(new Error('两次输入密码不一致!'))
      } else {
        callback()
      }
    }
    return {
      changePwdVisible: false,
      rules: {
        oldPwd: [
          { required: true, message: '请输入原密码', trigger: 'blur' },
          {
            min: 6,
            max: 25,
            message: '长度在 6 到 25 个字符',
            trigger: 'blur'
          }
        ],
        newPwd: [
          { required: true, message: '请输入新密码', trigger: 'blur' },
          {
            min: 6,
            max: 25,
            message: '长度在 6 到 25 个字符',
            trigger: 'blur'
          }
        ],
        confirmPwd: [{ validator: validateConfirmPwd, trigger: 'blur' }]
      },
      form: {
        oldPwd: '',
        newPwd: '',
        confirmPwd: ''
      }
    }
  },
  computed: {
    ...mapGetters(['sidebar', 'name', 'avatar'])
  },
  methods: {
    toggleSideBar() {
      this.$store.dispatch('toggleSideBar')
    },
    handleCommand(command) {
      if (command === 'changePwd') {
        this.changePwdVisible = true
      }
    },
    confirmChangePwd() {
      this.$refs['pwdForm'].validate(valid => {
        if (valid) {
          changePwd(this.form).then(() => {
            Message({
              message: '修改成功',
              type: 'success'
            })
            this.$refs['pwdForm'].resetFields()
            this.changePwdVisible = false
          })
        } else {
          return false
        }
      })
    },
    logout() {
      this.$store.dispatch('LogOut').then(() => {
        location.reload() // In order to re-instantiate the vue-router object to avoid bugs
      })
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
.navbar {
  height: 50px;
  line-height: 50px;
  border-radius: 0px !important;
  .hamburger-container {
    line-height: 58px;
    height: 50px;
    float: left;
    padding: 0 10px;
  }
  .breadcrumb-container {
    float: left;
  }
  .errLog-container {
    display: inline-block;
    vertical-align: top;
  }
  .right-menu {
    float: right;
    height: 100%;
    &:focus {
      outline: none;
    }
    .right-menu-item {
      display: inline-block;
      margin: 0 8px;
    }
    .screenfull {
      height: 20px;
    }
    .international {
      vertical-align: top;
    }
    .theme-switch {
      vertical-align: 15px;
    }
    .avatar-container {
      height: 50px;
      margin-right: 30px;
      .avatar-wrapper {
        cursor: pointer;
        margin-top: 5px;
        position: relative;
        .user-avatar {
          width: 40px;
          height: 40px;
          border-radius: 10px;
        }
        .el-icon-caret-bottom {
          position: absolute;
          right: -20px;
          top: 25px;
          font-size: 12px;
        }
      }
    }
  }
}
</style>
