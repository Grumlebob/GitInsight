namespace GitInsight.Core;

public interface ICommitRepository
{
    (CommitDTO? commit, Response response) Find(int id);
    (IReadOnlyCollection<CommitDTO> commits, Response response) FindAll();
    (Response response, CommitDTO? commit) Create(CommitCreateDTO DTO);
    (Response response, CommitDTO? commit) Update(CommitDTO commit);
    Response Delete(int id);
}
