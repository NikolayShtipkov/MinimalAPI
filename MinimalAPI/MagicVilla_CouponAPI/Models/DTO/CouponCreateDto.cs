namespace MagicVilla_CouponAPI.Models.DTO
{
    public class CouponCreateDto
    {
        public string Name { get; set; }
        public int Percent { get; set; }
        public bool IsActive { get; set; }
    }
}
