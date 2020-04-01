namespace ObjectQL.DataExtTests.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserOrgan
    {
        public string Id { set; get; }
        public virtual string UserId { get; set; }
        public virtual string OrganId { get; set; }
    }
}