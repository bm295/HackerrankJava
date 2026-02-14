using HackerrankJava.Application;
using HackerrankJava.Domain;

namespace HackerrankJava.Infrastructure;

public sealed class FileSystemChallengeSolutionRepository(string repositoryRoot) : IChallengeSolutionRepository
{
    public IReadOnlyCollection<ChallengeSolution> GetAll()
    {
        var javaDirectory = Path.Combine(repositoryRoot, "java");
        if (!Directory.Exists(javaDirectory))
        {
            return [];
        }

        return Directory.EnumerateFiles(javaDirectory, "*.java", SearchOption.TopDirectoryOnly)
            .Select(path => new ChallengeSolution(Path.GetFileNameWithoutExtension(path), Path.GetRelativePath(repositoryRoot, path)))
            .OrderBy(solution => solution.Name, StringComparer.Ordinal)
            .ToArray();
    }
}
