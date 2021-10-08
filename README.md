```
# 快速开始 参考：https://hub.docker.com/repository/docker/shoy160/spear-payment
> docker run -v @PWD/appsettings.json:/publish/appsettings.json -e SPEAR_MODE:Prod -p 5000:5000 -d shoy160/spear-payment

> cat appsettings.json
{
  "site": "payment",
  "mode": "Test",
  "dapper": {
    "default": {
      "ConnectionString": "server=127.0.0.1;user=root;database=db_payment;port=3306;password=xxx;Pooling=true;Charset=utf8;SslMode=none;",
      "Name": "default",
      "ProviderName": "mysql"
    }
  },
  "rabbit": {
    "default": {
      "host": "127.0.0.1",
      "port": 5672,
      "user": "payment",
      "password": "xxx",
      "broker": "payment_event",
      "virtualHost": "payment"
    }
  },
  "const": {
    "enableStatistic": true
  },
  "sites": {
    "pay": "http://localhost:5000"
  }
}

# 数据库结构 支持MySQL和PostgreSQL
https://github.com/shoy160/spear-payment/tree/master/docs/database
```
