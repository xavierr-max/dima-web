using MudBlazor;
using MudBlazor.Utilities;

namespace Dima.Web;

//Atua como um contêiner de constantes e configurações globais para a aplicação frontend
public static class Configuration
{
    public const string HttpClientName = "dima";

    public static string BackendUrl { get; set; } = "http://localhost:5161";
    public static string StripePublicKey { get; set; } = "";
    
    public static MudTheme Theme = new()
    {
        Typography = new Typography
        {
            
            Default = new DefaultTypography()
            {
                FontFamily = ["Raleway", "sans-serif"]
            }
        },
        PaletteLight = new PaletteLight
        {
            Primary = "#f58723",
            Secondary = Colors.Orange.Darken3,
            Background = Colors.Gray.Lighten4,
            AppbarBackground = new MudColor("#f58723"),
            AppbarText = Colors.Shades.Black,
            TextPrimary = Colors.Shades.Black,
            DrawerText = Colors.Shades.Black,
            DrawerBackground = Colors.Gray.Lighten3,
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#a16eff",
            Secondary = Colors.LightBlue.Accent1,
            Background = Colors.Gray.Darken4,
            AppbarBackground = new MudColor("#a16eff"),
            AppbarText = Colors.Shades.Black,
        }
    };
}