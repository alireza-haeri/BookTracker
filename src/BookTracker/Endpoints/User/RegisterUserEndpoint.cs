using BookTracker.Services;
using Carter;
using FluentValidation;
using MongoDB.Driver;

namespace BookTracker.Endpoints.User;

public class RegisterUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/user",
            async (RegisterUserRequest request, IValidator<RegisterUserRequest> validator, IMongoDatabase db,JwtTokenService jwtTokenService) =>
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return Results.ValidationProblem(validationResult.ToDictionary());

                var collection = db.GetCollection<EntityModels.User>(EntityModels.User.CollectionName);

                var existUser = Builders<EntityModels.User>.Filter.Eq(u => u.PhoneNumber, request.PhoneNumber);
                if (await collection.Find(existUser).AnyAsync())
                    return Results.Problem("User already exists", statusCode: StatusCodes.Status409Conflict);

                var entity = request.ToEntity();
                await collection.InsertOneAsync(entity);

                var token = jwtTokenService.GenerateToken(entity.Id.ToString(), entity.PhoneNumber);
                
                return Results.Ok(new RegisterUserResponse(token.Token, token.Expire));
            });
    }

    public record RegisterUserRequest(
        string PhoneNumber,
        string Password
    )
    {
        public EntityModels.User ToEntity()
            => EntityModels.User.Create(PhoneNumber, Password);
    };

    public record RegisterUserResponse(
        string Token,
        int ExpireMinute
    );
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(r => r.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Length(11).WithMessage("Phone number must be 11 digits");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(4).WithMessage("Password must be at least 4 characters")
                .MaximumLength(20).WithMessage("Password must not exceed 20 characters");
        }
    }
}