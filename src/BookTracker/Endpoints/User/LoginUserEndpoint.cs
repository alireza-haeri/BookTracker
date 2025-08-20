using Carter;
using FluentValidation;
using MongoDB.Driver;
using BookTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookTracker.Endpoints.User;

public class LoginUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/user/login", async (LoginUserRequest request, [FromServices]IValidator<LoginUserRequest> validator, IMongoDatabase db,JwtTokenService jwtTokenService) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            var userCollection = db.GetCollection<EntityModels.User>(EntityModels.User.CollectionName);
            var userFilter = Builders<EntityModels.User>.Filter.Eq(u => u.PhoneNumber, request.PhoneNumber);
            var user = await userCollection.Find(userFilter).FirstOrDefaultAsync();

            if (user is null)
                return Results.Problem("User not found", statusCode: StatusCodes.Status404NotFound);

            if (request.Password != user.Password)
                return Results.Problem("Invalid password", statusCode: StatusCodes.Status401Unauthorized);

            var token = jwtTokenService.GenerateToken(user.Id.ToString(), user.PhoneNumber);
            var response = new LoginUserResponse(token.Token,token.Expire);

            return Results.Ok(response);
        });
    }

    public record LoginUserRequest(
        string PhoneNumber,
        string Password
    );

    public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserRequestValidator()
        {
            RuleFor(r => r.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Length(11).WithMessage("Phone number must be 11 digits");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }

    public record LoginUserResponse(
        string Token,
        int ExpireMinute
    );
}
