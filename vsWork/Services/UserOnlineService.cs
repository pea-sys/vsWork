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
        public void Connect(string circuitId, User user, SessionRepository sessionRepository)
        {
            sessionRepo = sessionRepository;
            sessionRepository.Add(new Session() { SessionId = circuitId, UserId = user.UserId });
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
