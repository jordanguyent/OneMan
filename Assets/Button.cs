using Godot;
using System;

public class Button : Area2D
{
    [Signal] public delegate void area_entered();

    [Signal] public delegate void toggle_on(int id);

    private int toggle = 0;

    private AnimatedSprite animatedSprite = null;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
       
        animatedSprite.Play("Up");

        //Connect("toggle_on", door, "ToggleDoor");
        Connect("area_entered", this, "Toggle");
    }

    private void Toggle(object area)
    {
        toggle = (toggle + 1) % 2;

        switch (toggle)
        {
            case 0:
                animatedSprite.Play("Up");
                //EmitSignal("toggle_on", BUTTONID);
                break;

            case 1:
                animatedSprite.Play("Down");
                //EmitSignal("toggle_on", BUTTONID);
                break;
        }
    }
}
