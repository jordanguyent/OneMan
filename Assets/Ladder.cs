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

    [Export] int GRAVITY = 500;
    [Export] int SPEEDYMAX = 200;

    private Player player = null;

    private Area2D area2D = null;
    private Vector2 gravVec = new Vector2();

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
        gravVec = gravVec.MoveToward(new Vector2(0, SPEEDYMAX), GRAVITY * delta);
        gravVec = MoveAndSlide(gravVec);
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
