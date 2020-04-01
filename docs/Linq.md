# Linq简明语法

命名空间：
```
using ObjectQL.Linq;
```

## Insert

语法：
```C#
public void Insert<T>(T model) where T : class, new();
//批量插入
public void InsertBatch<T>(IEnumerable<T> models) where T : class, new();
```
举例：

```C#
var gateway = new DataGateway();
var model = new Student
{
    Age=1,
    Name="John"
};

gateway.Insert(model);

var list=List<Student>();

list.Add(...);


gateway.InsertBatch(list);

```

## Delete

语法：

```C#
// expression:删除条件
public NonQueryResult Delete<T>(Expression<Func<T, bool>> expression) where T : class, new();
```

```C#
gateway.Delete<LoginLog>(x => x.UserID == "1234567890123456");
```

## Update

语法：

```C#
// updateCriteria:更新内容
// expression:更新条件
public NonQueryResult Update<T>(UpdateCriteria<T> updateCriteria, Expression<Func<T, bool>> expression) where T : class, new();
```

举例：
```C#
var updateCriteria = UpdateCriteria<SeqGen>
                    .Builder
                    .Set(item => item.SeqValue, 10)
                    .Set(item => item.SeqLoop, 1);
                
gateway.Update(updateCriteria, item => item.SeqName == SeqName);
```

Query

条件查询：
```C#
gateway.Where<User>(item => item.Id == "1704060000008640").Select();
```

多条件查询：
```C#
gateway.Where<User>(item => item.Id == "1704060000008640")
    .AppendAndWhere(item => item.Id.Contains("0")) // And
    .AppendOrWhere(item => item.Id.StartsWith("1")) // or
    .Select();
```

判断是否存在满足条件的记录：
```C#
gateway.Where<User>(item => item.Id == "1704060000008640").Exists()
```

满足条件的总数：
```C#
gateway.Where<User>(item => item.Id.Contains("0")).Count();
```

查询字段：
```C#
var query = gateway.Where<User>(item => item.Id == "1704060000008640");

//只查询Email,Password 2个字段
User user = query.Select(item => new { item.UserName, item.Password }).FirstOrDefault();

//满足条件的记录数
int sumCount;
query.Select(out sumCount);
query.Select(out sumCount, item => new { item.UserName, item.Password });
```

分页:
```C#
var query = gateway.Where<User>(item => item.Id.Contains("0"));

//分页，跳过5条记录，取10条
query
    .Skip(5)
    .Take(10)
    .Select()
    .ToList();
```

关联查询:

```C#
// User Model
public class User
{
    public String Id { set; get; }

    public String UserName { set; get; }

    . . .

    public IEnumerable<UserRole> UserRoles { set; get; }

    public IEnumerable<Role> Roles { set; get; }
}
```

```C#
var query = gateway.Where<User>(item => item.Id == "1704100000015110");

// 2表
//User.Id == UserRole.UserID
query.Join<UserRole>(u => u.Id, ur => ur.UserID)
    .Load(x => x.UserRoles);

// 多表
// User.Id==UserRole.UserID
query.Join<UserRole>(u => u.Id, ur => ur.UserID)
    // UserRole.RoleID==Role.RoleID
    .JoinNext<Role>(ur => ur.RoleID, r => r.RoleID)
    // 加载User对象的Roles属性
    .Load(x => x.Roles);
```

Exists查询：

```C#
var query = gateway.Where<User>(item => item.Id == "1704100000015110");

// exists( select 1 from UserRole where User.Id=UserRole.UserID and UserRole.RoleID="1111")
query.Exists<UserRole>(u => u.Id, ur => ur.UserID, ur => ur.RoleID == "1111");
```