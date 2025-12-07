using System;
using System.Collections.Generic;
using System.Text;

namespace astar_algorithme
{
	public class AStarAlgo // code provient de https://www.geeksforgeeks.org/dsa/a-search-algorithm/
	{
		// Raccourci de KeyValuePair<int, int>
		public struct Pair // Structure utilisé pour les positions dans la carte (X, Y)
		{
			public int premier, second;

			public Pair(int x, int y) // Constructeur de Pair
			{
				premier = x;
				second = y;
			}
		}


		// Structure permettant de conserver les paramètres nécessaires
		public struct Cell
		{
			// Index de la ligne et de la colonne du parent
			// i = X et j = Y
			// 0 <= i <= ROW - 1 et 0 <= j <= COL - 1
			public int parent_i, parent_j;

			// f = g + h
			public double f, g, h;
		}


		// La fonction utilise l'algorithme A* pour
		// Trouver le chemin, de façon approximative,
		// le plus court entre le debut et la sortie
		public static void AStar(int[,] grid, Pair src, Pair dest)
		{
			int ROW = grid.GetLength(0); // Nombre de Lignes
			int COL = grid.GetLength(1); // Nombre de colonnes

			// Verification si le debut ou la sortie est invalide
			if (!IsValid(src.premier, src.second, ROW, COL) || !IsValid(dest.premier, dest.second, ROW, COL))
			{
				Console.WriteLine("le début ou la sortie est invalide!");
				return;
			}

			// Verification si le debut ou la sortie est bloqué
			if (!IsUnBlocked(grid, src.premier, src.second) || !IsUnBlocked(grid, dest.premier, dest.second))
			{
				Console.WriteLine("le début ou la sortie est bloqué!");
				return;
			}

			// Verification si la destination est a la même position que le début
			if (src.premier == dest.premier && src.second == dest.second)
			{
				Console.WriteLine("la sortie est a la meme position que le debut");
				return;
			}

			// Array 2d de la taille de la carte
			// Boolean indiquant si une cellule a été incluse
			bool[,] closedList = new bool[ROW, COL];
			 
			// Array 2d de la taille de la carte
			// Array de cellule comportant les details de chaque case
			Cell[,] cellDetails = new Cell[ROW, COL];

			for (int i = 0; i < ROW; i++) // Initialise les valeurs dans l`Array
			{
				for (int j = 0; j < COL; j++)
				{
					cellDetails[i, j].f = double.MaxValue;
					cellDetails[i, j].g = double.MaxValue;
					cellDetails[i, j].h = double.MaxValue;
					cellDetails[i, j].parent_i = -1;
					cellDetails[i, j].parent_j = -1;
				}
			}

			// Initialiser les parametres du début
			int x = src.premier, y = src.second;
			cellDetails[x, y].f = 0.0;
			cellDetails[x, y].g = 0.0;
			cellDetails[x, y].h = 0.0;
			cellDetails[x, y].parent_i = x; // Position en X du début
			cellDetails[x, y].parent_j = y; // Position en Y du début


			/*
				SortedSet de tuples
				<f, <i, j>>
				f = g + h,
				i = X et j = Y
				0 <= i <= ROW - 1 et 0 <= j <= COL - 1
				les tuples sont comparer par la valeur de f
			*/
			SortedSet<(double, Pair)> openList = new SortedSet<(double, Pair)>(
				Comparer<(double, Pair)>.Create((a, b) => a.Item1.CompareTo(b.Item1)));

			// la première cellules de la liste a la valeur
			// 'f' = 0
			openList.Add((0.0, new Pair(x, y)));

			// Boolean est a true si la sortie est trouvé
			bool foundDest = false;

			while (openList.Count > 0)
			{
				(double f, Pair pair) p = openList.Min; // Recupere le tuple possedant le plus petit f
				openList.Remove(p); // retire le tuple recupere de la liste

				// Ajoute la coordonee du tuple recupéré
				x = p.pair.premier;
				y = p.pair.second;
				closedList[x, y] = true;

				// Genère les 8 enfants de la cellule
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						if (i == 0 && j == 0) // Empèche de vérifier le tuple recupéré
							continue;

						int newX = x + i; // position en X du tuples + i
						int newY = y + j; // position en Y du tuples + i

						// Vérifie si l'enfant est une cellule valide
						if (IsValid(newX, newY, ROW, COL))
						{
							// Vérifie si l'enfant est la sortie
							if (IsDestination(newX, newY, dest))
							{
								cellDetails[newX, newY].parent_i = x; // Met le parent i la position en X du tuple récupéré
								cellDetails[newX, newY].parent_j = y; // Met le parent j la position en Y du tuple récupéré
								Console.WriteLine("La sortie est trouvé");
								TracePath(cellDetails, dest);
								foundDest = true;
								return;
							}

							// Vérifie si l'enfant est dans la closedList
							// et si il est pas bloqué
							if (!closedList[newX, newY] && IsUnBlocked(grid, newX, newY)) // Doit etre non bloqué
							{
								double gNew = cellDetails[x, y].g + 1.0;
								double hNew = CalculateHValue(newX, newY, dest);
								double fNew = gNew + hNew;

								// Si la cellule n'est pas dans openList,
								// Il est ajouté, Le parent de cette cette
								// Cellule est la cellule courante et change
								// f, g, et h
								if (cellDetails[newX, newY].f == double.MaxValue || cellDetails[newX, newY].f > fNew)
								{
									openList.Add((fNew, new Pair(newX, newY)));

									// Change les details de cette cellule
									cellDetails[newX, newY].f = fNew;
									cellDetails[newX, newY].g = gNew;
									cellDetails[newX, newY].h = hNew;
									cellDetails[newX, newY].parent_i = x;
									cellDetails[newX, newY].parent_j = y;
								}
							}
						}
					}
				}
			}

