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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WildCore.Common.Hosting
{
    public static class HostBuilderExtensions
    {
        private const string ConfigureServicesMethodName = "ConfigureServices";

        public static IHostBuilder UseStartup<TStartup>(
        this IHostBuilder hostBuilder) where TStartup : class
        {
            hostBuilder.ConfigureServices((context, serviceCollection) =>
            {
                var cfgServicesMethod = typeof(TStartup).GetMethod(
                    ConfigureServicesMethodName, new Type[] { typeof(IServiceCollection) });

                var hasConfigCtor = typeof(TStartup).GetConstructor(
                    new Type[] { typeof(IConfiguration) }) != null;

                TStartup? startUpObj = (TStartup?)(hasConfigCtor ?
                    Activator.CreateInstance(typeof(TStartup), context.Configuration) :
                    Activator.CreateInstance(typeof(TStartup), null));

                cfgServicesMethod?.Invoke(startUpObj, new object[] { serviceCollection });
            });

            return hostBuilder;
        }
    }
}
