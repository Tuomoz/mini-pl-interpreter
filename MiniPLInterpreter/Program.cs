using System;
using System.Collections.Generic;
using System.Linq;
using Frontend;
using System.IO;

namespace Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: interpreter filename [-p]\n\n-p: Print the parsed abstract syntax tree.");
                return;
            }
            string filename = args[0];
            StreamReader sourceFile;
            try
            {
                sourceFile = new StreamReader(filename);
            }
            catch (Exception)
            {
                Console.WriteLine("File " + filename + " not found.");
                return;
            }
            
            SourceReader source = new SourceReader(sourceFile);
            ErrorHandler errors = new ErrorHandler();
            Scanner lexer = new Scanner(source, errors);
            Parser parser = new Parser(lexer, errors);
            StmtList ast = parser.Parse();
            TypeCheckerVisitor checker = new TypeCheckerVisitor(errors, ast);
            SymbolTable SymbolTable = checker.TypeCheck();
            if (errors.HasErrors)
            {
                Console.WriteLine("Given program contains following errors:");
                foreach (Error error in errors.GetErrors())
                {
                    Console.WriteLine(error);
                }
                return;
            }
            if (args.Length > 1 && args[1] == "-p")
            {
                Console.WriteLine("Parsed abstact syntax tree:");
                AstPrinterVisitor printer = new AstPrinterVisitor(ast);
                printer.Print();
                Console.WriteLine();
                Console.WriteLine("Program output:");
            }
            ExecutorVisitor executor = new ExecutorVisitor(SymbolTable, ast);
            try
            {
                executor.Execute();
            }
            catch (RuntimeException e)
            {
                Console.WriteLine("Runtime exception: " + e.Message);
            }
        }
    }
}
