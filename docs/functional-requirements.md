# Functional requirements

## FR1

When running get command with a repository path (user/repoName), the relevant data of the repository should be stored in a database. If the data of a repository is already up to date in the database, it should not be updated and the commands should return the already stored data as a json object.

## FR2

When running a get command with commit frequency mode and a repository path the program should return a json object showing commits distributed on dates for the given repository.

## FR3

When running a get command with commit author mode and a repository path the program should return a json object showing commits per author for the given repository.
