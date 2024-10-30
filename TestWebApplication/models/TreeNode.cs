namespace TestWebApplication.models
{
    public class TreeNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public virtual TreeNode Parent { get; set; }
        public virtual ICollection<TreeNode> Children { get; set; }
        public int TreeId { get; set; }
    }
}
