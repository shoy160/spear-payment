<template>
  <div class="backstretch">
    <transition v-for="(img,index) in images" :key="index" name="fade">
      <img v-show="current === index" :key="index" :src="img" alt="">
    </transition>
  </div>
</template>
<script>
export default {
  name: 'DynamicBackground',
  props: {
    images: {
      type: Array,
      required: true,
      default: function() {
        return []
      }
    },
    interval: {
      type: Number,
      default: 8000
    }
  },
  data() {
    return {
      timer: null,
      show: true,
      current: 0
    }
  },
  mounted() {
    clearInterval(this.timer)
    if (!this.images || this.images.length <= 1) {
      return
    }
    this.timer = setInterval(() => {
      this.changeImage()
    }, this.interval)
  },
  destroyed() {
    clearInterval(this.timer)
  },
  methods: {
    changeImage() {
      this.current++
      if (this.current >= this.images.length) {
        this.current = 0
      }
    }
  }
}
</script>
<style lang="scss" scoped>
.backstretch {
  position: absolute;
  width: 100%;
  height: 100%;
  z-index: -1;
  overflow: hidden;
  img {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    filter: blur(0.2rem) brightness(.5);
  }
}
.fade-enter-active,
.fade-enter {
  animation: bounce-in 1.2s;
}

.fade-leave-active,
.fade-leave-to {
  animation: bounce-in 1.2s reverse;
}
@keyframes bounce-in {
  0% {
    opacity: 0;
  }
  50% {
    opacity: 0.5;
  }
  100% {
    opacity: 1;
  }
}
</style>

