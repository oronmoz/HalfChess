using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HalfChess.WebAPI.Models;
using HalfChess.Core.Interfaces;
using HalfChess.Core.Game;
using HalfChess.Core.Domain;

namespace HalfChess.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameSessionManager _sessionManager;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GameController> _logger;

        public GameController(
            IGameSessionManager sessionManager,
            IGameRepository gameRepository,
            ILogger<GameController> logger)
        {
            _sessionManager = sessionManager;
            _gameRepository = gameRepository;
            _logger = logger;
        }

        [HttpPost("start")]
        public async Task<ActionResult<GameStartResponse>> StartGame([FromBody] GameStartRequest request)
        {
            try
            {
                // Create game record in database
                var gameEntity = await _gameRepository.CreateGame(
                    request.PlayerId,
                    request.TimePerMove);

                // Create game session
                var session = _sessionManager.CreateSession(
                    request.PlayerId,
                    TimeSpan.FromSeconds(request.TimePerMove));

                session.GameStateChanged += async (s, state) =>
                {
                    if (state is GameState.Checkmate or GameState.Stalemate or GameState.Timeout)
                    {
                        await _gameRepository.EndGame(gameEntity.GameId.ToString(), state.ToString());
                    }
                };

                session.StartGame();

                return Ok(new GameStartResponse
                {
                    GameId = session.Id,
                    InitialBoard = session.GetBoardSnapshot(),
                    PlayerColor = session.PlayerColor,
                    TimePerMove = session.TimePerMove.TotalSeconds
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting game for player {PlayerId}", request.PlayerId);
                return StatusCode(500, "Unable to start game");
            }
        }

        [HttpPost("{gameId}/move")]
        public async Task<ActionResult<MoveResponse>> MakeMove(Guid gameId, [FromBody] MoveRequest request)
        {
            try
            {
                var session = _sessionManager.GetSession(gameId);
                if (session == null)
                    return NotFound("Game session not found");

                var moveResult = session.MakeMove(
                    Position.FromAlgebraic(request.FromPosition),
                    Position.FromAlgebraic(request.ToPosition));

                if (!moveResult.IsValid)
                    return BadRequest(moveResult.Message);

                // Record move in database
                await _gameRepository.RecordMove(gameId.ToString(),
                    request.FromPosition,
                    request.ToPosition,
                    true);

                return Ok(new MoveResponse
                {
                    IsValid = true,
                    GameState = moveResult.NewState.ToString(),
                    Message = moveResult.Message,
                    BoardState = session.GetBoardSnapshot(),
                    LastMoveTime = session.LastMoveTime,
                    CurrentTurn = session.CurrentTurn
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing move for game {GameId}", gameId);
                return StatusCode(500, "Unable to process move");
            }
        }

        [HttpGet("{gameId}/state")]
        public ActionResult<GameStateResponse> GetGameState(Guid gameId)
        {
            var session = _sessionManager.GetSession(gameId);
            if (session == null)
                return NotFound("Game session not found");

            return Ok(new GameStateResponse
            {
                State = session.State,
                CurrentTurn = session.CurrentTurn,
                LastMoveTime = session.LastMoveTime,
                BoardState = session.GetBoardSnapshot()
            });
        }

        [HttpPost("{gameId}/resign")]
        public async Task<ActionResult> ResignGame(Guid gameId)
        {
            var session = _sessionManager.GetSession(gameId);
            if (session == null)
                return NotFound("Game session not found");

            await _gameRepository.EndGame(gameId.ToString(), "Resigned");
            _sessionManager.EndSession(gameId);

            return Ok();
        }
    }
}

