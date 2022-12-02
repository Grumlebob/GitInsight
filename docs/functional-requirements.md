# Functional requirements


## FR1

The program must exposes a REST API

## FR2

The API must be able to receive a link to a repository on github, in the form of
<github_user>/<repository_name> or of the form <github_organization>/<repository_name>, through a GET request on the web application route.
Example: if the application is running on local host port 8000, localhost:8000/mono/xwt, the API should receive a link to the repository mono/xwt on Github.

## FR3
When a GET request with a link to a git repository has been made,
the API must return two types of analysis on the given repository:
A list the number of commits per day.
A list the number of commits per day per author.

## FR4
The API must expose an endpoint that lets you retrieve information about the forks of a repository.

## FR5
The results from the analysis must be delivered as JSON objects

## FR6
The program must have a web based frontend.

## FR7
The frontend must be able to retrieve data from the analysis (commits by date, commits by author and forks) from the web API.
It should then visualise the data in a suitable format.

## FR8
The web API must implement at least one more analyisis, and the frontend must be able to show a suitable visualisation.

## FR9
All communication between the frontend and the API must be encrypted.

## FR10
The user must be not be able to access the API or the frontend services without being authenticated. 

## FR11
When the users accesses the frontend, they must be told to authenticate themselves first, and have the ability to do so.








