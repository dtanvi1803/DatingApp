using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user =await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        // public async Task<IEnumerable<User>> GetUsers()
        // {
        //     var users = await _context.Users.Include(p => p.Photos).ToListAsync();
        //     return users;
        // }
        public async Task<PagedList<User>> GetUsers(UserParams useParamas)
        {
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where(u => u.Id != useParamas.UserId);
            users = users.Where(u => u.Gender == useParamas.Gender);
            if(useParamas.Likers) {
                var userLikers = await GetUsersLikes(useParamas.UserId, useParamas.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }
            if(useParamas.Likees) {
                var userLikees = await GetUsersLikes(useParamas.UserId, useParamas.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));                
            }            
            if(useParamas.MinAge != 18 || useParamas.MaxAge !=99) {
                var minDob = DateTime.Today.AddYears(- useParamas.MaxAge-1);
                var maxDob = DateTime.Today.AddYears(- useParamas.MinAge);
                users = users.Where( u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            }
            if (!string.IsNullOrWhiteSpace(useParamas.OrderBy)){
                switch (useParamas.OrderBy) {
                    case "created":
                        users= users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, useParamas.PageNumber, useParamas.PageSize);
        }

        private async Task<IEnumerable<int>> GetUsersLikes(int id, bool likers) {
            var user = await _context.Users
                .Include(x => x.Likers)
                .Include(x => x.Likees)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            };
        }
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id) {
            var photo = await _context.Photos.FirstOrDefaultAsync(u => u.Id == id);
            return photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }
    }
}