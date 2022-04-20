using System;
using System.Threading;

namespace ECOResolution
{
	class Program
	{
		static void Main(string[] args)
		{
			Three();
			//Four();

			Console.Read();
		}

		static void Three()
		{
			Board board = new();

			Block A = new("A", (2, 2), (1, 0), board);
			Block B = new("B", (2, 0), (1, 1), board);
			Block C = new("C", (2, 1), (1, 2), board);

			Block[,] blocks = new Block[3, 3];

			blocks[1, 0] = A;
			blocks[1, 1] = B;
			blocks[1, 2] = C;

			board.SetBoard(blocks);

			Thread tA = new(new ThreadStart(A.Start));
			Thread tB = new(new ThreadStart(B.Start));
			Thread tC = new(new ThreadStart(C.Start));

			tA.Start();
			tB.Start();
			tC.Start();
			tA.Join();
			tB.Join();
			tC.Join();
		}

		static void Four()
		{
			Board board = new();

			Block A = new("A", (2, 2), (1, 0), board, 4);
			Block B = new("B", (2, 1), (1, 3), board, 4);
			Block C = new("C", (2, 3), (1, 1), board, 4);
			Block D = new("D", (2, 0), (1, 2), board, 4);

			Block[,] blocks = new Block[4, 4];

			blocks[1, 0] = A;
			blocks[1, 3] = B;
			blocks[1, 1] = C;
			blocks[1, 2] = D;

			board.SetBoard(blocks, 4);

			Thread tA = new(new ThreadStart(A.Start));
			Thread tB = new(new ThreadStart(B.Start));
			Thread tC = new(new ThreadStart(C.Start));
			Thread tD = new(new ThreadStart(D.Start));

			tA.Start();
			tB.Start();
			tC.Start();
			tD.Start();
			tA.Join();
			tB.Join();
			tC.Join();
			tD.Join();
		}
	}
}