			// Si la cellules de sortie n'est pas trouvé
			// et que openList est vide. La recherche 
			// A echoué, car il n'y a pas de chemin trouvé entre
			// Le début et la sortie
			if (!foundDest)
				Console.WriteLine("La sortie n'a pas été trouvé");
		}

		// Vérification si la cellule a l'emplacement est valide
		// Boolean true si la position est dans la carte
		public static bool IsValid(int row, int col, int ROW, int COL)
		{
			return (row >= 0) && (row < ROW) && (col >= 0) && (col < COL);
		}

		// Verification si la case est bloqué
		// Boolean true si elle est non bloqué
		public static bool IsUnBlocked(int[,] grid, int row, int col)
		{
			return grid[row, col] == 1; // 1 = case non bloqué
		}

		// Verifier si la cellule a cette coordonée est la destination
		// Boolean true si la cellule est la sortie
		public static bool IsDestination(int row, int col, Pair dest)
		{
			return (row == dest.premier && col == dest.second);
		}

		// Calcule la valeur de h.
		// la distance entre la cellule et la destination
		public static double CalculateHValue(int row, int col, Pair dest)
		{
			// baser sur le Théoreme de pythagore
			// Donne la distance diagonale de la cellule à la destination
			return Math.Sqrt(Math.Pow(row - dest.premier, 2) + Math.Pow(col - dest.second, 2));
		}

		// Ecriture de chemin dans la console
		public static void TracePath(Cell[,] cellDetails, Pair dest)
		{
			Console.WriteLine("\nLe chemin est:");

			// Position de la sortie
			int row = dest.premier;
			int col = dest.second;

			Stack<Pair> Path = new Stack<Pair>(); // Chemin entre le début et la sortie

			// Recupération du chemin
			while (!(cellDetails[row, col].parent_i == row && cellDetails[row, col].parent_j == col))
			{
				Path.Push(new Pair(row, col)); // ajout de la cellule
				// Recupération de la prochaine cellule du chemin
				int temp_row = cellDetails[row, col].parent_i;
				int temp_col = cellDetails[row, col].parent_j;
				row = temp_row;
				col = temp_col;
			}

			// Écriture du chemin
			Path.Push(new Pair(row, col));
			while (Path.Count > 0)
			{
				Pair p = Path.Pop();
				Console.Write($" -> ({p.premier},{p.second}) ");
			}
		}

		// Test de A*.
		public static void Main(string[] args)
		{
			/* Map
				1 = passage
				0 = bloqué
			*/
			int[,] grid =
			{
			{1, 0, 1, 1, 1, 1, 0, 1, 1, 1},
			{1, 1, 1, 0, 1, 1, 1, 0, 1, 1},
			{1, 1, 1, 0, 1, 1, 0, 1, 0, 1},
			{0, 0, 1, 0, 1, 0, 0, 0, 0, 1},
			{1, 1, 1, 0, 1, 1, 1, 0, 1, 0},
			{1, 0, 1, 1, 1, 1, 0, 1, 0, 0},
			{1, 0, 0, 0, 0, 1, 0, 0, 0, 1},
			{1, 0, 1, 1, 1, 1, 0, 1, 1, 1},
			{1, 1, 1, 0, 0, 0, 1, 0, 0, 1}
		};

			Pair src = new Pair(7, 2);

			Pair dest = new Pair(0, 0);

			AStar(grid, src, dest);
		}
	}
}

