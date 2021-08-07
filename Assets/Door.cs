using Godot;
using System;

public class Door : KinematicBody2D
{
    private void OpenDoor(object area)
    {
        Visible = false;
        SetCollisionMaskBit(1, false);
    }
}
