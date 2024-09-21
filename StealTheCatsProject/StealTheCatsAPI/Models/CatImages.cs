namespace StealTheCatsAPI.Models
{
    public class CatImages
    {
        public string Id { get; set; }=string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public string Url { get; set; } = string.Empty;
        public List<CatBreed> CatBreeds { get; set; } = new List<CatBreed>();
    }
    public class CatBreed
    {
        public string Name { get; set; } = string.Empty;
        public string Temperament { get; set; } = string.Empty;
    }
}
