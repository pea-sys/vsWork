using Microsoft.AspNetCore.Components.Server.Circuits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace vsWork.Services
{
    public class CircuitHandlerService : CircuitHandler
    {
        public string CircuitId { get; set; }

        IUserOnlineService useronlineservice;
        public CircuitHandlerService(IUserOnlineService useronlineservice)
        {
            this.useronlineservice = useronlineservice;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            CircuitId = circuit.Id;
            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            useronlineservice.DisConnect(circuit.Id);
            return base.OnCircuitClosedAsync(circuit, cancellationToken);
        }
    }
}
