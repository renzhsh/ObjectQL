# Code First模式

使用Code First模式，你可以专注于领域设计，而不必关心如何设计数据库。__ObjectQL__ 会根据你设计的类，自动创建数据库，完成模型的映射。

## 简单4个步骤快速使用

建Model ==> 注册 ==> 配置 ==> 启动

1. 建Model

```C#
    public class Student
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Address { get; set; }
    }

```

2. 注册

```C#
using ObjectQL.Mapping;

public class CodeFirstRegister : ObjectQL.CodeFirstRegister
{
    public override void Configure()
    {
        // 添加要注册的model
        AddEntity<Student>();
    }
}
```

3. 配置

`ObjectQL`在安装时配置结构已设置好，修改具体的值就可以了。

连接字符串：
```xml
<connectionStrings>
    <add connectionString="Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=orcl)));User Id=objectql;Password=objectql;"
      name="BaseConnectString" providerName="Oracle"/>
  </connectionStrings>
```

> 至少有一个`BaseConnectString`。`providerName`与下面的驱动要匹配。

数据库驱动：

`ObjectQL.OracleClient`在安装时已设置好。
```xml
<objectQL>
    <drivers>
      <add name="Oracle" provider="ObjectQL.Data.OracleDirverProvider, ObjectQL.OracleClient"></add>
    </drivers>
</objectQL>
```

添加注册：

```xml
<objectQL>
    <containers>
        <mapContainer connectionKey="BaseConnectString" buildingPolicy="Initialize">
            <register name="XXXXXX.Domain.CodeFirstRegister, XXXXXX.Domain"></register>
        </mapContainer>
    </containers>
</objectQL>
```

## 建Model的特殊要求

在某些特殊情况下，比如需要自定义字段名、修改字段大小、添加约束时，可以通过`DataAnnotation`来实现。

> 命名空间`ObjectQL.DataAnnotation`

1. TableAttribute
 + TableName : 表名
 + Schema :所在的表空间
 + Prefix：表的前缀,默认`T_`

 ```c#
    [Table("MyStudent", Schema = "objectql")]
    public class Student
    {
    }
 ```

 2. ColumnAttribute
  + ColumnName: 字段名
  + Prefix：前缀，默认'F_'
  + AllowNull：允许为空，默认true
  + MaxLength: 最大长度
  + Scale: 小数点位数
  + Unicode: 字符串Unicode编码，默认true; `true => nvarchar | false => varchar`

   ```c#
    [Table("MyStudent", Schema = "objectql")]
    public class Student
    {
        [Column(MaxLength = 200, Unicode = false)]
        public string Name { get; set; }

    }
  ```

 3. NotMappedAttribute

 如果不想给某些字段建立映射，可以添加 `NotMappedAttribute`;

 4. PrimaryKeyAttribute

 主键约束，指明一个字段是主键。一个Model必须有且仅有一个主键。

  ```c#
    public class Student
    {
        [PrimaryKey]
        public string ID { get; set; }
    }
  ```

  在没有明确指明主键的情况下，下列字段自动成为主键：（大小写不敏感）
   + ID
   + 表名+ID
 
 5. UniqueAttribute

 唯一性约束。

一个字段：
 ```c#
    public class Student
    {
        [Unique]
        public string Name { get; set; }
    }
 ```

 多个字段：
 ```c#
    [Unique(Property ="Name,Age")]
    public class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
 ```

 6. 支持的字段类型以及数据库类型，请参考[数据类型](./数据类型.md).

## 建库策略

__ObjectQL__ 支持 初始化（Initialize）、模型修正（Modify）2种策略，具体内容参考[建库策略](./Building.md).

请在配置文件中添加`buildingPolicy`属性(大小写不敏感)，默认值`Modify`。

```xml
<containers>
      <mapContainer connectionKey="BaseConnectString" buildingPolicy="Modify">
            <register name="ObjectQL.DataExtTests.MapRegister, UnitTest.Domain"></register>
      </mapContainer>
</containers>
```