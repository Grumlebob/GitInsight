# Functional requirements


## FR1

The program must be a web application that exposes a REST API

## FR2

The API must be able to receive a link to a repository on github, in the form of
<github_user>/<repository_name> or of the form <github_organization>/<repository_name>, through a GET request on the web application route.
Example: if the application is running on local host port 8000, http://localhost:8000/mono/xwt, the API should receive a link to the repository mono/xwt on Github

## FR3
When a GET request with a link to a git repository has been made,
the API must return two types of analysis on the given repository:
A list the number of commits per day.
A list the number of commits per day per author.

## FR4
The results from the analysis must be delivered as JSON objects




