using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Web.Filters
{
    public class AdvancedErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
        public DateTime Timestamp { get; set; }
        public string RequestId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Path { get; set; }
        public Dictionary<string, object> Details { get; set; }
    }
}