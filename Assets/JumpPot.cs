using Godot;
using System;

public class JumpPot : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Signal] public delegate void area_entered();

    private Player player = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        player = GetNode<Player>("/root/World/Player");

        Connect("area_entered", player, "AddJump");

        Connect("area_entered", this, "Die");
    }

    private void Die(object param) {
        QueueFree();
    }
}
