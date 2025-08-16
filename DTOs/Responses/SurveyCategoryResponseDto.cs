namespace Ankets.DTOs.Responses
{
    public class SurveyCategoryResponseDto
    {
        public int CategoriesId { get; set; }
        public string CategoriesName { get; set; }
        public string? CategoriesDescription { get; set; }
        public string? Color { get; set; }
    }
}
