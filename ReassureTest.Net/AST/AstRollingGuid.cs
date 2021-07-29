using System;

namespace ReassureTest.AST
{
    class AstRollingGuid
    {
        public readonly int RollingValue;

        public AstRollingGuid(int rollingGuid)
        {
            RollingValue = rollingGuid;
        }

        public override string ToString()
        {
            return $"guid-{RollingValue}";
        }
    }
}