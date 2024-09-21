namespace StealTheCatsAPI.Models
{
    public class TagEntity
    {
        public int Id { get; set; } // Auto-incrementing primary key
        public string Name { get; set; } = string.Empty; // Tag name
        public DateTime Created { get; set; } = DateTime.UtcNow; // Timestamp when the tag was created
        public ICollection<CatTagEntity> CatTags { get; set; } = new List<CatTagEntity>();
    }
}
