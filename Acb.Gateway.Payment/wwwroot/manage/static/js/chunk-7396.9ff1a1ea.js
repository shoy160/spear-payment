(window.webpackJsonp=window.webpackJsonp||[]).push([["chunk-7396"],{"+GAq":function(t,a,n){"use strict";var e=function(){var t=this,a=t.$createElement,n=t._self._c||a;return n("el-row",{staticClass:"panel-group",attrs:{gutter:40}},[n("el-col",{staticClass:"card-panel-col",attrs:{xs:12,sm:12,lg:6}},[n("div",{staticClass:"card-panel"},[n("div",{staticClass:"card-panel-icon-wrapper icon-people"},[n("svg-icon",{attrs:{"icon-class":"people","class-name":"card-panel-icon"}})],1),t._v(" "),n("div",{staticClass:"card-panel-description"},[n("div",{staticClass:"card-panel-text"},[t._v("今日交易数")]),t._v(" "),n("count-to",{staticClass:"card-panel-num",attrs:{"start-val":0,"end-val":t.panelData.todayCount,duration:1500}})],1)])]),t._v(" "),n("el-col",{staticClass:"card-panel-col",attrs:{xs:12,sm:12,lg:6}},[n("div",{staticClass:"card-panel"},[n("div",{staticClass:"card-panel-icon-wrapper icon-money"},[n("svg-icon",{attrs:{"icon-class":"money","class-name":"card-panel-icon"}})],1),t._v(" "),n("div",{staticClass:"card-panel-description"},[n("div",{staticClass:"card-panel-text"},[t._v("今日交易额")]),t._v(" "),n("count-to",{staticClass:"card-panel-num",attrs:{"start-val":0,"end-val":t.panelData.todayAmount/100,duration:2200,decimals:2}})],1)])]),t._v(" "),n("el-col",{staticClass:"card-panel-col",attrs:{xs:12,sm:12,lg:6}},[n("div",{staticClass:"card-panel"},[n("div",{staticClass:"card-panel-icon-wrapper icon-people"},[n("svg-icon",{attrs:{"icon-class":"peoples","class-name":"card-panel-icon"}})],1),t._v(" "),n("div",{staticClass:"card-panel-description"},[n("div",{staticClass:"card-panel-text"},[t._v("总交易数")]),t._v(" "),n("count-to",{staticClass:"card-panel-num",attrs:{"start-val":0,"end-val":t.panelData.count,duration:1800}})],1)])]),t._v(" "),n("el-col",{staticClass:"card-panel-col",attrs:{xs:12,sm:12,lg:6}},[n("div",{staticClass:"card-panel"},[n("div",{staticClass:"card-panel-icon-wrapper icon-money"},[n("svg-icon",{attrs:{"icon-class":"money","class-name":"card-panel-icon"}})],1),t._v(" "),n("div",{staticClass:"card-panel-description"},[n("div",{staticClass:"card-panel-text"},[t._v("总交易额")]),t._v(" "),n("count-to",{staticClass:"card-panel-num",attrs:{"start-val":0,"end-val":t.panelData.amount/100,duration:2500,decimals:2}})],1)])])],1)},i=[];n.d(a,"a",function(){return e}),n.d(a,"b",function(){return i})},"/dfR":function(t,a,n){"use strict";var e=n("Cn/V");n.n(e).a},"1H6p":function(t,a,n){"use strict";n.r(a);var e=n("59ge"),i=n.n(e);for(var s in e)"default"!==s&&function(t){n.d(a,t,function(){return e[t]})}(s);a.default=i.a},"59ge":function(t,a,n){"use strict";Object.defineProperty(a,"__esModule",{value:!0});var e=r(n("gW20")),i=r(n("HzsB")),s=n("MZEg");function r(t){return t&&t.__esModule?t:{default:t}}a.default={name:"Home",components:{PanelGroup:e.default,LineChart:i.default},data:function(){return{countChartData:{xAxis:[],yAxis:[]},amountChartData:{xAxis:[],yAxis:[]},statistics:{}}},mounted:function(){var t=this;(0,s.homedata)().then(function(a){console.log(a),t.statistics=a.statistic;var n=[{color:"#FF005A",easing:"cubicInOut",duration:2800},{color:"#3888fa",easing:"quadraticOut",duration:2800}],e=[],i=[],s=[],r=[],o=[];a.platforms.forEach(function(t,a){e.push(t.date),i.push(t.alipayCount),r.push(t.alipayAmount/100),s.push(t.wechatCount),o.push(t.wechatAmount/100)}),t.countChartData.xAxis=t.amountChartData.xAxis=e,t.countChartData.yAxis.push({title:"支付宝交易数",option:n[0],data:i}),t.countChartData.yAxis.push({title:"微信交易数",option:n[1],data:s}),t.amountChartData.yAxis.push({title:"支付宝交易额(元)",option:n[0],data:r}),t.amountChartData.yAxis.push({title:"微信交易额(元)",option:n[1],data:o})})}}},ACTI:function(t,a,n){"use strict";n.r(a);var e=n("KWpY"),i=n.n(e);for(var s in e)"default"!==s&&function(t){n.d(a,t,function(){return e[t]})}(s);a.default=i.a},"Cn/V":function(t,a,n){},Cs6j:function(t,a,n){"use strict";var e=function(){var t=this.$createElement;return(this._self._c||t)("div",{class:this.className,style:{height:this.height,width:this.width}})},i=[];n.d(a,"a",function(){return e}),n.d(a,"b",function(){return i})},HzsB:function(t,a,n){"use strict";n.r(a);var e=n("Cs6j"),i=n("ACTI");for(var s in i)"default"!==s&&function(t){n.d(a,t,function(){return i[t]})}(s);var r=n("KHd+"),o=Object(r.a)(i.default,e.a,e.b,!1,null,null,null);o.options.__file="LineChart.vue",a.default=o.exports},"Jn/t":function(t,a,n){"use strict";n.r(a);var e=n("PK59"),i=n("1H6p");for(var s in i)"default"!==s&&function(t){n.d(a,t,function(){return i[t]})}(s);var r=n("KHd+"),o=Object(r.a)(i.default,e.a,e.b,!1,null,null,null);o.options.__file="index.vue",a.default=o.exports},Jr4G:function(t,a,n){"use strict";n.r(a);var e=n("L6Ha"),i=n.n(e);for(var s in e)"default"!==s&&function(t){n.d(a,t,function(){return e[t]})}(s);a.default=i.a},KWpY:function(t,a,n){"use strict";Object.defineProperty(a,"__esModule",{value:!0});var e=function(t){return t&&t.__esModule?t:{default:t}}(n("MT78")),i=n("7Qib");n("gX0l"),a.default={props:{className:{type:String,default:"chart"},width:{type:String,default:"100%"},height:{type:String,default:"350px"},autoResize:{type:Boolean,default:!0},chartData:{type:Object,required:!0}},data:function(){return{chart:null}},watch:{chartData:{deep:!0,handler:function(t){this.setOptions(t)}}},mounted:function(){var t=this;this.initChart(),this.autoResize&&(this.__resizeHanlder=(0,i.debounce)(function(){t.chart&&t.chart.resize()},100),window.addEventListener("resize",this.__resizeHanlder)),document.getElementsByClassName("sidebar-container")[0].addEventListener("transitionend",this.__resizeHanlder)},beforeDestroy:function(){if(this.chart){this.autoResize&&window.removeEventListener("resize",this.__resizeHanlder);var t=document.getElementsByClassName("sidebar-container");t.length>0&&t[0].removeEventListener("transitionend",this.__resizeHanlder),this.chart.dispose(),this.chart=null}},methods:{setOptions:function(){var t=arguments.length>0&&void 0!==arguments[0]?arguments[0]:{},a={xAxis:{data:t.xAxis,boundaryGap:!1,axisTick:{show:!1}},grid:{left:10,right:50,bottom:20,top:30,containLabel:!0},tooltip:{trigger:"axis",axisPointer:{type:"cross"},padding:[5,10]},yAxis:{axisTick:{show:!1}},legend:{data:[]},series:[]};t.yAxis.forEach(function(t){a.legend.data.push(t.title);var n=t.option;a.series.push({name:t.title,itemStyle:{normal:{color:n.color,lineStyle:{color:n.color,width:2}}},smooth:!0,type:"line",data:t.data,animationDuration:n.duration,animationEasing:n.easing})}),this.chart.setOption(a)},initChart:function(){this.chart=e.default.init(this.$el,"macarons"),this.setOptions(this.chartData)}}}},L6Ha:function(t,a,n){"use strict";Object.defineProperty(a,"__esModule",{value:!0});var e=function(t){return t&&t.__esModule?t:{default:t}}(n("7BsA"));a.default={components:{CountTo:e.default},props:{panelData:{type:Object,default:function(){return{todayCount:0,todayAmount:0,count:0,amount:0}}}}}},MZEg:function(t,a,n){"use strict";Object.defineProperty(a,"__esModule",{value:!0}),a.homedata=void 0;var e=function(t){return t&&t.__esModule?t:{default:t}}(n("t3Un"));a.homedata=function(){return e.default.get("/manage/home/statistic")}},PK59:function(t,a,n){"use strict";var e=function(){var t=this.$createElement,a=this._self._c||t;return a("div",{staticClass:"app-container"},[a("panel-group",{attrs:{"panel-data":this.statistics}}),this._v(" "),a("el-row",{staticStyle:{background:"#fff",padding:"16px 16px 0","margin-bottom":"32px"}},[a("line-chart",{attrs:{"chart-data":this.countChartData}})],1),this._v(" "),a("el-row",{staticStyle:{background:"#fff",padding:"16px 16px 0","margin-bottom":"32px"}},[a("line-chart",{attrs:{"chart-data":this.amountChartData}})],1)],1)},i=[];n.d(a,"a",function(){return e}),n.d(a,"b",function(){return i})},gW20:function(t,a,n){"use strict";n.r(a);var e=n("+GAq"),i=n("Jr4G");for(var s in i)"default"!==s&&function(t){n.d(a,t,function(){return i[t]})}(s);n("/dfR");var r=n("KHd+"),o=Object(r.a)(i.default,e.a,e.b,!1,null,"3f256189",null);o.options.__file="PanelGroup.vue",a.default=o.exports}}]);