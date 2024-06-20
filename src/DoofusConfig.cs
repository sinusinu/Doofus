// Copyright 2024 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System.Text.Json.Serialization;

namespace Doofus;

public class DoofusConfig {
    [JsonPropertyName("openai-key")]
    public string? OpenAIKey { get; set; }
    [JsonPropertyName("use-sample-plugins")]
    public bool? UseSamplePlugins { get; set; }
    [JsonPropertyName("lang")]
    public string? Language { get; set; }
}