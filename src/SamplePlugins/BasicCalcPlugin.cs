// Copyright 2024 Woohyun Shin (sinusinu)
// SPDX-License-Identifier: GPL-3.0-only

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doofus;

public class BasicCalcPlugin : IDoofusPlugin {
    public string Id => id;
    public string Name => name;
    public string Description => description;
    public string? Argument => argument;

    private string id = "basic-calc";
    private string name = "Basic Calculator";
    private string description = "Basic calculator that can perform addition, subtraction, multiplication, and division.";
    private string? argument = "Postfix notation of the problem (e.g. 3 4 +). Use symbols + - * / for operands.";

    public void Initialize(IDoofusEngineBridge engine) {
        if (engine.Language == "ko-KR") {
            name = "간단한 계산기";
            description = "덧셈, 뺄셈, 곱셈, 나눗셈을 할 수 있는 간단한 계산기입니다.";
            argument = "Postfix notation 형식의 해결해야 할 문제 (예: 3 4 +), 연산자 기호는 + - * /를 사용할 것";
        }
    }

    public async Task DoActionAsync(IDoofusEngineBridge engine, string? argument) {
        await Task.Delay(1);
        if (engine.Verbose) engine.Write($"Solving: {argument!}");
        var tokens = argument!.Split(new char[0], System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);
        var stack = new Stack<long>();
        long i1, i2;
        try {
            foreach (var token in tokens) {
                switch (token) {
                    case "+": i1 = stack.Pop(); i2 = stack.Pop(); stack.Push(i1 + i2); break;
                    case "-": i1 = stack.Pop(); i2 = stack.Pop(); stack.Push(i1 - i2); break;
                    case "*": i1 = stack.Pop(); i2 = stack.Pop(); stack.Push(i1 * i2); break;
                    case "/": i1 = stack.Pop(); i2 = stack.Pop(); stack.Push(i1 / i2); break;
                    default: stack.Push(long.Parse(token)); break;
                }
            }
            i1 = stack.Pop();
            var outputMessage = "The answer is {0}.";
            if (engine.Language == "ko-KR") outputMessage = "답은 {0}입니다.";
            engine.Write(string.Format(outputMessage, i1));
        } catch (InvalidOperationException) {
            var outputMessage = "An error has occurred while solving the problem.";
            if (engine.Language == "ko-KR") outputMessage = "계산 중 오류가 발생하였습니다.";
            engine.Write(outputMessage);
        }
    }
}