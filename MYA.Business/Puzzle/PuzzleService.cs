using MYA.Data.DbUnico;
using MYA.Data.Puzzle;
using MYA.Models.Common;
using MYA.Models.Puzzle;
using System;
using System.Collections.Generic;
using System.Text;

namespace MYA.Business.Puzzle
{
    public sealed class PuzzleService
    {
        private readonly PuzzleDataAccess _puzzleDataAccess;

        public PuzzleService(PuzzleDataAccess puzzleDataAccess)
        {
            _puzzleDataAccess = puzzleDataAccess;
        }

        public Task<List<Periferica>> GetPerifericheByIdSito(int idSito, string soc, CancellationToken cancellationToken = default)
        {
            return _puzzleDataAccess.GetPerifericheByIdSito(idSito, soc, cancellationToken);
        }
    }
}
