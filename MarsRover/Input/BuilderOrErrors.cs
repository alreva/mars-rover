namespace MarsRover.Input;

public class BuilderOrErrors<TBuidlder>
{
    public BuilderOrErrors()
    {
        Errors = new List<Error>();
    }

    public List<Error> Errors { get; set; }
    
    public TBuidlder? Builder { get; set; }
    
    public bool HasErrors => Errors.Any();
    
    public void AddError(Error error)
    {
        Errors.Add(error);
    }
    
    // cast from Builder to BuilderOrErrors
    public static implicit operator BuilderOrErrors<TBuidlder>(TBuidlder builder)
    {
        return new BuilderOrErrors<TBuidlder> { Builder = builder };
    }
}