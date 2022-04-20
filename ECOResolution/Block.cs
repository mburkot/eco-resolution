using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ECOResolution
{
	public class Block
	{
		private readonly int _dimensao;
		private readonly Board _board;
		private readonly (int i, int j) Objetivo;
		private (int i, int j) LocalAtual;
		private Dictionary<Block, List<(int i, int j)>> RestricoesRecebidas;
		private Dictionary<Block, List<(int i, int j)>> RestricoesImpostas;
		public string Nome { get; private init;}
		public Estados Estado { get; private set;}

		public Block(string nome, (int i, int j) objetivo, (int i, int j) localAtual, Board board, int dimensao = 3)
		{
			this.Nome = nome;
			this.Objetivo = objetivo;
			this.Estado = Estados.BuscandoSatisfacao;
			this.RestricoesRecebidas = new();
			this.RestricoesImpostas = new();
			this.LocalAtual = localAtual;
			this._dimensao = dimensao;
			_board = board;
		}

		public void Start()
		{
			Console.WriteLine($"Block {this.Nome} Started");

			while (!_board.Resolvido())
			{
				_board.TomarAcao();

				//Console.WriteLine("===========================================");
				//Console.WriteLine($"{this.Nome} - Tomando ação.");
				//PrintStatus();

				switch (this.Estado)
				{
					case Estados.BuscandoSatisfacao:
						BuscarSatisfacao();
						break;
					case Estados.BuscandoFuga:
						BuscarFuga();
						break;
					case Estados.Satisfeito:
						break;
				}

				//Console.WriteLine($"{this.Nome} - Devolvendo ação.");
				//PrintStatus();

				_board.DevolverAcao();

				Thread.Sleep(500);
			}
		}

		private void PrintStatus()
		{
			Console.WriteLine($"Estado Atual: {this.Estado}");
			Console.WriteLine($"Local Atual: {this.LocalAtual}");
			Console.WriteLine($"Restrições Recebidas:");
			PrintRestricoes(RestricoesRecebidas);
			Console.WriteLine($"Restrições Impostas:");
			PrintRestricoes(RestricoesImpostas);
		}

		private void PrintRestricoes(Dictionary<Block, List<(int i, int j)>> restricoes)
		{
			foreach (var item in restricoes)
			{
				string restricoesList = "[";

				foreach (var item2 in item.Value)
				{
					restricoesList += $"({item2.i},{item2.j}),";
				}

				restricoesList = restricoesList.Substring(0, restricoesList.Length - 1) + "]";

				Console.WriteLine($"{item.Key.Nome} => {restricoesList}");
			}
		}

		private void BuscarSatisfacao()
		{
			var acima = _board.GetPosition(this.LocalAtual.i, this.LocalAtual.j + 1);
			if (acima != null)
			{
				Atacar(acima);
			}
			else
			{
				var objetivo = _board.GetPosition(this.Objetivo.i, this.Objetivo.j);
				if (objetivo != null)
					Atacar(objetivo);
				else
					BuscarObjetivo();
			}
		}

		private void BuscarFuga()
		{
			var acima = _board.GetPosition(this.LocalAtual.i, this.LocalAtual.j + 1);
			if (acima != null)
			{
				Atacar(acima);
			}
			else
			{
				var localFuga = BuscarLocalFuga();
				if (localFuga.i >= 0)
				{
					Mover(LocalAtual, localFuga);
					this.Estado = Estados.BuscandoSatisfacao;
				}
			}
		}

		private void Atacar(Block b)
		{
			//Console.WriteLine($"Atacando {b.Nome}");

			List<(int i, int j)> restricoes = new();
			restricoes.Add(this.Objetivo);
			restricoes.Add((this.LocalAtual.i, this.LocalAtual.j + 1));

			this.RestricoesImpostas[b] = restricoes;
			b.ReceberAtaque(this, restricoes);

			foreach (var item in this.RestricoesRecebidas)
			{
				item.Key.RestricoesImpostas[b] = item.Value;
				b.ReceberAtaque(item.Key, item.Value);
			}
		}

		private void Mover((int i, int j) localAtual, (int i, int j) objetivo)
		{
			//Console.WriteLine($"Movendo De {localAtual} Para {objetivo}");

			_board.MovePara(this, localAtual, objetivo);

			this.LocalAtual = objetivo;

			foreach (var item in this.RestricoesImpostas)
			{
				RemoverRestricoes(item.Key);
			}
		}

		private (int i, int j) BuscarLocalFuga()
		{
			for (int i = 0; i < _dimensao; i++)
			{
				for (int j = 0; j < _dimensao; j++)
				{
					if (this.LocalAtual == (i, j))
						break;

					if (Restricoes().Contains((i, j)))
						continue;

					var local = _board.GetPosition(i, j);
					if (local == null && TemBase((i, j)))
						return (i, j);
				}
			}
			return (-1, -1);
		}

		private bool TemBase((int i, int j) objetivo)
		{
			bool temBase = true;

			for (int j = 0; j < objetivo.j; j++)
			{
				var local = _board.GetPosition(objetivo.i, j);
				if (local == null)
					temBase = false;
			}

			return temBase;
		}

		private void BuscarObjetivo()
		{
			var i = this.Objetivo.i;
			var j = 0;

			while (j <= this.Objetivo.j)
			{
				if ((i, j) == this.LocalAtual)
					break;

				var novoLocal = _board.GetPosition(i, j);
				if (novoLocal == null)
				{
					if (!Restricoes().Contains((i, j)) && TemBase((i, j)))
					{
						Mover(this.LocalAtual, (i, j));
						break;
					}
				}
					
				j++;
			}

			if (this.LocalAtual == this.Objetivo)
				this.Estado = Estados.Satisfeito;
			
		}

		private IEnumerable<(int i, int j)> Restricoes()
		{
			List<(int i, int j)> lista = new();

			foreach(var list in RestricoesRecebidas.Values)
			{
				lista.AddRange(list);
			}

			return lista;
		}

		private void RemoverRestricoes(Block b)
		{
			RestricoesImpostas.Remove(b);
			b.RestricoesRecebidas.Remove(this);
		}

		public void ReceberAtaque(Block b, List<(int i, int j)> restricoes)
		{
			this.Estado = Estados.BuscandoFuga;
			this.RestricoesRecebidas[b] = restricoes;
		}
	}
}
