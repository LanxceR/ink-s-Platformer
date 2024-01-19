using Godot;
using System;

public partial class TestWorld : Node
{
    [Export]
    private Polygon2D _polygon_2d;

    [Export]
    private CollisionPolygon2D _col_poly_2d;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Hello C#!");

        // Set clear background to black
        RenderingServer.SetDefaultClearColor(new Color(0, 0, 0, 1));

        // Set polygon_2d vertices the same with col_poly_2d vertices
        _polygon_2d.Polygon = _col_poly_2d.Polygon;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}
