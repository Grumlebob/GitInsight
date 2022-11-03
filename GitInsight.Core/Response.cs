namespace GitInsight.Core;

public enum Response
{
    Ok = 200,
    Created = 201,
    Deleted = 204,
    BadRequest = 400,
    NotFound = 404,
    Conflict = 409,
}