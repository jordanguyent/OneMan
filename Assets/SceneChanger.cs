using Godot;
using System;

public class SceneChanger : Area2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    [Export] public String SCENE_NAME = "";

    [Signal] public delegate void area_entered();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect("area_entered", this, "ChangeScene");
    }

    private void ChangeScene(object param) {
        GetTree().ChangeScene("res://Levels/" + SCENE_NAME);
    }



//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
