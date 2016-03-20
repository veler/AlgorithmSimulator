using Algo.Runtime.Build.AlgorithmDOM.DOM;

namespace Algo.Runtime.Build.Parser.Lexer
{
    public delegate TokenEvaluatorResult TokenEvaluator(string text, string[] splittedText);
}
