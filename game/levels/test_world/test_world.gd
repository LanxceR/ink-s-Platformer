extends Node

@export var _level_completed_screen : Polygon2D

# Called when the node enters the scene tree for the first time.
func _ready():	
	print("Hello GDScript!")

	# Set clear background to black
	RenderingServer.set_default_clear_color(Color.BLACK)
	
	# Connect to Events autoload signal for level complete
	Events.LevelCompleted.emit(_on_level_completed)

func _on_level_completed():
	_level_completed_screen.show()
	get_tree().paused = true
