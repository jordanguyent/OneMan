using Godot;
using System;

public class Ladder : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Signal] public delegate void pass_self(KinematicBody2D self);
    [Signal] public delegate void area_entered();
    [Signal] public delegate void area_exited();

    private Player player = null;

    private Area2D area2D = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        player = GetNode<Player>("/root/World/Player");

        area2D = GetNode<Area2D>("Area2D");

        area2D.Connect("area_entered", this, "OnAreaEntered");
        area2D.Connect("area_exited", player, "OnExitLadder");

        Connect("pass_self", player, "OnEnterLadder");
    }

    public override void _PhysicsProcess(float delta) {
        this.Position = Position.Snapped(Vector2.One);
    }

    private void OnAreaEntered(object param) {
        EmitSignal("pass_self", this);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
