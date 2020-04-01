# 序列 Sequence

命名空间：
```C#
using ObjectQL.Sequences;
```

## 递增数列 `Increase`

构造函数：

```C#
public Increase();
public Increase(string name); // name:业务名称
// name:业务名称， firstValue:初始值，step: 递增大小，min:最小值， max:最大值
public Increase(string name, long firstValue, int step = 1, long min = 1, long max = long.MaxValue); 
```

属性：
```C#
// 当前值
public long CurrentValue;
```

方法：
```C#
// 获取下一个序列值；length:期望获取的序列长度；
public long Next(int length = 0)
```

## Snowflake
高性能、分布式的ID生成器。每个节点同一毫秒内可以产生4095个序列，且能保证由1024个节点组成的分布式系统中产生的序列不会重复，也保持整体递增。

构造函数：
```C#
public Snowflake();
public Snowflake(long machineId);
public Snowflake(long machineId, long datacenterId);
```

方法：
```C#
public long NextId();
```

## SingleSnowflake

简化版的 `Snowflake`，去掉了分布式的因素。保证在集中式系统中，在 __时间间隔__ 内，产生4095个不重复的序列，且保持整体递增。

> 时间间隔 IntervalUnit
 + MSecond, //毫秒
 + Second,
 + Minute,
 + Hour,
 + Day

 构造函数：
 ```C#
 public SingleSnowflake();
 public SingleSnowflake(IntervalUnit unit);
 ```

 方法：
 ```C#
public long NextId();
 ```

 ## Sequence
 对以上常用方法的封装，提供了简单、易用的接口。

 ### 递增序列：

 ```C#
public static long Next();
public static long Next(string name); // name: 业务名称
public static long Next(int length); // length: 序列长度
public static long Next(string name, int length);
public static string Next(string name, int length, string format = "{0}", params object[] args); // 格式化递增序列
 ```

 举例：
 ```C#
Sequence.Next('kw',4,"MAS{1}t{0}d",DateTime.toString('yyyyMMdd'))
// MAS20190101t1234d
 ```
### Flake序列

```C#
// SingleSnowflake
public static long NextFlakeValue(IntervalUnit unit = IntervalUnit.Minute);
```

### Luhm序列
带有`Luhm`校验位的序列，由 __时间__ + __序列__ + __1位校验值__ 组成。__序列__ 使用递增序列`Increase`。
```C#
// name:业务名称
public static long NextLuhmValue(string name = "default", int length = 4, string dateFormat = "yyyyMMdd"); 
```
