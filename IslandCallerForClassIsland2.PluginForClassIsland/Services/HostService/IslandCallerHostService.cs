using IslandCallerForClassIsland2.Services.NotificationProvidersNew;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared.Enums;
using IslandCallerForClassIsland2.Models;
using Microsoft.Extensions.Hosting;
using Status = IslandCallerForClassIsland2.Models.Status;
using System.IO;
using System.Reflection;
using Log = IslandCallerForClassIsland2.PluginForClassIsland.Models.Log;

namespace IslandCallerForClassIsland2.Services.IslandCallerHostService
{
    public class IslandCallerHostService : IHostedService
    {
        private ILessonsService LessonsService { get; }
        public IUriNavigationService UriNavigationService { get; }

        public IslandCallerHostService(Plugin plugin, IUriNavigationService uriNavigationService, ILessonsService lessonsService)
        {
            LessonsService = lessonsService;
            UriNavigationService = uriNavigationService;
            lessonsService.CurrentTimeStateChanged += (s, e) =>
            {
                Status.Instance.lessonstatu = lessonsService.CurrentState;
                // 尝试调用Core.ClearHistory()，如果Core.dll缺失则忽略
                try
                {
                    Core.ClearHistory();
                }
                catch (DllNotFoundException)
                {
                    // Core.dll缺失，忽略此调用
                    Log.WriteLog("IslandCallerHostService.cs", "Warning", "Core.dll not found, clear history skipped");
                }
            };
            // 在ClassIsland 2.0中，HandlePluginsNavigation方法可能已经过时或更改
            // 尝试使用新的注册方式
            try
            {
                // 保留旧的注册方式，同时添加新的注册方式
                UriNavigationService.HandlePluginsNavigation(
                    "Plugin.IslandCaller.ClassIsland2/Run",
                    args =>
                    {
                        new IslandCallerNotificationProviderNew().RandomCall(1);
                    }
                );
                UriNavigationService.HandlePluginsNavigation(
                    "Plugin.IslandCaller.ClassIsland2/Simple",
                    args =>
                    {
                        new IslandCallerNotificationProviderNew().RandomCall(1);
                    }
                );
                UriNavigationService.HandlePluginsNavigation(
                    "Plugin.IslandCaller.ClassIsland2/Advanced/GUI",
                    args =>
                    {
                        new IslandCallerNotificationProviderNew().RandomCall(1);
                    }
                );
            }
            catch (Exception ex)
            {
                Log.WriteLog("IslandCallerHostService.cs", "Error", $"Failed to register navigation: {ex.Message}");
            }
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Log.WriteLog("IslandCallerHostService.cs", "Debug", "IslandCallerHostService Started");
            
            // 尝试加载必要的程序集
            try
            {
                string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "Plugin.IslandCaller.ClassIsland2", "iNKORE.UI.WPF.Modern.Controls.dll");
                if (File.Exists(dllPath))
                {
                    Assembly.LoadFrom(dllPath);
                    Log.WriteLog("IslandCallerHostService.cs", "Success", "Loaded iNKORE.UI.WPF.Modern.Controls.dll");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("IslandCallerHostService.cs", "Error", $"Failed to load assembly: {ex.Message}");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
