namespace ConsoleApplication1
{
    using System;
    using System.Diagnostics;

    using PortableSimulator;
    using PortableSimulator.Actions;
    using PortableSimulator.Build.Simulator;
    using PortableSimulator.Project;
    using PortableSimulator.Project.Algorithm;

    class Program
    {
        static void Main(string[] args)
        {
            /* PROJECT / ALGORITHM */
            StaticVariables.CurrentProject = new Project();

            // base
            var document = new Document("MyClass");
            document.Functions.Add(new Function("MyFunction"));
            StaticVariables.CurrentProject.Documents.Add(document);

            // variables
            StaticVariables.CurrentProject.Variables.Add(new Variable("Path", "path to the file to read", false));
            StaticVariables.CurrentProject.Variables.Add(new Variable("FileContent", "the file content", false));

            // algorithm
            var assignAction = new AssignAction(new ActionTools());
            assignAction.Edit();
            StaticVariables.CurrentProject.Documents[0].Functions[0].Algorithm.Add(assignAction);

            var readFileAction = new ReadFileAction(new ActionTools());
            readFileAction.Edit();
            StaticVariables.CurrentProject.Documents[0].Functions[0].Algorithm.Add(readFileAction);

            var badAssignAction = new BadAssignAction(new ActionTools());
            badAssignAction.Edit();
            StaticVariables.CurrentProject.Documents[0].Functions[0].Algorithm.Add(badAssignAction); // This will provoke an exception in the next action during simulaiton.

            var readFileAction2 = new ReadFileAction(new ActionTools());
            readFileAction2.Edit();
            StaticVariables.CurrentProject.Documents[0].Functions[0].Algorithm.Add(readFileAction2);


            /* SIMULATION */
            StaticVariables.CurrentSimulator = new Simulator();
            StaticVariables.CurrentSimulator.Start();

            Console.ReadKey();
        }
    }
}
