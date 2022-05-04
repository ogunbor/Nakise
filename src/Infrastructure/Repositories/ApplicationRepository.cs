﻿using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;

namespace Infrastructure.Repositories
{
    public class ApplicantRepository : Repository<ApplicantDetail>, IApplicantRepository
    {
        public ApplicantRepository(AppDbContext context) : base(context)
        {
        }
    }
}
