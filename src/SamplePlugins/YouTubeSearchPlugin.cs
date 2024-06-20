// Copyright 2024 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Doofus;

public class YouTubeSearchPlugin : IDoofusPlugin {
    public string Id => id;
    public string Name => name;
    public string Description => description;
    public string? Argument => argument;

    private string id = "youtube-search";
    private string name = "YouTube Search";
    private string description = "Opens a web browser and searches for videos on YouTube.";
    private string? argument = "Keyword to search";

    public void Initialize(IDoofusEngineBridge engine) {
        if (engine.Language == "ko-KR") {
            name = "YouTube 검색";
            description = "웹 브라우저를 열어 YouTube에서 동영상을 검색합니다.";
            argument = "검색할 키워드";
        }
    }

    public async Task DoActionAsync(IDoofusEngineBridge engine, string? argument) {
        await Task.Delay(1);
        Process.Start(new ProcessStartInfo() {
            FileName = $"\"https://www.youtube.com/results?search_query={WebUtility.HtmlEncode(argument!)}\"",
            UseShellExecute = true
        });
    }
}