using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Web.Filters
{
     public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public System.Collections.Generic.Dictionary<string, string[]> Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}