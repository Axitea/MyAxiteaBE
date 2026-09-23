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

        #region Periferiche

        public Task<List<Pz_Periferica>> GetPerifericheByIdSito(int idSito, string soc, CancellationToken cancellationToken = default)
        {
            return _puzzleDataAccess.GetPerifericheByIdSito(idSito, soc, cancellationToken);
        }

        public Task<List<Pz_Periferica>> GetPerifericheByNPeriferica(string nPeriferica, string soc, CancellationToken cancellationToken = default)
        {
            return _puzzleDataAccess.GetPerifericheByNPeriferica(nPeriferica, soc, cancellationToken);
        }

        public Task<List<Pz_Periferica>> GetPerifericheByIdPeriferica(int idPeriferica, string soc, CancellationToken cancellationToken = default)
        {
            return _puzzleDataAccess.GetPerifericheByIdPeriferica(idPeriferica, soc, cancellationToken);
        }

        #endregion Periferiche

        #region Canali

        public Task<List<Pz_Canale>> GetCanaliByNPeriferica(int nPeriferica, string soc, CancellationToken cancellationToken = default)
        {
            return _puzzleDataAccess.GetCanaliByNPeriferica(nPeriferica, soc, cancellationToken);
        }

        #endregion Canali
    }
}
