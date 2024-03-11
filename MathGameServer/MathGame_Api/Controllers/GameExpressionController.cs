using MathGame_Service.Interfaces;
using MathGame_Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MathGame_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameExpressionController : ControllerBase
    {
        private readonly IGameExpressionService _gameExpressionService;

        public GameExpressionController(IGameExpressionService gameExpressionService)
        {
            _gameExpressionService = gameExpressionService;
        }

        [Authorize]
        [HttpPost("answer-expression")]
        public async Task<IActionResult> AnswerExpression([FromBody] AnswerExpressionRequestModel model)
        {
            try
            {
                var response = await _gameExpressionService.AnswerExpression(model);
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(new { success = false, errorMessage = response.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, new { success = false, errorMessage = "An error occurred while processing the request." });
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGameExpression(int gameSessionId)
        {
            var response = await _gameExpressionService.CreateGameExpression(gameSessionId);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.ErrorMessage);
            }
        }

        [HttpGet("game-session/{gameSessionId}")]
        public async Task<IActionResult> GetAllGameExpressionsForGameSession(int gameSessionId)
        {
            try
            {
                var gameExpressions = await _gameExpressionService.GetlAllGameExpressionsForGivenGameSession(gameSessionId);
                return Ok(gameExpressions);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, "An error occurred while retrieving game expressions.");
            }
        }

    }
}
