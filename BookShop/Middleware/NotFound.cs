namespace BookShop.Middleware
{
    public class NotFound
    {
        private readonly RequestDelegate _next;

        public NotFound(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 404)
            {
                context.Response.Redirect("/Home/NotFound");
            }
        }
    }
}
