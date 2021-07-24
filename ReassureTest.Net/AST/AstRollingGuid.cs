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

        protected bool Equals(AstRollingGuid other)
        {
            return RollingValue == other.RollingValue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AstRollingGuid) obj);
        }

        public override int GetHashCode()
        {
            return RollingValue;
        }

        public override string ToString()
        {
            return $"guid-{RollingValue}";
        }
    }
}