﻿using GitInsight.Core;

namespace GitInsight.Web.Services;

public interface IAnalysisService
{
    Task<List<CommitsByDateByAuthor>> GetCommitsByAuthor(string repoPath);

    Task<IEnumerable<ForkDto>> GetForksFromApi(string repoPath);
}
