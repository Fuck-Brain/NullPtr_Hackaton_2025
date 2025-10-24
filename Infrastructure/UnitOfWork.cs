using Back.Domain.Interfaces;
using Back.Infrastructure.DataBase;

namespace Back.Infrastructure
{
    public class UnitOfWork 
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository Users { get; }
        public IUserSkillRepository Skills { get; }
        public IUserInterestRepository Interests { get; }
        public IRequestRepository Request { get; }
        public IUserHobbyRepository Hobbies { get; }
        public IResultRequestRepository Result { get; }
        public IUserLikeRepository userLikeRepository { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IUserRepository users,
            IUserSkillRepository skills,
            IUserInterestRepository interests,
            IRequestRepository req,
            IUserHobbyRepository hobbies,
            IResultRequestRepository result,
            IUserLikeRepository userLikeRepository)
        {
            _context = context;
            Users = users;
            Skills = skills;
            Interests = interests;
            Result = result;
            Hobbies = hobbies;
            Request = req;
            this.userLikeRepository = userLikeRepository;
        }

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    }

}
