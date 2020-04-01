# ObjectQL 使用说明

## 安装ObjectQL

> 配置Nuget服务：http://192.168.1.90:81/repository/nuget-group/

安装 ObjectQL 和 驱动程序 ObjectQL.OracleClient.

## 创建数据库

1、在Oracle数据库中创建表空间objectql;
2、执行createTable.sql;
3、在App.config(或Web.config)中修改ConnectionString;