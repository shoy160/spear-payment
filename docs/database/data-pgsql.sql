-- ----------------------------
-- Table structure for t_account
-- ----------------------------
DROP TABLE IF EXISTS "public"."t_account";
CREATE TABLE "public"."t_account" (
  "id" varchar(36) NOT NULL,
  "account" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "password" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "password_salt" varchar(16) COLLATE "pg_catalog"."default" NOT NULL,
  "role" int2 NOT NULL,
  "create_time" timestamp(0) NOT NULL,
  "last_login_time" timestamp(0),
  "project_id" varchar(36),
  "avatar" varchar(255) COLLATE "pg_catalog"."default",
  "status" int2 NOT NULL
)
;
COMMENT ON COLUMN "public"."t_account"."account" IS '帐号';
COMMENT ON COLUMN "public"."t_account"."password" IS '密码';
COMMENT ON COLUMN "public"."t_account"."password_salt" IS '密码盐';
COMMENT ON COLUMN "public"."t_account"."role" IS '角色：0.项目组,64.管理员';
COMMENT ON COLUMN "public"."t_account"."create_time" IS '创建时间';
COMMENT ON COLUMN "public"."t_account"."last_login_time" IS '最后登录时间';
COMMENT ON COLUMN "public"."t_account"."project_id" IS '项目ID';
COMMENT ON COLUMN "public"."t_account"."avatar" IS '头像';
COMMENT ON TABLE "public"."t_account" IS '帐号表';

-- ----------------------------
-- Table structure for t_channel
-- ----------------------------
DROP TABLE IF EXISTS "public"."t_channel";
CREATE TABLE "public"."t_channel" (
  "id" varchar(36) NOT NULL,
  "name" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "remark" varchar(512) COLLATE "pg_catalog"."default",
  "mode" int2 NOT NULL,
  "app_id" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "config" json NOT NULL,
  "create_time" timestamp(0) NOT NULL,
  "is_default" bool NOT NULL DEFAULT false,
  "status" int2 NOT NULL DEFAULT 0,
  "types" json
)
;
COMMENT ON COLUMN "public"."t_channel"."name" IS '渠道名称';
COMMENT ON COLUMN "public"."t_channel"."remark" IS '渠道备注';
COMMENT ON COLUMN "public"."t_channel"."mode" IS '支付方式：0,支付宝;1,微信...';
COMMENT ON COLUMN "public"."t_channel"."app_id" IS '合作商ID';
COMMENT ON COLUMN "public"."t_channel"."config" IS '支付配置';
COMMENT ON COLUMN "public"."t_channel"."create_time" IS '创建时间';
COMMENT ON COLUMN "public"."t_channel"."is_default" IS '是否默认';
COMMENT ON COLUMN "public"."t_channel"."status" IS '渠道状态';
COMMENT ON COLUMN "public"."t_channel"."types" IS '支持的支付类型';
COMMENT ON TABLE "public"."t_channel" IS '支付渠道表';

-- ----------------------------
-- Table structure for t_platform
-- ----------------------------
DROP TABLE IF EXISTS "public"."t_platform";
CREATE TABLE "public"."t_platform" (
  "id" varchar(36) NOT NULL,
  "type" int2 NOT NULL,
  "open_id" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "union_id" varchar(128) COLLATE "pg_catalog"."default",
  "access_token" varchar(1024) COLLATE "pg_catalog"."default" NOT NULL,
  "refresh_token" varchar(512) COLLATE "pg_catalog"."default" NOT NULL,
  "expire_in" int4 NOT NULL,
  "create_time" time(0) NOT NULL,
  "channel_id" varchar(36) NOT NULL
)
;
COMMENT ON COLUMN "public"."t_platform"."type" IS '类型：0,支付宝;1,微信';
COMMENT ON COLUMN "public"."t_platform"."union_id" IS '平台唯一ID';
COMMENT ON COLUMN "public"."t_platform"."channel_id" IS '渠道ID';
COMMENT ON TABLE "public"."t_platform" IS '第三方平台表';

