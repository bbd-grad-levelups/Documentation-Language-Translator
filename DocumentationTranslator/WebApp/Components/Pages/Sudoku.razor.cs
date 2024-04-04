using Microsoft.AspNetCore.Components;
using System;

namespace WebApp.Components.Pages;

public partial class Sudoku
{
	// Set up all variables:
	private int[,] sudokuAnswer = new int[9, 9];
	private int[,] sudokuGrid = new int[9, 9];
	private int[,] startingGrid = new int[9, 9];
	private int diffMode = 0;
	private int selectedRow = -1, selectedCol = -1;
	private List<(int, int)> highlightedCells = new List<(int, int)>();
	private bool isFinished = false;
	bool isFailed = false;
	private int numMistakes = 0;
	private const int maxErrors = 3;

	private void updateGetFinished()
	{
		isFinished = true;
		for(int i = 0; i < 9; i++)
		{
			for(int j = 0; j < 9; j++)
			{
				if(sudokuGrid[i, j] != sudokuAnswer[i, j])
				{
					isFinished = false;
					return;
				}
			}
		}
	}

	// This will be called when the page loads:
	protected override void OnInitialized()
	{
		for(int row = 0; row < 9; row++)
		{
			for(int col = 0; col < 9; col++)
			{
				sudokuAnswer[row, col] = 0;
			}
		}
		generateAnswerAndBoard();
		updateGetFinished();



		isFinished = false;
		isFailed = false;
		numMistakes = 0;


		base.OnInitialized();
	}

	// Generating the sudoku:
	private void generateAnswerAndBoard()
	{
		for(int row = 0; row < 9; row++)
		{
			for(int col = 0; col < 9; col++)
			{
				sudokuAnswer[row, col] = 0;
			}
		}

		generateAnswer();

		for(int r = 0; r < 9; r++)
		{
			for(int c = 0; c < 9; c++)
			{
				Random random = new Random();
				int tempNum = random.Next(1, 11);
				if(tempNum <= (diffMode + 1))
				{
					sudokuGrid[r, c] = 0;
					startingGrid[r, c] = 0;
				}
				else
				{
					sudokuGrid[r, c] = sudokuAnswer[r, c];
					startingGrid[r, c] = sudokuAnswer[r, c];
				}
			}
		}
	}

	private bool generateAnswer()
	{
		var qwang = new List<int>();
		for(int k = 1; k < 10; k++)
		{
			qwang.Add(k);
		}

		qwang = randomlyReorderList(qwang);

		for(int i = 0; i < 9; i++)
		{
			for(int j = 0; j < 9; j++)
			{
				if(sudokuAnswer[i, j] == 0)
				{
					for(int num = 0; num < 9; num++)
					{
						if(isValid(i, j, qwang[num]))
						{
							sudokuAnswer[i, j] = qwang[num];

							if(generateAnswer())
							{
								return true;
							}
							sudokuAnswer[i, j] = 0;
						}
					}
					return false;
				}
			}
		}
		return true;
	}

	private List<int> randomlyReorderList(List<int> list)
	{
		Random random = new Random();
		int n = list.Count;

		while(n > 1)
		{
			n--;
			int k = random.Next(n + 1);
			int val = list[k];
			list[k] = list[n];
			list[n] = val;
		}

		return list;
	}

	private bool isValid(int row, int col, int num)
	{
		var possNums = findPossibleNumbers(row, col);
		if(possNums.Contains(num))
		{
			return true;
		}

		return false;
	}

	private List<int> findPossibleNumbers(int row, int col)
	{
		var possibleNumbers = new List<int>();
		for(int i = 1; i < 10; i++)
		{
			possibleNumbers.Add(i);
		}

		for(int i = 0; i < 9; i++)
		{
			if(possibleNumbers.Contains(sudokuAnswer[row, i]))
			{
				possibleNumbers.Remove(sudokuAnswer[row, i]);
			}

			if(possibleNumbers.Contains(sudokuAnswer[i, col]))
			{
				possibleNumbers.Remove(sudokuAnswer[i, col]);
			}
		}

		int rLow = (row / 3) * 3;
		int cLow = (col / 3) * 3;

		for(int i = 0; i < 3; i++)
		{
			for(int j = 0; j < 3; j++)
			{
				if(possibleNumbers.Contains(sudokuAnswer[rLow + i, cLow + j]))
				{
					possibleNumbers.Remove(sudokuAnswer[rLow + i, cLow + j]);
				}
			}
		}

		return possibleNumbers;
	}

	// Changing the difficulty
	private void setDifficulty(int value)
	{
		diffMode = value;

		generateAnswerAndBoard();

		StateHasChanged();
	}

	// The onclick for the cell and the associated functions:
	private void CellClicked(int r, int c)
	{
		selectedRow = r;
		selectedCol = c;

		UpdateHighlightedCells();

		StateHasChanged();
	}

	private string GetCellCssClass(int row, int col)
	{
		var finalAns = "";

		if(sudokuGrid[row, col] != sudokuAnswer[row, col] && sudokuGrid[row, col] != 0)
		{
			return "wrong";
		}

		if(startingGrid[row, col] != 0)
		{
			finalAns += "starter-";
		}

		if(row == selectedRow && col == selectedCol)
		{
			finalAns += "mainSelected-";
		}
		var ans = highlightedCells.Contains((row, col)) ? "highlighted-" : "";
		finalAns = finalAns + ans;

		// Remove last character from finalAns
		if(finalAns.Length == 0)
			return "";
		finalAns = finalAns.Substring(0, finalAns.Length - 1);

		return finalAns;
	}

	private void UpdateHighlightedCells()
	{
		highlightedCells.Clear();

		// Get the row and column
		for(int num = 0; num < 9; num++)
		{
			if(highlightedCells.Contains((selectedRow, num)) == false)
				highlightedCells.Add((selectedRow, num));

			if(highlightedCells.Contains((num, selectedCol)) == false)
				highlightedCells.Add((num, selectedCol));
		}

		int r_lowerBound = (selectedRow / 3) * 3;
		int c_lowerBound = (selectedCol / 3) * 3;
		for(int addR = 0; addR < 3; addR++)
		{
			for(int addC = 0; addC < 3; addC++)
			{
				if(highlightedCells.Contains((r_lowerBound + addR, c_lowerBound + addC)) == false)
					highlightedCells.Add((r_lowerBound + addR, c_lowerBound + addC));
			}
		}

		highlightedCells.Remove((selectedRow, selectedCol));
	}

	private void InputClicked(int inputClick)
	{
		if(selectedRow < 0)
		{
			return;
		}

		if(startingGrid[selectedRow, selectedCol] != 0)
		{
			StateHasChanged();
			return;
		}

		if(inputClick == 0)
		{


			sudokuGrid[selectedRow, selectedCol] = 0;

		}
		else
		{
			sudokuGrid[selectedRow, selectedCol] = inputClick;

			if(sudokuGrid[selectedRow, selectedCol] != sudokuAnswer[selectedRow, selectedCol])
			{
				numMistakes++;
			}

			if(numMistakes >= maxErrors)
			{
				isFailed = true;
			}
		}

		updateGetFinished();
		StateHasChanged();
	}

	private void SolveSudoku()
	{
		for(int r = 0; r < 9; r++)
		{
			for(int c = 0; c < 9; c++)
			{
				sudokuGrid[r, c] = sudokuAnswer[r, c];
			}
		}

		updateGetFinished();
	}
}
