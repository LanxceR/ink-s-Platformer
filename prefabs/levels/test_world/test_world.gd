extends Node

# @onready var collision_polygon_2d = $StaticBody2D/CollisionPolygon2D
@export var _col_poly_2d : CollisionPolygon2D
# @onready var _polygon_2d = $StaticBody2D/CollisionPolygon2D/Polygon2D
@export var _polygon_2d : Polygon2D

# Called when the node enters the scene tree for the first time.
func _ready():	
	print("Hello GDScript!")

	# Set clear background to black
	RenderingServer.set_default_clear_color(Color.BLACK)

	# Set _polygon_2d vertices the same with _col_poly_2d vertices
	_polygon_2d.polygon = _col_poly_2d.polygon
