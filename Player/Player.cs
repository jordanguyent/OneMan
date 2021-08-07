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
        Move
    }

    // Universal Constants
	private Vector2 E1 = new Vector2(1, 0);
	private Vector2 E2 = new Vector2(0, -1);

    // Nodes
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
    private float inputDirX = 0;
    private float inputDirY = 0;
    private int jumps = 1;
    private int jumpBufferFrame = 0;


    public override void _Ready()
    {
        floorRayLeft = GetNode<RayCast2D>("FloorRayLeft");
        floorRayRight = GetNode<RayCast2D>("FloorRayRight");

        InitializeVariables();
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
                UpdateVelocityX(delta);
                UpdateVelocityClimb(delta);

                velocity = MoveAndSlide(velocity, E2);

                break;

            case PlayerState.Death:
                ResetVariables();

                Position = spawnPos;
                state = PlayerState.Init;
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

                if (RayIsOnFloor() && justPressedJump && jumps > 0)
                {
                    state = PlayerState.Jump;
                    jumps--;
                    GD.Print("Jumps: " + jumps);
                }
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


    private float HelperMoveToward(float current, float desire, float acceleration)
	{
		return (E1 * current).MoveToward(E1 * desire, acceleration).x;
	}

    private void InitializeVariables()
    {
    }

    private bool RayIsOnFloor()
    {
        return floorRayRight.IsColliding() || floorRayLeft.IsColliding();
    }

    private void ResetVariables()
    {

    } 

    private void UpdateInputs()
    {
        inputDirX = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        inputDirY = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
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

    // SIGNALS

    private void AddJump(object area) {
        jumps++;
        // DEBUG
        GD.Print("ADDED JUMP");
        GD.Print("Jumps: " + jumps);
    }

    private void OnEnterLadder(object param) {
        state = PlayerState.Climb;
        GD.Print("Entered Ladder");
    }

    private void OnExitLadder(object param) {
        state = PlayerState.Move;
        velocity.y = 0;
        GD.Print("Exited Ladder");
    }

    private void UpdateCheckpoint(Vector2 sp)
    {
        spawnPos = sp;
    }
}
