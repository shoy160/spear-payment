-- ----------------------------
-- Table structure for t_account
-- ----------------------------
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS `t_account`;
CREATE TABLE `t_account` (
  `id` varchar(36) NOT NULL,
  `account` varchar(64) NOT NULL COMMENT '帐号',
  `password` varchar(128) NOT NULL COMMENT '密码',
  `password_salt` varchar(16) NOT NULL COMMENT '密码盐',
  `role` tinyint(4) NOT NULL COMMENT '角色：0.项目组,64.管理员',
  `create_time` datetime NOT NULL COMMENT '创建时间',
  `last_login_time` datetime COMMENT '最后登录时间',
  `project_id` varchar(36) COMMENT '项目ID',
  `avatar` varchar(255) COMMENT '头像',
  `status` tinyint(4) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '帐号表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_channel
-- ----------------------------
DROP TABLE IF EXISTS `t_channel`;
CREATE TABLE `t_channel` (
  `id` varchar(36) NOT NULL,
  `name` varchar(64) NOT NULL COMMENT '渠道名称',
  `remark` varchar(512) COMMENT '渠道备注',
  `mode` tinyint(4) NOT NULL COMMENT '支付方式：0,支付宝;1,微信...',
  `app_id` varchar(128) NOT NULL COMMENT '合作商ID',
  `config` json NOT NULL COMMENT '支付配置',
  `create_time` datetime NOT NULL COMMENT '创建时间',
  `is_default` tinyint(1) NOT NULL DEFAULT 0 COMMENT '是否默认', 
  `status` tinyint(4) NOT NULL DEFAULT 0 COMMENT '渠道状态',
  `types` json COMMENT '支持的支付类型',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '支付渠道表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_platform
-- ----------------------------
DROP TABLE IF EXISTS `t_platform`;
CREATE TABLE `t_platform` (
  `id` varchar(36) NOT NULL,
  `type` tinyint(4) NOT NULL COMMENT '类型：0,支付宝;1,微信',
  `open_id` varchar(64) NOT NULL,
  `union_id` varchar(128) COMMENT '平台唯一ID',
  `access_token` varchar(1024) NOT NULL,
  `refresh_token` varchar(512) NOT NULL,
  `expire_in` int(11) NOT NULL,
  `create_time` time(0) NOT NULL,
  `channel_id` varchar(36) NOT NULL COMMENT '渠道ID',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '第三方平台表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_project
-- ----------------------------
DROP TABLE IF EXISTS `t_project`;
CREATE TABLE `t_project` (
  `id` varchar(36) NOT NULL,
  `secret` varchar(128) NOT NULL COMMENT '支付密钥',
  `name` varchar(64) NOT NULL COMMENT '项目名称',
  `notify_url` varchar(512) NOT NULL COMMENT '通知URL',
  `redirect_url` varchar(512) COMMENT '重定向地址',
  `queue_name` varchar(128) COMMENT '队列名称(订阅发布模式)',
  `status` tinyint(4) COMMENT '状态',
  `create_time` datetime NOT NULL,
  `code` varchar(16) NOT NULL COMMENT '项目编号',
  `channels` json COMMENT '渠道列表',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '项目表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_trade
-- ----------------------------
DROP TABLE IF EXISTS `t_trade`;
CREATE TABLE `t_trade` (
  `id` varchar(36) NOT NULL,
  `mode` tinyint(4) COMMENT '支付方式：支付宝，微信...',
  `type` varchar(16) COMMENT '支付类型：APP,Web,H5...',
  `project_id` varchar(36) NOT NULL COMMENT '项目ID',
  `order_no` varchar(64) NOT NULL COMMENT '商户订单号',
  `amount` bigint NOT NULL COMMENT '金额(分)',
  `title` varchar(128) NOT NULL COMMENT '支付标题',
  `body` varchar(512) COMMENT '支付描述',
  `status` tinyint(4) NOT NULL COMMENT '支付状态',
  `out_trade_no` varchar(64) COMMENT '支付订单号',
  `raw_params` varchar(1024) NOT NULL COMMENT '支付原始参数',
  `create_time` datetime NOT NULL,
  `paid_user` varchar(32) COMMENT '支付用户',
  `paid_account` varchar(128) COMMENT '支付帐号',
  `paid_time` datetime COMMENT '支付时间',
  `extend` varchar(128) COMMENT '扩展字段',
  `channel_id` varchar(36) COMMENT '渠道ID',
  `redirect_url` varchar(1024) COMMENT '跳转地址',
  `trade_no` varchar(128) NOT NULL COMMENT '交易号',
  `platform_id` varchar(36) COMMENT '平台ID',
  `scan_url` varchar(512) COMMENT '扫码支付链接',
  `refund_no` varchar(64) COMMENT '退款订单号',
  `out_refund_no` varchar(64) COMMENT '支付平台退款单号',
  `refund_amount` bigint COMMENT '退款金额',
  `refund_reason` varchar(255) COMMENT '退款原因',
  `refund_time` timestamp(6) COMMENT '退款时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '交易表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_trade_notify
-- ----------------------------
DROP TABLE IF EXISTS `t_trade_notify`;
CREATE TABLE `t_trade_notify` (
  `id` varchar(36) NOT NULL,
  `trade_id` varchar(36) COMMENT '交易ID',
  `create_time` datetime NOT NULL COMMENT '通知时间',
  `type` tinyint(4) NOT NULL COMMENT '通知类型：0.接收,1.发送',
  `content` text NOT NULL COMMENT '通知参数',
  `status` tinyint(4) NOT NULL COMMENT '通知状态',
  `result` text COMMENT '通知结果',
  `url` varchar(2048) NOT NULL COMMENT '通知URL',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '交易通知表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_trade_payment
-- ----------------------------
DROP TABLE IF EXISTS `t_trade_payment`;
CREATE TABLE `t_trade_payment` (
  `id` varchar(36) NOT NULL,
  `trade_id` varchar(32) NOT NULL COMMENT '交易ID',
  `mode` tinyint(4) NOT NULL COMMENT '支付方式',
  `type` varchar(64) NOT NULL COMMENT '支付类型',
  `value` varchar(2048) NOT NULL COMMENT 'url或者参数',
  `create_time` datetime NOT NULL COMMENT '创建时间',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '交易支付表' ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;

INSERT INTO `t_account` VALUES ('ddff9497-c692-caf7-a0f4-08d6459b53bb', 'admin', 'D781D657059BC06A5AD5ABEBAC8EC995', 'e5f647603c4adb00', 64, now(), NULL, NULL, 'http://ztj.test.apis.yunzhicloud.com/file/image/0bc045c7/a4ea664ea9f44a7faeaa0893cde5bf31.png', 0);