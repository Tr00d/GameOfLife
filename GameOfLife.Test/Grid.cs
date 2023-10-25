using System.Drawing;

namespace GameOfLife.Test;

public readonly struct Grid
{
	private readonly Cell[] cells;
	private readonly int height;
	private readonly int width;

	private Grid(int height, int width, IEnumerable<Cell> cells)
	{
		this.height = height;
		this.width = width;
		this.cells = cells.ToArray();
	}

	public IEnumerable<Cell> OrderedCells =>
		this.cells.OrderBy(cell => cell.Position.X).ThenBy(cell => cell.Position.Y);

	public static Grid Initialize(int height, int width) => InitializeEmptyGrid(height, width);

	public Grid Next() => new(this.height, this.width, this.OrderedCells.Select(this.UpdateState));

	public Grid WithLivingCell(Point position)
	{
		var deadCells = this.OrderedCells.Where(cell => cell.Position != position).ToList();
		deadCells.Add(Cell.Alive(position));
		return new Grid(this.height, this.width, deadCells);
	}

	public Grid WithLivingCells(params Point[] positions)
	{
		var deadCells = this.OrderedCells.Where(cell => !positions.Contains(cell.Position)).ToList();
		positions.ToList().ForEach(position => deadCells.Add(Cell.Alive(position)));
		return new Grid(this.height, this.width, deadCells);
	}

	private int FindLivingNeighbours(Point position) =>
		this.cells.Count(cell => cell.IsAlive && cell.IsNeighbour(position));

	private static Grid InitializeEmptyGrid(int height, int width)
	{
		var deadCells = new List<Cell>();
		for (var widthIndex = 0; widthIndex < width; widthIndex++)
		{
			for (var heightIndex = 0; heightIndex < height; heightIndex++)
			{
				deadCells.Add(Cell.Dead(new Point(widthIndex, heightIndex)));
			}
		}

		return new Grid(height, width, deadCells);
	}

	private Cell UpdateState(Cell cell)
	{
		var livingNeighbours = this.FindLivingNeighbours(cell.Position);
		return !cell.IsAlive & (livingNeighbours == 3)
		       || cell.IsAlive & (livingNeighbours == 2)
		       || cell.IsAlive & (livingNeighbours == 3)
			? Cell.Alive(cell.Position)
			: Cell.Dead(cell.Position);
	}
}