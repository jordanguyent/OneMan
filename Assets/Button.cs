using Godot;
using System;

public class Button : Area2D
{
    [Signal] public delegate void area_entered();

    [Export] int DOORID = 1;

    private Door door = null;
    private ColorRect rect = null;

    public override void _Ready()
    {
        door = GetNode<Door>("../Door" + DOORID);
        rect = GetNode<ColorRect>("ColorRect");

        Connect("area_entered", door, "OpenDoor");
        Connect("area_entered", this, "ButtonDown");
    }

    private void ButtonDown(object area)
    {
        rect.Modulate = new Color(1, 0, 0, 1);
    }
}
