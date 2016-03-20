# March 20th, 2016

0. Beginning of a programming language parser. The goal is to be able to type a code, generate an AlgorithmProgram and interpret it.
0. Support of variables size trace removed, because it has been judged useless. The dependence BareMetal of this feature has been removed.
0. Unit test added and updated.
0. Optimizations.

### Know issues

0. As the parser is not complete, it does not works correctly.
0. The interpreter is still not able to access the UI.

# March 13th, 2016

0. When the algorithm interpreter is in pause in debug mode, we can now perform a Step Into.
0. Step Over.
0. Step Out.
0. New unit tests.
0. A lot of refactoring and comments added.
0. Bug when we don't put a "return" at the end of a condition fixed.
0. Bug that show less log than excpected with iteration and condition fixed.
0. Know performance issue => when using operators, the interpreter is quite slower at first run than previous check-in.

# March 12th, 2016

0. We can now pause the algorithm interpreter.
0. We can add a breakpoint to an algorithm with AlgorithmBreakpointStatement.
0. We can resume a paused (by breakpoint or not) algorithm interpreter.
0. Bug fixed.

# March 9th, 2016

0. Stop any algorithm at runtime even with an infinite loop.
0. Keep a call stack and the values of each accessibles variables.
0. Refactoring.
0. Performance improvement.

# March 7th, 2016

0. StackOverflowException with recursivity avoided. 

# March 6th, 2016

0. Performance improvement with Binary Operator.
0. Bug fixed.

# April 19th, 2015

0. First prototype.