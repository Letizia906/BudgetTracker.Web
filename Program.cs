namespace BudgetTracker.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Aggiunge MVC + HttpClient
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient<BudgetTracker.Web.Services.ExchangeService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Imposta come pagina iniziale Movimenti/Index
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Movimenti}/{action=Index}/{id?}");

            app.Run();

        }
    }
}

