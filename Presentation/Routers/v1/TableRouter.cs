using Domain.Model.Table;
using Infrastructure.Data.Entity;
using Infrastructure.Data.Interface;

namespace Presentation.Routers
{
    public static class TableRouter
    {
        public static void ConfigureTableRouter(this WebApplication app)
        {
            var group = app.MapGroup("api/v1").RequireAuthorization();
            group.MapGet("/table/{id}", GetTable);
            group.MapGet("/table/user/{id}", GetTablesByUsers);
            group.MapPut("/table/{id}", UpdateTable);
            group.MapPost("/table", CreateTable);
            group.MapPost("/table/{tableId}/user/{userId}/join", JoinTable);
            group.MapPost("/table/{tableId}/user/{userId}/leave", LeaveTable);
            group.MapDelete("/table/{id}", DeleteTable);
        }

        private static async Task<IResult> CreateTable(TableModel request, ITableRepository repository)
        {
            try
            {
                if (request is null) return Results.BadRequest("Invalid request. Table data is missing.");

                await repository.CreateAsync(new TableEntity
                {
                    Id = Guid.NewGuid(),
                    Owner = request.Owner,
                    Title = request.Title,
                    ImgUrl = request.ImgUrl,
                    Description = request.Description,
                    MaxPlayers = request.MaxPlayers == 0 ? 1 : request.MaxPlayers,
                    Platform = request.Platform,
                    SystemGame = request.SystemGame,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
                    Active = true,
                    Genres = request.Genres,
                    Participants = new List<TableEntity.Participant>()
                });

                return Results.Ok();
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating table: " + ex.Message);
            }
        }

        private static async Task<IResult> GetTable(Guid id, ITableRepository tableRepository, IUserProfileRepository userRepository)
        {
            try
            {
                var table = await tableRepository.GetTableAsync(id);

                if (table is null) return Results.BadRequest("Table not found");

                var response = new TableResponseModel
                {
                    Title = table.Title,
                    Description = table.Description,
                    Genres = table.Genres,
                    ImgUrl = table.ImgUrl,
                    SystemGame = table.SystemGame,
                    Platform = table.Platform,
                    MaxPlayers = table.MaxPlayers,
                    Active = table.Active,
                    CreatedDate = table.CreatedDate,
                    LastUpdatedDate = table.LastUpdatedDate,
                    Participants = new List<Participant>()
                };

                var user = await userRepository.GetUserAsync(table.Owner);
                response.Owner = new Participant{
                    Email = user.Email,
                    ImgUrl = user.ImgUrl,
                    UserName = user.UserName
                };

                if (table.Participants.Any()){
                    foreach(var participant in table.Participants){
                        user = await userRepository.GetUserAsync(participant.Id);
                        response.Participants.Add(new Participant{
                            Email = user.Email,
                            ImgUrl = user.ImgUrl,
                            UserName = user.UserName
                        });
                    }
                }

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching table by ID: " + ex.Message);
            }
        }

        private static async Task<IResult> UpdateTable(Guid id, TableUpdateModel request, ITableRepository repository)
        {
            try
            {
                var table = await repository.GetTableAsync(id);
                
                if (table is null) return Results.BadRequest("Table Not Found");

                if (!table.Active) return Results.BadRequest("Table is not active");

                if (request is null) return Results.BadRequest("Request cannot be null");

                table.Title = request.Title;
                table.ImgUrl = request.ImgUrl;
                table.Description = request.Description;
                table.MaxPlayers = request.MaxPlayers;
                table.Platform = request.Platform;
                table.SystemGame = request.SystemGame;
                table.Genres = request.Genres;
                table.LastUpdatedDate = DateTime.UtcNow;

                await repository.UpdateAsync(id, table);

                return Results.Ok(table.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating the table: " + ex.Message);
            }
        }

        private static async Task<IResult> DeleteTable(Guid id, ITableRepository repository)
        {
            try
            {
                var table = await repository.GetTableAsync(id);
                
                if (table is null) return Results.BadRequest("Table Not Found");

                if (!table.Active) return Results.BadRequest("Table is not active");

                await repository.RemoveAsync(id, table);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing the table: " + ex.Message);
            }
        }

        private static async Task<IResult> GetTablesByUsers(Guid id, ITableRepository repository)
        {
            try
            {
                var tables = await repository.GetTableByUserIdAsync(id);
                
                if (tables is null || tables.Count == 0) return Results.NotFound("No tables found for the specified user ID");

                return Results.Ok(tables);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching tables by user ID: " + ex.Message);
            }
        }

        private static async Task<IResult> JoinTable(Guid tableId, Guid userId, ITableRepository repository)
        {
            try
            {
                if (tableId.Equals(Guid.Empty) || userId.Equals(Guid.Empty)) return Results.BadRequest("Invalid table or user ID");

                var table = await repository.GetTableAsync(tableId);

                if (table is null) return Results.BadRequest("Table Not Found");

                if (!table.Active) return Results.BadRequest("Table is not active");

                if (table.Owner.Equals(userId) || table.Participants.Any(x => x.Id.Equals(userId))) return Results.BadRequest("This user owns or already participates");

                if (table.Participants.Count == table.MaxPlayers) return Results.BadRequest("Table is full");

                table.Participants.Add(new TableEntity.Participant { Id = userId, Notified = false });

                await repository.UpdateAsync(table.Id, table);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                throw new Exception("Error joining table: " + ex.Message);
            }
        }

        private static async Task<IResult> LeaveTable(Guid tableId, Guid userId, ITableRepository repository)
        {
            try
            {
                if (tableId.Equals(Guid.Empty) || userId.Equals(Guid.Empty)) return Results.BadRequest("Invalid table or user ID");

                var table = await repository.GetTableAsync(tableId);

                if (table is null) return Results.BadRequest("Table Not Found");

                if (!table.Active) return Results.BadRequest("Table is not active");
                
                var participantIndex = table.Participants.FindIndex(x => x.Id.Equals(userId));
                
                if (participantIndex != -1)
                    table.Participants.RemoveAt(participantIndex);
                else
                    return Results.BadRequest("User is not participating in the table");

                await repository.UpdateAsync(table.Id, table);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                throw new Exception("Error leaving table: " + ex.Message);
            }
        }
    }
}