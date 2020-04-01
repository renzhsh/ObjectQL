using UserCenter.Model;
using System.Collections.Generic;
using System.Linq;
using ObjectQL;
using ObjectQL.Linq;

namespace UserCenter.Usages
{
    public class QueryTest
    {
        public QueryTest()
        {
            gateway = new DataGateway();
        }


        public DataGateway gateway { get; private set; }


        public void Test_GetAll()
        {
            List<User> list = gateway.Where<User>().Select().ToList();
        }

        /// <summary>
        /// 条件查询
        /// </summary>
        public void Condition_Query()
        {
            List<User> list = gateway.Where<User>(item => item.Id == "1704060000008640").Select().ToList();


            //字符串like只支持StartWith,EndWith,Contains,Equals
            list = gateway.Where<User>(item => item.Id.Contains("000") && item.Id.StartsWith("1704")).Select().ToList();

        }

        /// <summary>
        /// Sql语句查询
        /// </summary>
        //public void SQL_Condition_Query()
        //{
        //    List<User> list = gateway.Query<User>("select * from SYS_USER where ID={0}", "1704060000008640").ToList();
        //}

        /// <summary>
        /// 多条件查询
        /// </summary>
        public void AppendWhere()
        {
            gateway.Where<User>(item => item.Id == "1704060000008640")
                .AppendAndWhere(item => item.Id.Contains("0")) // And
                .AppendOrWhere(item => item.Id.StartsWith("1")) // or
                .Select();

            //gateway.Where<User>(item => item.Id == "1704060000008640")
            //    .AppendAndWhere("ID like '{0}'", "1704") // And sql语句
            //    .AppendOrWhere("Name={0}", "asfda") // or sql语句
            //    .Select();
        }

        /// <summary>
        /// 判断是否存在满足条件的记录
        /// </summary>
        public void Exists()
        {
            bool isExists = gateway.Where<User>(item => item.Id == "1704060000008640").Exists();

        }

        public void Select()
        {
            IDataQueryable<User> query = gateway.Where<User>(item => item.Id == "1704060000008640");

            //只查询Email,Password 2个字段
            User user = query.Select(item => new { item.UserName, item.Password }).FirstOrDefault();


            //满足条件的记录数
            int sumCount;
            query.Select(out sumCount);
            query.Select(out sumCount, item => new { item.UserName, item.Password });
        }

        /// <summary>
        /// 排序
        /// </summary>
        public void Order()
        {
            gateway.Where<User>()
                .OrderBy(u => u.CreateDate)
                .Desc()
                .Select();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public void Page_Query()
        {
            //指定查询条件
            IDataQueryable<User> query = gateway.Where<User>(item => item.Id.Contains("0"));

            //统计总数
            int sumCount = query.Count();

            //分页，跳过5条记录，取10条
            List<User> result = query.Skip(5).Take(10).Select().ToList();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        public void Page_2_Query()
        {
            //指定查询条件
            IDataQueryable<User> query = gateway.Where<User>(item => item.Id.Contains("0"));

            //统计总数
            int sumCount;

            //分页，跳过5条记录，取10条
            List<User> result = query
                .Skip(5)
                .Take(10)
                .Select(out sumCount)
                .ToList();

        }

        /// <summary>
        /// 关联查询
        /// </summary>
        public void Relational_Query()
        {
            IDataQueryable<User> query = gateway.Where<User>(item => item.Id == "1704100000015110");

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

            User user = query.Select().FirstOrDefault();
        }


        public void Relation_Exists()
        {
            IDataQueryable<User> query = gateway.Where<User>(item => item.Id == "1704100000015110");

            // exists( select 1 from UserRole where User.Id=UserRole.UserID and UserRole.RoleID="1111")
            query.Exists<UserRole>(u => u.Id, ur => ur.UserID, ur => ur.RoleID == "1111");

            User user = query.Select().First();
        }

        public void Relational_single_Query()
        {
            IDataQueryable<User> query = gateway.Where<User>();


            query.Join<UserRole>(u => u.Id, ur => ur.UserID)
                .Load(u => u.UserRole);

            query.Select();
        }
    }
}
