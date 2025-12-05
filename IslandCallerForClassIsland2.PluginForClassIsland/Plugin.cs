using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using IslandCallerForClassIsland2.Models;
using IslandCallerForClassIsland2.PluginForClassIsland.Models;
using IslandCallerForClassIsland2.Services.IslandCallerHostService;
using IslandCallerForClassIsland2.Services.NotificationProvidersNew;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
namespace IslandCallerForClassIsland2;

[PluginEntrance]
public class Plugin : PluginBase
{
    public static string PlugincfgFolder;
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        Log.WriteLog("Plugin.cs", "Debug", "IslandCaller Plugin Initializing");
        PlugincfgFolder = PluginConfigFolder;
        services.AddHostedService<IslandCallerHostService>();
        services.AddNotificationProvider<IslandCallerNotificationProviderNew>();

        // 保留原有的AppStarted事件处理，确保插件能够正确初始化
        try
        {
            AppBase.Current.AppStarted += (_, _) =>
            {
                new Models.Settings().Load();
                
                // 显示悬停窗口（如果启用）
                if (Settings.Instance.Hover.IsEnable)
                {
                    Status.Instance.fluenthover.Show();
                }
                
                // 尝试初始化Core.dll，但不阻止插件加载
                try
                {
                    if (Settings.Instance.Profile.ProfileList.TryGetValue(Settings.Instance.Profile.DefaultProfile, out string value))
                    {
                        int result = Core.RandomImport(value);
                        if (result == 0) 
                            Log.WriteLog("Plugin.cs", "Success", "Core Initialized");
                        else 
                            Log.WriteLog("Plugin.cs", "Error", $"Core Initialize Failed with code: {result}");
                    }
                    else
                    {
                        Log.WriteLog("Plugin.cs", "Error", "Profile not found");
                    }
                }
                catch (DllNotFoundException ex)
                {
                    Log.WriteLog("Plugin.cs", "Error", $"Core.dll not found: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Log.WriteLog("Plugin.cs", "Error", $"Failed to initialize Core: {ex.Message}");
                }
            };
        }
        catch (Exception ex)
        {
            Log.WriteLog("Plugin.cs", "Error", $"Failed to register AppStarted event: {ex.Message}");
            // 如果AppStarted事件失败，尝试在服务启动时初始化
            Log.WriteLog("Plugin.cs", "Debug", "Will initialize in service StartAsync");
        }
    }

}
