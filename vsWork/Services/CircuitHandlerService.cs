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
        private readonly UserActionService userActionService;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CircuitHandlerService(UserActionService userActionService)
        {
            this.userActionService = userActionService;
        }
        /// <summary>
        /// セッション開始イベント
        /// </summary>
        /// <param name="circuit"></param>
        /// <param name="cancellationToken"></param>
        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            userActionService.Connect(circuit.Id);
            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }
        /// <summary>
        /// セッション終了イベント
        /// </summary>
        /// <param name="circuit"></param>
        /// <param name="cancellationToken"></param>
        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            userActionService.DisConnect(circuit.Id);
            return base.OnCircuitClosedAsync(circuit, cancellationToken);
        }
    }
}
