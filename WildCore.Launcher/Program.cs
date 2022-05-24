/*
 * MIT License
 * 
 * Copyright (c) 2022 Stanislaw Schlosser <https://github.com/0x2aff>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
*/

using System.Text.Json;
using static WildCore.Launcher.NativeHelpers;

namespace WildCore.Launcher
{
    internal class Program
    {
        private static readonly string CONFIG_PATH = "Config.json";

        private static readonly string LAUNCH_ARGUMENTS_FORMAT = 
            "/auth {0} /authNc {0} /lang {1} /patcher {0} /SettingsKey WildStar /realmDataCenterId 9";

        internal static void Main()
        {
            string executablePath = GetExecutablePath();
            string executableLaunchArgs = GetExecutableArguments();

            if (!LaunchExecutable(executablePath, executableLaunchArgs))
                throw new InvalidOperationException($"Unable to launch {executablePath} with arguments: {executableLaunchArgs}.");
        }

        private static string GetExecutablePath()
        {
            string executablePath = "./Client64/WildStar64.exe";

            if (!File.Exists(executablePath))
                executablePath = "./Client32/WildStar32.exe";

            return executablePath;
        }

        private static string GetExecutableArguments()
        {
            Config config = GetConfig();
            return string.Format(LAUNCH_ARGUMENTS_FORMAT, config.Host, config.Language);
        }

        private static Config GetConfig()
        {
            if (!File.Exists(CONFIG_PATH))
                return GenerateNewConfig();

            
            string configJson = File.ReadAllText(CONFIG_PATH);
            Config? config = JsonSerializer.Deserialize<Config>(configJson);

            if (config == null)
                throw new Exception("Unable to parse Config.json");

            return config;
        }

        private static Config GenerateNewConfig()
        {
            var config = new Config();

            JsonSerializerOptions serializerOptions = new() { WriteIndented = true };
            string configJson = JsonSerializer.Serialize(config, serializerOptions);

            File.WriteAllText(CONFIG_PATH, configJson);

            return config;
        }

        private static bool LaunchExecutable(string executablePath, string executableArgs)
        {
            StartupInfo startupInfo = new();

            return CreateProcess(
                executablePath,
                executableArgs,
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                0,
                IntPtr.Zero,
                null,
                ref startupInfo,
                out _);
        }
    }
}