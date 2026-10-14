namespace Quark.Web.RouteConfigs
{
    public static class AuthorRouteConfig
    {
        public static void RegisterRoutes(IEndpointRouteBuilder app)
        {
            #region Authors Page

            app.MapControllerRoute(
                name: "Dynamic_AuthorDetails_TodayNewsList",
                pattern: "gunun-kose-yazilari/",
                defaults: new { controller = "Author", action = "AuthorNewsList" }
            );

            app.MapControllerRoute(
             name: $"Dynamic_Authors",
             pattern: $"yazarlar/",
             defaults: new { controller = "Author", action = "Authors" }
         );


            app.MapControllerRoute(
                name: "Dynamic_AuthorDetails_TodayCategoryNewsList",
                pattern: "/yazarlar/{slug}-yazarlari",
                defaults: new { controller = "Author", action = "AuthorCategoryNewsList" }
            );

         
            app.MapControllerRoute(
                name: $"Dynamic_AuthorDetails",
                pattern: $"yazarlar/{{slug}}",
                defaults: new { controller = "Author", action = "AuthorDetails" }
            );

            #endregion

        }
    }

}
