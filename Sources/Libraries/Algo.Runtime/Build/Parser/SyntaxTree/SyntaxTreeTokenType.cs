using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algo.Runtime.Build.AlgorithmDOM.DOM;

namespace Algo.Runtime.Build.Parser.SyntaxTree
{
    public enum SyntaxTreeTokenType
    {
        Unknow,

        BeginProgram,
        BeginClass,
        BeginMethod,
        BeginBlock,

        EndBlock,
        EndMethod,
        EndClass,
        EndProgram
    }
}
