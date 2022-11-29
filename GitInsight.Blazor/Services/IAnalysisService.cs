﻿using GitInsight.Core;

namespace GitInsight.Web.Services;

public interface IAnalysisService
{
    Task<List<CommitsByDateByAuthor>> GetCommitsByAuthor(string repoPath);
    Task<GitAwardWinner> EarlyBird(string repoPath);
    Task<List<CommitsByDate>> GetCommitsByDate(string repoPath);
    public Task<GitAwardWinner> NightOwl(string repoPath);

    Task<IEnumerable<ForkDto>> GetForksFromApi(string repoPath);
}