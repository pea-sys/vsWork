using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace vsWork.Services
{
    /// <summary>
    /// Circit.Id:暗号化された乱数生成器から生成される回路ID。SignalRのセッション管理に使用。
    /// https://source.dot.net/#Microsoft.AspNetCore.Components.Server/Circuits/CircuitIdFactory.cs,fb8c407b00e7f8e2,references
    /// </summary>
    public class TrackingCircuitHandler : CircuitHandler
    {
        private UsersStateContainer _usersStateContainer;
        private AuthenticationStateProvider _authenticationStateProvider;
        public TrackingCircuitHandler(UsersStateContainer usersStateContainer, AuthenticationStateProvider authenticationStateProvider)
        {
            _usersStateContainer = usersStateContainer;
            _authenticationStateProvider = authenticationStateProvider;
        }
        public override async Task OnConnectionUpAsync(Circuit circuit,
            CancellationToken cancellationToken)
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            _usersStateContainer.Update(circuit.Id, state.User.Identity.Name);
            return;
        }
        public override Task OnConnectionDownAsync(Circuit circuit,
            CancellationToken cancellationToken)
        {
            _usersStateContainer.Remove(circuit.Id);
            return Task.CompletedTask;
        }
    }
}
