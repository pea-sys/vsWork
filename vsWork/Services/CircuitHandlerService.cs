
using Microsoft.AspNetCore.Components.Server.Circuits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using vsWork.Stores;
using Fluxor;
namespace vsWork.Services
{
    public class CircuitHandlerService : CircuitHandler
    {
        private readonly IDispatcher dispatcher;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CircuitHandlerService(IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
        /// <summary>
        /// セッション開始イベント
        /// </summary>
        /// <param name="circuit"></param>
        /// <param name="cancellationToken"></param>
        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            dispatcher.Dispatch(new ConnectAction(circuit.Id));
            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }
        /// <summary>
        /// セッション終了イベント
        /// </summary>
        /// <param name="circuit"></param>
        /// <param name="cancellationToken"></param>
        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            dispatcher.Dispatch(new DisConnectAction(circuit.Id));
            return base.OnCircuitClosedAsync(circuit, cancellationToken);
        }
    }
}
