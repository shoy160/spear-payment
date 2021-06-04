<style lang="scss">
@import '../styles/unlock.scss';
</style>

<template>
  <transition name="show-unlock">
    <div v-if="showUnlock" class="unlock-body-con" @keydown.enter="handleUnlock">
      <div :style="{marginLeft: avatorLeft}" class="unlock-avator-con" @click="handleClickAvator">
        <div class="unlock-avator-img">
          <img :src="avatar+'?imageView2/1/w/80/h/80'" class="user-avatar" width="100">
        </div>
        <div class="unlock-avator-cover">
          <span>
            <i class="fa fa-unlock" />
          </span>
          <p>解锁</p>
        </div>
      </div>
      <div :style="{marginLeft: avatorLeft}" class="unlock-avator-under-back" />
      <div class="unlock-input-con">
        <div class="unlock-input-overflow-con">
          <div :style="{right: inputLeft}" class="unlock-overflow-body">
            <input ref="inputEle" v-model="password" class="unlock-input" type="password" placeholder="密码同登录密码">
            <button ref="unlockBtn" class="unlock-btn" @mousedown="unlockMousedown" @mouseup="unlockMouseup" @click="handleUnlock">
              <i class="fa fa-unlock" />
            </button>
          </div>
        </div>
      </div>
      <div class="unlock-locking-tip-con">已锁定</div>
    </div>
  </transition>
</template>

<script>
import { mapGetters } from 'vuex'
import Cookies from 'js-cookie'
import { checkPwd } from '@/api/auth'
export default {
  name: 'Unlock',
  props: {
    showUnlock: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      avatorLeft: '0px',
      inputLeft: '400px',
      password: '',
      check: null
    }
  },
  computed: {
    ...mapGetters(['name', 'avatar'])
  },
  methods: {
    handleClickAvator() {
      this.avatorLeft = '-180px'
      this.inputLeft = '0px'
      this.$refs.inputEle.focus()
    },
    handleUnlock() {
      checkPwd(this.password)
        .then(() => {
          this.avatorLeft = '0px'
          this.inputLeft = '400px'
          this.password = ''
          Cookies.set('locking', '0')
          this.$emit('on-unlock')
        })
        .catch(e => {
          this.password = ''
        })
    },
    unlockMousedown() {
      this.$refs.unlockBtn.className = 'unlock-btn click-unlock-btn'
    },
    unlockMouseup() {
      this.$refs.unlockBtn.className = 'unlock-btn'
    }
  }
}
</script>
