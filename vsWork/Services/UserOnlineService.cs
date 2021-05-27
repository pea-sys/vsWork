using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vsWork.Data;

namespace vsWork.Services
{
    public class UserOnlineService : IUserOnlineService
    {
        private SessionRepository sessionRepo;
        public void Connect(CurrentUserService currentUserService, SessionRepository sessionRepository)
        {
            sessionRepo = sessionRepository;
            sessionRepository.Add(new Session() { SessionId = currentUserService.CircuitId, UserId = currentUserService.UserId });
        }

        public void DisConnect(string circuitId)
        {
            if (sessionRepo != null)
            {
                sessionRepo.Remove(circuitId);
            }
        }
    }
}
