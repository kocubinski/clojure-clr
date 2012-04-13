using System.Dynamic;

namespace clojure.lang.Hosting
{
    public class Clojure
    {
        private static readonly Var REQUIRE = RT.var("clojure.core", "require");

        class Namespace : DynamicObject
        {
            private readonly string nsname;

            public Namespace(string nsname)
            {
                this.nsname = nsname;
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                var varName = binder.Name;
                var fn = RT.var(nsname, varName);
                result = fn.applyTo(RT.seq(args));
                return true;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                var varName = binder.Name;
                result = RT.var(nsname, varName);
                return true;
            }
        }

        public static dynamic Require(string nsname)
        {
            if(nsname.Equals("clojure.core"))
                return new Namespace(nsname);
            var symbol = Symbol.intern(nsname);
            REQUIRE.invoke(symbol);
            return new Namespace(nsname);
        }

        public static void AddNamespaceDirectoryMapping(string baseNamespace, string directory)
        {
            RT.var("clojure.core", "add-ns-load-mapping").invoke(baseNamespace, directory);
        }
    }
}