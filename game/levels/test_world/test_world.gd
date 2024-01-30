extends Node

@export var _level_completed_screen : Control
@export var _next_level_scene : PackedScene

# Called when the node enters the scene tree for the first time.
func _ready():	
	print("Hello GDScript!")

	# Set clear background to black
	RenderingServer.set_default_clear_color(Color.BLACK)
	
	# Connect to Events autoload signal for level complete
	Events.LevelCompleted.connect(_on_level_completed)

# DOESN'T WORK (INTERACTION WITH C# IS KINDA BROKEN)
func _on_level_completed():
	_level_completed_screen.show()

	if (!_next_level_scene is PackedScene):
		return

	get_tree().paused = true
	await LevelTransition.FadeToBlack()
	get_tree().paused = false
	
	get_tree().change_scene_to_packed(_next_level_scene)
	await LevelTransition.FadeFromBlack()
