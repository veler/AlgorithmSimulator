namespace PortableSimulator.Project.Algorithm
{
    using System.Collections.ObjectModel;

    public class ActionTools : IActionTools
    {
        public Collection<Variable> GetCurrentProjectVariables()
        {
            return StaticVariables.CurrentProject.Variables;
        }
    }
}
