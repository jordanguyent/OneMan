using Godot;
using System;

public class Ball : KinematicBody2D
{
    [Signal] public delegate void area_entered();

    enum BallState
    {
        Active,
        Idle,
        Init
    }

    private Vector2 E1 = new Vector2(1, 0);

    private Player player = null;
    private Area2D hitBox = null;
    private AudioStreamPlayer sfx = null;

    private const int ACCELERATION = 500;
    private const int GRAVITY = 500;
    private const int SPEEDYMAX = 200;

    private BallState state = BallState.Init;
    private Vector2 velocity = new Vector2();
    public Vector2 dir = new Vector2();
    private float speed = 100;
    private int initTimer = 10;
    private int bounces = 2;

    public override void _Ready()
    {
        player = GetNode<Player>("/root/World/Player");
        hitBox = GetNode<Area2D>("HitBox");
        sfx = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

        hitBox.Connect("area_entered", this, "Delete");
        hitBox.Connect("area_entered", player, "AddBall");

        sfx.Play();
    }

    public override void _PhysicsProcess(float delta)
    {
        switch(state)
        {
            case BallState.Active:
                hitBox.SetCollisionMaskBit(1, true);
                MoveBall();
                velocity = MoveAndSlide(velocity, Vector2.Up);
                HandleBodyCollision();

                if (bounces <= 0)
                {
                    state = BallState.Idle;
                }
                break;
            
            case BallState.Idle:
                UpdateVelocityX(delta);
                UpdateVelocityGravity(delta);
                velocity = MoveAndSlide(velocity, Vector2.Up);
                break;

            case BallState.Init:
                MoveBall();
                velocity = MoveAndSlide(velocity, Vector2.Up);
                HandleBodyCollision();

                initTimer--;

                if (initTimer <= 0)
                {
                    state = BallState.Active;
                }
                break;
        }
    }

    private void HandleBodyCollision()
    {
        int collisionCount = GetSlideCount();

        if (collisionCount > 0)
        {
            sfx.Play();
            var currentCollision = GetSlideCollision(0);
            if (currentCollision != null)
            {
                if (currentCollision.Normal == Vector2.Right || currentCollision.Normal == Vector2.Left)
                {
                    dir.x = -dir.x;
                }
                else if (currentCollision.Normal == Vector2.Up || currentCollision.Normal == Vector2.Down)
                {
                    dir.y = -dir.y;
                }
                bounces--;
                GD.Print("Ball bounces: " + bounces);
            }            
        }
    }

    private float HelperMoveToward(float current, float desire, float acceleration)
	{
		return (E1 * current).MoveToward(E1 * desire, acceleration).x;
	}

    private void MoveBall() {
        velocity = dir.Normalized() * speed;
	}

    private void UpdateVelocityX(float delta)
    {
        velocity.x = HelperMoveToward(velocity.x, 0, ACCELERATION * delta);
    }

    private void UpdateVelocityGravity(float delta) {
        velocity.y = HelperMoveToward(velocity.y, SPEEDYMAX, GRAVITY * delta);
    }

    // Signals

    private void Delete(Area2D area)
    {
        // create area
        if (area.Name == "EffectBox")
        {
            QueueFree();
        }
    }
}
