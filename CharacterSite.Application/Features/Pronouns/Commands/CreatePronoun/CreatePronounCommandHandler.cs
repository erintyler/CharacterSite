using CharacterSite.Application.Models.Responses;
using CharacterSite.Domain.Common;
using CharacterSite.Domain.Entities;
using CharacterSite.Domain.Repositories;

namespace CharacterSite.Application.Features.Pronouns.Commands.CreatePronoun;

public class CreatePronounCommandHandler
{
    public async Task<Result<PronounResponse>> Handle(CreatePronounCommand command, IPronounRepository pronounRepository, IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        var exists = await pronounRepository.ExistsAsync(command.Subject, command.Object, command.Possessive, cancellationToken);

        if (exists)
        {
            return new Error("Pronoun.AlreadyExists", "The specified pronoun already exists.");
        }
        
        var pronoun = Pronoun.Create(Guid.NewGuid(), command.Subject, command.Object, command.Possessive);

        if (pronoun.IsFailure)
        {
            return pronoun.Error;
        }

        pronounRepository.Add(pronoun.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new PronounResponse(pronoun.Value.Id, pronoun.Value.Subject, pronoun.Value.Object, pronoun.Value.Possessive);
    }
}