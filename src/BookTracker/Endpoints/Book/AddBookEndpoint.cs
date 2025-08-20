using BookTracker.Common;
using BookTracker.Endpoints.User;
using BookTracker.EntityModels;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookTracker.Endpoints.Book;

public class AddBookEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/book", async (AddBookRequest request, IMongoDatabase db,HttpContext context,[FromServices] IValidator<AddBookRequest> validator) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());
            
            var userIdClaim = context.User.Claims.FirstOrDefault(c=>c.Type == JwtClaimTypes.UserId)?.Value;
            if (userIdClaim is null)
                return Results.BadRequest("UserId not found");

            if (!ObjectId.TryParse(userIdClaim, out var userId))
                return Results.BadRequest("UserId not valid");

            var book = request.ToEntity(userId);

            var collection = db.GetCollection<EntityModels.Book>("Books");
            await collection.InsertOneAsync(book);

            return Results.Created($"/book/{book.Id}", book);
        })
        .RequireAuthorization()
        .WithTags("Book");
    }

    public record AddBookRequest(string Title, string Author, string Notes, BookStatus Status)
    {
        public EntityModels.Book ToEntity(ObjectId userId)
            => EntityModels.Book.Create(Title, Author, Notes, Status, userId);
    }

    public class AddBookRequestValidator : AbstractValidator<AddBookRequest>
    {
        public AddBookRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Book title cannot be empty.")
                .MaximumLength(200).WithMessage("Book title must not exceed 200 characters.");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author name is required.")
                .MaximumLength(100).WithMessage("Author name must not exceed 100 characters.");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid book status.");

        }
    }
}