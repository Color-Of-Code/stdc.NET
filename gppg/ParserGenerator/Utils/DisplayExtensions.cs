using System.Linq;
using System.Text;
using QUT.GPGen;

namespace QUT.Gplib
{
    static class DisplayExtensions
    {
        public static string ItemDisplay(this IState state)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("State {0}", state.Id);
            foreach (IProductionRule item in state.KernelItems)
            {
                builder.AppendLine();
                builder.AppendFormat("    {0}", item);
            }
            return builder.ToString();
        }


        public static string ItemDisplay(this IProduction production)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("{0} -> ", production.lhs);
            if (production.rhs.Any())
                builder.Append(ListUtilities.GetStringFromList(production.rhs, ", ", builder.Length));
            else
                builder.Append("/* empty */");
            return builder.ToString();
        }
    }
}