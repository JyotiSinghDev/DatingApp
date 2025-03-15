using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository(DataContext dataContext, IMapper mapper) : IUserRepository
{
    public async Task<IEnumerable<MemberDto>> GetMembersAsync()
    {
        var memberList = await dataContext.Users.ProjectTo<MemberDto>(mapper.ConfigurationProvider)
        .ToListAsync();
        return memberList;
    }

    public async Task<MemberDto?> GetMemberUsernameAsync(string username)
    {
        var member = await dataContext.Users.Where(x=>x.UserName==username).ProjectTo<MemberDto>(mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();
        return member;
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
       return await dataContext.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
       return await dataContext.Users.Include(x=>x.Photos).SingleOrDefaultAsync(x=>x.UserName==username);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await dataContext.Users.Include(x=>x.Photos).
        ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
       return await dataContext.SaveChangesAsync()>0;
    }

    public void Update(AppUser appUser)
    {
        dataContext.Entry(appUser).State =EntityState.Modified;
    }

    
}
