using MathGame_Domain.EntityModels;
using MathGame_Service.Interfaces;
using MathGame_Service.Models;
using MathGame_Shared.AppConstants;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MathGame_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameSessionController : ControllerBase
    {

        private readonly IGameSessionService _gameSessionService;


        public GameSessionController(IGameSessionService gameSessionService)
        {
            _gameSessionService = gameSessionService;
        }


        [HttpPost("create-session")]
        public async Task<ActionResult<ServiceResponse<GameSession>>> CreateGameSession()
        {
            try
            {
                var result = await _gameSessionService.CreateGameSession();

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, new ServiceResponse<GameSession>(ErrorMessages.GenericErrorControllerMessage));
            }
        }

        [HttpPost("assign-player")]
        public async Task<IActionResult> AssignPlayerToSession([FromBody] GameSessionAssignModel AssingSessionModel)
        {
            var response = await _gameSessionService.AssignPlayerToSession(AssingSessionModel.PlayerEmail);
            if (response.Success)
            {
                return Ok(response.Data);
            }
            else
            {
                return BadRequest(response.ErrorMessage);
            }
        }

    }
}
