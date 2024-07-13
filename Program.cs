namespace CheckListJob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages()
                .AddMvcOptions(options =>
                {
                    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "Данное поле не должно быть пустым");
                });
            builder.Services.AddAuthentication("Cookies")
                .AddCookie(options => {
                    options.LoginPath = "/User/SignIn";
                    options.AccessDeniedPath = "/Home/AccessDenied";
                });
            builder.Services.AddAuthorization();
            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=WelcomeIndex}");

            app.Run();
        }
    }
}
