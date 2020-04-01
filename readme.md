# ObjectQL

## 介绍

## 安装使用

1. 配置内部nuget源
```
http://nexus.yungis.internal/repository/nuget-group/
```

2. 下载安装包
 + `ObjectQL (>=2.0.0)`

 > 已内置`sql server`驱动，但尚不支持 __CodeFirst 模式__。

3. 安装驱动
 + `ObjectQL.OracleClient (>=2.0.0)`
 + `ObjectQL.MySqlClient (>=2.0.0)`

## 2种开发模式

+ __[Model 映射模式](./docs/ModelMapping.md)__
+ __[CodeFirst 模式](./docs/CodeFirst.md)__

## ObjectQL.Linq 语法说明

 __[Linq 简明语法](./docs/Linq.md)__

 __[Sequence序列](./docs/Sequence.md)__