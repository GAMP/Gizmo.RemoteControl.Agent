﻿using Gizmo.RemoteControl.Agent.Shared.Enums;
using Gizmo.RemoteControl.Agent.Shared.Services;

namespace Gizmo.RemoteControl.Agent.Shared.Abstractions;

public interface IKeyboardMouseInput
{
    void Init();
    void SendKeyDown(string key);
    void SendKeyUp(string key);
    void SendMouseMove(double percentX, double percentY, IViewer viewer);
    void SendMouseWheel(int deltaY);
    void SendText(string transferText);
    void ToggleBlockInput(bool toggleOn);
    void SetKeyStatesUp();
    void SendMouseButtonAction(int button, ButtonAction buttonAction, double percentX, double percentY, IViewer viewer);
}
