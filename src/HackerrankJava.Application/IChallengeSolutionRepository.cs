using HackerrankJava.Domain;

namespace HackerrankJava.Application;

public interface IChallengeSolutionRepository
{
    IReadOnlyCollection<ChallengeSolution> GetAll();
}
