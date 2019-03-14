
using System.Collections.Generic;

namespace QUT.Gplib
{
    public interface IState
    {
        int Id { get; }

        List<IProductionRule> KernelItems { get; }
   }
}
