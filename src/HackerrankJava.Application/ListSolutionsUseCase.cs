using HackerrankJava.Domain;

namespace HackerrankJava.Application;

public sealed class ListSolutionsUseCase(IChallengeSolutionRepository repository)
{
    public IReadOnlyCollection<ChallengeSolution> Execute() => repository.GetAll();
}
