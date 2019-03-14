using System.Text;

namespace QUT.Gplib
{
    static class IStateExtensions
    {
        public static string ItemDisplay(this IState state)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("State {0}", state.num);
            foreach (IProductionRule item in state.kernelItems)
            {
                builder.AppendLine();
                builder.AppendFormat("    {0}", item);
            }
            return builder.ToString();
        }
    }
}