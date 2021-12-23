using System.Collections.Generic;
using System.Data;

namespace CluedIn.Connector.Common
{
    public class CommandBase<TParameter> where TParameter : IDbDataParameter
    {
        public string Text { get; set; }

        public IEnumerable<TParameter> Parameters { get; set; }
    }
}
