﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.Storage;
using api.Storage.Models;
using api.ViewModels;
using MediatR;

namespace api.Commands.Handlers
{
    public class RegisterGameHandler : IRequestHandler<RegisterGame, GameViewModel>
    {
        private readonly GameTrackerContext _context;

        public RegisterGameHandler(GameTrackerContext context)
        {
            _context = context;
        }

        public async Task<GameViewModel> Handle(RegisterGame request, CancellationToken cancellationToken)
        {
            var exists = _context.PlatformGames.SingleOrDefault(g => g.Code == request.Code);

            if (exists == null)
            {
                var game = new Game
                {
                    Name = request.Name,
                    Description = request.Description
                };

                _context.Add(game);

                var platformGame = new PlatformGame()
                {
                    GameId = game.Id,
                    Code = request.Code,
                    PlatformId = request.Platform,
                    Registered = DateTime.UtcNow
                };

                _context.Add(platformGame);

                await _context.SaveChangesAsync(cancellationToken);

                return new GameViewModel
                {
                    Id = game.Id,
                    Name = game.Name,
                    Description = game.Description,
                    Code = request.Code,
                    Registered = platformGame.Registered
                };
            }

            return null;
        }
    }
}