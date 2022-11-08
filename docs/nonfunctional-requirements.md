# Non-Functional requirements


## NFR1

When receiving a link to a repository, the application must temporarily store the repository as a local directory.
If the repository is already stored as a directory, the directory must be updated.

## NFR2

When running an analysis n a repository, the relevant data of the repository should be stored in a database. 
If the data of a repository is already up to date in the database, the database should not be updated,
and the analysis should use the already stored data.

## NFR3
The libgit2sharp library should be used to collect git repository data,from the given repositories.

## NFR4
The application should be written in C#.

## NFR5
The application should be made using .NET Core.

## NFR6
The application should be developed using an agile process.

## NFR7
The application should be developed using a test-driven process.

## NFR8
The Github should have one more Action workflows, 
that builds and tests the application everytime a push is made to the main branch, or a pull request is made to the main branch

## NFR9
Next to the code there should be a directory called docs, which contains files describing the project.

## NFR10
Established design patterns should be used when they are useful.
