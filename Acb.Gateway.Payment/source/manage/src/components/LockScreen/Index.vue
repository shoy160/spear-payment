<template>
  <div class="lock-screen" @click="lockScreen">
    <i class="fa fa-lock lock-svg" />
  </div>
</template>

<script>
import Cookies from 'js-cookie'
export default {
  name: 'LockScreen',
  props: {
    value: {
      type: Boolean,
      default: false
    }
  },
  mounted() {
    if (!document.getElementById('lock_screen_back')) {
      const lockdiv = document.createElement('div')
      lockdiv.setAttribute('id', 'lock_screen_back')
      lockdiv.setAttribute('class', 'lock-screen-back')
      document.body.appendChild(lockdiv)
      const lockScreenBack = document.getElementById('lock_screen_back')
      const x = document.body.clientWidth
      const y = document.body.clientHeight
      const r = Math.sqrt(x * x + y * y)
      const size = parseInt(r)
      this.lockScreenSize = size
      window.addEventListener('resize', () => {
        const x = document.body.clientWidth
        const y = document.body.clientHeight
        const r = Math.sqrt(x * x + y * y)
        const size = parseInt(r)
        this.lockScreenSize = size
        lockScreenBack.style.transition = 'all 0s'
        lockScreenBack.style.width = lockScreenBack.style.height = size + 'px'
      })
      lockScreenBack.style.width = lockScreenBack.style.height = size + 'px'
    }
  },
  methods: {
    lockScreen() {
      const lockScreenBack = document.getElementById('lock_screen_back')
      lockScreenBack.style.transition = 'all 3s'
      lockScreenBack.style.zIndex = 10000
      lockScreenBack.style.boxShadow =
        '0 0 0 ' + this.lockScreenSize + 'px #667aa6 inset'
      this.showUnlock = true
      Cookies.set('last_page_name', this.$route.name) // 本地存储锁屏之前打开的页面以便解锁后打开
      setTimeout(() => {
        lockScreenBack.style.transition = 'all 0s'
        this.$router.push({
          name: 'locking'
        })
      }, 800)
      Cookies.set('locking', '1')
    }
  }
}
</script>
<style scoped>
.lock-screen {
  height: 20px;
}
.lock-screen .lock-svg {
  display: inline-block;
  cursor: pointer;
  color: #5a5e66;
  font-size: 26px;
  vertical-align: 10px;
}
</style>

