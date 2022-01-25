using System.Diagnostics;
using Microsoft.Win32;

namespace Microsoft_Defender_Control
{
    internal class DefenderController
    {
        private readonly RegistryKey _currentUserKey = Registry.LocalMachine;

        public bool IsAutoRun
        {
            get
            {
                RegistryKey autoRunProgram = _currentUserKey.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
                return autoRunProgram.GetValue("Shell").ToString().Contains("Microsoft-Defender-Control.exe");
            }
        }

        public bool IsEnable
        {
            get
            {
                RegistryKey windowsDefenderKey = _currentUserKey.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection");
                return windowsDefenderKey?.GetValue("DisableRealtimeMonitoring")?.ToString() != "1";
            }
        }

        public void EnableDefender()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Now the pc will reboot!");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Press to do this..");
            Console.ResetColor();
            Console.ReadKey();

            RegistryKey windowsDefenderKey = _currentUserKey.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender");
            windowsDefenderKey.DeleteValue("AllowFastServiceStartup", false);
            windowsDefenderKey.DeleteValue("DisableAntiSpyware", false);
            windowsDefenderKey.DeleteValue("ServiceKeepAlive", false);

            windowsDefenderKey = _currentUserKey.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection");
            windowsDefenderKey.DeleteValue("DisableIOAVProtection", false);
            windowsDefenderKey.DeleteValue("DisableRealtimeMonitoring", false);

            windowsDefenderKey = _currentUserKey.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet");
            windowsDefenderKey.DeleteValue("DisableBlockAtFirstSeen", false);
            windowsDefenderKey.DeleteValue("LocalSettingOverrideSpynetReporting", false);
            windowsDefenderKey.DeleteValue("SubmitSamplesConsent", false);

            windowsDefenderKey = _currentUserKey.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\WinDefend");
            windowsDefenderKey.SetValue("Start", 2, RegistryValueKind.DWord);

            var p = new Process();
            p.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            p.StartInfo.WorkingDirectory = @"C:\Windows\System32";
            p.StartInfo.Arguments = @"/k shutdown /r /t 5";
            p.Start();
        }

        public void DisableDefender()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Now the PC will reboot in safe mode!");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Press to do this..");
            Console.ReadKey();

            RegistryKey windowsDefenderKey = _currentUserKey.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender");
            windowsDefenderKey.SetValue("DisableAntiSpyware", 1, RegistryValueKind.DWord);
            windowsDefenderKey.SetValue("AllowFastServiceStartup", 0, RegistryValueKind.DWord);
            windowsDefenderKey.SetValue("ServiceKeepAlive", 0, RegistryValueKind.DWord);

            windowsDefenderKey = _currentUserKey.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection");
            windowsDefenderKey.SetValue("DisableIOAVProtection", 1, RegistryValueKind.DWord);
            windowsDefenderKey.SetValue("DisableRealtimeMonitoring", 1, RegistryValueKind.DWord);

            windowsDefenderKey = _currentUserKey.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows Defender\Spynet");
            windowsDefenderKey.SetValue("DisableBlockAtFirstSeen", 1, RegistryValueKind.DWord);
            windowsDefenderKey.SetValue("LocalSettingOverrideSpynetReporting", 0, RegistryValueKind.DWord);
            windowsDefenderKey.SetValue("SubmitSamplesConsent", 2, RegistryValueKind.DWord);

            var p = new Process();
            p.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            p.StartInfo.WorkingDirectory = @"C:\Windows\System32";
            p.StartInfo.Arguments = @"/k bcdedit /set {current} safeboot minimal";
            p.Start();

            p.StartInfo.Arguments = @"/k shutdown /r /t 5";
            p.Start();

            RegistryKey autoRunProgram = _currentUserKey.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
            autoRunProgram.SetValue("Shell", $@"explorer.exe, {new DirectoryInfo(@".").FullName}\Microsoft-Defender-Control.exe");
        }

        public void SafeModeStep()
        {
            RegistryKey windowsDefenderKey = _currentUserKey.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\WinDefend");
            windowsDefenderKey.SetValue("Start", 4, RegistryValueKind.DWord);

            var p = new Process();
            p.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            p.StartInfo.WorkingDirectory = @"C:\Windows\System32";
            p.StartInfo.Arguments = @"/k bcdedit /deletevalue {current} safeboot";
            p.Start();

            p.StartInfo.Arguments = @"/k shutdown /r /t 0";
            p.Start();

            RegistryKey autoRunProgram = _currentUserKey.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon");
            autoRunProgram.SetValue("Shell", @"explorer.exe");
        }
    }
}
