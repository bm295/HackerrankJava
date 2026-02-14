using HackerrankJava.Application;
using HackerrankJava.Infrastructure;

var repositoryRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
var useCase = new ListSolutionsUseCase(new FileSystemChallengeSolutionRepository(repositoryRoot));

Console.WriteLine("Hackerrank solutions (layered architecture demo):");
foreach (var solution in useCase.Execute())
{
    Console.WriteLine($"- {solution.Name}: {solution.RelativeJavaPath}");
}