-- ----------------------------
-- Table structure for t_project
-- ----------------------------
DROP TABLE IF EXISTS "public"."t_project";
CREATE TABLE "public"."t_project" (
  "id" varchar(36) NOT NULL,
  "secret" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "name" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "notify_url" varchar(512) COLLATE "pg_catalog"."default" NOT NULL,
  "redirect_url" varchar(512) COLLATE "pg_catalog"."default",
  "queue_name" varchar(128) COLLATE "pg_catalog"."default",
  "status" int2,
  "create_time" timestamp(0) NOT NULL,
  "code" varchar(16) COLLATE "pg_catalog"."default" NOT NULL,
  "channels" json
)
;
COMMENT ON COLUMN "public"."t_project"."secret" IS '支付密钥';
COMMENT ON COLUMN "public"."t_project"."name" IS '项目名称';
COMMENT ON COLUMN "public"."t_project"."notify_url" IS '通知URL';
COMMENT ON COLUMN "public"."t_project"."redirect_url" IS '重定向地址';
COMMENT ON COLUMN "public"."t_project"."queue_name" IS '队列名称(订阅发布模式)';
COMMENT ON COLUMN "public"."t_project"."status" IS '状态';
COMMENT ON COLUMN "public"."t_project"."code" IS '项目编号';
COMMENT ON COLUMN "public"."t_project"."channels" IS '渠道列表';
COMMENT ON TABLE "public"."t_project" IS '项目表';

-- ----------------------------
-- Table structure for t_trade
-- ----------------------------
DROP TABLE IF EXISTS "public"."t_trade";
CREATE TABLE "public"."t_trade" (
  "id" varchar(36) NOT NULL,
  "mode" int2,
  "type" varchar(16) COLLATE "pg_catalog"."default",
  "project_id" varchar(36) NOT NULL,
  "order_no" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "amount" int8 NOT NULL,
  "title" varchar(128) COLLATE "pg_catalog"."default" NOT NULL,
  "body" varchar(512) COLLATE "pg_catalog"."default",
  "status" int2 NOT NULL,
  "out_trade_no" varchar(64) COLLATE "pg_catalog"."default",
  "raw_params" varchar(1024) COLLATE "pg_catalog"."default" NOT NULL,
  "create_time" timestamp(0) NOT NULL,
  "paid_user" varchar(32) COLLATE "pg_catalog"."default",
  "paid_account" varchar(128) COLLATE "pg_catalog"."default",
  "paid_time" timestamp(0),
  "extend" varchar(128) COLLATE "pg_catalog"."default",
  "channel_id" varchar(36),
  "redirect_url" varchar(1024) COLLATE "pg_catalog"."default",
  "trade_no" varchar(128) COLLATE "pg_catalog"."default" NOT NULL DEFAULT ''::character varying,
  "platform_id" varchar(36),
  "scan_url" varchar(512) COLLATE "pg_catalog"."default",
  "refund_no" varchar(64) COLLATE "pg_catalog"."default",
  "out_refund_no" varchar(64) COLLATE "pg_catalog"."default",
  "refund_amount" int8,
  "refund_reason" varchar(255) COLLATE "pg_catalog"."default",
  "refund_time" timestamp(6)
)
;
COMMENT ON COLUMN "public"."t_trade"."mode" IS '支付方式：支付宝，微信...';
COMMENT ON COLUMN "public"."t_trade"."type" IS '支付类型：APP,Web,H5...';
COMMENT ON COLUMN "public"."t_trade"."project_id" IS '项目ID';
COMMENT ON COLUMN "public"."t_trade"."order_no" IS '商户订单号';
COMMENT ON COLUMN "public"."t_trade"."amount" IS '金额(分)';
COMMENT ON COLUMN "public"."t_trade"."title" IS '支付标题';
COMMENT ON COLUMN "public"."t_trade"."body" IS '支付描述';
COMMENT ON COLUMN "public"."t_trade"."status" IS '支付状态';
COMMENT ON COLUMN "public"."t_trade"."out_trade_no" IS '支付订单号';
COMMENT ON COLUMN "public"."t_trade"."raw_params" IS '支付原始参数';
COMMENT ON COLUMN "public"."t_trade"."paid_user" IS '支付用户';
COMMENT ON COLUMN "public"."t_trade"."paid_account" IS '支付帐号';
COMMENT ON COLUMN "public"."t_trade"."paid_time" IS '支付时间';
COMMENT ON COLUMN "public"."t_trade"."extend" IS '扩展字段';
COMMENT ON COLUMN "public"."t_trade"."channel_id" IS '渠道ID';
COMMENT ON COLUMN "public"."t_trade"."redirect_url" IS '跳转地址';
COMMENT ON COLUMN "public"."t_trade"."trade_no" IS '交易号';
COMMENT ON COLUMN "public"."t_trade"."platform_id" IS '平台ID';
COMMENT ON COLUMN "public"."t_trade"."scan_url" IS '扫码支付链接';
COMMENT ON COLUMN "public"."t_trade"."refund_no" IS '退款订单号';
COMMENT ON COLUMN "public"."t_trade"."out_refund_no" IS '支付平台退款单号';
COMMENT ON COLUMN "public"."t_trade"."refund_amount" IS '退款金额';
COMMENT ON COLUMN "public"."t_trade"."refund_reason" IS '退款原因';
COMMENT ON COLUMN "public"."t_trade"."refund_time" IS '退款时间';
COMMENT ON TABLE "public"."t_trade" IS '交易表';

