// Copyright 2024 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System.Threading.Tasks;

namespace Doofus;

public class GeneralQuestionPlugin : IDoofusPlugin {
    public string Id => id;
    public string Name => name;
    public string Description => description;
    public string? Argument => argument;

    private string id = "general-question";
    private string name = "General Question";
    private string description = "Provides answers to general questions.";
    private string? argument = "Question given by user";

    public void Initialize(IDoofusEngineBridge engine) {
        if (engine.Language == "ko-KR") {
            name = "질문에 대답";
            description = "질문에 대한 대답을 제공합니다.";
            argument = "사용자의 질문";
        }
    }

    public async Task DoActionAsync(IDoofusEngineBridge engine, string? argument) {
        var prompt = "You are a helpful assistant. User is asking a question. Write an appropriate response to the question. The answer must be short and to the point. Do not use markdown or rich text formatting, write in plain text.";
        if (engine.Language == "ko-KR") prompt = "당신은 친절한 어시스턴트입니다. 방금 사용자가 당신에게 질문을 건넸습니다. 사용자의 질문에 적절한 대답을 출력하십시오. 대답은 짧고 간략해야 합니다. 대답은 한국어로 출력해야 합니다. 마크다운 등의 포맷을 사용하지 말고, 평문으로만 출력하십시오.";
        var response = await engine.AskAIAsync(prompt, [ argument! ]);
        engine.Write(response);
    }
}