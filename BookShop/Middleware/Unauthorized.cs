namespace BookShop.Middleware
{
    public class Unauthorized
    {
        private readonly RequestDelegate _next;

        public Unauthorized(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 401)
            {
                context.Response.Redirect("/Home/Login");
            }
        }
    }
}
