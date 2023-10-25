using System.Drawing;

namespace GameOfLife.Test;

public record Cell(Point Position, bool IsAlive)
{
	public static Cell Alive(Point position) => new(position, true);
	public static Cell Dead(Point position) => new(position, false);

	public bool IsNeighbour(Point position) => this.Position != position
	                                           && this.Position.X <= position.X + 1
	                                           && this.Position.X >= position.X - 1
	                                           && this.Position.Y <= position.Y + 1
	                                           && this.Position.Y >= position.Y - 1;
}