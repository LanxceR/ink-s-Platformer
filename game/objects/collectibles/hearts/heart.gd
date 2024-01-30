extends Area2D


# Called when the node enters the scene tree for the first time.
func _ready():
	body_entered.connect(_on_body_entered)

func _on_body_entered(body):
	queue_free()

	var hearts = get_tree().get_nodes_in_group("Hearts")
	if (hearts.size() <= 1):
		# This is the last heart
		print("Level Completed")
		Events.LevelCompleted.emit()
