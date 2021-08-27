using System;

namespace ReassureTest.AST
{
    class AstEnum
    {
        public readonly string EnumValue;

        public AstEnum(string enumValue)
        {
            EnumValue = enumValue ?? throw new ArgumentNullException(nameof(enumValue));
        }

        public override string ToString() => EnumValue;
    }
}