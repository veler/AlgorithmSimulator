# Algorithm Simulator
This is a `prototype` of a portable algorithm simulator based on the architecture of [SoftwareZator](http://softwarezator.velersoftware.com/).

# What Is The Concept

The concept is to be able to simulate and debug an algorithm by using C# at runtime.
For a bigger challenge, we want to be able to use it in a cross-platform project.

It means several things :
* I will be unable to use [CodeDom](https://msdn.microsoft.com/en-us/library/system.codedom(v=vs.110).aspx) or [Roslyn](https://roslyn.codeplex.com/) to compile code and run it.
* I also will be unable to use the `Windows Debugger` as it is `not available with WinRT`.
* So, I will make my own CodeDom-like architecture, named [AlgorithmDom](https://github.com/veler/AlgorithmSimulator/tree/master/PortableSimulator/Build/AlgorithmDOM/DOM).
* And make an `interpreter` that will `analyze the AlgorithmDom and perform an action` depending of it.

**Remark : the code is not very complicated, but the project architecture is complex because it uses a LOT of abstraction.**

### Be Indulgent

**It is just a prototype !** There is not a lot of comments and documentation, and the performances are poor.

# How Does It Work

0. First of all, we create a [Project](https://github.com/veler/AlgorithmSimulator/tree/master/PortableSimulator/Project) that represents a program with classes and methods.
0. We add our global variables (it is simpler to manage in the simulator).
0. In a method, we add an `algorithm`. This algorithm is represented by an [Action](https://github.com/veler/AlgorithmSimulator/tree/master/PortableSimulator/Algorithm). An action, in [SoftwareZator](http://softwarezator.velersoftware.com/), as in this simulator, it is a part of an algorithm that does something. For example :
  * [Assign a value to a variable](https://github.com/veler/AlgorithmSimulator/blob/master/PortableSimulator/Actions/AssignAction.cs)
  * [Read a file](https://github.com/veler/AlgorithmSimulator/blob/master/PortableSimulator/Actions/ReadFileAction.cs)
  * Do an HTTP request
  * Display a message
  * ...etc
0. And then, we start the [Simulator](https://github.com/veler/AlgorithmSimulator/tree/master/PortableSimulator/Build/Simulator).

# What Does The Simulator

0. The first step is to create a dictionary that contains a definition of each variable in the project and their associated values.
0. Then, we start to simulate/interpret each Action of the algorithm :
  * We display in the Debug the value of each variables of the project (like Visual Studio does on a breakpoint).
  * The action will generate an [AlgorithmDom](https://github.com/veler/AlgorithmSimulator/tree/master/PortableSimulator/Build/AlgorithmDOM/DOM) (my cross-plateform CodeDom-like architecture)
  * And we will ask to the [Interpreter](https://github.com/veler/AlgorithmSimulator/tree/master/PortableSimulator/Build/Simulator/Interpreter) to analyze it and change, for example, the value of a variable if tha AlgorithmDom is corresponding to an assignation.
  * In case of exception in the algorithm (for example : I want to read a file that does not exists), we ask to the current Action to bring us some tips to fix this error (imagine that we are in [SoftwareZator](http://softwarezator.velersoftware.com/) for example :-) ).
  
# Use Case
  
* Any programming learning app (SoftwareZator, Spark, Scratch...).
* If you want to download a part of an application from internet in WinRT and want to run it. I guess that we cannot load an Assembly at runtime with WinRT. Download an algorithm and simulate it can be a solution.
