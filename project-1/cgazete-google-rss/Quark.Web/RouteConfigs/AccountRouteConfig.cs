namespace Quark.Web.RouteConfigs
{
    public static class AccountRouteConfig
    {
        public static void RegisterRoutes(IEndpointRouteBuilder app)
        {
            app.MapControllerRoute(
                name: "Account_LogIn",
                    pattern: "uye/giris",
                    defaults: new { controller = "Account", action = "LogIn" }
                );

            app.MapControllerRoute(
            name: "Account_MembershipIntroduction",
                    pattern: "uye/abonelik",
                defaults: new { controller = "Account", action = "MembershipIntroduction" }
            );

            app.MapControllerRoute(
            name: "Account_ForgotPassword",
                pattern: "uye/sifremi-unuttum",
                defaults: new { controller = "Account", action = "ForgotPassword" }
            );

            app.MapControllerRoute(
            name: "Account_ChangePassword",
                pattern: "uye/sifremi-degistir",
                defaults: new { controller = "Account", action = "ChangePassword" }
            );

            app.MapControllerRoute(
            name: "Account_AccountInformations",
                pattern: "uye/hesap-bilgileri",
                defaults: new { controller = "Account", action = "AccountInformations" }
            );

            app.MapControllerRoute(
            name: "Account_MembershipInformations",
                pattern: "uye/abonelik-bilgileri",
                defaults: new { controller = "Account", action = "MembershipInformations" }
            );

            app.MapControllerRoute(
            name: "Account_Payment",
                pattern: "uye/odeme",
                defaults: new { controller = "Account", action = "Payment" }
            );

            app.MapControllerRoute(
            name: "Account_SuccessPayment",
                pattern: "uye/basarili-odeme",
                defaults: new { controller = "Account", action = "SuccessPayment" }
            );

            app.MapControllerRoute(
            name: "Account_Support",
                pattern: "uye/destek",
                defaults: new { controller = "Account", action = "Support" }
            );

            app.MapControllerRoute(
            name: "Account_AccountMainPage",
                pattern: "uye/anasayfa",
                defaults: new { controller = "Account", action = "AccountMainPage" }
            );


            app.MapControllerRoute(
            name: "Account_AccountLogout",
                pattern: "uye/cikis-yap",
                defaults: new { controller = "Account", action = "Logout" }
            );

			app.MapControllerRoute(
	          name: "Account_AccountRegister",
		          pattern: "uye/kayit-ol",
		          defaults: new { controller = "Account", action = "Register" }
	          );

		}
    }
}
