using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Game.Domain
{
    // TODO Сделать по аналогии с MongoUserRepository
    public class MongoGameRepository : IGameRepository
    {
        private readonly IMongoCollection<GameEntity> gameCollection;
        public const string CollectionName = "games";

        public MongoGameRepository(IMongoDatabase db)
        {
            gameCollection = db.GetCollection<GameEntity>(CollectionName);
        }

        public GameEntity Insert(GameEntity game)
        {
            gameCollection.InsertOne(game);
            return game;
        }

        public GameEntity FindById(Guid gameId)
        {
            var games= gameCollection.Find(x => x.Id == gameId);
            return games.FirstOrDefault();
        }

        public void Update(GameEntity game)
        {
            gameCollection.ReplaceOne(x => x.Id == game.Id, game);
        }

        // Возвращает не более чем limit игр со статусом GameStatus.WaitingToStart
        public IList<GameEntity> FindWaitingToStart(int limit)
        {
            //TODO: Используй Find и Limit
            return gameCollection.Find(x => x.Status == GameStatus.WaitingToStart).Limit(limit).ToList();
            
        }

        // Обновляет игру, если она находится в статусе GameStatus.WaitingToStart
        public bool TryUpdateWaitingToStart(GameEntity game)
        {
            //TODO: Для проверки успешности используй IsAcknowledged и ModifiedCount из результата
            var oldGame = gameCollection.Find(x => x.Id == game.Id).FirstOrDefault();
            if (oldGame is not { Status: GameStatus.WaitingToStart }) return false;
            {
                var result = gameCollection.ReplaceOne(x => x.Id == game.Id, game);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
        }
    }
}