using Godot;
using System;

public class Ladder : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Signal] public delegate void area_entered();
    [Signal] public delegate void area_exited();

    private Player player = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        player = GetNode<Player>("/root/World/Player");

        Connect("area_entered", player, "OnEnterLadder");
        Connect("area_exited", player, "OnExitLadder");

    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
