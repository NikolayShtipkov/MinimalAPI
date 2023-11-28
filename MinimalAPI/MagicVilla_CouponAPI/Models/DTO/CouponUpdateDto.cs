namespace MagicVilla_CouponAPI.Models.DTO
{
    public class CouponUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Percent { get; set; }
        public bool IsActive { get; set; }
    }
}
