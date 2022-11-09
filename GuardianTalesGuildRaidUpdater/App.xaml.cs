using GuardianTalesGuildRaidUpdater.Models;
using GuardianTalesGuildRaidUpdater.Options;
using GuardianTalesGuildRaidUpdater.Services;
using GuardianTalesGuildRaidUpdater.Services.QuartzJobs;
using GuardianTalesGuildRaidUpdater.ViewModels;
using GuardianTalesGuildRaidUpdater.Views;

namespace GuardianTalesGuildRaidUpdater
{
    public partial class App : Application
    {
        private readonly IHost host;
        public IServiceProvider Container { get; init; }

        public App()
        {
            host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    IHostEnvironment env = hostingContext.HostingEnvironment;
                    config.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                    IConfigurationRoot configurationRoot = config.Build();
                    GoogleOAuthClientOption authOpt = new();
                    configurationRoot.GetSection(nameof(GoogleOAuthClientOption)).Bind(authOpt);
                    GoogleApiEndPointsOption endpointOpt = new();
                    configurationRoot.GetSection(nameof(GoogleApiEndPointsOption)).Bind(endpointOpt);

                    #region App Center Issue Tracking
                    //AppCenter.Start($"{appCenterOptions.Secret}", typeof(Analytics), typeof(Crashes));
                    #endregion
                })
                .ConfigureServices((context, services) =>
                {
                    GoogleOAuthClientOption authOpt = new();
                    context.Configuration.GetSection(nameof(GoogleOAuthClientOption)).Bind(authOpt);
                    services.Configure<GoogleOAuthClientOption>(context.Configuration.GetSection(nameof(GoogleOAuthClientOption)));
                    GoogleApiEndPointsOption endpointOpt = new();
                    context.Configuration.GetSection(nameof(GoogleApiEndPointsOption)).Bind(endpointOpt);
                    services.Configure<GoogleApiEndPointsOption>(context.Configuration.GetSection(nameof(GoogleApiEndPointsOption)));

                    services.AddTransient<LoginWindow>();
                    services.AddTransient<LoginWindowViewModel>();
                    services.AddTransient<LoginView>();
                    services.AddTransient<LoginViewModel>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<MainView>();
                    services.AddSingleton<MainViewModel>();

                    services.AddSingleton<AuthInfo>();
                    services.AddSingleton<ContentImageIndex>();
                    services.AddTransient<GoogleOAuthService>();

                    services.AddHttpClient<GoogleOAuthService>();

                    services.AddQuartz(q =>
                    {
                        q.UseMicrosoftDependencyInjectionJobFactory();
                        q.UseSimpleTypeLoader();
                        q.UseInMemoryStore();
                        q.UseDefaultThreadPool(tp =>
                        {
                            tp.MaxConcurrency = 10;
                        });
                        JobKey jobKey = new(nameof(RandomImageJob));
                        q.AddJob<RandomImageJob>(j => j.WithIdentity(jobKey));
                        q.AddTrigger(t => t
                            .ForJob(jobKey)
                            .StartNow()
                            .WithIdentity($"{nameof(RandomImageJob)}_1")
                            .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(3)).RepeatForever())
                            );
                    });
                    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
                })
                .ConfigureLogging(logging =>
                {
                    
                })
                .UseEnvironment(Environments.Development)
                .Build();
            Container = host.Services;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            // App.xaml에 정의한 Resource는 이 OnStartup 이벤트가 끝나야 Loading 된다.
            // 따라서 OnStartup 이벤트가 완료되기 전인 지금 App.xaml의 리소스를 호출하면 null이 발생한다.
            // LoginWindow는 OnStartup 안에서 호출되도록 작성했으므로, 현시점에서 App.xaml의 리소스를 이용할 수 없다.
            // 만약, 이후에 다른 곳에서 LoginWindow를 호출했다면 그 때는 App.xaml을 사용할 수 있다.
            // 따라서 지금은 App.xaml의 리소스를 사용할 수 없기 때문에 ViewLocator를 LoginWindow 내에 정의해서 지역 리소스로 이용한다.

            // 어...그런데 Application_Startup 이벤트를 한번 추가한 뒤로는 이벤트를 다시 제거해도 이 시점에 리소스가 생성되어 있다....뭐지....?

            base.OnStartup(e);
            await host.StartAsync();

            var loginVM = Container.GetRequiredService<LoginViewModel>();
            var loginWindowVM = Container.GetRequiredService<LoginWindowViewModel>();
            loginWindowVM.LoginViewModel = loginVM;

            bool result = default;

            while (!result)
            {
                LoginWindow loginWindow = Container.GetRequiredService<LoginWindow>();
                loginWindow.DataContext = loginWindowVM;
                result = loginWindow.ShowDialog().GetValueOrDefault();

                // 인증 실패 시 할 작업이 있다면 아래 코드 살려서 작업.
                //if (!result)
                //{
                //    Current.Shutdown();

                //    break;
                //}
            }

            if (result)
            {
                MainWindow mainWindow = Container.GetRequiredService<MainWindow>(); // 왜 이걸 주석풀면 프로그램이 꺼지지?? 조사필요.
                mainWindow.Show();
                mainWindow.Activate();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (host)
            {
                await host.StopAsync(TimeSpan.FromSeconds(1));
            }

            Current.Shutdown();
            base.OnExit(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

        }
    }
}
