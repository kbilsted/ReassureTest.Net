using System;
using System.Linq;
using ReassureTest.Net.AST;

namespace ReassureTest.Net
{
    public static class ReassureSetup
    {
        public static void Is_old(this object o, object actual)
        {
            var sb = new IndentingStringBuilder();
            new Visitor().Visit(sb, o);
            Console.WriteLine(sb);
        }

        public static void Is(this object o, object actual)
        {
            Is_old(o, actual);

            Console.WriteLine("!!!!!!!!!!!!!");
            Console.WriteLine("!!!!!!!!!!!!!");
            Console.WriteLine("!!!!!!!!!!!!!");

            var ast = new ObjectVisitor().Visit(o);
            Console.WriteLine(new AstPrinter().PrintRoot(ast));
        }

        /// <summary>
        /// excpected, actual
        /// </summary>
        public static Action<object, object> Assert { get; set; }
    }
}
