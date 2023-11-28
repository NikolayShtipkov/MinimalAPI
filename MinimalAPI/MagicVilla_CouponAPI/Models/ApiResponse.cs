using System.Net;

namespace MagicVilla_CouponAPI.Models
{
    public class ApiResponse
    {
        public ApiResponse()
        {
            ErrorMessages = new List<string>();
        }

        public bool isSuccessful { get; set; }
        public Object Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
