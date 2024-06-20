// Copyright 2024 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.Chat;

namespace Doofus;

public partial class DoofusEngine : IDoofusEngineBridge {
    bool init = false;

    string openAIKey;

    bool alwaysLoadSamplePlugins = false;
    Dictionary<string, IDoofusPlugin> plugins;

    internal bool verbose = false;
    public bool Verbose { get => verbose; }
    internal string? langOverride = null;
    public string Language { get => langOverride ?? CultureInfo.CurrentUICulture.Name; }

    public DoofusEngine(DoofusConfig config) {
        if (config.OpenAIKey == null) throw new InvalidOperationException("OpenAI API Key is not set! Please put your OpenAI API key as \"openai-key\" in \"config.json\" file.");
        openAIKey = config.OpenAIKey;
        if (config.Language != null) langOverride = config.Language;
        if (config.UseSamplePlugins != null) alwaysLoadSamplePlugins = config.UseSamplePlugins.Value;
        plugins = new();
    }

    public void Initialize() {
        LoadLanguage();
        new Thread(() => {
            // load plugins
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "plugins"));
            var dlls = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "plugins"), "*.dll", SearchOption.AllDirectories);
            foreach (var dll in dlls) {
                try {
                    var assembly = Assembly.LoadFile(dll);
                    var pluginsInAssembly = from type in assembly.GetTypes()
                            where type.IsClass &&
                                  type.GetInterfaces().Select(i => i is IDoofusPlugin).Any() &&
                                  !type.IsDefined(typeof(CompilerGeneratedAttribute))
                            select (IDoofusPlugin?)Activator.CreateInstance(type, this);
                    foreach (var plugin in pluginsInAssembly) if (plugin != null) {
                        plugin.Initialize(this);
                        plugins.Add(plugin.Id, plugin);
                        if (Verbose) Console.WriteLine($"Loaded plugin {plugin.Name}");
                    }
                } catch (Exception e) {
                    Console.WriteLine($"Error loading plugin {dll}: {e.Message}");
                    Environment.Exit(-1);
                }
            }
            if (plugins.Count == 0 || alwaysLoadSamplePlugins) {
                if (Verbose) Console.WriteLine($"Loading sample plugins");
                plugins.Add("general-question", new GeneralQuestionPlugin());
                plugins.Add("basic-calc", new BasicCalcPlugin());
                plugins.Add("volume-control", new VolumeControlPlugin());
                plugins.Add("youtube-search", new YouTubeSearchPlugin());
            }
            init = true;
        }).Start();
    }

    public async Task WaitForInit() {
        while (!init) {
            await Task.Delay(100);
        }
    }

    public async Task Run(string input) {
        StringBuilder sb = new();
        foreach (var plugin in plugins.Values) {
            sb.Append(string.Format(lang[StrPluginInfo], plugin.Name, plugin.Id, plugin.Description, plugin.Argument is null ? "null" : plugin.Argument));
        }
        var dingus = await AskAIAsync(
            lang[StrPluginPick] + 
            sb.ToString(),
            [ input ],
            json: true
        );
        JsonDocument jd = JsonDocument.Parse(dingus);
        var pluginId = jd.RootElement.GetProperty("id").GetString();
        var pluginArgument = jd.RootElement.GetProperty("argument").GetString();
        if (pluginId != null && plugins.ContainsKey(pluginId)) {
            if (Verbose) Write($"Calling plugin {plugins[pluginId].Id}...");
            await plugins[pluginId].DoActionAsync(this, pluginArgument);
        } else {
            Write(lang[StrErrNoPlugin]);
        }
    }

    public async Task<string> AskAIAsync(string systemPrompt, string[]? userPrompts, bool json = false) {
        ChatClient client = new ChatClient("gpt-4o", new System.ClientModel.ApiKeyCredential(openAIKey));
        List<ChatMessage> messages = [ new SystemChatMessage(systemPrompt) ];
        if (userPrompts is not null) foreach (var userPrompt in userPrompts) messages.Add(new UserChatMessage(userPrompt));
        ChatCompletion completion = await client.CompleteChatAsync(messages, new() {
            ResponseFormat = json ? ChatResponseFormat.JsonObject : ChatResponseFormat.Text
        });
        return completion.ToString();
    }

    public void Write(string text, bool lineBreak = true) {
        Console.Write(text);
        if (lineBreak) Console.WriteLine();
    }
}