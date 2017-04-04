﻿namespace Gomoku
{
	using System;
	using Microsoft.Xna.Framework;

	internal class Bot2 : IBot
	{
		private readonly Random _rnd;
		private readonly Board _board;
		private readonly Gomoku _game;

		public Bot2(Gomoku game, Board board)
		{
			_game = game;
			_board = board;
			_rnd = new Random();
			_board.OneMoreStep += (p, v) => { if (v == CellValue.User) MakeMove(p); };
		}

		public void MakeMove(Point lastMove)
		{
			if (_game.State != GameState.Continues) return;

			var pos = new Point();
			if (_board.IsEmpty)
			{
				pos.X = _rnd.Next((int)(1f / 3 * _board.Dimension), (int)(2f / 3 * _board.Dimension));
				pos.Y = _rnd.Next((int)(1f / 3 * _board.Dimension), (int)(2f / 3 * _board.Dimension));
			}
			else
			{
				pos = GetBestMove(lastMove);
			}
			_board.MakeMove(pos, CellValue.Bot);
		}

		private Point GetBestMove(Point lastMove)
		{
			var max = 0;
			var res = new Point();
			for (var y = 0; y < _board.Dimension; y++)
				for (var x = 0; x < _board.Dimension; x++)
				{
					var pos = new Point(x, y);
					if (!_board.IsNone(pos)) continue;
					var buf = GetWeight(pos);
					if (buf > max) { max = buf; res = pos; }
				}
			return res;
		}

		private int GetWeight(Point pos)
		{
			var weight = 0;
			foreach (var dir in Direction.Values)
			{
				weight += GetOneNeighborWeight(pos, dir);
				weight += GetOneNeighborWeight(pos, dir * new Point(2));
			}

			if (_board.IsConnectAmount(pos, CellValue.User, 2)) weight += 10;
			if (_board.IsConnectAmount(pos, CellValue.Bot, 2)) weight += 20;
			if (_board.IsConnectAmount(pos, CellValue.User, 3)) weight += 40;
			if (_board.IsConnectAmount(pos, CellValue.Bot, 3)) weight += 80;
			if (_board.IsConnectAmount(pos, CellValue.User, 4)) weight += 160;
			if (_board.IsConnectAmount(pos, CellValue.Bot, 4)) weight += 320;
			if (_board.IsConnectAmount(pos, CellValue.User, 5)) weight += 640;
			if (_board.IsConnectAmount(pos, CellValue.Bot, 5)) weight += 1280;

			return weight;
		}

		private int GetOneNeighborWeight(Point pos, Point dir)
		{
			var newp = pos + dir;
			if (!_board.InBourd(newp)) return 0;
			if (_board.GetValue(newp) == CellValue.User) return 1;
			if (_board.GetValue(newp) == CellValue.Bot) return 2;
			return 0;
		}
	}
}
