using MoviesListProject.Views;
using TmdbServiceConnector;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MoviesListProject
{
    public partial class App : Application
    {

        public static DataServiceConnector service;
        public static INavigation navigator;

        public App()
        {
            InitializeComponent();           

            SetMainPage();
        }

        public static void SetMainPage()
        {
            service = new DataServiceConnector();
            

            Current.MainPage = new TabbedPage
            {
                Children =
                {
                    new NavigationPage(new SearchPage(service))
                    {
                        Title = "Search",
                        Icon = Device.OnPlatform("tab_feed.png",null,null)
                    },
                     new NavigationPage(new MoviesListPage(service, 0))
                    {
                        Title = "Upcoming",
                        Icon = Device.OnPlatform("tab_feed.png",null,null)
                    },                     
                      new NavigationPage(new MoviesListPage(service, 1))
                    {
                        Title = "Top Rated",
                        Icon = Device.OnPlatform("tab_feed.png",null,null)
                    },
                    new NavigationPage(new AboutPage())
                    {
                        Title = "About",
                        Icon = Device.OnPlatform("tab_about.png",null,null)
                    },
                }
            };
        }
    }
}
