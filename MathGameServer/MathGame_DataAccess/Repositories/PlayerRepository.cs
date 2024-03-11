using MathGame_DataAccess.Interfaces;
using MathGame_Domain;
using MathGame_Domain.EntityModels;
using MathGame_Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Serilog;

namespace MathGame_DataAccess.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly MathGameDbContext _dbContext;

        public PlayerRepository(MathGameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Player> CreatePlayer(Player playerModel)
        {
            try
            {
                await _dbContext.AddAsync(playerModel);
                await _dbContext.SaveChangesAsync();

                return playerModel;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

     
        public async Task<Player?> GetPlayer(string email, string password)
        {
            try
            {
                return await _dbContext.Players.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<Player?> GetPlayerByEmail(string email)
        {
            try
            {
                return await _dbContext.Players.FirstOrDefaultAsync(x => x.Email == email);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async  Task<Player?> UpdatePlayer(Player player)
        {
            try
            {
                _dbContext.Update(player);
                await _dbContext.SaveChangesAsync();
                return player;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public async Task<int> GetOnlinePlayers()
        {
            try
            {
                 return await _dbContext.LoggedPlayersInfo.CountAsync(x => x.LoginStatusId == (int)LoginStatus.LoggedIn);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}
