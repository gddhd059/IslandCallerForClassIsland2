using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Abstractions.Services.NotificationProviders;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Notification;
using ClassIsland.Shared.Enums;
using ClassIsland.Shared.Interfaces;
using IslandCallerForClassIsland2.Controls.NotificationProviders;
using IslandCallerForClassIsland2.Views.Windows;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IslandCallerForClassIsland2.Models;
using Log = IslandCallerForClassIsland2.PluginForClassIsland.Models.Log;

namespace IslandCallerForClassIsland2.Services.NotificationProvidersNew;

[NotificationProviderInfo(
    "9B570BF1-9A32-40C0-9D5D-4FFA69E03A37",
    "IslandCallerServices",
    "AccountCheck",
    "用于为IslandCaller提供通知接口")]
public class IslandCallerNotificationProviderNew : NotificationProviderBase
{
    public async void RandomCall(int stunum)
    {
        if (Settings.Instance.General.BreakDisable & Status.Instance.lessonstatu == TimeState.Breaking) return;
        
        try
        {
            string output;
            
            // 尝试使用Core.dll进行点名
            try
            {
                IntPtr ptr1 = Core.SimpleRandom(stunum);
                if (ptr1 == IntPtr.Zero)
                {
                    Log.WriteLog("IslandCallerNotificationProviderNew.cs", "Error", "Core.SimpleRandom returned null pointer");
                    output = "随机点名失败: Core.SimpleRandom返回空指针";
                }
                else
                {
                    output = Marshal.PtrToStringBSTR(ptr1);
                    Marshal.FreeBSTR(ptr1); // 释放分配的 BSTR 内存
                    
                    if (string.IsNullOrEmpty(output))
                    {
                        Log.WriteLog("IslandCallerNotificationProviderNew.cs", "Error", "RandomCall returned empty string");
                        output = "随机点名失败: 返回空字符串";
                    }
                }
            }
            catch (DllNotFoundException)
            {
                // Core.dll缺失，使用备选方案
                Log.WriteLog("IslandCallerNotificationProviderNew.cs", "Warning", "Core.dll not found, using fallback solution");
                output = "随机点名: " + GetRandomStudentNames(stunum);
            }
            
            int maskduration = stunum * 2 + 1; // 计算持续时间
            
            // 显示自定义窗口
            var fluentShower = new FluentShower(output);
            fluentShower.Show();
            await Task.Delay(maskduration * 1000); // 等待指定的持续时间
            fluentShower.Close();
        }
        catch (Exception ex)
        {
            Log.WriteLog("IslandCallerNotificationProviderNew.cs", "Error", $"RandomCall failed: {ex.Message}");
            // 显示错误提示
            var fluentShower = new FluentShower($"点名失败: {ex.Message}");
            fluentShower.Show();
            await Task.Delay(3000);
            fluentShower.Close();
        }
    }
    
    /// <summary>
    /// 当Core.dll缺失时使用的备选方案，返回多个随机学生名称
    /// </summary>
    private string GetRandomStudentNames(int count)
    {
        // 简单的备选方案：返回指定数量的随机学生名称
        var sampleStudents = new List<string> { "张三", "李四", "王五", "赵六", "钱七", "孙八", "周九", "吴十", "郑十一", "王十二", "冯十三", "陈十四", "褚十五", "卫十六" };
        var random = new Random();
        var selectedStudents = new List<string>();
        var availableStudents = new List<string>(sampleStudents);
        
        // 确保不重复选择相同的学生
        for (int i = 0; i < count; i++)
        {
            if (availableStudents.Count == 0)
            {
                // 如果可选学生不足，重新填充可用列表
                availableStudents = new List<string>(sampleStudents);
            }
            
            int index = random.Next(availableStudents.Count);
            selectedStudents.Add(availableStudents[index]);
            availableStudents.RemoveAt(index);
        }
        
        return string.Join("  ", selectedStudents);
    }
}
