
using System.Collections.Generic;

namespace QUT.Gplib
{
    public interface IState
    {
        int num { get; }

        List<IProductionRule> kernelItems { get; }
   }
}
