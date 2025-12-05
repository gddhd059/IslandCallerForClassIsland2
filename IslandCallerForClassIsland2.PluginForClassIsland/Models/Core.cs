using System.Runtime.InteropServices;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System;

namespace IslandCallerForClassIsland2.Models
{
    public static class Core
    {
        private static List<string> students = new List<string>();
        private static bool isInitialized = false;
        private static HashSet<string> RandomHashSet = new HashSet<string>(); // 用于存储已抽取的学生名单
        private static Random random = new Random();
        
        // 替代Core.dll的RandomImport函数
        public static int RandomImport(string filename)
        {
            students.Clear();
            RandomHashSet.Clear();
            
            try
            {
                string filenameWithExt = filename + ".csv";
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string filePath = Path.Combine(appDataPath, "IslandCaller", "Profile", filenameWithExt);
                
                IslandCallerForClassIsland2.PluginForClassIsland.Models.Log.WriteLog("Core.cs", "Debug", $"Attempting to import student list from: {filePath}");
                
                if (!File.Exists(filePath))
                {
                    string errorMsg = $"Failed to open file: {filenameWithExt}";
                    IslandCallerForClassIsland2.PluginForClassIsland.Models.Log.WriteLog("Core.cs", "Error", errorMsg);
                    // 显示错误消息
                    System.Windows.MessageBox.Show($"IslandCaller: {errorMsg}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return -1;
                }
                
                using (StreamReader reader = new StreamReader(filePath))
                {
                    // 跳过第一行标题
                    string line = reader.ReadLine();
                    
                    HashSet<string> importHashSet = new HashSet<string>();
                    
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] tokens = line.Split(',');
                        if (tokens.Length > 1)
                        {
                            string name = tokens[1].Trim();
                            
                            // 移除引号
                            if (name.StartsWith('"'))
                                name = name.Substring(1);
                            if (name.EndsWith('"'))
                                name = name.Substring(0, name.Length - 1);
                            
                            name = name.Trim();
                            
                            if (!string.IsNullOrEmpty(name) && !importHashSet.Contains(name))
                            {
                                students.Add(name);
                                importHashSet.Add(name);
                            }
                        }
                    }
                }
                
                if (students.Count == 0)
                {
                    string errorMsg = "Namelist is empty!";
                    IslandCallerForClassIsland2.PluginForClassIsland.Models.Log.WriteLog("Core.cs", "Error", errorMsg);
                    System.Windows.MessageBox.Show($"IslandCaller: {errorMsg}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return -1;
                }
                
                // 输出获取到的名单信息
                string studentListInfo = $"Successfully imported {students.Count} students. First 5 students: {string.Join(", ", students.Take(5))}";
                if (students.Count > 5)
                {
                    studentListInfo += $"... and {students.Count - 5} more";
                }
                IslandCallerForClassIsland2.PluginForClassIsland.Models.Log.WriteLog("Core.cs", "Success", studentListInfo);
                
                isInitialized = true;
                return 0;
            }
            catch (Exception ex)
            {
                string errorMsg = $"Error importing students: {ex.Message}";
                IslandCallerForClassIsland2.PluginForClassIsland.Models.Log.WriteLog("Core.cs", "Error", errorMsg);
                System.Windows.MessageBox.Show($"IslandCaller: {errorMsg}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return -1;
            }
        }
        
        // 替代Core.dll的SimpleRandom函数
        public static IntPtr SimpleRandom(int number)
        {
            try
            {
                if (!isInitialized)
                {
                    return Marshal.StringToBSTR("Not Initialized!");
                }
                
                if (number > students.Count)
                {
                    return Marshal.StringToBSTR("Not enough students!");
                }
                
                StringBuilder output = new StringBuilder();
                List<string> availableStudents = new List<string>(students);
                
                for (int i = 0; i < number; i++)
                {
                    if (availableStudents.Count == 0)
                    {
                        // 如果可选学生不足，重新填充可用列表
                        availableStudents = new List<string>(students);
                    }
                    
                    int randomIndex = random.Next(availableStudents.Count);
                    string randomStudent = availableStudents[randomIndex];
                    
                    output.Append(randomStudent);
                    if (i < number - 1)
                    {
                        output.Append("  ");
                    }
                    
                    availableStudents.RemoveAt(randomIndex);
                    RandomHashSet.Add(randomStudent);
                }
                
                return Marshal.StringToBSTR(output.ToString());
            }
            catch (Exception ex)
            {
                return Marshal.StringToBSTR($"Error: {ex.Message}");
            }
        }
        
        // 替代Core.dll的ClearHistory函数
        public static void ClearHistory()
        {
            RandomHashSet.Clear();
        }
        
        // 其他函数暂时返回默认值，因为它们不是点名功能的核心
        public static bool CreateHelloPasskey()
        {
            return false;
        }
        
        public static bool VerifyHelloPasskey()
        {
            return false;
        }
        
        public static IntPtr CreateTOTPUrl()
        {
            return Marshal.StringToBSTR(string.Empty);
        }
        
        public static bool VerifyTOTP(string user_code)
        {
            return false;
        }
        
        // Async wrapper methods
        public static Task<bool> CreateHelloPasskeyAsync()
        {
            return Task.Run(() => CreateHelloPasskey());
        }
        
        public static Task<bool> VerifyHelloPasskeyAsync()
        {
            return Task.Run(() => VerifyHelloPasskey());
        }
        
        public static Task<IntPtr> CreateTOTPUrlAsync()
        {
            return Task.Run(() => CreateTOTPUrl());
        }
        
        public static Task<bool> VerifyTOTPAsync(string user_code)
        {
            return Task.Run(() => VerifyTOTP(user_code));
        }
    }
}
