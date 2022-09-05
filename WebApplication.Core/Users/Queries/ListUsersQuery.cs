using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using WebApplication.Core.Common.Models;
using WebApplication.Core.Users.Common.Models;
using WebApplication.Infrastructure.Interfaces;

namespace WebApplication.Core.Users.Queries
{
   public class ListUsersQuery : IRequest<PaginatedDto<IEnumerable<UserDto>>>
   {
      public int PageNumber { get; set; }
      public int ItemsPerPage { get; set; } = 10;

      public class Validator : AbstractValidator<ListUsersQuery>
      {
         public Validator()
         {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0);
         }
      }

      public class Handler : IRequestHandler<ListUsersQuery, PaginatedDto<IEnumerable<UserDto>>>
      {
         private readonly IUserService _userService;
         private readonly IMapper _mapper;

         public Handler(IUserService userService, IMapper mapper)
         {
            _userService = userService;
            _mapper = mapper;
         }
         /// <inheritdoc />
         public async Task<PaginatedDto<IEnumerable<UserDto>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
         {
            var users = await _userService.GetPaginatedAsync(request.PageNumber, request.ItemsPerPage);                  
            var nexPageUsers = await _userService.GetPaginatedAsync(request.PageNumber + 1, request.ItemsPerPage);
            
            return new PaginatedDto<IEnumerable<UserDto>> { Data = users.Select(user => _mapper.Map<UserDto>(user)).ToList(), HasNextPage = nexPageUsers.Count() > 0 };
         }
      }
   }
}
