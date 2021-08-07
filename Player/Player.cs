using Godot;
using System;

public class Player : KinematicBody2D
{
    // Player States
    enum PlayerState
    {
        Climb,
        Death,
        Init,
        Jump,
        Move,
        Push
    }

    // Universal Constants
	private Vector2 E1 = new Vector2(1, 0);
	private Vector2 E2 = new Vector2(0, -1);

    // Nodes
    private PackedScene projectile = null;
    private RayCast2D floorRayLeft = null;
    private RayCast2D floorRayRight = null;
    
    // Player Constants
    [Export] int ACCELERATION = 1000;
    [Export] int CLIMBSPEED = 50;
    [Export] int GRAVITY = 500;
    [Export] int JUMPMAGNITUDE = 150;
    [Export] int INPUTBUFFERFRAMES = 5;
    [Export] int SPEEDXMAX = 50;
    [Export] int SPEEDYMAX = 200;

    // Player Variables
    private PlayerState state = PlayerState.Init;
    private Vector2 spawnPos = new Vector2();
    private Vector2 velocity = new Vector2();
    private bool justPressedJump = false;
    private float inputPush = 0;
    private bool justPressedShoot = false;
    private float inputDirX = 0;
    private float inputDirY = 0;
    private int balls = 1;
    private int jumps = 1;
    private int jumpBufferFrame = 0;

    private KinematicBody2D curLadder = null;

    public override void _Ready()
    {
        projectile = GD.Load<PackedScene>("res://Player/Ball.tscn");

        floorRayLeft = GetNode<RayCast2D>("FloorRayLeft");
        floorRayRight = GetNode<RayCast2D>("FloorRayRight");
    }

    public override void _PhysicsProcess(float delta)
    {
        // Action
        // Animation
        // Transition

        switch(state)
        {
            case PlayerState.Climb:
                UpdateInputs();
                HandleLadderStates();
                UpdateVelocityX(delta);
                UpdateVelocityClimb(delta);

                velocity = MoveAndSlide(velocity, E2);

                break;

            case PlayerState.Death:
                GetTree().ReloadCurrentScene();
                break;

            case PlayerState.Init:
                state = PlayerState.Move;
                break;

            case PlayerState.Jump:
                UpdateInputs();
                UpdateVelocityX(delta);
                UpdateVelocityGravity(delta);
                UpdateVelocityJump(delta);
                HandleEffectCollision();

                velocity = MoveAndSlide(velocity, E2);

                if (RayIsOnFloor())
                {  
                    state = PlayerState.Move;
                }
                break;

            case PlayerState.Move:
                UpdateInputs();
                UpdateVelocityX(delta);
                UpdateVelocityGravity(delta);
                HandleEffectCollision();

                velocity = MoveAndSlide(velocity, E2);

                if (balls > 0 && justPressedShoot)
                {
                    SpawnBall();
                }

                if (RayIsOnFloor() && justPressedJump && jumps > 0)
                {
                    state = PlayerState.Jump;
                    jumps--;
                    GD.Print("Jumps: " + jumps);
                }
                break;
            
            case PlayerState.Push:
                UpdateInputs();
                HandleLadderStates();
                UpdateVelocityX(delta);
                UpdateVelocityGravity(delta);
                UpdateVelocityLadder(delta);
                HandleEffectCollision();

                velocity = MoveAndSlide(velocity, E2);

                break;
        }

        Position = Position.Snapped(Vector2.One);
    }

    private void BufferJustPressedInput(ref bool selfBool, ref int inputBufferFrames, String keypress, bool condition)
    {
        if (Input.IsActionJustPressed(keypress))
        {
            selfBool = true;
        }

        if (selfBool && inputBufferFrames < INPUTBUFFERFRAMES)
        {
            inputBufferFrames++;
            // Checks if buffered input has been used
            if (condition)
            {
                inputBufferFrames = 0;
                selfBool = false;
            }
        }
        else
        {
            inputBufferFrames = 0;
            selfBool = false;
        }
    }

    private Vector2 GetDirectionBetweenMouseAndPlayer() {
		float disX = GetGlobalMousePosition().x - GlobalPosition.x;
		float disY =  GetGlobalMousePosition().y - GlobalPosition.y;
		return new Vector2(disX, disY);
	}

    private void HandleEffectCollision()
    {
        int collisionCount = GetSlideCount();

        for(int i = 0; i < collisionCount; i++)
        {
            KinematicCollision2D currentCollision = GetSlideCollision(i);
            Godot.Object collidedWith = currentCollision.Collider;
            if ((collidedWith as TMDanger) != null)
            {
                state = PlayerState.Death;
            }
        }
    }

    // Check whether player is climbing/moving
    private void HandleLadderStates() {
        if (inputPush != 0 && RayIsOnFloor()) {
            state = PlayerState.Push;
        } else {
            state = PlayerState.Climb;
        }
    }


    private float HelperMoveToward(float current, float desire, float acceleration)
	{
		return (E1 * current).MoveToward(E1 * desire, acceleration).x;
	}

    private void SpawnBall()
    {
        Ball ball = (Ball) projectile.Instance();
        ball.dir = GetDirectionBetweenMouseAndPlayer();
        ball.Position = Position;
        GetParent().AddChild(ball);
        balls--;
    }

    private bool RayIsOnFloor()
    {
        return floorRayRight.IsColliding() || floorRayLeft.IsColliding();
    }

    private void UpdateInputs()
    {
        inputDirX = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        inputDirY = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        inputPush = Input.GetActionStrength("ui_push");
        justPressedShoot = Input.IsActionJustPressed("ui_shoot");
        BufferJustPressedInput(ref justPressedJump, ref jumpBufferFrame, "ui_jump", state == PlayerState.Jump);
    }

    private void UpdateVelocityX(float delta)
    {
        velocity.x = HelperMoveToward(velocity.x, inputDirX * SPEEDXMAX, ACCELERATION * delta);
    }

    private void UpdateVelocityClimb(float delta) {
        velocity.y = HelperMoveToward(velocity.y, inputDirY * CLIMBSPEED, ACCELERATION * delta);
    }

    private void UpdateVelocityJump(float delta) {
        velocity.y -= JUMPMAGNITUDE;
    }

    private void UpdateVelocityGravity(float delta) {
        velocity.y = HelperMoveToward(velocity.y, SPEEDYMAX, GRAVITY * delta);
    }

    private void UpdateVelocityLadder(float delta) {
        if (curLadder != null) {
            Vector2 ladderVel = velocity;
            ladderVel.y = 0;
            curLadder.MoveAndSlide(ladderVel);
        }
    }
    
    // SIGNALS =======================================================================================================

    private void AddBall(object area)
    {
        balls++;
        GD.Print("ADDED BALL");
        GD.Print("Balls: " + balls);
    }

    private void AddJump(object area) 
    {
        jumps++;
        // DEBUG
        GD.Print("ADDED JUMP");
        GD.Print("Jumps: " + jumps);
    }

    private void OnEnterLadder(KinematicBody2D ladder) {
        HandleLadderStates();
        curLadder = ladder;
        GD.Print(curLadder.GetChildCount());
        GD.Print("Entered Ladder");
    }

    private void OnExitLadder(Area2D param) {
        state = PlayerState.Move;
        curLadder = null;
        velocity.y = 0;
        GD.Print("Exited Ladder");
    }
}
