namespace StealTheCatsAPI.Models
{
    public class CatTagEntity
    {
        public int CatId { get; set; }
        public int TagId { get; set; }

        public required CatEntity Cat { get; set; }
        public required TagEntity Tag { get; set; }

        // Adding composite key to ensure uniqueness
        public override bool Equals(object? obj)
        {
            if (obj is CatTagEntity other)
            {
                return CatId == other.CatId && TagId == other.TagId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CatId, TagId);
        }
    }
}
