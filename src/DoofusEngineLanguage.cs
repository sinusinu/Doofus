// Copyright 2024 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System.Collections.Generic;

namespace Doofus;

public partial class DoofusEngine : IDoofusEngineBridge {
    private Dictionary<string, string> lang = new();

    private const string StrPluginPick = "plugin-pick";
    private const string StrPluginInfo = "plugin-info";
    private const string StrErrNoPlugin = "err-no-plugin";

    private void LoadLanguage() {
        if (Language == "ko-KR") {
            lang.Add(StrPluginPick,
                "당신은 플러그인 기반의 어시스턴트입니다. 아래에 사용 가능한 플러그인 목록이 주어지며, 당신은 그 중에서 사용자의 입력을 처리하기 가장 적절한 플러그인을 선택해야 합니다. 적절한 플러그인을 선택하여 해당 플러그인의 ID(필요한 경우 매개 변수도)를 JSON 형식으로 반환하십시오.\n\n" +
                "# JSON 스키마\n```\n{ \"id\": string, \"argument\": string }\n```\n- id: 선택한 플러그인의 ID, 적절한 플러그인이 없을 경우 null\n- argument: 플러그인에 전달할 매개 변수, 필요 없을 경우 null\n\n" +
                "# 플러그인"
            );
            lang.Add(StrPluginInfo, "# {0}\n- ID: {1}\n- 설명: {2}\n- 매개 변수: {3}\n\n");
            lang.Add(StrErrNoPlugin, "이 명령을 수행할 수 있는 플러그인이 없습니다.");
        } else { // default
            lang.Add(StrPluginPick,
                "You are a helpful, plugin-based assistant. Below is a list of plugins you can use. You need to select the most appropriate plugin to handle user input. Select the plugin and return its ID (and argument, if necessary) as the JSON payload.\n\n" +
                "# JSON schema\n```\n{ \"id\": string, \"argument\": string }\n```\n- id: ID of the selected plugin, null if no suitable plugin is found\n- argument: Argument to pass to plugin, null if not needed\n\n" +
                "# Plugins"
            );
            lang.Add(StrPluginInfo, "# {0}\n- ID: {1}\n- Description: {2}\n- Argument: {3}\n\n");
            lang.Add(StrErrNoPlugin, "No suitable plugin is installed for this request.");
        }
    }
}