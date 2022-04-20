using System;
using System.Threading;

namespace ECOResolution
{
	public class Board
	{
		private Block[,] _espacos;
		private int _dimensao;
		private readonly SemaphoreSlim _semaphoresSlim = new(1);
		private int _countMove = 0;

		public void SetBoard(Block[,] espacos, int dimensao = 3)
		{
			this._espacos = espacos;
			this._dimensao = dimensao;
			PrintBoard();
		}

		public Block GetPosition(int i, int j)
		{
			try
			{
				return _espacos[i, j];
			}
			catch
			{
				return null;
			}
		}

		public void MovePara(Block b, (int i, int j) localAtual, (int i, int j) objetivo)
		{
			_espacos[localAtual.i, localAtual.j] = null;
			_espacos[objetivo.i, objetivo.j] = b;
			_countMove++;
			PrintBoard();
		}

		private void PrintBoard()
		{
			Console.WriteLine($"Movimento nro: {_countMove}");
			for (int j = (this._dimensao - 1); j >= 0; j--)
			{
				string linha = "";
				for (int i = 0; i < this._dimensao; i++)
				{
					if (_espacos[i, j] != null)
						linha += _espacos[i, j].Nome;
					else
						linha += "-";
					linha += "   ";
				}
				Console.WriteLine(linha);
			}
			Console.WriteLine(new string('-', this._dimensao * 3));
		}

		public void TomarAcao()
		{
			_semaphoresSlim.Wait();
		}

		public void DevolverAcao()
		{
			_semaphoresSlim.Release();
		}

		public bool Resolvido()
		{
			bool result = true;

			foreach (var item in _espacos)
			{
				if (item == null)
					continue;
				if (item.Estado != Estados.Satisfeito)
					result = false;
			}

			return result;
		}
	}
}
