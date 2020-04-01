namespace ObjectQL.DataExtTests.Models
{
    public class UserRole : UserRole<string>
    {
        public string Id { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class UserRole<TKey>
    {
        public virtual TKey RoleId { get; set; }
        public virtual TKey UserId { get; set; }
    }
}