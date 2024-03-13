﻿using Microsoft.Extensions.DependencyInjection;

namespace Agent.Headless;

internal class Program
{

    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddRemoteControlServices();
        var provider = services.BuildServiceProvider();
        var result = await provider.UseHeadlessClient("http://localhost:81", "695468c9-93f7-47ea-8622-85495b2e04f9", "password");

        Console.WriteLine(result);

    }
}