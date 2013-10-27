using System;

namespace GlobalPhone
{
    internal class Begin
    {
        internal static T Do<T>(Func<T> dofunc)
        {
            return dofunc();
        }
    }
}