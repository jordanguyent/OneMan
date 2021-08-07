using Godot;
using System;

public class MainMenu : Node2D
{
    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionJustPressed("ui_accept"))
        {
            GetTree().ChangeScene("res://Levels/Level1.tscn");
        }

        if (Input.IsActionJustPressed("ui_cancel"))
        {
            GetTree().Quit();
        }
    }
}
