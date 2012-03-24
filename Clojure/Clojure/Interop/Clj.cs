using System.Dynamic;
using clojure.lang;

namespace clojure.lang.Interop
{
    public class Clj
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
            var symbol = Symbol.intern(nsname);
            REQUIRE.invoke(symbol);
            return new Namespace(nsname);
        }
    }
}