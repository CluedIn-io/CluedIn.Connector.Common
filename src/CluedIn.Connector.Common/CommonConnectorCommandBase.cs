using System.Collections.Generic;
using System.Data;

namespace Connector.Common
{
    public class CommonConnectorCommandBase<TParameter> where TParameter : IDbDataParameter
    {
        public string Text { get; set; }

        public IEnumerable<TParameter> Parameters { get; set; }
    }
}
