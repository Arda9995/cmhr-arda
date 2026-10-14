namespace Quark.Web.RouteConfigs
{
    public static class NewsRouteConfig
    {
        public static void RegisterRoutes(IEndpointRouteBuilder app)
        {
            #region News Details Page!

    

            // Original routes
            app.MapControllerRoute(
             name: "News_ById",
             pattern: "{category}/-{id:int}",
             defaults: new { controller = "News", action = "Details" }
         );

            app.MapControllerRoute(
                 name: "News_ById_Redirect",
                 pattern: "{category}/{id:int}",
                 defaults: new { controller = "News", action = "RedirectToFullSlug" }
             );

            app.MapControllerRoute(
                    name: "Dynamic_News_Category",
                    pattern: "{category}/{slug}-{id:int}",
                    defaults: new { controller = "News", action = "Details" }
                );

            app.MapControllerRoute(
                    name: "Dynamic_News_SecondaryCategory",
                    pattern: "{category}/{secondaryCategory}/{slug}-{id:int}",
                    defaults: new { controller = "News", action = "Details" }
                );

            for (int i = 1; i <= 28; i++)
            {
                var segments = string.Join("/", Enumerable.Range(1, i).Select(x => $"{{segment{x}}}"));
                app.MapControllerRoute(
                    name: $"News_With{i}Segments",
                    pattern: $"{segments}-{{id:int}}",
                    defaults: new { controller = "News", action = "RedirectToFullSlug" }
                );
            }
            #endregion
        }
    }
}
