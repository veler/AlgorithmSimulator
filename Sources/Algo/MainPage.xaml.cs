﻿using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Algo.Runtime.Build.AlgorithmDOM.DOM;
using Algo.Runtime.Build.Runtime;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Algo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    internal sealed partial class MainPage : Page
    {
        private AlgorithmInterpreter _algorithmInterpreter;

        internal MainPage()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            var program = new AlgorithmProgram("MyApp");
            var firstClass = new AlgorithmClassDeclaration("FirstClass");

            // The following code is simulated

            /* 
               FUNCTION FirstMethod(num)
                    IF (num > 1)
                        RETURN FirstMethod(num - 1)
                    END IF
                    RETURN num
               END FUNCTION
               
               VARIABLE stopWatch = new StopWatch()
               stopWatch.Start()
               
               FirstMethod(100)
               
               stopWatch.Stop()
               VARIABLE messageDialog = new MessageDialog()
               messageDialog.ShowAsync(stopWatch.Elapsed.TotalMilliseconds.ToString())
             */

            var firstMethod = new AlgorithmClassMethodDeclaration("FirstMethod", false);
            firstMethod.Arguments.Add(new AlgorithmParameterDeclaration("num"));

            firstMethod.Statements.Add(new AlgorithmConditionStatement(new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.GreaterThan, new AlgorithmPrimitiveExpression(1)), new AlgorithmStatementCollection() { new AlgorithmReturnStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmBinaryOperatorExpression(new AlgorithmVariableReferenceExpression("num"), AlgorithmBinaryOperatorType.Subtraction, new AlgorithmPrimitiveExpression(1)))) }, null));
            firstMethod.Statements.Add(new AlgorithmReturnStatement(new AlgorithmVariableReferenceExpression("num")));

            firstClass.Members.Add(firstMethod);

            var entryPoint = new AlgorithmEntryPointMethod();

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("stopWatch"));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("stopWatch"), new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression(typeof(Stopwatch)))));
            entryPoint.Statements.Add(new AlgorithmExpressionStatement(new AlgorithmInvokeCoreMethodExpression(new AlgorithmVariableReferenceExpression("stopWatch"), "Start", null)));

            entryPoint.Statements.Add(new AlgorithmExpressionStatement(new AlgorithmInvokeMethodExpression(new AlgorithmThisReferenceExpression(), "FirstMethod", new AlgorithmPrimitiveExpression(100))));

            entryPoint.Statements.Add(new AlgorithmExpressionStatement(new AlgorithmInvokeCoreMethodExpression(new AlgorithmVariableReferenceExpression("stopWatch"), "Stop", null)));

            entryPoint.Statements.Add(new AlgorithmVariableDeclaration("messageDialog"));
            entryPoint.Statements.Add(new AlgorithmAssignStatement(new AlgorithmVariableReferenceExpression("messageDialog"), new AlgorithmInstanciateExpression(new AlgorithmClassReferenceExpression(typeof(MessageDialog)), new AlgorithmInvokeCoreMethodExpression(new AlgorithmPropertyReferenceExpression(new AlgorithmPropertyReferenceExpression(new AlgorithmVariableReferenceExpression("stopWatch"), "Elapsed"), "TotalMilliseconds"), "ToString", null))));
            entryPoint.Statements.Add(new AlgorithmExpressionStatement(new AlgorithmInvokeCoreMethodExpression(new AlgorithmVariableReferenceExpression("messageDialog"), "ShowAsync", null)));

            firstClass.Members.Add(entryPoint);

            program.Classes.Add(firstClass);
            program.UpdateEntryPointPath();

            _algorithmInterpreter = new AlgorithmInterpreter(program);
        }

        private async void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            await _algorithmInterpreter.StartAsync(debugMode: false);

            //task.Wait();
            //task.RunSynchronously();
        }
    }
}
