# Algorithm Simulator
This is a `prototype` of a portable algorithm simulator based on the architecture of [SoftwareZator](http://softwarezator.velersoftware.com/).

## What Is The Concept

The concept is to be able to simulate and debug an algorithm by using C# at runtime.
For a bigger challenge, we want to be able to use it in a cross-platform project.

It means several things :
* I will be unable to use [CodeDom](https://msdn.microsoft.com/en-us/library/system.codedom(v=vs.110).aspx) or [Roslyn](https://roslyn.codeplex.com/) to compile code and run it.
* I also will be unable to use the `Windows Debugger` as it is `not available with WinRT`.
* So, I will make my own CodeDom-like architecture, named [AlgorithmDom](https://github.com/veler/AlgorithmSimulator/tree/master/Sources/Libraries/Algo.Runtime/Build/AlgorithmDOM/DOM).
* And make an `interpreter` that will `analyze the AlgorithmDom and perform an action` depending of it.

**Remark : the code is not very complicated, but the project architecture is complex because it uses a LOT of abstraction.**
  
## Use Case
  
* Any programming learning app (SoftwareZator, Spark, Scratch...).
* If you want to download a part of an application from internet in WinRT and want to run it. I guess that we cannot load an Assembly at runtime with WinRT. Download an algorithm and simulate it can be a solution.

# Features

| Features                                                                               | Supported |
| -------------------------------------------------------------------------------------- |:---------:|
| variable                                                                               | yes       |
| class                                                                                  | yes       |
| method                                                                                 | yes       |
| recursivity                                                                            | yes       |
| loop (while/do while)                                                                  | yes       |
| call a method                                                                          | yes       |
| use feature from WinRT                                                                 | yes       |
| start the simulator                                                                    | yes       |
| stop the simulator (even in an infinite loop)                                          | yes       |
| pause the simulator                                                                    | yes       |
| breakpoint                                                                             | yes       |
| detect when a user call too many methods in the same thread (aka. stack overflow)      | yes       |
| async functions                                                                        | yes       |
| keep a call stack with info on variables values                                        | yes       |
| try catch                                                                              | **no**    |
| interaction with the UI                                                                | **no**    |
| inheritance with classes                                                               | **no**    |
| mode debug                                                                             | yes       |

# Performances

Performances are not comparable with a JavaScript runtime or a compiled language like C++. The reason is that this interpreter is made in C# and it's designed to be maintenable, more than fast.
It also use a lot of RAM.

| Scenarios                  | Execution time (in millisec) |
| -------------------------- |:----------------------------:|
| a loop with 100 iterations | 9.28760                      |

# How Does It Work

0. First of all, we create a [Program](https://github.com/veler/AlgorithmSimulator/blob/master/Sources/Libraries/Algo.Runtime/Build/AlgorithmDOM/DOM/AlgorithmProgram.cs) that represents a program with classes and methods.
0. We add our global variables (it is simpler to manage in the simulator).
0. In a method, we add an `algorithm`. This algorithm is represented by an action. An action, in [SoftwareZator](http://softwarezator.velersoftware.com/), as in this simulator, it is a part of an algorithm that does something. For example :
  * [Assign a value to a variable](https://github.com/veler/AlgorithmSimulator/blob/master/Sources/Libraries/Algo.Runtime/Build/Runtime/Interpreter/Statements/Assign.cs)
  * Read a file
  * Do an HTTP request
  * Display a message
  * ...etc
0. And then, we start the [Simulator](https://github.com/veler/AlgorithmSimulator/blob/master/Sources/Libraries/Algo.Runtime/Build/Runtime/Simulator.cs).

## What Does The Simulator

0. The first step is to create a dictionary that contains a definition of each variable in the project and their associated values.
0. Then, we start to simulate/interpret each Action of the algorithm :
  * We display in the Debug the value of each variables of the project (like Visual Studio does on a breakpoint).
  * The action will generate an [AlgorithmDom](https://github.com/veler/AlgorithmSimulator/tree/master/Sources/Libraries/Algo.Runtime/Build/AlgorithmDOM/DOM) (my cross-plateform CodeDom-like architecture)
  * And we will ask to the [Interpreter](https://github.com/veler/AlgorithmSimulator/tree/master/Sources/Libraries/Algo.Runtime/Build/Runtime/Interpreter) to analyze it and change, for example, the value of a variable if tha AlgorithmDom is corresponding to an assignation.
  * In case of exception in the algorithm (for example : I want to read a file that does not exists), we ask to the current Action to bring us some tips to fix this error (imagine that we are in [SoftwareZator](http://softwarezator.velersoftware.com/) for example :-) ).

# Sample

The following algorithm :

```
PROGRAM MyApp

    CLASS FirstClass

        FUNCTION Main()
            RETURN FirstMethod(10)
        END FUNCTION

        FUNCTION FirstMethod(num)
            IF (num > 1)
                RETURN FirstMethod(num - 1)
            RETURN num
        END FUNCTION

    END CLASS

END PROGRAM
```

Can be translated and interpreted by the simulator like this in C# :

```csharp
var program = new AlgorithmProgram("MyApp");
var firstClass = new AlgorithmClassDeclaration("FirstClass");

var entryPoint = new AlgorithmEntryPointMethod(); // FUNCTION Main()
entryPoint.Statements.Add(new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmPrimitiveExpression(10)))); // RETURN FirstMethod(10)
firstClass.Members.Add(entryPoint);
    
var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
firstMethod.Arguments.Add(new AlgorithmParameterDeclaration("num")); // FUNCTION FirstMethod(num)

var argument = new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Subtraction, new AlgorithmPrimitiveExpression(1)); // num - 1

var returnStatement = new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", argument)); // RETURN FirstMethod(num - 1)

var condition = new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.GreaterThan, new AlgorithmPrimitiveExpression(1));
firstMethod.Statements.Add(new AlgorithmConditionStatement(condition, new AlgorithmStatementCollection() { returnStatement }, null)); // IF (num > 1)
firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("num"))); // RETURN num

firstClass.Members.Add(firstMethod);

program.Classes.Add(firstClass);
program.UpdateEntryPointPath();

var simulator = new Simulator(program);

var task = simulator.StartAsync(debugMode: true);

task.Wait();
```

# Change log

[Check the change log here](https://github.com/veler/AlgorithmSimulator/blob/master/CHANGELOG.md)