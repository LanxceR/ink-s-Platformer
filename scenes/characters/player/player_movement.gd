extends CharacterBody2D

# Get the gravity from the project settings to be synced with RigidBody nodes.
# Defaults to 980.0f pixels per second (100px = 1m)
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

@export var _movement_data : MovementData
@export var _anim_sprite_2d : AnimatedSprite2D
@export var _coyote_timer : Timer

# By default, delta is 0.01666... (60 calc/sec)
func _physics_process(delta):
	_apply_gravity(delta)
	_handle_jump()
	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var input_axis : float = Input.get_axis("move_left", "move_right")
	_handle_acceleration_x(input_axis, delta)
	_handle_deceleration_x(input_axis, delta)
	_update_animation(input_axis)

	var was_on_floor : bool = is_on_floor()
	move_and_slide()
	_handle_coyote_time(was_on_floor)


func _apply_gravity(delta):
	if not is_on_floor():
		velocity.y += gravity * _movement_data.gravityScale * delta

func _handle_jump():
	if is_on_floor() or _coyote_timer.time_left > 0:
		if Input.is_action_just_pressed("jump"):
			velocity.y = _movement_data.jumpVelocity
	else: if not is_on_floor():
		if Input.is_action_just_released("jump") and velocity.y < 0:
			velocity.y = velocity.y / 2

func _handle_acceleration_x(input_axis : float, delta):
	if input_axis != 0:
		if is_on_floor():
			velocity.x = move_toward(velocity.x, input_axis * _movement_data.speed, _movement_data.groundAcceleration * delta)
		else:
			velocity.x = move_toward(velocity.x, input_axis * _movement_data.speed, _movement_data.airAcceleration * delta)

func _handle_deceleration_x(input_axis : float, delta):
	if sign(input_axis) != sign(velocity.x) && velocity.x != 0:
		if is_on_floor():
			velocity.x = move_toward(velocity.x, 0, _movement_data.groundDeceleration * delta)
		else:
			velocity.x = move_toward(velocity.x, 0, _movement_data.airDeceleration * delta)

func _update_animation(input_axis : float):
	if input_axis != 0:
		_anim_sprite_2d.Play("run")
		_anim_sprite_2d.flip_h = (input_axis < 0)
	else:
		_anim_sprite_2d.Play("idle")

	if not is_on_floor():
		_anim_sprite_2d.Play("jump")

func _handle_coyote_time(was_on_floor : bool):
	var just_left_ledge : bool = was_on_floor && !is_on_floor() && velocity.y >= 0
	if (just_left_ledge):
		_coyote_timer.Start();

func _print_position():
	print("Position = X: {x} | Y: {y}".format(["x",global_position.x, "y",global_position.y]))

func _print_velocity():
	print("Velocity = X: {x} | Y: {y}".format(["x",velocity.x, "y",velocity.y]))
