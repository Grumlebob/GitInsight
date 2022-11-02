namespace GitInsight.Core;

public enum Response
{
    Ok,
    Created,
    Deleted,
    Updated,
    NotFound,
    Conflict,
    UnMatchedForeignKey //may want to rename that 
}