using Microsoft.AspNetCore.Http;

namespace GoToSpeak.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message) {
            response.Headers.Add("Applciation-Error",message);
            response.Headers.Add("Access-control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin","*");
        }
    }
}