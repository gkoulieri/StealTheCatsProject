namespace StealTheCatsAPI.Models
{
    public class CatEntity
    {
        public int Id { get; set; } // Auto-incrementing unique ID for the cat
        public string CatId { get; set; } = string.Empty;// The id of the image returned from CaaS API
        public int Width { get; set; } // The width of the image returned from CaaS API
        public int Height { get; set; } // The height  of the image returned from CaaS API
        public string Image { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public ICollection<CatTagEntity> CatTags { get; set; } = new List<CatTagEntity>();



    }
}
