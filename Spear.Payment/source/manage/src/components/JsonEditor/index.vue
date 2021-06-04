<template>
  <div class="json-editor">
    <textarea ref="textarea" />
  </div>
</template>

<script>
import CodeMirror from 'codemirror'
import 'codemirror/addon/lint/lint.css'
import 'codemirror/lib/codemirror.css'
import 'codemirror/theme/idea.css'
require('script-loader!jsonlint')
import 'codemirror/mode/javascript/javascript'
import 'codemirror/addon/lint/lint'
import 'codemirror/addon/lint/json-lint'

export default {
  name: 'JsonEditor',
  /* eslint-disable vue/require-prop-types */
  props: {
    value: {
      require: true,
      default: function() {
        return {}
      }
    },
    lineNumbers: {
      type: Boolean,
      require: false,
      default: true
    },
    type: {
      type: String,
      require: false,
      default: 'application/json'
    },
    theme: {
      type: String,
      require: false,
      default: 'idea'
    }
  },
  data() {
    return {
      jsonEditor: false
    }
  },
  watch: {
    value(value) {
      const editor_value = this.jsonEditor.getValue()
      if (value !== editor_value) {
        this.setValue()
      }
    }
  },
  mounted() {
    this.jsonEditor = CodeMirror.fromTextArea(this.$refs.textarea, {
      lineNumbers: this.lineNumbers,
      mode: this.type,
      gutters: ['CodeMirror-lint-markers'],
      theme: this.theme,
      lineWrapping: true,
      lint: true
    })
    this.setValue()
    this.jsonEditor.on('change', cm => {
      this.$emit('changed', cm.getValue())
      this.$emit('input', cm.getValue())
    })
  },
  methods: {
    setValue() {
      var json = typeof this.value === 'string' ? JSON.parse(this.value) : this.value
      this.jsonEditor.setValue(JSON.stringify(json, null, 2))
    },
    getValue() {
      return this.jsonEditor.getValue()
    }
  }
}
</script>

<style scoped>
.json-editor {
  height: 100%;
  position: relative;
  line-height: 1.5;
}
.json-editor >>> .CodeMirror {
  height: auto;
  min-height: 300px;
}
.json-editor >>> .CodeMirror-scroll {
  min-height: 300px;
}
.json-editor >>> .cm-s-rubyblue span.cm-string {
  color: #f08047;
}
</style>
