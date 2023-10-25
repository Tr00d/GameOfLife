using System.Drawing;
using FluentAssertions;

namespace GameOfLife.Test;

/// <remarks>
///     1. Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
///     2. Any live cell with more than three live neighbours dies, as if by overcrowding.
///     3. Any live cell with two or three live neighbours lives on to the next generation.
///     4. Any dead cell with exactly three live neighbours becomes a live cell.
/// </remarks>
public class GridTest
{
	public static IEnumerable<object[]> MoreThanThreeLiveNeighbours =>
		new List<Cell[]>
		{
			new[]
			{
				Cell.Alive(new Point(1, 1)), Cell.Alive(new Point(0, 0)), Cell.Alive(new Point(0, 2)),
				Cell.Alive(new Point(2, 0)), Cell.Alive(new Point(2, 2)),
			},
			new[]
			{
				Cell.Alive(new Point(1, 1)), Cell.Alive(new Point(0, 0)), Cell.Alive(new Point(0, 2)),
				Cell.Alive(new Point(2, 0)), Cell.Alive(new Point(2, 2)), Cell.Alive(new Point(0, 1)),
			},
			new[]
			{
				Cell.Alive(new Point(1, 1)), Cell.Alive(new Point(0, 0)), Cell.Alive(new Point(0, 2)),
				Cell.Alive(new Point(2, 0)), Cell.Alive(new Point(2, 2)), Cell.Alive(new Point(0, 1)),
				Cell.Alive(new Point(0, 1)),
			},
			new[]
			{
				Cell.Alive(new Point(1, 1)), Cell.Alive(new Point(0, 0)), Cell.Alive(new Point(0, 2)),
				Cell.Alive(new Point(2, 0)), Cell.Alive(new Point(2, 2)), Cell.Alive(new Point(0, 1)),
				Cell.Alive(new Point(0, 1)), Cell.Alive(new Point(1, 2)),
			},
			new[]
			{
				Cell.Alive(new Point(1, 1)), Cell.Alive(new Point(0, 0)), Cell.Alive(new Point(0, 2)),
				Cell.Alive(new Point(2, 0)), Cell.Alive(new Point(2, 2)), Cell.Alive(new Point(0, 1)),
				Cell.Alive(new Point(0, 1)), Cell.Alive(new Point(1, 2)), Cell.Alive(new Point(2, 1)),
			},
		};

	[Fact]
	public void CellBecomesAlive_GivenThreeLivingNeighbours() =>
		Grid.Initialize(3, 3)
			.WithLivingCell(new Point(0, 0))
			.WithLivingCell(new Point(0, 2))
			.WithLivingCell(new Point(2, 2))
			.Next()
			.OrderedCells
			.Should().Contain(Cell.Alive(new Point(1, 1)));

	[Theory]
	[MemberData(nameof(MoreThanThreeLiveNeighbours))]
	public void CellDies_GivenMoreThanThreeLivingNeighbours(params Cell[] neighbours) => Grid.Initialize(3, 3)
		.WithLivingCells(neighbours.Select(neighbour => neighbour.Position).ToArray()).Next().OrderedCells.Should()
		.Contain(Cell.Dead(new Point(1, 1)));

	[Fact]
	public void CellDies_GivenNoLivingNeighbour()
	{
		var updatedCells = Grid.Initialize(3, 2)
			.WithLivingCell(new Point(0, 2))
			.WithLivingCell(new Point(1, 0))
			.Next()
			.OrderedCells
			.ToArray();
		updatedCells.Should().Contain(Cell.Dead(new Point(0, 2)));
		updatedCells.Should().Contain(Cell.Dead(new Point(1, 0)));
	}

	[Fact]
	public void CellDies_GivenOneLivingNeighbour()
	{
		var updatedCells = Grid.Initialize(3, 3)
			.WithLivingCell(new Point(0, 2))
			.WithLivingCell(new Point(1, 2))
			.Next()
			.OrderedCells
			.ToArray();
		updatedCells.Should().Contain(Cell.Dead(new Point(0, 2)));
		updatedCells.Should().Contain(Cell.Dead(new Point(1, 2)));
	}

	[Fact]
	public void CellRemainsAlive_GivenThreeLivingNeighbours() =>
		Grid.Initialize(3, 3)
			.WithLivingCell(new Point(1, 1))
			.WithLivingCell(new Point(0, 0))
			.WithLivingCell(new Point(0, 2))
			.WithLivingCell(new Point(2, 2))
			.Next()
			.OrderedCells
			.Should().Contain(Cell.Alive(new Point(1, 1)));

	[Fact]
	public void CellRemainsAlive_GivenTwoLivingNeighbours() =>
		Grid.Initialize(3, 3)
			.WithLivingCell(new Point(1, 1))
			.WithLivingCell(new Point(0, 0))
			.WithLivingCell(new Point(2, 2))
			.Next()
			.OrderedCells.Should().Contain(Cell.Alive(new Point(1, 1)));

	[Fact]
	public void ReturnsEmptyGrid_GivenNoLivingCells() =>
		Grid.Initialize(2, 2).Next().OrderedCells.Should().BeEquivalentTo(Grid.Initialize(2, 2).OrderedCells);
}