-- ----------------------------
-- Table structure for t_trade_notify
-- ----------------------------
DROP TABLE IF EXISTS "public"."t_trade_notify";
CREATE TABLE "public"."t_trade_notify" (
  "id" varchar(36) NOT NULL,
  "trade_id" varchar(36),
  "create_time" timestamp(0) NOT NULL,
  "type" int2 NOT NULL,
  "content" text COLLATE "pg_catalog"."default" NOT NULL,
  "status" int2 NOT NULL,
  "result" text COLLATE "pg_catalog"."default",
  "url" varchar(2048) COLLATE "pg_catalog"."default" NOT NULL
)
;
COMMENT ON COLUMN "public"."t_trade_notify"."trade_id" IS '交易ID';
COMMENT ON COLUMN "public"."t_trade_notify"."create_time" IS '通知时间';
COMMENT ON COLUMN "public"."t_trade_notify"."type" IS '通知类型：0.接收,1.发送';
COMMENT ON COLUMN "public"."t_trade_notify"."content" IS '通知参数';
COMMENT ON COLUMN "public"."t_trade_notify"."status" IS '通知状态';
COMMENT ON COLUMN "public"."t_trade_notify"."result" IS '通知结果';
COMMENT ON TABLE "public"."t_trade_notify" IS '交易通知表';

-- ----------------------------
-- Table structure for t_trade_payment
-- ----------------------------
DROP TABLE IF EXISTS "public"."t_trade_payment";
CREATE TABLE "public"."t_trade_payment" (
  "id" varchar(36) NOT NULL,
  "trade_id" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "mode" int2 NOT NULL,
  "type" varchar(64) COLLATE "pg_catalog"."default" NOT NULL,
  "value" varchar(2048) COLLATE "pg_catalog"."default" NOT NULL,
  "create_time" timestamp(0) NOT NULL
)
;
COMMENT ON COLUMN "public"."t_trade_payment"."trade_id" IS '交易ID';
COMMENT ON COLUMN "public"."t_trade_payment"."mode" IS '支付方式';
COMMENT ON COLUMN "public"."t_trade_payment"."type" IS '支付类型';
COMMENT ON COLUMN "public"."t_trade_payment"."value" IS 'url或者参数';
COMMENT ON COLUMN "public"."t_trade_payment"."create_time" IS '创建时间';
COMMENT ON TABLE "public"."t_trade_payment" IS '交易支付表';

-- ----------------------------
-- Primary Key structure for table t_account
-- ----------------------------
ALTER TABLE "public"."t_account" ADD CONSTRAINT "t_account_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table t_channel
-- ----------------------------
ALTER TABLE "public"."t_channel" ADD CONSTRAINT "t_channel_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table t_platform
-- ----------------------------
ALTER TABLE "public"."t_platform" ADD CONSTRAINT "t_platform_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table t_project
-- ----------------------------
ALTER TABLE "public"."t_project" ADD CONSTRAINT "t_project_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table t_trade
-- ----------------------------
ALTER TABLE "public"."t_trade" ADD CONSTRAINT "t_trade_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table t_trade_notify
-- ----------------------------
ALTER TABLE "public"."t_trade_notify" ADD CONSTRAINT "t_trade_notify_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table t_trade_payment
-- ----------------------------
ALTER TABLE "public"."t_trade_payment" ADD CONSTRAINT "t_trade_payment_pkey" PRIMARY KEY ("id");


INSERT INTO "public"."t_account" VALUES ('ddff9497-c692-caf7-a0f4-08d6459b53bb', 'admin', '686ED3BC42AD515CB15E3A4DC806D100', '77ce31dae5a007ef', 64, now(), NULL, NULL, NULL, 0);