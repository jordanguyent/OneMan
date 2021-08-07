using Godot;
using System;

public class Door : KinematicBody2D
{
    [Signal] public delegate void area_entered();

    [Export] int BUTTONID = 1;
    [Export] bool ISOPEN = false;

    private AnimatedSprite animatedSprite = null;
    private Button button = null;
    private int toggle = 0;

    public override void _Ready()
    {
        if (ISOPEN)
        {
            toggle = 2;
        }
        else
        {
            toggle = 0;
        }

        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        button = GetNode<Button>("../Button" + BUTTONID);

        button.Connect("area_entered", this, "ToggleDoor");
    }

    public override void _PhysicsProcess(float delta)
    {
        switch (toggle)
        {
            case 0:
                animatedSprite.Play("IdleClose");
                SetCollisionMaskBit(1, true);
                break;

            case 1:
                animatedSprite.Play("Open");
                if (animatedSprite.Frame > 6)
                {
                    toggle = (toggle + 1) % 4;
                }
                break;

            case 2:
                animatedSprite.Play("IdleOpen");
                SetCollisionMaskBit(1, false);
                break;

            case 3:
                animatedSprite.Play("Close");
                if (animatedSprite.Frame > 6)
                {
                    toggle = (toggle + 1) % 4;
                }
                break;
        }
    }

    private void ToggleDoor(object area)
    {
        toggle = (toggle + 1) % 4;
    }
}
