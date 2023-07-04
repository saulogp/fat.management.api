using Domain.Model.Table;
using Infrastructure.Data.Entity;
using Infrastructure.Data.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controller
{
    public static class TableRouter
    {
        public static void ConfigureTableRouter(this WebApplication app)
        {
            var tableGroup = app.MapGroup("api/v1");
            tableGroup.MapGet("table", GetTableByCategory);// implementar paginação
            tableGroup.MapGet("table/{id}", GetTableById);
            tableGroup.MapGet("table/user/{id}", GetTablesByUsersId);
            tableGroup.MapPut("table/{id}", UpdateTableById);
            tableGroup.MapPost("table/", CreateTable);
            tableGroup.MapPost("table/{tableId}/user/{userId}/join", JoinTable);
            tableGroup.MapPost("table/{tableId}/user/{userId}/leave", LeaveTable);
            tableGroup.MapDelete("table/{id}", DeleteTableById);
        }

        private static async Task<IResult> CreateTable(TableModel request, [FromServices] ITableRepository repository)
        {
            await repository.CreateAsync(new TableEntity { Id = Guid.NewGuid(), Owner = request.Owner, Title = request.Title, ImgUrl = request.ImgUrl, Description = request.Description, MaxPlayers = request.MaxPlayers, Platform = request.Platform, SystemGame = request.SystemGame, CreatedDate = DateTime.UtcNow, LastUpdatedDate = DateTime.UtcNow, Active = true, Genres = request.Genres, Participants = new List<TableEntity.Participant>()});
            return Results.Ok();
        }

        private static IResult GetTableByCategory([FromQuery] string[] filters) => throw new NotImplementedException();

        private static async Task<IResult> GetTableById(Guid id, [FromServices] ITableRepository repository)
        {
            var table = await repository.GetTableAsync(id);
            if (table is null) return Results.BadRequest("Table not found");

            return Results.Ok(table);
        }

        private static async Task<IResult> UpdateTableById(Guid id, TableUpdateModel request, [FromServices] ITableRepository repository)
        {
            var table = await repository.GetTableAsync(id);
            if (table is null || !table.Active) return Results.BadRequest("Table Not Found");

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

        private static async Task<IResult> DeleteTableById(Guid id, [FromServices] ITableRepository repository)
        {
            var table = await repository.GetTableAsync(id);
            if ((table is null) || !table.Active) return Results.BadRequest("Table Not Found");

            await repository.RemoveAsync(id, table);

            return Results.Ok(id);
        }

        private static async Task<IResult> GetTablesByUsersId(Guid id, [FromServices] ITableRepository repository)
        {
            var tables = await repository.GetTableByUserIdAsync(id);
            return Results.Ok(tables);
        }

        private static async Task<IResult> JoinTable(Guid tableId, Guid userId, [FromServices] ITableRepository repository)
        {
            var table = await repository.GetTableAsync(tableId);
            
            if ((table is null) || !table.Active) return Results.BadRequest("Table Not Found");

            if(table.Owner.Equals(userId) || table.Participants.Any(x => x.Id.Equals(userId))) return Results.BadRequest("This user owns or already participates");

            if(table.Participants.Count() == table.MaxPlayers) return Results.BadRequest("Table is full");

            table.Participants.Add(new TableEntity.Participant { Id = userId, Notified = false });
            
            await repository.UpdateAsync(table.Id, table);

            return Results.Ok();
        }

        private static async Task<IResult> LeaveTable(Guid tableId, Guid userId, [FromServices] ITableRepository repository)
        {
            var table = await repository.GetTableAsync(tableId);
            
            if ((table is null) || !table.Active) return Results.BadRequest("Table Not Found");

            table.Participants.RemoveAt(table.Participants.FindIndex(x => x.Id.Equals(userId)));
            
            await repository.UpdateAsync(table.Id, table);

            return Results.Ok();
        }
    }
}