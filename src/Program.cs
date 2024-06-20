// Copyright 2024 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Doofus;

class Program {
    static async Task Main(string[] args) {
        var configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
        if (!File.Exists(configPath)) File.Create(configPath).Close();
        var configFile = await File.ReadAllTextAsync(configPath);
        DoofusConfig? config;
        try { config = JsonSerializer.Deserialize<DoofusConfig>(configFile)!; } catch (Exception) { config = new DoofusConfig(); }
        DoofusEngine engine = new(config);

        StringBuilder sb = new();
        bool flagsDone = false;
        for (int i = 0; i < args.Length; i++) {
            if (flagsDone) {
                sb.Append(args[i]).Append(' ');
            } else {
                if (args[i] == "-v" || args[i] == "--verbose") {
                    engine.verbose = true;
                } else if (args[i] == "-l" || args[i] == "--language") {
                    engine.langOverride = args[++i];
                } else {
                    flagsDone = true;
                    sb.Append(args[i]).Append(' ');
                }
            }
        }
        if (sb.Length == 0) {
            Console.Write("? ");
            sb.Append(Console.ReadLine()!);
        } else {
            sb.Length = sb.Length - 1;
        }

        engine.Initialize();
        await engine.WaitForInit();
        await engine.Run(sb.ToString());
    }
}